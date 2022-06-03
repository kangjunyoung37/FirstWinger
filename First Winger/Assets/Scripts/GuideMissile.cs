using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GuideMissile : Bullet
{
    const float ChaseFector = 1.5f;//���� ��ȯ ����
    const float ChasingStartTime = 0.7f;//Ÿ�� ���� ���۽ð�
    const float ChasingEndTime = 4.5f;//Ÿ�� ���� ����ð�


    [SyncVar]
    [SerializeField]
    int TargetInstanceID;

    [SerializeField]
    Vector3 ChaseVector;
    [SerializeField]
    Vector3 rotateVector = Vector3.zero;

    [SerializeField]
    bool FlipDirection = true;

    bool needChase = true;

    public void FireChase(int targetIntanceID, int ownerInstanceID, Vector3 direction, float speed, int damage)
    {
        if (!isServer)
            return;
        RpcSetTargetInstanceID(targetIntanceID);//Host�÷��̾��� ���
        base.Fire(ownerInstanceID,direction,speed,damage);
    }
    [ClientRpc]
    public void RpcSetTargetInstanceID(int targetIntanceID)
    {
        TargetInstanceID = targetIntanceID;
        base.SetDirtyBit(1);

    }
    protected override void UpdateTransform()
    {

        UpdateMove();
        UpdateRotate();
    }

    protected override void UpdateMove()
    {
        if (!NeedMove)
            return;
        Vector3 moveVector = MoveDirection.normalized * Speed * Time.deltaTime;

        float deltaTime = Time.time - FiredTime;
        if(deltaTime > ChasingStartTime && deltaTime < ChasingEndTime)
        {
            Actor target = SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().ActorManager.GetActor(TargetInstanceID);
            if(target != null)
            {
                Vector3 targetVector = target.transform.position - transform.position;//Ÿ�ϱ����� ����

                ChaseVector = Vector3.Lerp(moveVector.normalized, targetVector.normalized, ChaseFector * Time.deltaTime);//�̵� ���Ϳ� Ÿ�� ���� ������ ���� ���
                moveVector += ChaseVector.normalized;//�̵����Ϳ� �������͸� ����
                moveVector = moveVector.normalized * Speed * Time.deltaTime;//���ǵ忡 ���� ���̸� �ٽ� ���

                MoveDirection = moveVector.normalized;
            }
        }
        moveVector = AdjustMove(moveVector);
        transform.position += moveVector;

        //moveVector�������� ȸ����Ű��
        rotateVector.z = Vector2.SignedAngle(Vector2.right, moveVector);
        if (FlipDirection)
            rotateVector.z += 180.0f;


    }
    void UpdateRotate()
    {
        Quaternion quat = Quaternion.identity;
        quat.eulerAngles = rotateVector;
        transform.rotation = quat;
    }
}
