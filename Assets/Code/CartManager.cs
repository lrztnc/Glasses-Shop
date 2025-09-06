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

    [Header("Popups")]
    [SerializeField] private GameObject cancelConfirmPopup;   
    [SerializeField] private Button cancelYesButton;
    [SerializeField] private Button cancelNoButton;

    [SerializeField] private GameObject confirmOrderPopup;    
    [SerializeField] private Button confirmYesButton;
    [SerializeField] private Button confirmNoButton;

    [SerializeField] private GameObject orderSentToast;           [SerializeField] private float toastDuration = 1.6f;

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

    void Start()
    {
        if (cancelConfirmPopup) cancelConfirmPopup.SetActive(false);
        if (confirmOrderPopup)  confirmOrderPopup.SetActive(false);
        if (orderSentToast)     orderSentToast.SetActive(false);

        if (cancelYesButton) cancelYesButton.onClick.AddListener(OnCancelYes);
        if (cancelNoButton)  cancelNoButton.onClick.AddListener(() =>
        {
            if (cancelConfirmPopup) cancelConfirmPopup.SetActive(false);
        });

        if (confirmYesButton) confirmYesButton.onClick.AddListener(OnConfirmYes);
        if (confirmNoButton)  confirmNoButton.onClick.AddListener(() =>
        {
            if (confirmOrderPopup) confirmOrderPopup.SetActive(false);
        });
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

                if (float.TryParse(norm, NumberStyles.AllowDecimalPoint, it, out float vIt))
                    return vIt;

                if (float.TryParse(norm.Replace(",", "."), NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out float vEn))
                    return vEn;
            }
        }
        return 0f;
    }


    public void OnCancelOrderClicked()
    {
        if (cancelConfirmPopup) cancelConfirmPopup.SetActive(true);
    }

    public void OnConfirmOrderClicked()
    {
        if (confirmOrderPopup) confirmOrderPopup.SetActive(true);
    }


    private void OnCancelYes()
    {
        if (cancelConfirmPopup) cancelConfirmPopup.SetActive(false);
        ClearCartAndZeroTotal();
    }

    private void OnConfirmYes()
    {
        if (confirmOrderPopup) confirmOrderPopup.SetActive(false);
        ClearCartAndZeroTotal();

        if (orderSentToast)
            StartCoroutine(ShowToast(orderSentToast, toastDuration));
    }


    private void ClearCartAndZeroTotal()
    {
        if (cartItems != null)
        {
            for (int i = 0; i < cartItems.Length; i++)
            {
                if (cartItems[i] != null && cartItems[i].activeSelf)
                {
                    cartItems[i].SetActive(false);
                    CartService.Remove(i);
                }
            }
        }

        if (totalValueText != null)
            totalValueText.text = 0f.ToString("N2", it) + " €";

        QueueRecalc();
    }

    private IEnumerator ShowToast(GameObject go, float seconds)
    {
        go.SetActive(true);
        yield return new WaitForSeconds(seconds);
        go.SetActive(false);
    }
}