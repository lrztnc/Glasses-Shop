using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

[System.Serializable]
public class Product
{
    public string id;
    public string name;
    public string description;
    public float price;
    public string color;
    public string imageName;
}

public class ShopManager : MonoBehaviour
{
    [Header("Prefabs & UI")]
    public GameObject productPrefab;
    public Transform contentPanel;
    public TMP_Dropdown colorDropdown;
    public TMP_Dropdown priceDropdown;

    [Header("Panels")]
    public GameObject tryOnPanel;
    public GameObject shopPanel;

    [Header("3D Models")]
    public GameObject[] glassesModels;

    [Header("Cart")]
    public CartManager cartManager;

    private List<GameObject> spawnedProducts = new List<GameObject>();
    private List<Product> allProducts = new List<Product>();

    void Start()
    {
        LoadProductsFromJson();

        // Assegna listener ai dropdown
        if (colorDropdown != null)
            colorDropdown.onValueChanged.AddListener(delegate { ApplyCombinedFilter(); });

        if (priceDropdown != null)
            priceDropdown.onValueChanged.AddListener(delegate { ApplyCombinedFilter(); });

        ApplyCombinedFilter();
    }

    void LoadProductsFromJson()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("products");
        if (jsonFile == null)
        {
            Debug.LogError("products.json non trovato in Resources!");
            return;
        }

        Product[] products = JsonHelper.FromJson<Product>(jsonFile.text);
        allProducts = new List<Product>(products);

        // Pulisci i vecchi oggetti
        foreach (Transform child in contentPanel)
        {
            Destroy(child.gameObject);
        }
        spawnedProducts.Clear();

        for (int i = 0; i < allProducts.Count; i++)
        {
            Product p = allProducts[i];

            GameObject item = Instantiate(productPrefab, contentPanel);
            item.name = p.id;

            // Testi
            item.transform.Find("Title").GetComponent<TMP_Text>().text = p.name;
            item.transform.Find("Description").GetComponent<TMP_Text>().text = p.description;
            item.transform.Find("Price").GetComponent<TMP_Text>().text = p.price.ToString("F2") + " €";

            // Immagine
            Image img = item.transform.Find("Image").GetComponent<Image>();
            Sprite sprite = Resources.Load<Sprite>(p.imageName);
            if (sprite != null) img.sprite = sprite;

            // Aggiunta alla lista prima di registrare l’indice
            spawnedProducts.Add(item);
            int correctIndex = spawnedProducts.Count - 1;

            // Listener bottoni
            item.transform.Find("TryOnButton").GetComponent<Button>().onClick.AddListener(() => OnTryOnButtonClick(correctIndex));
            item.transform.Find("AddToCartButton").GetComponent<Button>().onClick.AddListener(() => OnAddToCart(correctIndex));
        }
    }

    public void ApplyCombinedFilter()
    {
        if (colorDropdown == null || priceDropdown == null) return;

        string selectedColor = colorDropdown.options[colorDropdown.value].text;
        string selectedPrice = priceDropdown.options[priceDropdown.value].text;

        List<(Product, GameObject)> filtered = new List<(Product, GameObject)>();

        for (int i = 0; i < allProducts.Count; i++)
        {
            Product p = allProducts[i];
            GameObject go = spawnedProducts[i];

            bool colorMatch = (selectedColor == "All" || p.color == selectedColor);
            bool priceMatch = selectedPrice switch
            {
                "Low to high" => true,
                "High to low" => true,
                "Above 110,00 €" => p.price > 110f,
                "Below 110,00 €" => p.price <= 110f,
                _ => true
            };

            go.SetActive(colorMatch && priceMatch);

            if (go.activeSelf)
                filtered.Add((p, go));
        }

        // Ordinamento
        if (selectedPrice == "Low to high")
            filtered.Sort((a, b) => a.Item1.price.CompareTo(b.Item1.price));
        else if (selectedPrice == "High to low")
            filtered.Sort((a, b) => b.Item1.price.CompareTo(a.Item1.price));

        // Riordina visivamente nella scroll view
        for (int i = 0; i < filtered.Count; i++)
        {
            filtered[i].Item2.transform.SetSiblingIndex(i);
        }
    }

    void OnTryOnButtonClick(int index)
    {
        if (tryOnPanel != null) tryOnPanel.SetActive(true);
        if (shopPanel != null) shopPanel.SetActive(false);

        foreach (var g in glassesModels)
            g.SetActive(false);

        if (index >= 0 && index < glassesModels.Length)
            glassesModels[index].SetActive(true);
    }

    void OnAddToCart(int index)
    {
        if (cartManager != null)
            cartManager.ShowItem(index);
    }
}