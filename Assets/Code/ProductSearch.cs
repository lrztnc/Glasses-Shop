using System.Collections.Generic;
using System.Globalization;
using System.Text;
using TMPro;
using UnityEngine;

public class ProductSearch : MonoBehaviour
{
    [Header("Riferimenti UI")]
    [Tooltip("La TMP_InputField della barra di ricerca (es. SearchBar).")]
    public TMP_InputField searchBar;

    [Tooltip("Il contenitore degli item nella Scroll View (il 'Content' che contiene 1B, 2B, 3B, 4B, 5B).")]
    public Transform contentParent;

    [Header("Opzioni")]
    [Tooltip("Nome (o suffisso) del componente titolo dentro ogni item (es. '1BTitle', '2BTitle'...).")]
    public string titleObjectNameSuffix = "Title";

    [Tooltip("Se attivo, la query deve contenere TUTTE le parole (AND). Se off, basta una parola (OR).")]
    public bool requireAllWords = true;

    // Cache interna
    private readonly List<Entry> _entries = new List<Entry>();

    private class Entry
    {
        public GameObject itemGO;
        public string titleNorm; // titolo normalizzato e in minuscolo
    }

    void Awake()
    {
        BuildCache();

        if (searchBar != null)
        {
            // Filtraggio live mentre l'utente scrive
            searchBar.onValueChanged.AddListener(OnSearchChanged);
            // All'avvio mostra tutto
            OnSearchChanged(searchBar.text);
        }
        else
        {
            // Nessuna search bar -> mostra tutto
            ShowAll(true);
        }
    }

    private void BuildCache()
    {
        _entries.Clear();
        if (contentParent == null) return;

        for (int i = 0; i < contentParent.childCount; i++)
        {
            var item = contentParent.GetChild(i).gameObject;
            if (item == null) continue;

            // trova il TMP_Text del titolo cercando per nome (…Title)
            TMP_Text titleText = null;
            var texts = item.GetComponentsInChildren<TMP_Text>(true);
            foreach (var t in texts)
            {
                if (t != null && t.name.EndsWith(titleObjectNameSuffix, System.StringComparison.OrdinalIgnoreCase))
                {
                    titleText = t;
                    break;
                }
            }

            if (titleText == null) continue;

            _entries.Add(new Entry
            {
                itemGO = item,
                titleNorm = Normalize(titleText.text)
            });
        }
    }

    private void OnSearchChanged(string rawQuery)
    {
        string q = Normalize(rawQuery);

        if (string.IsNullOrWhiteSpace(q))
        {
            ShowAll(true);
            return;
        }

        // dividi la query in parole (spazi multipli ok)
        string[] words = q.Split(' ', System.StringSplitOptions.RemoveEmptyEntries);

        foreach (var e in _entries)
        {
            bool match;
            if (requireAllWords)
            {
                // AND: tutte le parole devono essere contenute nel titolo
                match = true;
                for (int i = 0; i < words.Length; i++)
                {
                    if (!e.titleNorm.Contains(words[i]))
                    {
                        match = false;
                        break;
                    }
                }
            }
            else
            {
                // OR: basta una parola
                match = false;
                for (int i = 0; i < words.Length; i++)
                {
                    if (e.titleNorm.Contains(words[i]))
                    {
                        match = true;
                        break;
                    }
                }
            }

            if (e.itemGO.activeSelf != match)
                e.itemGO.SetActive(match);
        }
    }

    private void ShowAll(bool value)
    {
        foreach (var e in _entries)
            if (e.itemGO.activeSelf != value)
                e.itemGO.SetActive(value);
    }

    // Normalizza: lowercase, rimuove accenti/diacritici e spazi doppi
    private static string Normalize(string s)
    {
        if (string.IsNullOrEmpty(s)) return string.Empty;

        s = s.ToLowerInvariant().Trim();

        // rimuove accenti (é -> e, ò -> o)
        string formD = s.Normalize(NormalizationForm.FormD);
        StringBuilder sb = new StringBuilder(formD.Length);
        foreach (char c in formD)
        {
            var uc = CharUnicodeInfo.GetUnicodeCategory(c);
            if (uc != UnicodeCategory.NonSpacingMark)
                sb.Append(c);
        }
        s = sb.ToString().Normalize(NormalizationForm.FormC);

        // sostituisce sequenze di spazi con uno spazio singolo
        while (s.Contains("  ")) s = s.Replace("  ", " ");

        return s;
    }
}