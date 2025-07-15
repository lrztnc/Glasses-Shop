using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class ShopManager : MonoBehaviour
{
    [Header("Search")]
    public TMP_InputField searchInputField;

    [Header("Content Parent")]
    public Transform contentParent; 

    [Header("Cart Button")]
    public Button cartButton;

    [Header("TryOn Buttons")]
    public Button[] tryOnButtons;

    [Header("Add To Cart Buttons")]
    public Button[] addToCartButtons;

    [Header("Product Containers")]
    public GameObject[] productObjects; 

    private List<string> cartItems = new List<string>();

    void Start()
    {
        foreach (Button btn in tryOnButtons)
        {
            btn.onClick.AddListener(() =>
            {
                SceneManager.LoadScene("TryOnScene");
            });
        }

        foreach (Button btn in addToCartButtons)
        {
            btn.onClick.AddListener(() =>
            {
                AddToCart(btn.transform.parent.name);
            });
        }

        cartButton.onClick.AddListener(() =>
        {
            PlayerPrefs.SetString("CartItems", string.Join(",", cartItems));
            SceneManager.LoadScene("CartScene");
        });

        searchInputField.onValueChanged.AddListener(OnSearchChanged);
    }

    void AddToCart(string productName)
    {
        cartItems.Add(productName);
        Debug.Log(productName + " aggiunto al carrello.");
    }