using UnityEngine;
using TMPro;

public class SignupManager : MonoBehaviour
{
    public TMP_InputField nameField;
    public TMP_InputField surnameField;
    public TMP_InputField emailField;
    public TMP_InputField passwordField;
    public GameObject signupErrorMessage;

    void OnEnable()
    {
        if (signupErrorMessage != null)
            signupErrorMessage.SetActive(false);
    }

    void Start()
    {
        // Configura il campo password per mostrare asterischi
        if (passwordField != null)
        {
            passwordField.contentType = TMP_InputField.ContentType.Password;
        }
    }

    public void OnSignupButtonPressed()
    {
        string name = nameField.text.Trim();
        string surname = surnameField.text.Trim();
        string email = emailField.text.Trim();
        string password = passwordField.text.Trim();

        if (string.IsNullOrEmpty(name) ||
            string.IsNullOrEmpty(surname) ||
            string.IsNullOrEmpty(email) ||
            string.IsNullOrEmpty(password))
        {
            if (signupErrorMessage != null)
                signupErrorMessage.SetActive(true);
            return;
        }

        string savedEmail = PlayerPrefs.GetString("email", "");
        if (savedEmail == email)
        {
            if (signupErrorMessage != null)
                signupErrorMessage.SetActive(true);
            return;
        }

        PlayerPrefs.SetString("email", email);
        PlayerPrefs.SetString("password", password);
        PlayerPrefs.Save();

        Debug.Log("Registrazione completata!");
        FindFirstObjectByType<UIManager>().ShowLogin();
    }
}