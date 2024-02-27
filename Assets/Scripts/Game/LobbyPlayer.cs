using TMPro;
using UnityEngine;

namespace Game
{
    public class LobbyPlayer : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI txtPlayerName;
        [SerializeField]
        private MeshRenderer isReady;

        private MaterialPropertyBlock mat;
        private LobbyPlayerData lobbyPlayerData;

        private void Start()
        {
            mat = new MaterialPropertyBlock();
        }
        public void SetData(LobbyPlayerData data)
        {
            lobbyPlayerData = data;
            txtPlayerName.text = data.Gamertag;

            if (lobbyPlayerData.IsReady)
            {
                if (isReady != null)
                {
                    isReady.GetPropertyBlock(mat);
                    mat.SetColor("_BaseColor", Color.green);
                    isReady.SetPropertyBlock(mat);
                }
            }

            gameObject.SetActive(true);
        }
    }
}