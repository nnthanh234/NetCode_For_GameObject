using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

namespace Game
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField]
        private GameObject objMenu;
        [SerializeField]
        private GameObject objJoin;
        [SerializeField]
        private Button btnHost;
        [SerializeField]
        private Button btnJoin;
        [SerializeField]
        private Button btnSubmit;
        [SerializeField]
        private TMP_InputField ipLobbyCode;


        private void OnEnable()
        {
            btnHost.onClick.AddListener(OnHostClicked);
            btnJoin.onClick.AddListener(OnJoinClicked);
            btnSubmit.onClick.AddListener(OnSubmitLobbyCodeClicked);
        }
        private void OnDisable()
        {
            btnHost.onClick.RemoveListener(OnHostClicked);
            btnJoin.onClick.RemoveListener(OnJoinClicked);
            btnSubmit.onClick.RemoveListener(OnSubmitLobbyCodeClicked);
        }
        private async void OnHostClicked()
        {
            bool succeeded = await GameLobbyManager.Ins.CreateLobby();
            if (succeeded)
            {
                SceneManager.LoadSceneAsync("Lobby");
            }
        }
        private void OnJoinClicked()
        {
            objMenu.SetActive(false);
            objJoin.SetActive(true);
        }
        private async void OnSubmitLobbyCodeClicked()
        {
            string code = ipLobbyCode.text;
            code = code.TrimEnd(' ');
            Debug.Log(code);

            bool succeeded = await GameLobbyManager.Ins.JoinLobby(code);
            if (succeeded)
            {
                SceneManager.LoadSceneAsync("Lobby");
            }
        }

    }
}
