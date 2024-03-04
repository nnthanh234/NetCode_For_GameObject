using Game;
using System;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private void Start()
    {
        NetworkManager.Singleton.NetworkConfig.ConnectionApproval = true;
        if (RelayManager.Ins.IsHost)
        {
            NetworkManager.Singleton.ConnectionApprovalCallback = OnConnectionApproval;
            var cInfo = RelayManager.Ins.GetHostConnectionInfo();
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetHostRelayData(cInfo.dtlsAddress, (ushort)cInfo.dtlsPort, cInfo.AllocationId, cInfo.Key, cInfo.ConnectionData, isSecure: true);
            NetworkManager.Singleton.StartHost();
        }
        else
        {
            NetworkManager.Singleton.ConnectionApprovalCallback = OnConnectionApproval;
            var cInfo = RelayManager.Ins.GetClientConnectionInfo();
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetClientRelayData(cInfo.dtlsAddress, (ushort)cInfo.dtlsPort, cInfo.AllocationId, cInfo.Key, cInfo.ConnectionData, cInfo.HostConnectionData, isSecure: true);
            NetworkManager.Singleton.StartClient();
        }
    }

    private void OnConnectionApproval(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        response.Approved = true;
        response.CreatePlayerObject = true;
        response.Pending = false;
    }
}
