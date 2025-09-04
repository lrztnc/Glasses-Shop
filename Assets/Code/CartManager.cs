using System.Collections;
using System.Globalization;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CartManager : MonoBehaviour
{
    [Header("UI - voci carrello (1B, 2B, 3B, 4B, 5B)")]
    public GameObject[] cartItems;          

    [Header("Bottoni Remove (opzionali)")]
    public Button[] removeButtons;          

    [Header("Prezzi (opzionale ma consigliato)")]
    public float[] itemPrices;              

    [Header("Totale")]
    public TMP_Text totalValueText;         

    private readonly CultureInfo it = new CultureInfo("it-IT");

    private bool recalcDirty = false;
    private Coroutine pendingCoroutine = null;

    void Awake()
    {
        if (removeButtons != null)
        {
            for (int i = 0; i < removeButtons.Length; i++)
            {
                int index = i;
                if (removeButtons[i] != null)
                    removeButtons[i].onClick.AddListener(() =>
                    {
                        HideItem(index);       
                        CartService.Remove(index); 
                        QueueRecalc();
                    });
            }
        }
    }

    void OnEnable()
    {
        ApplyStateFromService();
        QueueRecalc();
    }

    private void ApplyStateFromService()
    {
        if (cartItems == null) return;
        int n = Mathf.Min(cartItems.Length, CartService.added.Length);
        for (int i = 0; i < n; i++)
        {
            if (cartItems[i] == null) continue;
            cartItems[i].SetActive(CartService.added[i]);
        }
    }

    public void ShowItem(int index)
    {
        if (!IsValid(index)) return;
        if (cartItems[index] != null)
            cartItems[index].SetActive(true);
        QueueRecalc();
    }

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

    private void QueueRecalc()
    {
        recalcDirty = true;
        if (isActiveAndEnabled)
            ScheduleNextFrameRecalc();
    }

    private void ScheduleNextFrameRecalc()
    {
        if (pendingCoroutine != null)
            StopCoroutine(pendingCoroutine);
        pendingCoroutine = StartCoroutine(UpdateTotalNextFrame());
    }

    private IEnumerator UpdateTotalNextFrame()
    {
        yield return null;
        yield return new WaitForEndOfFrame();
        UpdateTotal();
        recalcDirty = false;
        pendingCoroutine = null;
    }

    public void UpdateTotal()
    {
        float total = 0f;

        if (cartItems != null)
        {
            for (int i = 0; i < cartItems.Length; i++)
            {
                var go = cartItems[i];
                if (go == null) continue;
                if (go.activeInHierarchy)              
                    total += GetItemPrice(i, go);
            }
        }

        if (totalValueText != null)
            totalValueText.text = total.ToString("N2", it) + " €";
    }

    private float GetItemPrice(int index, GameObject itemGO)
    {
        if (itemPrices != null && index >= 0 && index < itemPrices.Length && itemPrices[index] > 0f)
            return itemPrices[index];

        var texts = itemGO.GetComponentsInChildren<TMP_Text>(true);
        foreach (var t in texts)
        {
            if (string.IsNullOrWhiteSpace(t.text)) continue;

            var m = Regex.Matches(t.text, @"\d{1,3}(\.\d{3})*(,\d{1,2})?|\d+(\.\d{1,2})?");
            if (m.Count > 0)
            {
                string num = m[m.Count - 1].Value;

                string norm = num.Replace(" ", "").Replace("\u00A0", "");
                if (norm.Contains(",") && norm.Contains(".")) norm = norm.Replace(".", "");

                if (float.TryParse(norm, System.Globalization.NumberStyles.AllowDecimalPoint, it, out float vIt))
                    return vIt;

                if (float.TryParse(norm.Replace(",", "."), System.Globalization.NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out float vEn))
                    return vEn;
            }
        }
        return 0f;
    }
}