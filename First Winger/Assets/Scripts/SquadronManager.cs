using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class SquadronManager : MonoBehaviour
{

    float GameStartTime;

    int SquadronIndex;

    [SerializeField]
    SquadronTable[] squadronDatas;

    [SerializeField]
    SquadronScheduleTable squadronScheduleTable;


    bool running = false;

    void Start()
    {
        squadronDatas = GetComponentsInChildren<SquadronTable>();
        for(int i = 0; i < squadronDatas.Length; i++)
        {
            squadronDatas[i].Load();

            squadronScheduleTable.Load();
        }
    }

 
    void Update()
    {

        CheckSquadronGenerateings();
    }
    public void StartGame()
    {
        GameStartTime = Time.time;
        SquadronIndex = 0;
        running = true;
        Debug.Log("Start");

    }
    void CheckSquadronGenerateings()
    {
        if (!running)
        {
            return;
        }
        if(Time.time - GameStartTime >= squadronScheduleTable.GetSquadronScheduleDataStruct(SquadronIndex).GenerateTime)
        {
            GenerateSquadron(squadronDatas[SquadronIndex]);
            SquadronIndex += 1;
            if(SquadronIndex >= squadronDatas.Length)
            {
                AllSquadronGenerated();
                return;
            }
        }
    }
    void GenerateSquadron(SquadronTable table)
    {
        Debug.Log("GenerateSquadron");
        for(int i = 0; i < table.GetCount(); i++)
        {
            SquadronStruct squardronMember = table.GetSquadron(i);
            SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().EnemyManager.GenerateEnemy(squardronMember);
        }
    }

    void AllSquadronGenerated()
    {
        Debug.Log("AllSqadronGenerateed");
        running = false;
    }

}
