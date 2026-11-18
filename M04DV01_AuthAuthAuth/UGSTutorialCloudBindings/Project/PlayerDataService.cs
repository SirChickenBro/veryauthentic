using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Unity.Services.CloudCode.Apis;
using Unity.Services.CloudCode.Core;
using Unity.Services.CloudCode.Shared;
using Unity.Services.CloudSave.Model;

namespace UnityGamingServicesCloud;

public class PlayerDataService
{
    readonly IGameApiClient m_GameApiClient;
    readonly ILogger<PlayerDataService> m_Logger;
    private readonly ILogger<PlayerDataService> _logger;
    
    public const string k_PlayerNameKey = "PLAYER_NAME";
    
    public PlayerDataService(IGameApiClient gameApiClient, ILogger<PlayerDataService> logger)
    {
        m_GameApiClient = gameApiClient;
        m_Logger = logger;
    }

    
    private async Task SavePlayerData(IExecutionContext context, IGameApiClient gameApiClient, string key, string value)
    {
        try
        {
            await gameApiClient.CloudSaveData.SetItemAsync(
                context, 
                context.AccessToken!, 
                context.ProjectId!,
                context.PlayerId!, 
                new SetItemBody(key, value));
                
            _logger.LogInformation("Successfully saved data for key: {Key}", key);
        }
        catch (ApiException ex)
        {
            _logger.LogError("Failed to save data for key {Key}. Error: {Error}", key, ex.Message);
            throw new Exception($"Unable to save player data: {ex.Message}");
        }
    }
    
    
    private async Task<string> GetPlayerData(IExecutionContext context, IGameApiClient gameApiClient, string key)
    {
        try
        {
            var result = await gameApiClient.CloudSaveData.GetItemsAsync(
                context, 
                context.AccessToken!,
                context.ProjectId!, 
                context.PlayerId!, 
                new List<string> { key });
                
            var data = result.Data.Results.FirstOrDefault()?.Value?.ToString() ?? string.Empty;
            _logger.LogInformation("Successfully retrieved data for key: {Key}", key);
            return data;
        }
        catch (ApiException ex)
        {
            _logger.LogError("Failed to retrieve data for key {Key}. Error: {Error}", key, ex.Message);
            throw new Exception($"Unable to retrieve player data: {ex.Message}");
        }
    }

    [CloudCodeFunction("SayHello")]
    public string Hello(string name)
    {
        return $"Hello, {name}!";
    }

    [CloudCodeFunction("GetServerTime")]
    public string GetServerTime(IExecutionContext context)
    {
        return DateTime.UtcNow.ToString(CultureInfo.InvariantCulture);
    }
    
    [CloudCodeFunction("HandleNewPlayerNameEntry")]
    public string HandleNewPlayerNameEntry(IExecutionContext context, IGameApiClient gameApiClient, string newPlayerName)
    {
        if (IsPlayerNameValid(newPlayerName))
        {
            SavePlayerData(context, gameApiClient, k_PlayerNameKey, newPlayerName);
            return newPlayerName;
        }
        
        throw new ArgumentException($"Invalid player name: {newPlayerName}");
    }


    private bool IsPlayerNameValid(string playerName)
    {
        if (playerName.Length < 4 || playerName.Length > 16)
        {
            return false;
        }

        return true;
    }
}