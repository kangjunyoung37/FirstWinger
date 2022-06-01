using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Bomb : Bullet
{
    const float MaxRotateTime = 30.0f;
    const float MaxRotateZ = 90.0f;

    [SerializeField]
    Rigidbody selfRigidbody;

    [SyncVar]
    float RotateStartTime = 0.0f;

    [SyncVar]
    [SerializeField]
    float CurrentRotateZ;

    Vector3 currentEulerAngles = Vector3.zero;

    [SerializeField]
    Vector3 Force;

    [SerializeField]
    SphereCollider ExplodeArea;
    protected override void UpdateTransform()
    {
        if (!NeedMove)
            return;
        if (CheckScreenBottom())
            return;
        UpdateRotate();
    }
    bool CheckScreenBottom()
    {
        Transform mainBGQuadTransform = SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().MainBGQuadTransform;
        if(transform.position.y < -mainBGQuadTransform.localScale.y * 0.5f)
        {
            Vector3 newPos = transform.position;
            newPos.y = -mainBGQuadTransform.localScale.y * 0.5f;
            transform.position = newPos;

            StopForExplosion(newPos);
            Explode();
            //selfRigidbody.useGravity = false;
            //selfRigidbody.velocity = Vector3.zero;
            //NeedMove = false;
            return true;
        }
        return false;
    }
    void StopForExplosion(Vector3 stopPos)
    {
        transform.position = stopPos;
        selfRigidbody.useGravity = false; //�߷� ����
        selfRigidbody.velocity = Vector3.zero;//�ӵ� ����
        NeedMove = false;//������ false
    }
    void UpdateRotate()
    {
        CurrentRotateZ = Mathf.Lerp(CurrentRotateZ, MaxRotateZ, (Time.time - RotateStartTime) / MaxRotateTime);
        currentEulerAngles.z  = -CurrentRotateZ;

        Quaternion rot = Quaternion.identity;
        rot.eulerAngles = currentEulerAngles;
        transform.localRotation = rot;
    }


    public override void Fire(int ownerIntanceID, Vector3 firePosition, Vector3 direction, float speed, int damage)
    {
       base.Fire(ownerIntanceID, firePosition, direction, speed, damage);
        
        AddForce(Force);
    }
    void InternelAddForce(Vector3 force)
    {

        selfRigidbody.velocity = Vector3.zero;//�ʱ�ȭ�۾�
        selfRigidbody.AddForce(force);//�ʱ�ȭ�۾�
        RotateStartTime = Time.time;//�ʱ�ȭ�۾�
        CurrentRotateZ = 0.0f;//�ʱ�ȭ�۾�
        transform.localRotation = Quaternion.identity;
        selfRigidbody.useGravity = true;
        ExplodeArea.enabled = false;
    }
    void AddForce(Vector3 force)
    {
        if (isServer)
        {
            RpcAddForce(force);
        }
        else
        {
            CmdAddForce(force);
            if (isLocalPlayer)
                selfRigidbody.AddForce(force);
        }
    }
    [Command]
    public void CmdAddForce(Vector3 force)
    {
        InternelAddForce(force);

        base.SetDirtyBit(1);
    }
    [ClientRpc]
    public void RpcAddForce(Vector3 force)
    {
        InternelAddForce(force);

        base.SetDirtyBit(1);
    }

    void InternelExplode()
    {
        Debug.Log("InternelExplode is called");
        GameObject go = SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().EffectManager.GenerateEffect(EffectManager.BombExpodeFxIndex, transform.position);
        ExplodeArea.enabled = true;
        List<Enemy> targetList = SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().EnemyManager.GetContainEnemies(ExplodeArea);
        for(int i = 0; i <targetList.Count; i++)
        {
            if (targetList[i].IsDead)
                continue;
            targetList[i].OnBulletHited(Damage, targetList[i].transform.position);
        }
        if (gameObject.activeSelf)//active�� �����ִ���
            Disapper();
    }
    void Explode()
    {
        if(isServer)
        {
            RpcExplode();
        }
        else
        {
            CmdExplode();
            if (isLocalPlayer)
                InternelExplode();
        }
    }
    [Command]
    void CmdExplode()
    {
        InternelExplode();
        base.SetDirtyBit(1);
    }
    [ClientRpc]
    void RpcExplode()
    {
        InternelExplode();
        base.SetDirtyBit(1);
    }
    protected override bool OnBulletCollision(Collider collider)
    {
        if(!base.OnBulletCollision(collider))
        {
            return false;

        }
        Explode();
        return true;
    }
}
