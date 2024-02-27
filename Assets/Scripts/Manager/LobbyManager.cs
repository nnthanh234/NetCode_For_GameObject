using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LobbyManager : Singleton<LobbyManager>
{
    private Lobby lobby;
    private Coroutine C_HearthbeatLobby;
    private Coroutine C_RefreshLobby;

    public string GetLobbyCode()
    {
        return lobby?.LobbyCode;
    }
    public async Task<bool> CreateLobby(int maxPlayers, bool isPrivate, Dictionary<string, string> data, Dictionary<string, string> lobbyData)
    {
        Dictionary<string, PlayerDataObject> playerData = SerializePlayerData(data);
        Player player = new Player(AuthenticationService.Instance.PlayerId, null, playerData);

        CreateLobbyOptions options = new CreateLobbyOptions()
        {
            IsPrivate = isPrivate,
            Player = player,
            Data = SerializeLobbyData(lobbyData)
        };

        try
        {
            lobby = await LobbyService.Instance.CreateLobbyAsync("Lobby", maxPlayers, options);
        }
        catch (Exception)
        {
            return false;
        }

        Debug.Log($"lobby id: {lobby.Id}");
        C_HearthbeatLobby = StartCoroutine(HearthbeatLobbyCoroutine(lobby.Id, 6f));
        C_RefreshLobby = StartCoroutine(RefreshLobbyCoroutine(lobby.Id, 1f));

        return true;
    }

    private IEnumerator RefreshLobbyCoroutine(string lobbyId, float waitTime)
    {
        while (true)
        {
            Task<Lobby> task = LobbyService.Instance.GetLobbyAsync(lobbyId);
            yield return new WaitUntil(() => task.IsCompleted);

            Lobby newLobby = task.Result;
            if (newLobby.LastUpdated > lobby.LastUpdated)
            {
                lobby = newLobby;
                GameFramework.Events.LobbyEvents.OnLobbyUpdate?.Invoke(lobby);
            }

            yield return new WaitForSecondsRealtime(waitTime);
        }
    }

    private IEnumerator HearthbeatLobbyCoroutine(string lobbyId, float waitTime)
    {
        while (true)
        {
            Debug.Log("Hearthbeat");
            LobbyService.Instance.SendHeartbeatPingAsync(lobbyId);
            yield return new WaitForSecondsRealtime(waitTime);
        }
    }
    private Dictionary<string, PlayerDataObject> SerializePlayerData(Dictionary<string, string> data)
    {
        Dictionary<string, PlayerDataObject> playerData = new Dictionary<string, PlayerDataObject>();
        foreach (var (key, value) in data)
        {
            playerData.Add(key, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, value));
        }
        return playerData;
    }
    private Dictionary<string, DataObject> SerializeLobbyData(Dictionary<string, string> data)
    {
        Dictionary<string, DataObject> playerData = new Dictionary<string, DataObject>();
        foreach (var (key, value) in data)
        {
            playerData.Add(key, new DataObject(DataObject.VisibilityOptions.Member, value));
        }
        return playerData;
    }
    private void OnApplicationQuit()
    {
        if (lobby != null && lobby.HostId == AuthenticationService.Instance.PlayerId)
        {
            LobbyService.Instance.DeleteLobbyAsync(lobby.Id);
        }
    }
    internal async Task<bool> JoinLobby(string code, Dictionary<string, string> data)
    {
        Dictionary<string, PlayerDataObject> playerData =  SerializePlayerData(data);
        Player player = new Player(AuthenticationService.Instance.PlayerId, null, playerData);

        JoinLobbyByCodeOptions options = new JoinLobbyByCodeOptions();
        options.Player = player;

        try
        {
            lobby = await LobbyService.Instance.JoinLobbyByCodeAsync(code, options);
        }
        catch (Exception)
        {
            return false;
        }

        C_RefreshLobby = StartCoroutine(RefreshLobbyCoroutine(lobby.Id, 1f));
        return true;
    }
    public List<Dictionary<string, PlayerDataObject>> GetPlayersData()
    {
        List<Dictionary<string, PlayerDataObject>> datas = new List<Dictionary<string, PlayerDataObject>>();
        foreach (var player in lobby.Players)
        {
            datas.Add(player.Data);
        }

        return datas;
    }
    public async Task<bool> UpdatePlayerData(string playerId, Dictionary<string, string> data, string allocationId = default, string connectionData = default)
    {
        Dictionary<string, PlayerDataObject> playerData = SerializePlayerData(data);

        UpdatePlayerOptions options = new UpdatePlayerOptions()
        {
            Data = playerData,
            AllocationId = allocationId,
            ConnectionInfo = connectionData
        };
        try
        {
            lobby = await LobbyService.Instance.UpdatePlayerAsync(lobby.Id, playerId, options);
        }
        catch (Exception)
        {
            return false;
        }

        GameFramework.Events.LobbyEvents.OnLobbyUpdate?.Invoke(lobby);
        return true;
    }
    public async Task<bool> UpdateLobbyData(Dictionary<string, string> data)
    {
        Dictionary<string, DataObject> lobbyData = SerializeLobbyData(data);
        UpdateLobbyOptions options = new UpdateLobbyOptions()
        {
            Data = lobbyData
        };

        try
        {
            lobby = await LobbyService.Instance.UpdateLobbyAsync(lobby.Id, options);
        }
        catch (Exception)
        {
            return false;
        }

        GameFramework.Events.LobbyEvents.OnLobbyUpdate?.Invoke(lobby);
        return true;
    }

    internal string GetHostId()
    {
        return lobby.HostId;
    }
}
