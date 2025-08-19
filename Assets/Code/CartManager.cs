using System.Collections;
using System.Globalization;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CartManager : MonoBehaviour
{
    [Header("UI - voci carrello (1B, 2B, 3B, 4B, 5B)")]
    [Tooltip("Trascina qui 1B..5B nell'ordine dei prodotti.")]
    public GameObject[] cartItems;          // 1B..5B (gli item nella Scroll View)

    [Header("Bottoni Remove (opzionali)")]
    [Tooltip("Pulsanti 'Remove item' corrispondenti agli item, stesso ordine di cartItems.")]
    public Button[] removeButtons;          // stessi indici di cartItems

    [Header("Prezzi (opzionale)")]
    [Tooltip("Se valorizzato, usa questi prezzi (in €) invece del parsing dal testo.")]
    public float[] itemPrices;              // es. 89, 109, ...

    [Header("Totale")]
    [Tooltip("TMP_Text a destra di 'Total:' (es. nodo 'Amount')")]
    public TMP_Text totalValueText;

    private readonly CultureInfo it = new CultureInfo("it-IT");

    // --- gestione ricalcolo differito ---
    private bool recalcDirty = false;
    private Coroutine pendingCoroutine = null;

    void Awake()
    {
        HideAll();

        // Collega i pulsanti Remove
        if (removeButtons != null)
        {
            for (int i = 0; i < removeButtons.Length; i++)
            {
                int index = i;
                if (removeButtons[i] != null)
                    removeButtons[i].onClick.AddListener(() =>
                    {
                        HideItem(index);     // nasconde
                        QueueRecalc();       // ricalcola quando possibile
                    });
            }
        }

        // Primo ricalcolo appena possibile
        QueueRecalc();
    }

    void OnEnable()
    {
        // Se erano avvenute modifiche mentre il pannello era spento,
        // ricalcola ora (al frame successivo)
        if (recalcDirty)
            ScheduleNextFrameRecalc();
    }

    public void HideAll()
    {
        if (cartItems == null) return;
        foreach (var go in cartItems)
            if (go != null) go.SetActive(false);
    }

    /// <summary>Mostra la voce del carrello corrispondente all’indice prodotto.</summary>
    public void ShowItem(int index)
    {
        if (!IsValid(index)) return;
        if (cartItems[index] != null)
            cartItems[index].SetActive(true);

        QueueRecalc();
    }

    /// <summary>Nasconde (rimuove) una singola voce dal carrello.</summary>
    public void HideItem(int index)
    {
        if (!IsValid(index)) return;
        if (cartItems[index] != null)
            cartItems[index].SetActive(false);

        QueueRecalc();
    }

    private bool IsValid(int index)
    {
        return cartItems != null && index >= 0 && index < cartItems.Length;
    }

    // --------- RICALCOLO DIFFERITO / SAFE ---------

    /// <summary>
    /// Segna che serve ricalcolare e, se il GameObject è attivo, pianifica il calcolo
    /// al frame successivo; altrimenti lo farà in OnEnable.
    /// </summary>
    private void QueueRecalc()
    {
        recalcDirty = true;
        if (isActiveAndEnabled)
            ScheduleNextFrameRecalc();
        // se non è attivo, OnEnable lo eseguirà più tardi
    }

    private void ScheduleNextFrameRecalc()
    {
        // evita più coroutine sovrapposte
        if (pendingCoroutine != null)
            StopCoroutine(pendingCoroutine);
        pendingCoroutine = StartCoroutine(UpdateTotalNextFrame());
    }

    private IEnumerator UpdateTotalNextFrame()
{
    // attende un frame per lasciare aggiornare layout/TMP
    yield return null;
    yield return new WaitForEndOfFrame();   // ← parentesi tonde!
    UpdateTotal();
    recalcDirty = false;
    pendingCoroutine = null;
}

    /// <summary>Somma i prezzi degli item attivi e aggiorna il testo del totale.</summary>
    public void UpdateTotal()
    {
        float total = 0f;

        if (cartItems != null)
        {
            for (int i = 0; i < cartItems.Length; i++)
            {
                var go = cartItems[i];
                if (go == null) continue;

                if (go.activeInHierarchy)   // solo visibili
                    total += GetItemPrice(i, go);
            }
        }

        if (totalValueText != null)
            totalValueText.text = total.ToString("N2", it) + " €";
    }

    /// <summary>
    /// Prezzo dell'item i-esimo:
    /// 1) usa itemPrices se valorizzato;
    /// 2) altrimenti prova a leggerlo dal testo (es. "89,00 €").
    /// </summary>
    private float GetItemPrice(int index, GameObject itemGO)
    {
        // 1) array manuale (consigliato per evitare parsing)
        if (itemPrices != null && index >= 0 && index < itemPrices.Length && itemPrices[index] > 0f)
            return itemPrices[index];

        // 2) parsing dal testo (anche se l'item è inattivo)
        var texts = itemGO.GetComponentsInChildren<TMP_Text>(true);
        foreach (var t in texts)
        {
            if (string.IsNullOrWhiteSpace(t.text)) continue;

            // pattern numerico robusto: 89, 89,00, 1.299,50, 109.00, ecc.
            var m = Regex.Matches(t.text, @"\d{1,3}(\.\d{3})*(,\d{1,2})?|\d+(\.\d{1,2})?");
            if (m.Count > 0)
            {
                string num = m[m.Count - 1].Value;

                // normalizza: rimuovi NBSP/spazi, togli i punti migliaia se esiste anche la virgola
                string norm = num.Replace(" ", "").Replace("\u00A0", "");
                if (norm.Contains(",") && norm.Contains(".")) norm = norm.Replace(".", "");

                // IT (virgola decimale)
                if (float.TryParse(norm, NumberStyles.AllowDecimalPoint, it, out float vIt))
                    return vIt;

                // EN (punto decimale)
                if (float.TryParse(norm.Replace(",", "."), NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out float vEn))
                    return vEn;
            }
        }

        return 0f;
    }
}