using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ItemBox : NetworkBehaviour
{
    const int HPRecoveryValue = 20;
    const int ScoreAddValue = 100;
    public enum ItemEffect : int
    {
        HPRecovery = 0,
        ScoreAdd = 1,
        UsableItemAdd,
    }
    [SerializeField]
    ItemEffect itemEffect = ItemEffect.HPRecovery;
    
    [SerializeField]
    Transform selfTransform;
    
    [SerializeField]
    Vector3 RotateAngle = new Vector3(0.0f, 0.5f, 0.0f);
   
    [SyncVar]
    [SerializeField]
    string filePath;
    public string FilePath
    {
        get { return filePath; }
        set { filePath = value; }
    }
    [SerializeField]
    Vector3 MoveVector = Vector3.zero;
    void Start()
    {
        if (!((FWNetworkManager)FWNetworkManager.singleton).isServer)
        {
            InGameSceneMain inGameSceneMain = SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>();
            transform.SetParent(inGameSceneMain.ItemBoxManager.transform);
            inGameSceneMain.ItemBoxCacheSystem.Add(FilePath, gameObject);
            gameObject.SetActive(false);
        }
    }

    
    void Update()
    {
        UpdateMove();
        UpdateRotate();
    }
    void UpdateRotate()
    {
        Vector3 eulerAnlges = selfTransform.localRotation.eulerAngles;
        eulerAnlges += RotateAngle;
        selfTransform.Rotate(RotateAngle, Space.Self);
    }
    void UpdateMove()
    {
        selfTransform.position += MoveVector * Time.deltaTime;

    }

    [ClientRpc]
    public void RpcSetActive(bool value)
    {
        this.gameObject.SetActive(value);
        base.SetDirtyBit(1);
    }

    [ClientRpc]
    public void RpcSetPosition(Vector3 position)
    {
        this.transform.position = position;
        base.SetDirtyBit(1);
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer != LayerMask.NameToLayer("Player"))
        {
            return;
        }
        OnItemCollision(other);
    }
    void OnItemCollision(Collider other)
    {
        Player player = other.GetComponentInParent<Player>();
        if(player == null)
        {
            return;

        }
        if (player.IsDead)
        {
            return;
        }
        if(player.isLocalPlayer)
        {
            switch(itemEffect)
            {
                case ItemEffect.HPRecovery:
                    player.IncreaseHP(HPRecoveryValue);
                    break;
                case ItemEffect.ScoreAdd:
                    InGameSceneMain inGamSceneMain = SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>();
                    inGamSceneMain.GamePointAccumlator.Accumulate(ScoreAddValue);
                    break;
                case ItemEffect.UsableItemAdd:
                    player.IncreaseUsableItem();
                    break;
            }
            Diappear();
        }
    }
    void Diappear()
    {
        SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().ItemBoxManager.Remove(this);
    }
}
