using System.Threading.Tasks;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Authentication.PlayerAccounts;

public class LoginManager : MonoBehaviour
{

    private async void Awake()
    {
        if(UnityServices.State == ServicesInitializationState.Uninitialized)
        {
            Debug.Log("Services Initializing");
            await UnityServices.InitializeAsync();
        }

        PlayerAccountService.Instance.SignedIn += SignInOrLinkWithUnity;
    }

    async void Start()
    {
        if(!AuthenticationService.Instance.SessionTokenExists)
        {
            Debug.Log("Session token not found");
            return;
        }

        Debug.Log("Returning player signing in...");
        await SignInAnonymouslyAsync();
    }
    public async void StartAnonymousSignIn()
    {
        await SignInAnonymouslyAsync();
    }

    private async Task SignInAnonymouslyAsync()
    {
        try
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            Debug.Log("Sign in anonymously succeeded!");

            Debug.Log($"PlayerID: { AuthenticationService.Instance.PlayerId}");
        }
        catch(AuthenticationException ex)
        {
            Debug.LogException(ex);
        }
        catch(RequestFailedException ex)
        {
            Debug.LogException(ex);
        }
    }

    public async void StartUnitySignInAsync()
    {
        if (PlayerAccountService.Instance.IsSignedIn)
        {
            SignInOrLinkWithUnity();
            return;
        }

        try
        {
            await PlayerAccountService.Instance.StartSignInAsync();
        }
        catch (RequestFailedException ex)
        {
            Debug.LogException(ex);
        }
    }

    private async void SignInOrLinkWithUnity()
    {
        try
        {
            Debug.Log("Signing up with Unity Player Account...");
            if (!AuthenticationService.Instance.IsSignedIn)
            {
                await AuthenticationService.Instance.SignInWithUnityAsync(PlayerAccountService.Instance.AccessToken);
                Debug.Log("Succesfully signed up with Unity Player Account");
                return;
            }

            if(!HasUnityID())
            {
                Debug.Log("Linking anonymous account to Unity...");
                await LinkWithUnityAsync(PlayerAccountService.Instance.AccessToken);
                Debug.Log("Successfully linked anonymous account!");
                return;
            }

            Debug.Log("Player is already signed into their Unity Player Account");
        }

        catch (RequestFailedException ex)
        {
            Debug.LogException(ex);
        }
    }

    async Task LinkWithUnityAsync(string accessToken)
    {
        try
        {
            await AuthenticationService.Instance.LinkWithUnityAsync(accessToken);
            Debug.Log("Link is successful.");
        }
        catch(AuthenticationException ex) when (ex.ErrorCode == AuthenticationErrorCodes.AccountAlreadyLinked)
        {
            Debug.LogError("This user is already linked with another account. Log in instead.");
        }
        catch(AuthenticationException ex)
        {
            Debug.LogException(ex);
        }
        catch(RequestFailedException ex)
        {
            Debug.LogException(ex);
        }
    }

    private bool HasUnityID()
    {
        return AuthenticationService.Instance.PlayerInfo.GetUnityId() != null;
    }
}
