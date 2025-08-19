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
    public GameObject[] glassesModels;     // Prefab occhiali nel TryOnPanel
    public Button[] tryOnButtons;          // Bottoni "Try On"
    public Button[] addToCartButtons;      // <-- Assicurati sia popolato in Inspector
    public GameObject[] productObjects;

    [Header("Cart")]
    public CartManager cartManager;         // <-- Riferimento al CartManager sul CartPanel

    void Start()
    {
        // Nasconde TryOnPanel all'avvio
        if (tryOnPanel != null)
            tryOnPanel.SetActive(false);

        // Listener bottoni Try On
        for (int i = 0; i < tryOnButtons.Length; i++)
        {
            int index = i;
            tryOnButtons[i].onClick.AddListener(() => OnTryOnButtonClick(index));
        }

        // >>> AGGIUNTA: Listener bottoni Add To Cart <<<
        if (addToCartButtons != null)
        {
            for (int i = 0; i < addToCartButtons.Length; i++)
            {
                int index = i; // cattura correttamente l'indice
                if (addToCartButtons[i] != null)
                    addToCartButtons[i].onClick.AddListener(() => OnAddToCart(index));
            }
        }
        // <<< FINE AGGIUNTA
    }

    // >>> AGGIUNTA: funzione Add To Cart <<<
    private void OnAddToCart(int productIndex)
    {
        if (cartManager != null)
        {
            // Mappa 0->1B, 1->2B, ... (ordina gli array di bottoni e cartItems nello stesso ordine)
            cartManager.ShowItem(productIndex);
        }
        else
        {
            Debug.LogWarning("CartManager non assegnato nello ShopManager.");
        }
    }
    // <<< FINE AGGIUNTA

    public void OnTryOnButtonClick(int glassesIndex)
    {
        if (tryOnPanel != null)
            tryOnPanel.SetActive(true);
        if (shopPanel != null)
            shopPanel.SetActive(false);

        foreach (GameObject g in glassesModels)
            if (g != null) g.SetActive(false);

        if (glassesIndex >= 0 && glassesIndex < glassesModels.Length && glassesModels[glassesIndex] != null)
            glassesModels[glassesIndex].SetActive(true);
    }

    public void BackToShop()
    {
        if (shopPanel != null)
            shopPanel.SetActive(true);
        if (tryOnPanel != null)
            tryOnPanel.SetActive(false);
        foreach (GameObject g in glassesModels)
            if (g != null) g.SetActive(false);
    }
}