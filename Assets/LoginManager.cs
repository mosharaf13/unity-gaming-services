using System;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Authentication.PlayerAccounts;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.UI;

public class LoginManager : MonoBehaviour
{
    [SerializeField] TMP_Text m_StatusText;
    [SerializeField] TMP_Text m_ExceptionText;
    // [SerializeField] GameObject m_SignOut;
    // [SerializeField] Toggle m_PlayerAccountSignOut;
    
    public Action PlayerSignedIn;
    
    private async void Awake()
    {
        if(UnityServices.State == ServicesInitializationState.Uninitialized)
        {
            Debug.Log("Unity Services Initializing");
            await UnityServices.InitializeAsync();
        }
    }
    public async void StartAnonymousSignIn()
    {
        await SignUpAnonymouslyAsync();
    }
    
    async Task SignUpAnonymouslyAsync()
    {
        try
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            Debug.Log("Sign in anonymously succeeded!");
    
            // Shows how to get the playerID
            Debug.Log($"PlayerID: {AuthenticationService.Instance.PlayerId}");
            PlayerSignedIn.Invoke();
        }
        catch (AuthenticationException ex)
        {
            // Compare error code to AuthenticationErrorCodes
            // Notify the player with the proper error message
            Debug.LogException(ex);
        }
        catch (RequestFailedException ex)
        {
            // Compare error code to CommonErrorCodes
            // Notify the player with the proper error message
            Debug.LogException(ex);
         }
    }
    
    /// <summary>
    /// Initialize unity services and setup event handlers.
    /// </summary>
    private async void Start()
    {
        await UnityServices.InitializeAsync();
        PlayerAccountService.Instance.SignedIn += SignInWithUnity;
    }

    /// <summary>
    /// Start the browser-based Unity Player Accounts sign-in flow. If successful this will provide
    /// a Unity Player Accounts access token, which can be used to sign in to the Unity Authentication service.
    /// </summary>
    public async void StartSignInWithUnityAsync()
    {
        if (PlayerAccountService.Instance.IsSignedIn)
        {
            SignInWithUnity();
            return;
        }

        try
        {
            await PlayerAccountService.Instance.StartSignInAsync();
        }
        catch (RequestFailedException ex)
        {
            Debug.LogException(ex);
            SetException(ex);
        }
    }

        /// <summary>
        /// Sign out from Unity Authentication service, and optionally from Unity Player Accounts as well
        /// </summary>
        // public void SignOut()
        // {
        //     AuthenticationService.Instance.SignOut();
        //
        //     if (m_PlayerAccountSignOut.isOn)
        //     {
        //         PlayerAccountService.Instance.SignOut();
        //     }
        //
        //     UpdateUI();
        // }

        public void OpenAccountPortal()
        {
            Application.OpenURL(PlayerAccountService.Instance.AccountPortalUrl);
        }

        /// <summary>
        /// Sign in to Unity Authentication using the access token from Unity Player Accounts.
        /// This will be called after the player has successfully signed in to Unity Player Accounts.
        /// </summary>
        private async void SignInWithUnity()
        {
            try
            {
                await AuthenticationService.Instance.SignInWithUnityAsync(PlayerAccountService.Instance.AccessToken);
                UpdateUI();
            }
            catch (RequestFailedException ex)
            {
                Debug.LogException(ex);
                SetException(ex);
            }
        }

        private void UpdateUI()
        {
            var statusBuilder = new StringBuilder();

            statusBuilder.AppendLine($"Player Accounts state: <b>{(PlayerAccountService.Instance.IsSignedIn ? "Signed in" : "Signed out")}</b>");
            statusBuilder.AppendLine($"Player Accounts access token: <b>{(string.IsNullOrEmpty(PlayerAccountService.Instance.AccessToken) ? "Missing" : "Exists")}</b>\n");
            statusBuilder.AppendLine($"Authentication service state: <b>{(AuthenticationService.Instance.IsSignedIn ? "Signed in" : "Signed out")}</b>");

            if (AuthenticationService.Instance.IsSignedIn)
            {
                // m_SignOut.SetActive(true);
                var externalIds = FormatExternalIds(AuthenticationService.Instance.PlayerInfo);
                statusBuilder.AppendLine($"Player ID: <b>{AuthenticationService.Instance.PlayerId}</b>");
                statusBuilder.AppendLine($"Linked external ID providers: <b>{externalIds}</b>");
            }

            m_StatusText.text = statusBuilder.ToString();
            SetException(null);
        }

        private static string FormatExternalIds(PlayerInfo playerInfo)
        {
            if (playerInfo.Identities == null)
            {
                return "None";
            }

            var sb = new StringBuilder();
            foreach (var id in playerInfo.Identities)
            {
                sb.Append(" " + id.TypeId);
            }

            return sb.ToString();
        }

        private void SetException(Exception ex)
        {
            m_ExceptionText.text = ex != null ? $"{ex.GetType().Name}: {ex.Message}" : "";
        }

}
