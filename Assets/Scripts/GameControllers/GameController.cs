using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public enum GameState { Starting, Running, Ended };

    private float _startTime;
    public GameState state;
    private List<NetworkPlayerController> _networkPlayerControllers = new List<NetworkPlayerController>();

    public NetworkPlayerController GetNetworkPlayerController(NetworkPlayer player)
    {
        foreach (NetworkPlayerController controller in _networkPlayerControllers)
        {
            if (controller.networkPlayer == player)
            {
                return controller;
            }
        }
        return null;
    }

    public float GameTime
    {
        get { return Time.time - _startTime; }
    }

    [RPC]
    void AddNetworkPlayerController(int playerID, string playerName, int Team, NetworkPlayer networkPlayer)
    {
        _networkPlayerControllers.Add(new NetworkPlayerController(playerID, playerName, Team, networkPlayer));
    }

    [RPC]
    void SetGameState(int state)
    {
        this.state = (GameState)state;
        if (this.state == GameState.Running)
        {
            _startTime = Time.time;
        }
    }
}
