using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Bullet : NetworkBehaviour
{
    const float LifeTime = 15.0f;

    [SyncVar]
    [SerializeField]
    int OwnerInstanceID;

    [SyncVar]
    [SerializeField]
    protected Vector3 MoveDirection = Vector3.zero;

    [SyncVar]
    [SerializeField]
    protected float Speed = 0.0f;
    [SyncVar]
    protected bool NeedMove = false;
   
    [SyncVar]
    bool Hited = false;
    
    [SyncVar]
    protected float FiredTime;
    
    [SyncVar]
    [SerializeField]
    protected int Damage = 1;

    [SyncVar]
    [SerializeField]
    string filePath;

    public string FilePath
    {
        get { return filePath; }
        set { filePath = value; }
    }
    void Start()
    {
        if (!((FWNetworkManager)FWNetworkManager.singleton).isServer)
        {
            InGameSceneMain inGameSceneMain = SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>();
            transform.SetParent(inGameSceneMain.BulletManager.transform);
            inGameSceneMain.BulletCacheSystem.Add(FilePath, gameObject);
            gameObject.SetActive(false);

        }
    }

    // Update is called once per frame
    void Update()
    {
        if (ProcessDisappearCondition())
            return;
        UpdateTransform();


    }
    protected virtual void UpdateTransform()
    {
        UpdateMove();
    }
    protected virtual void UpdateMove()
    {
        if (!NeedMove)
            return;

        Vector3 moveVector = MoveDirection.normalized * Speed * Time.deltaTime;
        moveVector = AdjustMove(moveVector);
        transform.position += moveVector;


    }

    void InternelFire(int ownerIntanceID,Vector3 direction, float speed, int damage)
    {
        OwnerInstanceID = ownerIntanceID;
        
        
        MoveDirection = direction;
        Speed = speed;
        Damage = damage;
        NeedMove = true;
        FiredTime = Time.time;
    }
    public virtual void Fire(int ownerIntanceID, Vector3 direction, float speed, int damage)
    {
        if (isServer)
        {
            RpcFire(ownerIntanceID,  direction, speed, damage);
        }
        else
        {
            CmdFire(ownerIntanceID, direction, speed, damage);
            if (isLocalPlayer)
                InternelFire(ownerIntanceID,direction, speed, damage);
        }
        //UpdateNetworkBullet();
    }
    [Command]
    public void CmdFire(int ownerIntanceID, Vector3 direction, float speed, int damage)
    {
        InternelFire(ownerIntanceID,  direction, speed, damage);
        base.SetDirtyBit(1);
    }
    [ClientRpc]
    public void RpcFire(int ownerIntanceID,  Vector3 direction, float speed, int damage)
    {
        InternelFire(ownerIntanceID,direction, speed, damage);
        base.SetDirtyBit(1);
    }
    protected Vector3 AdjustMove(Vector3 moveVector)
    {
        RaycastHit hitInfo;
       if( Physics.Linecast(transform.position,transform.position+moveVector ,out hitInfo ))
       {
            int colliderLayer = hitInfo.collider.gameObject.layer;
            if(colliderLayer != LayerMask.NameToLayer("Enemy") && colliderLayer != LayerMask.NameToLayer("Player"))
            {
                return moveVector;
            }
            moveVector = hitInfo.point-transform.position;
            OnBulletCollision(hitInfo.collider);
       }
       return moveVector;
    }

    protected virtual bool OnBulletCollision(Collider collider)
    {
        if (Hited)
            return false;
        
        if(collider.gameObject.layer == LayerMask.NameToLayer("EnemyBullet")||collider.gameObject.layer == LayerMask.NameToLayer("PlayerBullet"))
        {
            return false;
        }
        Actor owner = SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().ActorManager.GetActor(OwnerInstanceID);
        if(owner == null) // ȣ��Ʈ�� Ŭ���̾�Ʈ�� ������ ����������
        {
            return false;
        }
        Actor actor = collider.GetComponentInParent<Actor>();
        if(actor == null)
        {
            return false;
        }
        if (actor && actor.IsDead || actor.gameObject.layer == owner.gameObject.layer)
        {
            return false;
        }

        actor.OnBulletHited(Damage, transform.position);
       

        //Collider myCollider = GetComponentInChildren<Collider>();
        //myCollider.enabled = false;
        Hited = true;
        NeedMove = false;
        GameObject go = SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().EffectManager.GenerateEffect(EffectManager.BulletDisappearFxIndex, transform.position);
        go.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
        Disapper();

        return true;
    }

    private void OnTriggerEnter(Collider other)
    {
        int colliderLayer = other.gameObject.layer;
        if (colliderLayer != LayerMask.NameToLayer("Enemy") && colliderLayer != LayerMask.NameToLayer("Player"))
        {
            return;
        }
        OnBulletCollision(other);   
    }
    bool ProcessDisappearCondition()
    {
        if(transform.position.x > 15.0f || transform.position.x < -15.0f|| transform.position.y > 15.0f || transform.position.y < -15.0f)
        {
            Disapper();
            return true;
        }
        else if(Time.time - FiredTime > LifeTime)
        {
            Disapper();
            return true;
        }
            return false;
    }
    protected void Disapper()
    {
        SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().BulletManager.Remove(this);
    }
    [ClientRpc]
    public void RpcSetActive(bool value)
    {
        this.gameObject.SetActive(value);
        base.SetDirtyBit(1);
    }
    public void SetPosition(Vector3 position)
    {
        if (isServer)
        {
            RpcSetPosition(position);
        }
        else
        {
            CmdSetPosition(position);
            if(isLocalPlayer)
                transform.position = position;
        }
    }
    [Command]
    public void CmdSetPosition(Vector3 position)
    {
        this.transform.position = position;
        base.SetDirtyBit(1);
    }
    [ClientRpc]
    public void RpcSetPosition(Vector3 position)
    {
        this.transform.position = position;
        base.SetDirtyBit(1);
    }
   
}
