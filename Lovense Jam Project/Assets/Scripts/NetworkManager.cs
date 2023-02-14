using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using Fusion;
using Fusion.Sockets;
using System;
using Fusion.Photon.Realtime;

public class NetworkManager : MonoBehaviour, INetworkRunnerCallbacks
{
    public static NetworkManager instance;
    [SerializeField] NetworkPrefabRef playerPrefab;
    public static int ping
    {
        get
        {
            if (BoardPlayer.Local)
            {
                return (int)(1000 * BoardPlayer.Local.Runner.GetPlayerRtt(BoardPlayer.Local.Runner.LocalPlayer));
            }
            else
                return 0;
        }
    }
    NetworkRunner _runner;
    Dictionary<PlayerRef, NetworkObject> spawnedPlayers = new Dictionary<PlayerRef, NetworkObject>();

    private void Awake()
    {
        instance = this;
        _runner = gameObject.AddComponent<NetworkRunner>();
        _runner.ProvideInput = true;
    }

    async public void CreateOrJoinSession()
    {
        var appSettings = PhotonAppSettings.Instance.AppSettings.GetCopy();
        var region = Menu.instance.regionDropdown.options[Menu.instance.regionDropdown.value].text;
        appSettings.FixedRegion = region;
        var args = new StartGameArgs()
        {
            GameMode = GameMode.AutoHostOrClient,
            SessionName = Menu.instance.sessionName.text,
            CustomPhotonAppSettings = appSettings,
            Scene = 1,
            PlayerCount = 2,
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
        };
        var startGameTast = await _runner.StartGame(args);
        if (startGameTast.Ok)
        {
            Menu.OnEnterGame();
        }
    }
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        var rotation = Quaternion.identity;
        if (spawnedPlayers.Count > 0)
            rotation *= Quaternion.Euler(0, 180, 0);

        NetworkObject networkPlayerObject = runner.Spawn(playerPrefab, Vector3.zero, rotation, player);
        spawnedPlayers.Add(player, networkPlayerObject);
    }
    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        if (spawnedPlayers.TryGetValue(player, out NetworkObject networkObject))
        {
            runner.Despawn(networkObject);
            spawnedPlayers.Remove(player);
        }
    }
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        Menu.LeaveSession();
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        //if(BoardPlayer.Local != null)
        //    input.Set(BoardPlayer.Local.GetInputData());
    }

    public void OnConnectedToServer(NetworkRunner runner) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnDisconnectedFromServer(NetworkRunner runner) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data) { }
    public void OnSceneLoadDone(NetworkRunner runner) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
}