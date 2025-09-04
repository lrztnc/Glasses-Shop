using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopManager : MonoBehaviour
{
    [Header("Search & UI")]
    public TMP_InputField searchBar;
    public RectTransform contentParent;
    public Button cartButton;

    [Header("Panels")]
    public GameObject tryOnPanel;
    public GameObject shopPanel;

    [Header("Product References")]
    public GameObject[] glassesModels;     
    public Button[] tryOnButtons;          
    public Button[] addToCartButtons;      
    public GameObject[] productObjects;

    [Header("Cart (facoltativo)")]
    public CartManager cartManager;        
    void Start()
    {
        if (tryOnPanel != null)
            tryOnPanel.SetActive(false);

        if (tryOnButtons != null)
        {
            for (int i = 0; i < tryOnButtons.Length; i++)
            {
                int index = i;
                if (tryOnButtons[i] != null)
                    tryOnButtons[i].onClick.AddListener(() => OnTryOnButtonClick(index));
            }
        }

        if (addToCartButtons != null)
        {
            for (int i = 0; i < addToCartButtons.Length; i++)
            {
                int index = i;
                if (addToCartButtons[i] != null)
                    addToCartButtons[i].onClick.AddListener(() => OnAddToCart(index));
            }
        }
    }

    private void OnAddToCart(int productIndex)
    {
        CartService.Add(productIndex);

        if (cartManager != null && cartManager.isActiveAndEnabled)
        {
            cartManager.ShowItem(productIndex);
        }

        Debug.Log($"[ShopManager] Added product #{productIndex} to cart (saved in CartService).");
    }

    public void OnTryOnButtonClick(int glassesIndex)
    {
        if (tryOnPanel != null) tryOnPanel.SetActive(true);
        if (shopPanel != null) shopPanel.SetActive(false);

        foreach (GameObject g in glassesModels)
            if (g != null) g.SetActive(false);

        if (glassesIndex >= 0 && glassesIndex < glassesModels.Length && glassesModels[glassesIndex] != null)
            glassesModels[glassesIndex].SetActive(true);
    }

    public void BackToShop()
    {
        if (shopPanel != null) shopPanel.SetActive(true);
        if (tryOnPanel != null) tryOnPanel.SetActive(false);

        foreach (GameObject g in glassesModels)
            if (g != null) g.SetActive(false);
    }
}