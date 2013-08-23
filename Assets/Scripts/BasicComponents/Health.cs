using System.Linq;
using System.Xml;
using UnityEngine;

[RequireComponent(typeof(NetworkView))]
public class Health : MonoBehaviour
{
    private XmlDocument heroInfo = new XMLReader("Hero01.xml").GetXML();
    private XmlDocument minionInfo = new XMLReader("Minion.xml").GetXML();
    private XmlDocument settingsInfo = new XMLReader("GameSettings.xml").GetXML();

    public float _healthPoints = 10f;
    private float _maxHealth = 10f;
    private float _incMaxHealth;
    private float _minHealth = 0f;
    private float _maxHealthMultiplier = 1f;
    private float _healthRegeneration = 1f;

    private float _deadCounter;

    public float keepDeadUnitTime = 5f;

    public bool immortal;
    private bool _invulnerable;
    private bool _alive = true;

    public bool Invulnerable
    {
        get
        {
            return _invulnerable;
        }
        set
        {
            networkView.RPC("SetInvulnerability", RPCMode.AllBuffered, value);
        }
    }

    public float HealthPoints
    {
        get { return _healthPoints; }
    }
    public float MaxHealth
    {
        get { return (_maxHealth * _maxHealthMultiplier) + _incMaxHealth; }
    }
    public float MinHealth
    {
        get { return _minHealth; }
    }
    public float HealthRegeneration
    {
        get { return _healthRegeneration; }
    }
    public bool IsAlive()
    {
        return _alive;
    }


    /// <summary>
    /// Do not use this method, use setter Invulnerable instead.
    /// </summary>
    [RPC]
    public void SetInvulnerability(bool value)
    {
        _invulnerable = value;
    }

    /// <summary>
    ///     set the maximum health
    /// </summary>
    /// <param name="maxHealth">set maxHealth multiply with maxHealthMultiplier</param>
    /// <param name="incHealth">set the health to the max if true</param>
    [RPC]
    public void SetMaxHealth(float maxHealth, bool incHealth)
    {
        _maxHealth = maxHealth;

        if (!networkView.isMine) return;

        networkView.RPC("SetMaxHealth", RPCMode.OthersBuffered, maxHealth, incHealth);
        if (incHealth)
            SetToMaxHealth();
    }
    /// <summary>
    ///     set the minimum health
    /// </summary>
    /// <param name="minHealth">value will never be under 0</param>
    [RPC]
    public void SetMinHealth(float minHealth)
    {
        _minHealth = Mathf.Max(0, minHealth);

        if (networkView.isMine)
        {
            networkView.RPC("SetMinHealth", RPCMode.OthersBuffered, MinHealth);
        }

    }
    /// <summary>
    ///     set the increased maximum health, no maxHealthMultiplier needed
    /// </summary>
    /// <param name="incMaxHealth"></param>
    [RPC]
    public void SetIncreasedMaxHealth(float incMaxHealth)
    {
        _incMaxHealth = incMaxHealth;
        if (networkView.isMine)
        {
            networkView.RPC("SetIncreasedMaxHealth", RPCMode.OthersBuffered, incMaxHealth);
            IncHealth(incMaxHealth);
        }

    }
    /// <summary>
    ///     set the health to the minimum
    /// </summary>
    public void SetToMinHealth()
    {
        if (networkView.isMine)
            SetHealth(MinHealth);
        else
            networkView.RPC("SetHealth", networkView.owner, MinHealth);
    }
    /// <summary>
    ///     set the health to the MAX !!
    /// </summary>
    public void SetToMaxHealth()
    {
        if (networkView.isMine)
            SetHealth(MaxHealth);
        else
            networkView.RPC("SetHealth", networkView.owner, MaxHealth);
    }
    /// <summary>
    ///     set and return the health
    /// </summary>
    /// <param name="healthValue">set the health dependet on maxHealth and minHealth</param>
    /// <returns>return the new health</returns>
    [RPC]
    public float SetHealth(float healthValue)
    {
        _healthPoints = Mathf.Clamp(healthValue, MinHealth, MaxHealth);
        if (networkView.isMine)
        {
            networkView.RPC("SetHealth", RPCMode.OthersBuffered, HealthPoints);
        }
        return HealthPoints;
    }
    /// <summary>
    ///     set the multiplier for health buff
    /// </summary>
    /// <param name="maxHealthMultiplier"></param>
    [RPC]
    public void SetMaxHealthMultiplier(float maxHealthMultiplier)
    {
        float tempMaxHealth = MaxHealth;
        _maxHealthMultiplier = maxHealthMultiplier;
        if (networkView.isMine)
        {
            networkView.RPC("SetMaxHealthMultiplier", RPCMode.OthersBuffered, maxHealthMultiplier);
            IncHealth(MaxHealth - tempMaxHealth);
        }
    }
    /// <summary>
    ///     set the health regeneration value
    /// </summary>
    /// <param name="maxHealthMultiplier"></param>
    [RPC]
    public void SetHealthRegeneration(float healthRegeneration)
    {
        if (networkView.isMine)
            _healthRegeneration = healthRegeneration;
        else
            networkView.RPC("SetHealthRegeneration", networkView.owner, healthRegeneration);
    }
    /// <summary>
    ///     increases the current health
    /// </summary>
    /// <param name="healthValue">the value added to current health</param>
    /// <returns>value is never higher than maxHealth</returns>
    public float IncHealth(float healthValue)
    {
        if (networkView.isMine)
            SetHealth(HealthPoints + healthValue);
        else
            networkView.RPC("SetHealth", networkView.owner, HealthPoints + healthValue);

        if (IsAlive() && healthValue < 0f) networkView.RPC("PlayHealthSound", RPCMode.All, false);

        return HealthPoints;
    }
    /// <summary>
    ///     decreases the current health
    /// </summary>
    /// <param name="healthValue">the value substract to current health</param>
    /// <returns>value is never lower than minHealth</returns>
    public float DecHealth(float healthValue)
    {
        if (HealthPoints == MinHealth || Invulnerable || immortal)
            return 0;

        return IncHealth(-healthValue);
    }

    void Update()
    {
        if (networkView.isMine)
            CheckHealthState();
    }

    private void CheckHealthState()
    {
        if (IsAlive())
        {
            if (HealthPoints <= 0 && !immortal)
            {
                Animator animator = GetComponent<Animator>();
                NetworkAnimator networkAnimator = GetComponent<NetworkAnimator>();
                if (animator && networkAnimator)
                {
                    animator.SetLayerWeight(0, 1f);
                    networkAnimator.PlayAnimation("Dying");
                }

                SetAlive(false);
            }
            else if (HealthPoints <= MaxHealth)
                IncHealth(Time.deltaTime * HealthRegeneration);
        }
        else
        {
            if (HealthPoints > 0)
            {
                _deadCounter = 0;
                SetAlive(true);
                return;
            }

            if (_deadCounter <= 0f)
            {
                networkView.RPC("PlayHealthSound", RPCMode.All, true);
            }

            _deadCounter += Time.deltaTime;

            if (_deadCounter >= keepDeadUnitTime)
            {
                networkView.RPC("KillObject", RPCMode.AllBuffered);
            }
        }
    }

    [RPC]
    public void SetAlive(bool alive)
    {
        if (networkView.isMine)
        {
            if (!alive)
            {
                _deadCounter = 0;
                if (GetComponent<Target>().type != TargetType.Dead)
                    GetComponent<Target>().SwitchTargetType((int)TargetType.Dead);
            }

            if (alive && gameObject.GetComponent<Target>().type == TargetType.Dead)
                gameObject.GetComponent<Target>().RestoreOldType();

            networkView.RPC("SetAlive", RPCMode.OthersBuffered, alive);
        }

        _alive = alive;


    }

    [RPC]
    public void KillObject()
    {
        Destroy(gameObject);
    }

    [RPC]
    public void PlayHealthSound(bool status)
    {
        if (status)
        {
            if (GetComponent<CharController>() != null)
            {
                transform.FindChild("sounds_hero01").GetComponent<AudioLibrary>().StartSound(heroInfo.GetElementsByTagName("die")[0].InnerText, 0f);
                foreach (GameObject player in GameObject.FindGameObjectsWithTag(Tags.player).Where(player => player.networkView.isMine))
                {
                    if (player.GetComponent<Team>().ID != GetComponent<Team>().ID)
                    {
                        GameObject.Find("sounds_Vocal").GetComponent<AudioLibrary>().StartSound(settingsInfo.GetElementsByTagName("enemyHeroDead")[0].InnerText, 0f);
                    }
                    break;
                }
            }
            if (GetComponentInChildren<MinionAgent>() != null)
            {
                transform.FindChild("sound_minion").GetComponent<AudioLibrary>().StartSound(minionInfo.GetElementsByTagName("die")[0].InnerText, 0f);
            }
        }
        else
        {
            int rnd = Random.Range(1, 2);
            if (GetComponent<CharController>() != null)
            {
                transform.FindChild("sounds_hero01")
                         .GetComponent<AudioLibrary>()
                         .StartSound(
                             rnd == 1
                                 ? heroInfo.GetElementsByTagName("beingHitVariation1")[0].InnerText
                                 : heroInfo.GetElementsByTagName("beingHitVariation2")[0].InnerText, 0f);
            }
            if (GetComponent<MinionAgent>() != null)
            {
                transform.FindChild("sound_minion")
                         .GetComponent<AudioLibrary>()
                         .StartSound(
                             rnd == 1
                                 ? minionInfo.GetElementsByTagName("beingHitVariation1")[0].InnerText
                                 : minionInfo.GetElementsByTagName("beingHitVariation2")[0].InnerText, 0f);
            }
        }
    }
}
