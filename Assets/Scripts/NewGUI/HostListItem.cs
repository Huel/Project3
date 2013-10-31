using UnityEngine;

public class HostListItem : MonoBehaviour
{
    private NetworkManager _networkManager;
    private ServerInfo _serverInfo;
    void Awake()
    {
        _networkManager = GameObject.FindGameObjectWithTag(Tags.networkManager).GetComponent<NetworkManager>();
    }

    public void Bind(ServerInfo data)
    {
        _serverInfo = data;
        dfControl control = GetComponent<dfControl>();
        control.Find<dfLabel>("label_name").Text = data.Name;
    }

    public void OnClick()
    {
        _networkManager.TargetServer = _serverInfo;
        _networkManager.ConnectToServer();
    }
}
