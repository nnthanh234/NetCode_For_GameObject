using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Game
{
    public class RelayManager : Singleton<RelayManager>
    {
        private string joinCode;
        private string ip;
        private int port;
        private Guid allocationId;
        private byte[] connectionData;

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
            return true;
        }        
    }
}
