using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UILogin : MonoBehaviour
{
    [SerializeField] private Button loginButton;

    [SerializeField] private TMP_Text userIdText;
    [SerializeField] private TMP_Text userNameText;

    [SerializeField] private Transform loginPanel, userPanel;

    [SerializeField] private LoginController loginController;

    private PlayerProfile playerProfile;

    private void OnEnable()
    {
        loginButton.onClick.AddListener(LoginButtonPressed);
        loginController.OnSignedIn += LoginController_OnSignedIn;
        loginController.OnAvatarUpdate += LoginController_OnAvatarUpdate;
        loginController.OnSignedOut += LoginController_OnSignedOut;
    }

    private void OnDisable()
    {
        loginButton.onClick.RemoveListener(LoginButtonPressed);
        loginController.OnSignedIn -= LoginController_OnSignedIn;
        loginController.OnAvatarUpdate -= LoginController_OnAvatarUpdate;
        loginController.OnSignedOut -= LoginController_OnSignedOut;
    }

    private async void LoginButtonPressed()
    {
        await loginController.InitSignIn();
    }

    private void LoginController_OnSignedIn(PlayerProfile profile)
    {
        playerProfile = profile;
        loginPanel.gameObject.SetActive(false);
        userPanel.gameObject.SetActive(true);
       
        userIdText.text = $"id_{playerProfile.playerInfo.Id}";
        userNameText.text = profile.Name;
    }
    private void LoginController_OnSignedOut()
    {
        loginPanel.gameObject.SetActive(true);
        userPanel.gameObject.SetActive(false);

        userIdText.text = string.Empty;
        userNameText.text = string.Empty;
    }
    private void LoginController_OnAvatarUpdate(PlayerProfile profile)
    {
        playerProfile = profile;
    }

   
}