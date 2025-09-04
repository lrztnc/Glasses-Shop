using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Linq;

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

        List<UserData> users = LoadUsers();

        UserData matchedUser = users.FirstOrDefault(
            u => u.Email == enteredEmail && u.Password == enteredPassword
        );

        if (matchedUser != null)
        {
            SessionManager.CurrentUser = matchedUser;

            Debug.Log("Login avvenuto con successo!");

#if UNITY_2023_1_OR_NEWER
            FindFirstObjectByType<UIManager>().ShowShop();
#else
            FindObjectOfType<UIManager>().ShowShop();
#endif
        }
        else
        {
            if (loginErrorMessage != null)
                loginErrorMessage.SetActive(true);
        }
    }

    private List<UserData> LoadUsers()
    {
        string json = PlayerPrefs.GetString("users", "");
        if (string.IsNullOrEmpty(json)) return new List<UserData>();
        return JsonUtility.FromJson<UserListWrapper>(json).users;
    }
}