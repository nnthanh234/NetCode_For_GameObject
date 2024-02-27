using Game.Events;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class LobbySpawner : MonoBehaviour
    {
        [SerializeField]
        private List<LobbyPlayer> lsPlayers;

        private void OnEnable()
        {
            LobbyEvents.OnLobbyUpdate += OnLobbyUpdate;
        }
        private void OnDisable()
        {
            LobbyEvents.OnLobbyUpdate -= OnLobbyUpdate;
        }
        private void OnLobbyUpdate()
        {
            List<LobbyPlayerData> datas = GameLobbyManager.Ins.GetLobbyDatas();
            for (int i = 0; i < datas.Count; i++)
            {
                lsPlayers[i].SetData(datas[i]);
            }
        }
    }
}
