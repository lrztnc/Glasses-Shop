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
    public Button[] addToCartButtons;
    public GameObject[] productObjects;

    void Start()
    {
        // Nasconde TryOnPanel all'avvio
        if (tryOnPanel != null)
            tryOnPanel.SetActive(false);

        // Registra i listener dei bottoni Try On
        for (int i = 0; i < tryOnButtons.Length; i++)
        {
            int index = i; // Necessario per catturare il valore corretto nel loop
            tryOnButtons[i].onClick.AddListener(() => OnTryOnButtonClick(index));
        }
    }

    public void OnTryOnButtonClick(int glassesIndex)
    {
        // Mostra il pannello Try-On
        if (tryOnPanel != null)
            tryOnPanel.SetActive(true);

        // Nasconde lo shop
        if (shopPanel != null)
            shopPanel.SetActive(false);

        // Disattiva tutti gli occhiali
        foreach (GameObject g in glassesModels)
        {
            if (g != null)
                g.SetActive(false);
        }

        // Attiva il modello selezionato
        if (glassesIndex >= 0 && glassesIndex < glassesModels.Length && glassesModels[glassesIndex] != null)
        {
            glassesModels[glassesIndex].SetActive(true);
        }
    }

    public void BackToShop()
    {
        // Mostra lo shop
        if (shopPanel != null)
            shopPanel.SetActive(true);

        // Nasconde il try-on
        if (tryOnPanel != null)
            tryOnPanel.SetActive(false);

        // Disattiva tutti gli occhiali
        foreach (GameObject g in glassesModels)
        {
            if (g != null)
                g.SetActive(false);
        }
    }
}