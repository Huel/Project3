using System;

[Serializable]
public class HostedGameInfo
{
    public string GameName;
    public string Mode;
    public int Players;
    public int MaxPlayers;

    public HostedGameInfo(string gameName, string mode, int players, int maxPlayers)
    {
        GameName = gameName;
        Mode = mode;
        Players = players;
        MaxPlayers = maxPlayers;
    }
}
