
using Unity.Services.Lobbies.Models;

namespace GameFramework.Events
{
    public static class LobbyEvents
    {
        public delegate void LobbyUpdate(Lobby lobby);
        public static LobbyUpdate OnLobbyUpdate;
    }
}
