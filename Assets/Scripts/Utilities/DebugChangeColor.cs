using UnityEngine;

public enum DebugColor { Team, TeamLight, Hit, Dead };
public class DebugChangeColor : MonoBehaviour
{

    private DebugColor _objectColor;
    private int _team;

    private float _effectTimer = 0f;
    private static float _effectTime = 0.1f;
    private bool _effectRunning = false;
    private bool _stableEffect = false;
    public static Color blue = new Color(0f, 0f, 1f, 1f);
    public static Color lightBlue = new Color(0.2f, 0.2f, 1f, 1f);
    public static Color green = new Color(0f, 1f, 0f, 1f);
    public static Color lightGreen = new Color(0.4f, 1f, 0.4f, 1f);
    public static Color yellow = new Color(1, 0.88f, 0);
    public static Color lightYellow = new Color(1, 1, 0.2f);
    public static Color red = new Color(1f, 0f, 0f, 1f);
    public static Color black = new Color(0f, 0f, 0f, 1f);
    public static Color[] colorSet0 = { blue, lightBlue, red, black };
    public static Color[] colorSet1 = { green, lightGreen, red, black };
    public static Color[] colorSet2 = { yellow, lightYellow, red, black };
    public static Color[][] debugColors = { colorSet0, colorSet1, colorSet2 };

    // Use this for initialization
    void Awake()
    {
        _team = (int)GetComponent<Team>().ID;
        SetColor(DebugColor.Team);
    }

    [RPC]
    public void SetColor(int color)
    {
        _objectColor = (DebugColor)color;

        string meshName;
        if (gameObject.GetComponent<Target>().type == TargetType.Minion)
            gameObject.transform.FindChild("mesh_minion").FindChild("lamp").renderer.material.color = debugColors[_team][color];
        //else if (gameObject.GetComponent<Target>().type == TargetType.Hero)
        // gameObject.transform.FindChild("mesh_hero01").renderer.material.color = debugColors[_team][color];
        else return;


        if (networkView.isMine)
        {
            networkView.RPC("SetColor", RPCMode.OthersBuffered, color);
        }
    }

    public void SetColor(DebugColor color)
    {
        SetColor((int)color);
    }

    [RPC]
    public void SetEffect(int color, bool stable)
    {
        if (!_stableEffect)
        {
            SetColor(color);
            _stableEffect = stable;
            _effectRunning = !stable;
        }

    }

    public void SetEffect(DebugColor color, bool stable = false)
    {
        SetEffect((int)color, stable);
    }



    // Update is called once per frame
    void Update()
    {
        if ((int)GetComponent<Team>().ID != _team)
        {
            _team = (int)GetComponent<Team>().ID;
            SetColor(_objectColor);
        }
        if (!_stableEffect && _effectRunning)
        {
            _effectTimer += Time.deltaTime;
            if (_effectTimer >= _effectTime)
            {
                SetColor(DebugColor.Team);
                _effectTimer = 0f;
                _effectRunning = false;
            }
        }


    }
}
