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

    void OnSearchChanged(string searchText)
    {
        searchText = searchText.Trim().ToLower();

        foreach (GameObject obj in productObjects)
        {
            obj.SetActive(false);
        }

        if (string.IsNullOrEmpty(searchText))
        {
            foreach (GameObject obj in productObjects)
            {
                obj.SetActive(true);
                obj.transform.SetSiblingIndex(obj.transform.GetSiblingIndex());
            }
            return;
        }

        List<GameObject> matches = new List<GameObject>();

        foreach (GameObject obj in productObjects)
        {
            Transform titleTransform = obj.transform.Find(obj.name + "Title");
            if (titleTransform != null)
            {
                string titleText = titleTransform.GetComponent<Text>().text.ToLower();
                if (titleText.Contains(searchText))
                {
                    matches.Add(obj);
                }
            }
        }

        int index = 0;
        foreach (GameObject match in matches)
        {
            match.SetActive(true);
            match.transform.SetSiblingIndex(index++);
        }

        foreach (GameObject obj in productObjects)
        {
            if (!matches.Contains(obj))
            {
                obj.SetActive(true);
                obj.transform.SetSiblingIndex(index++);
            }
        }
    }
}