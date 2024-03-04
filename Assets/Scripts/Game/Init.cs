using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace Game
{
    public class Init : MonoBehaviour
    {
        async void Start()
        {
            await UnityServices.InitializeAsync();

            if (UnityServices.State == ServicesInitializationState.Initialized)
            {
                AuthenticationService.Instance.SignedIn += OnSignedIn;

                await AuthenticationService.Instance.SignInAnonymouslyAsync();

                if (AuthenticationService.Instance.IsSignedIn)
                {
                    string username = PlayerPrefs.GetString("username");
                    if (string.IsNullOrEmpty(username))
                    {
                        username = "Player";
                        PlayerPrefs.SetString("username", username);
                    }

                    SceneManager.LoadScene("MainMenu");
                }
            }
        }
        private void OnSignedIn()
        {
            Debug.Log($"Player Id: {AuthenticationService.Instance.PlayerId}");
            Debug.Log($"Token: {AuthenticationService.Instance.AccessToken}");
        }
    }
}
