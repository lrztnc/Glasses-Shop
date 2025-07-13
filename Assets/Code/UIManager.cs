using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject loginPanel;
    public GameObject signupPanel;
    public GameObject shopPanel;
    public GameObject cartPanel;
    public GameObject tryOnPanel;

    void Start()
    {
        ShowLogin();
    }

    public void ShowLogin()
    {
        HideAll();
        loginPanel.SetActive(true);
    }

    public void ShowSignup()
    {
        HideAll();
        signupPanel.SetActive(true);
    }

    public void ShowShop()
    {
        HideAll();
        shopPanel.SetActive(true);
    }

    public void ShowCart()
    {
        HideAll();
        cartPanel.SetActive(true);
    }

    public void ShowTryOn()
    {
        HideAll();
        tryOnPanel.SetActive(true);
    }

    void HideAll()
    {
        loginPanel.SetActive(false);
        signupPanel.SetActive(false);
        shopPanel.SetActive(false);
        cartPanel.SetActive(false);
        tryOnPanel.SetActive(false);
    }
}