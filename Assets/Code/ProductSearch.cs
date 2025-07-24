using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ProductSearch : MonoBehaviour
{
    public InputField searchInput; // o TMP_InputField se usi TextMeshPro
    public Transform productContainer; // Il contenitore padre dei prodotti (es: Content panel)

    private List<GameObject> productItems = new List<GameObject>();

    void Start()
    {
        // Seleziona tutti i figli del contenitore che hanno ProductUI
        foreach (Transform child in productContainer)
        {
            if (child.GetComponent<ProductUI>() != null)
            {
                productItems.Add(child.gameObject);
            }
        }

        searchInput.onValueChanged.AddListener(OnSearchValueChanged);
    }

    void OnSearchValueChanged(string input)
    {
        string query = input.ToLower();

        foreach (GameObject item in productItems)
        {
            ProductUI product = item.GetComponent<ProductUI>();
            if (product == null) continue;

            string title = product.titleText.text.ToLower();
            string description = product.descriptionText.text.ToLower();
            string price = product.priceText.text.ToLower();

            bool match = title.Contains(query) || description.Contains(query) || price.Contains(query);
            item.SetActive(match);
        }
    }
}