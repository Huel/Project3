using System.Collections.Generic;
using UnityEngine;

public class CharacterState : MonoBehaviour
{

    private GameObject _player;
    private Health _healthComp;
    private UISprite _healthBar;
    private Speed _speedComp;
    private UISprite _staminaBar;
    private Trophy _trophyComp;
    public GameObject _trophy01;
    public GameObject _trophy02;
    public GameObject _trophy03;
    #region bullshit
    //private GameObject _freshMeat;
    //private GameObject _shieldwall;
    //private GameObject _kamikazeMission;
    //private GameObject _battlecry;
    //private Vector3 skill01Position = new Vector3(-131, 52, 0);
    //private Vector3 skill02Position = new Vector3(-23, 52, 0);
    //private Vector3 skill03Position = new Vector3(85, 52, 0);
    //private Vector3 skill04Position = new Vector3(191, 52, 0);
    #endregion
    
    public List<GameObject> skills = new List<GameObject>();
    private List<Vector3> skillPositions = new List<Vector3>(); 

    private bool init = false;

    // Use this for initialization
    void Awake()
    {
        _healthBar = transform.FindChild("health").GetComponent<UISprite>();
        _staminaBar = transform.FindChild("stamina").GetComponent<UISprite>();

        skillPositions.Add(new Vector3(-131, 52, 0));
        skillPositions.Add(new Vector3(-23, 52, 0));
        skillPositions.Add(new Vector3(85, 52, 0));
        skillPositions.Add(new Vector3(191, 52, 0));

        #region bullshit
        //_trophy01 = transform.FindChild("trophy01").gameObject;
        //_trophy02 = transform.FindChild("trophy02").gameObject;
        //_trophy03 = transform.FindChild("trophy03").gameObject;
        //_freshMeat = transform.FindChild("Fresh Meat").gameObject;
        //skills.Add(_freshMeat);
        //_shieldwall = transform.FindChild("Shieldwall").gameObject;
        //skills.Add(_shieldwall);
        //_kamikazeMission = transform.FindChild("Kamikaze Mission").gameObject;
        //skills.Add(_kamikazeMission);
        //_battlecry = transform.FindChild("Battlecry").gameObject;
        //skills.Add(_battlecry);
        #endregion
        
    }

    private void SetSkillPosiotion()
    {
        for (int i = 0; i < 4; i++)
            skills[i].transform.localPosition = skillPositions[i];

        #region bullshit
        //switch (position)
        //{
        //    case 1:
        //        {
        //            foreach (GameObject skill in skills)
        //            {
        //                if (skill.name.Equals(skillName))
        //                {
        //                    skill.transform.localPosition = skill01Position;
        //                    break;
        //                }
        //            }
        //            break;
        //        }
        //    case 2:
        //        {
        //            foreach (GameObject skill in skills)
        //            {
        //                if (skill.name.Equals(skillName))
        //                {
        //                    skill.transform.localPosition = skill02Position;
        //                    break;
        //                }
        //            }
        //            break;
        //        }
        //    case 3:
        //        {
        //            foreach (GameObject skill in skills)
        //            {
        //                if (skill.name.Equals(skillName))
        //                {
        //                    skill.transform.localPosition = skill03Position;
        //                    break;
        //                }
        //            }
        //            break;
        //        }
        //    case 4:
        //        {
        //            foreach (GameObject skill in skills)
        //            {
        //                if (skill.name.Equals(skillName))
        //                {
        //                    skill.transform.localPosition = skill04Position;
        //                    break;
        //                }
        //            }
        //            break;
        //        }
        //}
        #endregion
        
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
        if (FindPlayer() && !init)
        {
            SetSkillPosiotion(); /*1, _player.GetComponent<CharController>().skill1.skillName*/
            init = true;
        }

        foreach (Skill skill in _player.GetComponents<Skill>())
        {
            foreach (GameObject skillSprite in skills)
            {
                if (skill.SkillName == skillSprite.name)
                {
                    skillSprite.GetComponent<UISprite>().fillAmount = skill.getCooldownInPercent();

                    if (skill.State == Skill.SkillState.InExecution || skill.State == Skill.SkillState.Active)
                        skillSprite.GetComponent<UISprite>().color = new Color(0.5f, 1f, 0.5f);

                    else if (skill.State != Skill.SkillState.Ready)
                        skillSprite.GetComponent<UISprite>().color = new Color(0.5f,0.5f,0.5f);
                    else
                        skillSprite.GetComponent<UISprite>().color = new Color(1f, 1f, 1f);
                }
            }
        }
    }

    private bool FindPlayer()
    {
        if (!_player)
        {
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
        return true;
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
            case 0:
                _trophy03.SetActive(false);
                _trophy02.SetActive(false);
                _trophy01.SetActive(false);
                break;
            case 1:
                _trophy03.SetActive(false);
                _trophy02.SetActive(false);
                _trophy01.SetActive(true);
                break;
            case 2:
                _trophy03.SetActive(false);
                _trophy02.SetActive(true);
                _trophy01.SetActive(true);
                break;
            case 3:
                _trophy03.SetActive(true);
                _trophy02.SetActive(true);
                _trophy01.SetActive(true);
                break;
            default:
                _trophy03.SetActive(true);
                _trophy02.SetActive(true);
                _trophy01.SetActive(true);
                break;
        }
    }
}
