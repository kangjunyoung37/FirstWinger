using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

[System.Serializable]
[StructLayout(LayoutKind.Sequential,CharSet = CharSet.Ansi)]//���������� Charset�� �Ƚ�������
public struct EnemyStruct
{
    public int index;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MarshalTableConstant.charBufferSize)]//MarshalTableConstant���� ������ ���ۻ������ ����
    public string FilePath;
    public int MaxHP;
    public int Damage;
    public int CrashDamage;
    public int BulletSpeed;
    public int FireReaminCount;
    public int GamePoint;
}

public class EnemyTable : TableLoader<EnemyStruct>//public class TableLoader<TMarshalStruct> : MonoBehaviour ���̺� �δ��� Ŭ������ ���
{
    Dictionary<int, EnemyStruct> tableDatas = new Dictionary<int, EnemyStruct>();

    void Start()
    {
        Load();   
    }

    protected override void AddData(EnemyStruct data)
    {
        Debug.Log("data.index = " + data.index);
        Debug.Log("data.FilePath = " + data.FilePath);
        Debug.Log("data.MaxHP" + data.MaxHP);
        Debug.Log("data.Damage" + data.Damage);
        Debug.Log("data.CrashDamage" + data.CrashDamage);
        Debug.Log("data.FireRemainCount" + data.FireReaminCount);
        Debug.Log("data.Gamepoint" + data.GamePoint);

        tableDatas.Add(data.index, data);

    }

    public EnemyStruct GetEnemy(int index)
    {
        if(!tableDatas.ContainsKey(index))
        {
            Debug.LogError("GetEnemy Error" + index);
            return default(EnemyStruct);
        }

        return tableDatas[index];
    }


}
