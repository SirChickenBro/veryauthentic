using System;
using System.Globalization;
using Microsoft.Extensions.Logging;
using Unity.Services.CloudCode.Apis;
using Unity.Services.CloudCode.Core;

namespace UGSTutorialCloudBindings;

public class PlayerDataService
{
    readonly IGameApiClient m_GameApiClient;
    readonly ILogger<PlayerDataService> m_Logger;

    public PlayerDataService(IGameApiClient gameApiClient, ILogger<PlayerDataService> logger)
    {
        m_GameApiClient = gameApiClient;
        m_Logger = logger;
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
}


