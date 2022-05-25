using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

//�о�� ���̺��� �ε� ��Ű�� ��ũ��Ʈ.
public class TableLoader<TMarshalStruct> : MonoBehaviour
{
    [SerializeField]
    protected string FilePath;

    TableRecordParser<TMarshalStruct> tableRecordParser = new TableRecordParser<TMarshalStruct> ();

    public bool Load()
    {
        TextAsset textAsset = Resources.Load<TextAsset>(FilePath);
        if(textAsset == null)
        {
            Debug.LogError("Load Failed" + FilePath);
            return false;
        }
        ParseTable(textAsset.text);

        return true;
    }

    void ParseTable(string text)
    {
        StringReader reader = new StringReader(text); //Systme.IO�� StringReader

        string line = null;
        bool fieldRead = false;
        while((line = reader.ReadLine()) != null) // ���ϳ����� �����鼭 �Ľ�
        {
            
            if (!fieldRead)
            {
                fieldRead = true;
                continue;
            }
            TMarshalStruct data = tableRecordParser.ParseRecordLine(line);//ó���� �� �پ� ����
            
            AddData(data);
        }
    }

    protected virtual void AddData(TMarshalStruct data)
    {

    }
    void Start()
    {
        
    }

    
    void Update()
    {
        
    }
}
