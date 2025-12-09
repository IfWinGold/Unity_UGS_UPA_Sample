using System;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Authentication.PlayerAccounts;
using Unity.Services.Core;
using UnityEngine;

public class LoginController : MonoBehaviour
{
    public event Action<PlayerProfile> OnSignedIn;
    public event Action OnSignedOut;
    public event Action<PlayerProfile> OnAvatarUpdate;

    private PlayerInfo playerInfo;

    private PlayerProfile playerProfile;
    public PlayerProfile PlayerProfile => playerProfile;

    private async void Awake()
    {
        await UnityServices.InitializeAsync();
        RegisterEvents();
        PlayerAccountService.Instance.SignedIn += SignedIn;
    
        // 자동 로그인 시도
        await SignInCachedUserAsync();
    }

    private async void SignedIn()
    {
        try
        {
            var accessToken = PlayerAccountService.Instance.AccessToken;
            await SignInWithUnityAsync(accessToken);

        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
        }
    }

    public async Task InitSignIn()
    {
        await PlayerAccountService.Instance.StartSignInAsync();
    }

    private async Task SignInWithUnityAsync(string accessToken)
    {
        try
        {
            await AuthenticationService.Instance.SignInWithUnityAsync(accessToken);
            Debug.Log("SignIn is successful.");
            await LoadPlayerProfile();
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }
    }


    private void OnDestroy()
    {
        PlayerAccountService.Instance.SignedIn -= SignedIn;
    }
    

    
    private async Task LoadPlayerProfile()
    {
        playerInfo = AuthenticationService.Instance.PlayerInfo;
        var name = await AuthenticationService.Instance.GetPlayerNameAsync();

        playerProfile = new PlayerProfile
        {
            playerInfo = playerInfo,
            Name = name
        };

        OnSignedIn?.Invoke(playerProfile);
    }
    
    
    async Task SignInCachedUserAsync()
    {
        // 세션 토큰이 존재하는지 확인하여 캐시된 플레이어가 이미 존재하는지 확인합니다
        if (!AuthenticationService.Instance.SessionTokenExists)
        {
            // 그렇지 않으면 아무것도 하지 않습니다
            return;
        }

        // 익명으로 로그인
        // 이 호출은 캐시된 플레이어에 서명할 것입니다.
        try
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            Debug.Log("Sign in anonymously succeeded!");

            // Shows how to get the playerID
            Debug.Log($"PlayerID: {AuthenticationService.Instance.PlayerId}");

            LoadPlayerProfile();
        }
        catch (AuthenticationException ex)
        {
            // 오류 코드와 인증 오류 코드 비교
            // 플레이어에게 올바른 오류 메시지 알림
            Debug.LogException(ex);
        }
        catch (RequestFailedException ex)
        {
            // 오류 코드를 CommonErrorCodes와 비교하기
            // 플레이어에게 올바른 오류 메시지 알림
            Debug.LogException(ex);
        }
    }

    
    #region Clear Session Token
    void SimpleSignOut()
    {
        // The session token will remain but the player will not be authenticated
        AuthenticationService.Instance.SignOut();
    }

    public void SignOutAndClearSession()
    {
        // The session token will be deleted immediately, allowing for a new anonymous player to be created
        AuthenticationService.Instance.SignOut(true);
    }

    void SignOutThenClearSession()
    {
        AuthenticationService.Instance.SignOut();
        // Do something else...

        // Now clear the session token to allow a new anonymous player to be created
        AuthenticationService.Instance.ClearSessionToken();
    }
    #endregion
    
    
    void RegisterEvents()
    {
        AuthenticationService.Instance.SignedIn += () =>
        {
            //SignedIn();
            Debug.Log($"The player has successfully signed in");
        };

        AuthenticationService.Instance.Expired += () =>
        {
            Debug.Log($"The access token was not refreshed and has expired");
        };

        AuthenticationService.Instance.SignedOut += () =>
        {
            Debug.Log($"The player has successfully signed out");
            PlayerAccountService.Instance.SignOut();
            OnSignedOut?.Invoke();
        };
        
        
    }
}


[Serializable]
public struct PlayerProfile
{
    public PlayerInfo playerInfo;
    public string Name;
}