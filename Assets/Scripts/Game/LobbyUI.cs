using Game.Events;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class LobbyUI : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI txtLobbyCode;
        [SerializeField]
        private Button btnReady;
        [SerializeField]
        private Image imgMap;
        [SerializeField]
        private Button leftButton;
        [SerializeField]
        private Button rightButton;
        [SerializeField]
        private TextMeshProUGUI txtMapName;
        [SerializeField]
        private MapSellectionData mapData;
        [SerializeField]
        private Button btnStart;

        private int currentMapIndex;


        private void OnEnable()
        {
            if (GameLobbyManager.Ins.IsHost)
            {
                leftButton.onClick.AddListener(OnLeftButtonClicked);
                rightButton.onClick.AddListener(OnRightButtonClicked);
            }

            btnReady.onClick.AddListener(OnReadyPressed);
            btnStart.onClick.AddListener(OnStartClicked);
            LobbyEvents.OnLobbyUpdate += OnLobbyUpdated;
            LobbyEvents.OnLobbyReady += OnLobbyReady;
        }

        private void OnDisable()
        {
            leftButton.onClick.RemoveListener(OnLeftButtonClicked);
            rightButton.onClick.RemoveListener(OnRightButtonClicked);
            btnReady.onClick.RemoveListener(OnReadyPressed);
            btnStart.onClick.RemoveListener(OnStartClicked);
            LobbyEvents.OnLobbyUpdate -= OnLobbyUpdated;
            LobbyEvents.OnLobbyReady -= OnLobbyReady;
        }
        private void Start()
        {
            txtLobbyCode.text = "Lobby Code: " + GameLobbyManager.Ins.GetLobbyCode();

            imgMap.color = mapData.Maps[currentMapIndex].MapThumbnail;
            txtMapName.text = mapData.Maps[currentMapIndex].MapName;

            if (!GameLobbyManager.Ins.IsHost)
            {
                leftButton.gameObject.SetActive(false);
                rightButton.gameObject.SetActive(false);
            }
        }
        private async void OnReadyPressed()
        {
            bool succeed = await GameLobbyManager.Ins.SetPlayerLobby();
            if (succeed)
            {
                btnReady.gameObject.SetActive(false);
            }
        }
        private async void OnRightButtonClicked()
        {
            if (currentMapIndex + 1 < mapData.Maps.Count - 1)
                currentMapIndex++;
            else
                currentMapIndex = mapData.Maps.Count - 1;

            UpdateMap();
            await GameLobbyManager.Ins.SelectionMap(currentMapIndex);
        }
        private async void OnLeftButtonClicked()
        {
            if (currentMapIndex - 1 > 0)
                currentMapIndex--;
            else
                currentMapIndex = 0;

            UpdateMap();
            await GameLobbyManager.Ins.SelectionMap(currentMapIndex);
        }
        private void UpdateMap()
        {
            imgMap.color = mapData.Maps[currentMapIndex].MapThumbnail;
            txtMapName.text = mapData.Maps[currentMapIndex].MapName;
        }
        private void OnLobbyUpdated()
        {
            currentMapIndex = GameLobbyManager.Ins.GetMapIndex();
            UpdateMap();
        }
        private void OnLobbyReady()
        {
            btnStart.gameObject.SetActive(true);
        }
        private async void OnStartClicked()
        {
            await GameLobbyManager.Ins.StartGame(mapData.Maps[currentMapIndex].SceneName);
        }
    }
}