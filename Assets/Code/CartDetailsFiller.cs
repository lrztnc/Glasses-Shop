using UnityEngine;
using TMPro;

public class CartDetailsFiller : MonoBehaviour
{
    [Header("Campi testo")]
    public TMP_Text nameField;
    public TMP_Text surnameField;
    public TMP_Text emailField;
    public TMP_Text storeNumberField;

    private void OnEnable()
    {
        FillDetails();
    }

    private void FillDetails()
    {
        if (SessionManager.CurrentUser == null)
        {
            Debug.LogWarning("Nessun utente loggato trovato.");
            return;
        }

        if (nameField != null)
            nameField.text = SessionManager.CurrentUser.Name;

        if (surnameField != null)
            surnameField.text = SessionManager.CurrentUser.Surname;

        if (emailField != null)
            emailField.text = SessionManager.CurrentUser.Email;

        if (storeNumberField != null)
            storeNumberField.text = "632"; // valore fisso
    }
}