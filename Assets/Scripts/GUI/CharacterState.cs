using UnityEngine;

public class CharacterState : MonoBehaviour
{

    private GameObject _player;
    private Health _healthComp;
    private UISprite _healthBar;
    private Speed _speedComp;
    private UISprite _staminaBar;

    // Use this for initialization
    void Awake()
    {
        _healthBar = transform.FindChild("health").GetComponent<UISprite>();
        _staminaBar = transform.FindChild("stamina").GetComponent<UISprite>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateHealth();
        UpdateStamina();
    }

    private void UpdateStamina()
    {
        if (!_speedComp)
        {
            if (FindPlayer())
                _speedComp = _player.GetComponent<Speed>();
            else
                return;
        }
        _staminaBar.fillAmount = _speedComp.Stamina / _speedComp.MaxStamina;
    }

    private bool FindPlayer()
    {
        if (_player)
        {
            return true;
        }
        GameObject[] players = GameObject.FindGameObjectsWithTag(Tags.player);
        foreach (GameObject player in players)
        {
            if (player.networkView.isMine)
            {
                _player = player;
                return true;
            }
        }
        return false;
    }

    private void UpdateHealth()
    {

        if (!_healthComp)
        {
            if (FindPlayer())
                _healthComp = _player.GetComponent<Health>();
            else
                return;
        }
        _healthBar.fillAmount = _healthComp.HealthPoints / _healthComp.MaxHealth;

    }
}
