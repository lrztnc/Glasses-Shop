using UnityEngine;
using TMPro;

public class LoginManager : MonoBehaviour
{
    public TMP_InputField loginEmailField;
    public TMP_InputField loginPasswordField;
    public GameObject loginErrorMessage;

    void OnEnable()
    {
        if (loginErrorMessage != null)
            loginErrorMessage.SetActive(false);

        if (loginPasswordField != null)
        {
            loginPasswordField.contentType = TMP_InputField.ContentType.Password;
            loginPasswordField.ForceLabelUpdate();
        }
    }

    public void OnLoginButtonPressed()
    {
        string enteredEmail = loginEmailField.text.Trim();
        string enteredPassword = loginPasswordField.text.Trim();

        string savedEmail = PlayerPrefs.GetString("email", "");
        string savedPassword = PlayerPrefs.GetString("password", "");

        if (enteredEmail == savedEmail && enteredPassword == savedPassword)
        {
            Debug.Log("Login avvenuto con successo!");
            FindFirstObjectByType<UIManager>().ShowShop();
        }
        else
        {
            if (loginErrorMessage != null)
                loginErrorMessage.SetActive(true);
        }
    }
}