using System;
using System.Linq;
using System.Threading.Tasks;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;

namespace Game
{
    public class RelayManager : Singleton<RelayManager>
    {
        private string joinCode;
        private string ip;
        private int port;
        private Guid allocationId;
        private byte[] connectionData;
        private byte[] key;
        private byte[] allocationHostData;
        private byte[] allocationIdBytes;
        private bool isHost;

        public bool IsHost => isHost;

        public string GetAllocationId()
        {
            return allocationId.ToString();
        }
        public string GetConnectionData()
        {
            return connectionData.ToString();
        }
        public async Task<string> CreateRelay(int maxConnection)
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(maxConnection);
            joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            RelayServerEndpoint dtlsEndpoint = allocation.ServerEndpoints.First(conn => conn.ConnectionType == "dtls");
            ip = dtlsEndpoint.Host;
            port = dtlsEndpoint.Port;

            allocationId = allocation.AllocationId;
            connectionData = allocation.ConnectionData;
            key = allocation.Key;
            allocationIdBytes = allocation.AllocationIdBytes;

            isHost = true;
            return joinCode;
        }
        public async Task<bool> JoinRelay(string joinCode)
        {
            this.joinCode = joinCode;
            JoinAllocation allocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

            RelayServerEndpoint dtlsEndpoint = allocation.ServerEndpoints.First(conn => conn.ConnectionType == "dtls");
            ip = dtlsEndpoint.Host;
            port = dtlsEndpoint.Port;

            allocationId = allocation.AllocationId;
            connectionData = allocation.ConnectionData;
            allocationHostData = allocation.HostConnectionData;
            key = allocation.Key;
            allocationIdBytes = allocation.AllocationIdBytes;
            return true;
        }  
        public (byte[] AllocationId, byte[] Key, byte[] ConnectionData, string dtlsAddress, int dtlsPort) GetHostConnectionInfo()
        {
            return (allocationIdBytes, key, connectionData, ip, port);
        }
        public (byte[] AllocationId, byte[] Key, byte[] ConnectionData, byte[] HostConnectionData, string dtlsAddress, int dtlsPort) GetClientConnectionInfo()
        {
            return (allocationIdBytes, key, connectionData, allocationHostData, ip, port);
        }
    }
}
