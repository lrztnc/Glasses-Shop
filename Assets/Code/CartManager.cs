using UnityEngine;
using UnityEngine.UI;

public class CartManager : MonoBehaviour
{
    [Header("UI - voci carrello (1B, 2B, 3B, 4B, 5B)")]
    [Tooltip("Trascina qui le voci del carrello nell'ordine dei prodotti: 1B, 2B, 3B, 4B, 5B")]
    public GameObject[] cartItems;   // 1B..5B

    [Header("Bottoni Remove (opzionali, se già dentro gli item)")]
    public Button[] removeButtons;   // pulsanti "Remove item", stesso ordine di cartItems

    void Awake()
    {
        HideAll();

        // Collega i pulsanti "Remove"
        if (removeButtons != null)
        {
            for (int i = 0; i < removeButtons.Length; i++)
            {
                int index = i; // cattura corretta del valore
                if (removeButtons[i] != null)
                    removeButtons[i].onClick.AddListener(() => HideItem(index));
            }
        }
    }

    public void HideAll()
    {
        if (cartItems == null) return;
        foreach (var go in cartItems)
            if (go != null) go.SetActive(false);
    }

    /// <summary>
    /// Mostra la voce del carrello corrispondente all’indice prodotto
    /// </summary>
    public void ShowItem(int index)
    {
        if (cartItems == null || index < 0 || index >= cartItems.Length) return;
        if (cartItems[index] != null)
            cartItems[index].SetActive(true);
    }

    /// <summary>
    /// Nasconde (rimuove) una singola voce dal carrello
    /// </summary>
    public void HideItem(int index)
    {
        if (cartItems == null || index < 0 || index >= cartItems.Length) return;
        if (cartItems[index] != null)
            cartItems[index].SetActive(false);
    }
}