using GameFramework.Events;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Lobbies.Models;
using UnityEngine.SceneManagement;

namespace Game
{
    public class GameLobbyManager : Singleton<GameLobbyManager>
    {
        public List<LobbyPlayerData> lobbyPlayerDatas = new List<LobbyPlayerData>();

        private LobbyPlayerData localLobbyPlayerData;
        private LobbyData lobbyData;
        private int maxNumberOfPlayer = 4;

        public bool IsHost => localLobbyPlayerData.Id == LobbyManager.Ins.GetHostId();

        private void OnEnable()
        {
            LobbyEvents.OnLobbyUpdate += LobbyUpdate;
        }
        private void OnDisable()
        {
            LobbyEvents.OnLobbyUpdate -= LobbyUpdate;
        }
        public async Task<bool> CreateLobby()
        {
            localLobbyPlayerData = new LobbyPlayerData();
            localLobbyPlayerData.Initialize(AuthenticationService.Instance.PlayerId, "HostPlayer");

            lobbyData = new LobbyData();
            lobbyData.Initialized(0);
            bool succeeded = await LobbyManager.Ins.CreateLobby(maxNumberOfPlayer, true, localLobbyPlayerData.Serialize(), lobbyData.Serialize());
            return succeeded;
        }
        public string GetLobbyCode()
        {
            return LobbyManager.Ins.GetLobbyCode();
        }
        public async Task<bool> JoinLobby(string code)
        {
            localLobbyPlayerData = new LobbyPlayerData();
            localLobbyPlayerData.Initialize(AuthenticationService.Instance.PlayerId, "JoinPlayer");

            bool succeeded = await LobbyManager.Ins.JoinLobby(code, localLobbyPlayerData.Serialize());
            return succeeded;
        }
        private async void LobbyUpdate(Lobby lobby)
        {
            List<Dictionary<string, PlayerDataObject>> playersDatas = LobbyManager.Ins.GetPlayersData();
            lobbyPlayerDatas.Clear();

            int numberOfPlayerReady = 0;

            foreach (var playerData in playersDatas)
            {
                LobbyPlayerData lobbyPlayerData = new LobbyPlayerData();
                lobbyPlayerData.Initialize(playerData);

                if (lobbyPlayerData.IsReady)
                {
                    numberOfPlayerReady++;
                }

                if (lobbyPlayerData.Id == AuthenticationService.Instance.PlayerId)
                {
                    localLobbyPlayerData = lobbyPlayerData;
                }

                lobbyPlayerDatas.Add(lobbyPlayerData);
            }

            lobbyData = new LobbyData();
            lobbyData.Initialized(lobby.Data);
            Events.LobbyEvents.OnLobbyUpdate?.Invoke();

            if (numberOfPlayerReady == lobby.Players.Count)
            {
                Events.LobbyEvents.OnLobbyReady?.Invoke();
            }
            if (lobbyData.RelayJoinCode != default)
            {
                await JoinRelayServer(lobbyData.RelayJoinCode);
                SceneManager.LoadSceneAsync(lobbyData.SceneName);
            }
        }   
        public List<LobbyPlayerData> GetLobbyDatas()
        {
            return lobbyPlayerDatas;
        }

        public async Task<bool> SetPlayerLobby()
        {
            localLobbyPlayerData.IsReady = true;
            return await LobbyManager.Ins.UpdatePlayerData(localLobbyPlayerData.Id, localLobbyPlayerData.Serialize());
        }

        public int GetMapIndex()
        {
            return lobbyData.MapIndex;
        }

        public async Task<bool> SelectionMap(int curMapIndex, string sceneName)
        {
            lobbyData.MapIndex = curMapIndex;
            lobbyData.SceneName = sceneName;
            return await LobbyManager.Ins.UpdateLobbyData(lobbyData.Serialize());
        }

        public async Task StartGame()
        {
            string joinRelayCode = await RelayManager.Ins.CreateRelay(maxNumberOfPlayer);

            lobbyData.RelayJoinCode = joinRelayCode;
            await LobbyManager.Ins.UpdateLobbyData(lobbyData.Serialize());

            string allocationId = RelayManager.Ins.GetAllocationId();
            string connectionData = RelayManager.Ins.GetConnectionData();

            await LobbyManager.Ins.UpdatePlayerData(localLobbyPlayerData.Id, localLobbyPlayerData.Serialize(), allocationId, connectionData);

            SceneManager.LoadSceneAsync(lobbyData.SceneName);
        }
        private async Task<bool> JoinRelayServer(string relayJoinCode)
        {
            await RelayManager.Ins.JoinRelay(relayJoinCode);

            string allocationId = RelayManager.Ins.GetAllocationId();
            string connectionData = RelayManager.Ins.GetConnectionData();

            await LobbyManager.Ins.UpdatePlayerData(localLobbyPlayerData.Id, localLobbyPlayerData.Serialize(), allocationId, connectionData);

            return true;
        }
    }
}
