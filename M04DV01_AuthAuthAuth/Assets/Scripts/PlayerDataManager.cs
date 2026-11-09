using UnityEngine;
using System.Linq;
using Unity.Services.CloudCode;
using Unity.Services.CloudCode.GeneratedBindings;

public class PlayerDataManager : MonoBehaviour
{
    private MyModuleBindings m_Bindings;
    public LoginManager LoginManager;
    public string PlayerName;

    void Start()
    {
        LoginManager.PlayerSignedIn += InitializePlayer;
        m_Bindings = new MyModuleBindings(CloudCodeService.Instance);
    }

    private async void InitializePlayer()
    {
        try
        {
            var resultFromCloud = await m_Bindings.SayHello(PlayerName);
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
            PlayerName = await m_Bindings.HandleNewPlayerNameEntry(PlayerName);
            Debug.Log($"Saved new player name in the cloud: {PlayerName}");
        }
        catch(CloudCodeException ex)
        {
            Debug.LogWarning("Player name invalid");
            Debug.LogException(ex);
        }
    }

    private bool IsPlayerNameValid(string name)
    {
        if (name.Length is < 4 or > 16)
            return false;

        if (!name.All(c => char.IsLetterOrDigit(c)))
            return false;

        return true;
    }
}
