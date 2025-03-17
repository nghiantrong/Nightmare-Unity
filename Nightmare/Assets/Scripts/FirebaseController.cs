using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FirebaseController : MonoBehaviour
{
    public GameObject loginPanel, signUpPanel, profilePanel, forgetPasswordPanel, 
        notificationPanel;
    public TMP_InputField loginEmail, loginPassword, 
        signUpEmail, signUpPassword, signUpCPassword, signUpUsername, 
        forgetPasswordEmail;

    public TextMeshProUGUI notificationTileText, notificationMessageText, 
        profileUsername, profileEmail;

    public Toggle rememberMeToggle;

    public void OpenLoginPanel()
    {
        loginPanel.SetActive(true);
        signUpPanel.SetActive(false);
        profilePanel.SetActive(false);
        forgetPasswordPanel.SetActive(false);
    }

    public void OpenSignUpPanel()
    {
        loginPanel.SetActive(false);
        signUpPanel.SetActive(true);
        profilePanel.SetActive(false);
        forgetPasswordPanel.SetActive(false);

    }

    public void OpenProfilePanel()
    {
        loginPanel.SetActive(false);
        signUpPanel.SetActive(false);
        profilePanel.SetActive(true);
        forgetPasswordPanel.SetActive(false);
    }

    public void OpenForgetPasswordPanel()
    {
        loginPanel.SetActive(false);
        signUpPanel.SetActive(false);
        profilePanel.SetActive(false);
        forgetPasswordPanel.SetActive(true);
    }

    public void LoginUser()
    {
        if (string.IsNullOrEmpty(loginEmail.text) && 
            string.IsNullOrEmpty(loginPassword.text))
        {
            ShowNotificationMessage("Error", "All fields are required");
            return;
        }


    }

    public void LogoutUser()
    {
        profileEmail.text = "";
        profileUsername.text = "";
        OpenLoginPanel();

    }

    public void SignUpUser()
    {
        if (string.IsNullOrEmpty(signUpEmail.text) && 
            string.IsNullOrEmpty(signUpPassword.text) && 
            string.IsNullOrEmpty(signUpCPassword.text) && 
            string.IsNullOrEmpty(signUpUsername.text))
        {
            ShowNotificationMessage("Error", "All fields are required");
            return;
        }

        if (signUpPassword.text != signUpCPassword.text)
        {
            ShowNotificationMessage("Error", "Password and Confirm Password does not match");
            return;
        }
    }

    public void ForgetPassword()
    {
        if (string.IsNullOrEmpty(forgetPasswordEmail.text))
        {
            ShowNotificationMessage("Error", "Email is required");
            return;
        }
    }

    private void ShowNotificationMessage(string title, string message)
    {
        notificationPanel.SetActive(true);
        notificationTileText.text = title;
        notificationMessageText.text = message;
    }

    public void CloseNotificationPanel()
    {
        notificationPanel.SetActive(false);
        notificationTileText.text = "";
        notificationMessageText.text = "";
    }
}
