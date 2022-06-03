using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class SceneNameConstants
{
    public static string TitleScene = "TitleScene";
    public static string LoadingScene = "LoadingScene";
    public static string InGame = "InGame";

}
public class SceneController : MonoBehaviour
{
    private static SceneController instance;
    public static SceneController Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject go = GameObject.Find("Scenecontroller");
                if(go == null)
                {
                    go = new GameObject("SceneController");
                    SceneController controller = go.AddComponent<SceneController>();
                    return controller;
                }
            }
            return instance;
        }
    }

    void Awake()
    {
        if(instance != null)
        {
            Debug.LogWarning("Can't have two instance of singletone");
            DestroyImmediate(this);
            return;
        }
        DontDestroyOnLoad(this);
    }
    void Start()
    {
        //��ȭ�� ���� �ٸ� �̺�Ʈ �޼ҵ� ����

        SceneManager.activeSceneChanged += OnActiveSceneChanaged;
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    void Update()
    {
        
    }


    //���� ��ũ���� Unload�ϰ� �ε�
    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneAsync(sceneName, LoadSceneMode.Single));//scene �ϳ��� �ؼ� �ε�
    }

    //���� ��ũ���� ��ε� ���� �ε�
    public void LoadSceneAdditive(string sceneName)
    {
        StartCoroutine(LoadSceneAsync(sceneName, LoadSceneMode.Additive));//scene�� �������� �ؼ� �ε�
    }

    IEnumerator LoadSceneAsync(string sceneName , LoadSceneMode loadSceneMode)//�񵿱� �ε�
    {
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName, loadSceneMode);

        while (!asyncOperation.isDone)
        {
            yield return null;  
        }
        Debug.Log("LozdSceneAsync is complete");
    }

    public void OnActiveSceneChanaged(Scene scene0 , Scene scene1)
    {
        Debug.Log("OnActiveSceneChagged is called! " + scene0.name + scene1.name);


    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        Debug.Log("OnSceneLoaded is called" + scene.name+ loadSceneMode.ToString());
        BaseSceneMain baseSceneMain = GameObject.FindObjectOfType<BaseSceneMain>();
        Debug.Log("OnSceneLoaded! " + baseSceneMain.name);
        SystemManager.Instance.CurrentSceneMain = baseSceneMain;
    }

    public void OnSceneUnloaded(Scene scene)
    {
        Debug.Log("OnSceneUnloaded is called" + scene.name);
    }
    public void LoadSceneImmediate(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
