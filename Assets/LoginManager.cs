using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

public class LoginManager : MonoBehaviour
{
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

}
