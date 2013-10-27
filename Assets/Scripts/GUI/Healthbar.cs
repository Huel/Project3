using UnityEngine;

[RequireComponent(typeof(GUIText))]
public class Healthbar : MonoBehaviour
{
    private float _maxHealth = 0;
    private float _curHealth = 0;
    private Health _health;
    public UISprite healthbar;
    public bool enemyOnly = false;
    private GameObject _gameObject;
    private Team _team;

    void Start()
    {
        _health = GetComponent<Health>();
        _team = GetComponent<Team>();
        _gameObject = healthbar.transform.parent.gameObject;



    }
    // Update is called once per frame
    void Update()
    {
        if (_team && _team.ID != Team.TeamIdentifier.NoTeam)
        {
            if (enemyOnly &&
                _team.IsOwnTeam((Team.TeamIdentifier)
                    GameObject.FindGameObjectWithTag(Tags.localPlayerController)
                              .GetComponent<LocalPlayerController>()
                              .networkPlayerController.team))
            {
                Destroy(_gameObject);
                Destroy(this);
            }
            healthbar.color = Team.teamColors[(int)_team.ID];
            _team = null;
        }

        _curHealth = _health.HealthPoints;
        _maxHealth = _health.MaxHealth;

        if (_curHealth / _maxHealth == 1f || _curHealth <= 0)
        {
            _gameObject.SetActive(false);
            return;
        }
        _gameObject.SetActive(true);

        _gameObject.transform.rotation = Camera.main.transform.rotation;

        healthbar.alpha = 1;
        healthbar.fillAmount = _curHealth / _maxHealth;
    }
}