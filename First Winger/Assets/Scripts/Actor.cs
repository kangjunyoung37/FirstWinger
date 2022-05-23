using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor : MonoBehaviour
{

    [SerializeField]
    protected int MaxHP = 100;

    [SerializeField]
    protected int CurrentHP;

    [SerializeField]
    protected int Damage = 1;

    [SerializeField]
    bool isDead = false;

    [SerializeField]
    protected int crashDamage = 100;


    protected int CrashDamage
    {
        get { return crashDamage; }
    }
    public bool IsDead
    {
        get { return isDead; }
    }

    void Start()
    {
        Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateActor();
    }

    protected virtual void Initialize()
    {
        CurrentHP = MaxHP;
    }

    protected virtual void UpdateActor()
    {

    }
    public virtual void OnBulletHited(Actor attacker, int damage)
    {
        Debug.Log("OnBulletHited" + damage);
        DecreaseHP(attacker, damage);
    }
    public virtual void OnCrash(Actor attacker, int damage)
    {
        Debug.Log("OnCrash damage" + damage);
        DecreaseHP(attacker, damage);
    }
    protected virtual void DecreaseHP(Actor attacker, int value)
    {
        if (isDead)
            return;
        CurrentHP -= value;
        if(CurrentHP < 0)
        {
            CurrentHP = 0;
        }
        if(CurrentHP == 0)
        {
            OnDead(attacker);
        }

    }
    protected virtual void OnDead(Actor killer)
    {
        Debug.Log(name + "OnDead");
        isDead = true;

        SystemManager.Instance.EffectManager.GenerateEffect(EffectManager.ActorDeadFxIndex, transform.position);
    }

}
