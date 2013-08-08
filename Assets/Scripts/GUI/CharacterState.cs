using UnityEngine;

public class CharacterState : MonoBehaviour
{

	private GameObject _player;
	private Health _healthComp;
	private UISprite _healthBar;
	private Speed _speedComp;
	private UISprite _staminaBar;
	private Trophy _trophyComp;
	private GameObject _trophy01;
	private GameObject _trophy02;
	private GameObject _trophy03;
	private GameObject _freshMeat;
	private GameObject _shieldwall;
	private Vector3 skill01Position = new Vector3(-131, 52, 0);
	private Vector3 skill02Position = new Vector3(- 23, 52, 0);
	private Vector3 skill03Position = new Vector3(  85, 52, 0);
	private Vector3 skill04Position = new Vector3( 191, 52, 0);

	// Use this for initialization
	void Awake()
	{
		_healthBar = transform.FindChild("health").GetComponent<UISprite>();
		_staminaBar = transform.FindChild("stamina").GetComponent<UISprite>();
		_trophy01 = transform.FindChild("trophy01").gameObject;
		_trophy02 = transform.FindChild("trophy02").gameObject;
		_trophy03 = transform.FindChild("trophy03").gameObject;
	}

	// Update is called once per frame
	void Update()
	{
		UpdateHealth();
		UpdateStamina();
		UpdateTrophies();
		UpdateSkills();
	}

	private void UpdateSkills()
	{
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

	private void UpdateTrophies()
	{
		if (!_trophyComp)
		{
			if (FindPlayer())
				_trophyComp = _player.GetComponent<Trophy>();
			else
				return;
		}
		switch (_trophyComp.trophyLevel)
		{
			case 3:
				_trophy03.SetActive(true);
				_trophy02.SetActive(true);
				_trophy01.SetActive(true);
				break;
			case 2:
				_trophy03.SetActive(false);
				_trophy02.SetActive(true);
				_trophy01.SetActive(true);
				break;
			case 1:
				_trophy03.SetActive(false);
				_trophy02.SetActive(false);
				_trophy01.SetActive(true);
				break;
			default:
				_trophy03.SetActive(false);
				_trophy02.SetActive(false);
				_trophy01.SetActive(false);
				break;
		}
	}
}
