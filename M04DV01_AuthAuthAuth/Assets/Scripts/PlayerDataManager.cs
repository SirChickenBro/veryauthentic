using UnityEngine;
using Unity.Services.CloudCode;
using Unity.Services.CloudCode.GeneratedBindings;

public class PlayerDataManager : MonoBehaviour
{
    public UGSTutorialCloudBindings UGSTutorialBindings;
    public LoginManager LoginManager;
    public string PlayerName;

    void Start()
    {
        LoginManager.PlayerSignedIn += InitializePlayer;
        UGSTutorialBindings = new UGSTutorialCloudBindings(CloudCodeService.Instance);
    }

    private async void InitializePlayer()
    {
        try
        {
            var resultFromCloud = await UGSTutorialBindings.SayHello(PlayerName);
            Debug.Log($"{resultFromCloud}");
        }
        catch(CloudCodeException ex)
        {
            Debug.LogException(ex);
        }
    }

    private void OnDisable()
    {
        LoginManager.PlayerSignedIn -= InitializePlayer;
    }

    public async void SaveNewPlayerName()
    {
        try
        {
            //PlayerName = await UGSTutorialBindings.Han
            Debug.Log($"Saved new player name in the cloud: {PlayerName}");
        }
        catch(Exception ex)
        {
            Debug.LogException(ex);
        }
    }
}
