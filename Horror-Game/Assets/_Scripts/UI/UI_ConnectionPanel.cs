using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using Unity.Services.Relay;
using Unity.Services.Core;
using Unity.Services.Authentication;
using TMPro;
using Unity.Services.Relay.Models;
using Unity.Networking.Transport.Relay;
using Unity.Netcode.Transports.UTP;
using UnityEngine.SceneManagement;


public class UI_ConnectionPanel : MonoBehaviour
{
    [SerializeField] string gameScene;

    [SerializeField] Button hostBtn;
    [SerializeField] Button clientBtn;
    [SerializeField] TMP_InputField joinCodeInput;

    async void Start()
    {
        await UnityServices.InitializeAsync();

        // ParrelSync should only be used within the Unity Editor so you should use the UNITY_EDITOR define
#if UNITY_EDITOR
        if (ParrelSync.ClonesManager.IsClone())
        {
            // When using a ParrelSync clone, switch to a different authentication profile to force the clone
            // to sign in as a different anonymous user account.
            string customArgument = ParrelSync.ClonesManager.GetArgument();
            AuthenticationService.Instance.SwitchProfile($"Clone_{customArgument}_Profile");
        }
#endif

        await AuthenticationService.Instance.SignInAnonymouslyAsync();

        hostBtn.onClick.AddListener(CreateRelay);
        clientBtn.onClick.AddListener(() => JoinRelay(joinCodeInput.text));
    }

    public async void CreateRelay()
    {
        Allocation allocation = await RelayService.Instance.CreateAllocationAsync(2);
        string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
        Debug.Log(joinCode);

        NetworkPlayersManager.instance.lobbyCode = joinCode;

        RelayServerData relayServerData = new RelayServerData(allocation, "dtls");
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

        if (NetworkManager.Singleton.StartHost())
        {
            Debug.Log("Host started");
            NetworkManager.Singleton.SceneManager.LoadScene(gameScene, LoadSceneMode.Single);
        }
    }

    public async void JoinRelay(string joinCode)
    {
        var joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
        var relayServerData = new RelayServerData(joinAllocation, "dtls");
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

        if (NetworkManager.Singleton.StartClient())
        {
            Debug.Log("Client started");
        }
    }
}
