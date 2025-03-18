using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Auth;
using Firebase.Extensions;
using System;
using System.Threading.Tasks;

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

    Firebase.Auth.FirebaseAuth auth;
    Firebase.Auth.FirebaseUser user;

    bool isSignIn = false;
    bool isSigned = false;

    void Update()
    {
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                // Create and hold a reference to your FirebaseApp,
                // where app is a Firebase.FirebaseApp property of your application class.
                InitializeFirebase();

                // Set a flag here to indicate whether Firebase is ready to use by your app.
            }
            else
            {
                UnityEngine.Debug.LogError(System.String.Format(
                  "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                // Firebase Unity SDK is not safe to use here.
            }
        });

        if (isSignIn && !isSigned)
        {
            isSigned = true;
            profileEmail.text = user.Email;
            profileUsername.text = user.DisplayName;

            OpenProfilePanel();
        }
    }

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

        SignInUser(loginEmail.text, loginPassword.text);
    }

    public void LogoutUser()
    {
        auth.SignOut();
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

        CreateUser(signUpEmail.text, signUpPassword.text, signUpUsername.text);
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

    void CreateUser(string email, string password, string username)
    {
        auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                return;
            }

            // Firebase user has been created.
            Firebase.Auth.AuthResult result = task.Result;
            Debug.LogFormat("Firebase user created successfully: {0} ({1})",
                result.User.DisplayName, result.User.UserId);

            UpdateUserProfile(username);

        });
    }

    public void SignInUser(string email, string password)
    {
        auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("SignInWithEmailAndPasswordAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("SignInWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                return;
            }

            Firebase.Auth.AuthResult result = task.Result;
            Debug.LogFormat("User signed in successfully: {0} ({1})",
                result.User.DisplayName, result.User.UserId);

            profileEmail.text = result.User.Email;
            profileUsername.text = result.User.DisplayName;

            OpenProfilePanel();
        });
    }

    void InitializeFirebase()
    {
        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        auth.StateChanged += AuthStateChanged;
        AuthStateChanged(this, null);
    }

    void AuthStateChanged(object sender, System.EventArgs eventArgs)
    {
        if (auth.CurrentUser != user)
        {
            bool signedIn = user != auth.CurrentUser && auth.CurrentUser != null
                && auth.CurrentUser.IsValid();
            if (!signedIn && user != null)
            {
                Debug.Log("Signed out " + user.UserId);
            }
            user = auth.CurrentUser;
            if (signedIn)
            {
                Debug.Log("Signed in " + user.UserId);
                isSignIn = true;
            }
        }
    }

    void OnDestroy()
    {
        auth.StateChanged -= AuthStateChanged;
        auth = null;
    }

    void UpdateUserProfile(string username)
    {
        Firebase.Auth.FirebaseUser user = auth.CurrentUser;
        if (user != null)
        {
            Firebase.Auth.UserProfile profile = new Firebase.Auth.UserProfile
            {
                DisplayName = username,
                PhotoUrl = new System.Uri("https://dummyimage.com/250/ffffff/000000"),
            };
            user.UpdateUserProfileAsync(profile).ContinueWith(task =>
            {
                if (task.IsCanceled)
                {
                    Debug.LogError("UpdateUserProfileAsync was canceled.");
                    return;
                }
                if (task.IsFaulted)
                {
                    Debug.LogError("UpdateUserProfileAsync encountered an error: " + task.Exception);
                    return;
                }

                Debug.Log("User profile updated successfully.");

                ShowNotificationMessage("Success", "User created successfully");
            });
        }
    }
}
