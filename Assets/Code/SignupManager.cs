using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Linq;

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
        if (passwordField != null)
        {
            passwordField.contentType = TMP_InputField.ContentType.Password;
            passwordField.ForceLabelUpdate();
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

        List<UserData> users = LoadUsers();
        if (users.Any(u => u.Email == email))
        {
            if (signupErrorMessage != null)
                signupErrorMessage.SetActive(true);
            return;
        }

        users.Add(new UserData(name, surname, email, password));
        PlayerPrefs.SetString("users", JsonUtility.ToJson(new UserListWrapper(users)));
        PlayerPrefs.Save();

        Debug.Log("Registrazione completata!");
        FindFirstObjectByType<UIManager>().ShowLogin();
    }

    private List<UserData> LoadUsers()
    {
        string json = PlayerPrefs.GetString("users", "");
        if (string.IsNullOrEmpty(json)) return new List<UserData>();
        return JsonUtility.FromJson<UserListWrapper>(json).users;
    }
}