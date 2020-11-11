using System;
using System.Collections;
using System.Collections.Generic;
using BNG;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

[RequireComponent(typeof(JsonController))]
[RequireComponent(typeof(CDiseaseMgr))]
public class GameManager : CEvenetManager
{
    public static GameManager Manager;
    [SerializeField]private JsonController.StageDB stageDB;

    public JsonController.StageDB StageDatabase
    {
        get
        {
            return stageDB;
        }
    }

    public GameObject guestPrefab;
    public Transform guestInitialPosition;
    public Transform guestTargetPosition;
    private GameObject _currentGuest;
    public UnityEvent onStageStart;

    [HideInInspector]public int lastSelectedStage;
    [HideInInspector] public float timeAtStageStart;
    private bool m_IsStageStared = false;
    public bool IsStageStarted
    {
        get
        {
            return m_IsStageStared;
        }
    }

    #region DISEASE_MANAGER_VARIABLES
    private CDiseaseMgr m_DiseaseMgr;
    public CDiseaseMgr GetDiseaseMgr
    {
        get
        {
            //Find disease manager and cache it if m_DiseaseMgr is null.
            if (m_DiseaseMgr == null)
            {
                GameObject[] obj = GameObject.FindGameObjectsWithTag("GameManager");
                if (obj.Length == 0)
                {
                    Debug.LogError("[GameManager] No object with tag \"GameManager\".");
                    return null;
                }

                if (obj.Length > 1) Debug.LogError("[GameManager] There are more than 1 game manager.");
                CDiseaseMgr mgr = obj[0].GetComponent<CDiseaseMgr>();
                if (mgr != null)
                {
                    m_DiseaseMgr = mgr;
                }
                else
                {
                    Debug.LogError(
                        "[GameManager] The object that with tag named \"GameManager\" has no CDiseaseMgr component.");
                    return null;
                }
            }
            
            return m_DiseaseMgr;
        }
    }
    #endregion

    public string sceneNameHospital;

    public bool testDontLoadScene;

    private void Awake()
    {
        if(Manager == null) Manager = this;
        DontDestroyOnLoad(gameObject);
    }

    private IEnumerator Start()
    {
        if (!testDontLoadScene)
        {
            yield return StartCoroutine(LoadSceneAdditive(sceneNameHospital));
        }
        
        stageDB = GetComponent<JsonController>().LoadStageDB();
        
    }
    
    #if UNITY_EDITOR
    [ContextMenu("Guest Test")]
    public void GuestTest()
    {
        StartStage(0);
    }
    
    #endif
    
    
    

    //######################################
    #region MainLobby

    public enum LobbyBtns
    {
        Start,
        Option,
        Quit
    }

    public void QuitApplication()
    {
        #if UNITY_ANDROID
        Application.Quit();
        #endif
        
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
    #endregion

    public CSubtitleMgr subtitle;//임시
    //from TableSMS(class)
    public void StartStage(int stageNum)
    {
        print("GameManager / StageStart("+stageNum+")");
        lastSelectedStage = stageNum;
        _currentGuest = Instantiate(guestPrefab, guestInitialPosition.position, guestInitialPosition.rotation);
        _currentGuest.SetActive(true);
        _currentGuest.GetComponent<GuestAi>().OnInstantiate(guestTargetPosition, subtitle, stageDB.StageInfos[stageNum].customerTalkScript);
    }

    #region MEDICAL_SCENE

    public void StartCurrentMedicalScene()
    {
        StartMedicalScene(stageDB.StageInfos[lastSelectedStage].levelData[0]);
    }
    
    private void StartMedicalScene(string _stageName)
    {
        Debug.Log("[GameManager] Medical stage has started");
        StartCoroutine(LoadMedicalScene(_stageName));
    }

    public void EndMedicalScene(string _stageName)
    {
        Debug.Log("[GameManager] Medical stage has ended");
        StartCoroutine(UnloadMedicalScene(_stageName));
    }

    IEnumerator LoadMedicalScene(string _stageName)
    {
        m_IsStageStared = true;
        OnSceneBeginLoad();
        
        var controller = InputBridge.Instance.CameraFadeController;
        controller.FadeOut();
        yield return new WaitForSeconds(3);

        yield return StartCoroutine(LoadSceneAdditive(_stageName));
        
        var startPos = GameObject.FindWithTag("MedicalStartPosition");

        if (startPos)
        {
            Debug.Log("Initialize position at start position.");
            InputBridge.Instance.transform.GetChild(0).position = startPos.transform.position;
        }
        
        OnSceneLoaded();
        Destroy(_currentGuest); //Remove guest prefab
        
        controller.OnLevelFinishedLoading(0);
        timeAtStageStart = Time.time;
        
        yield return new WaitForSeconds(3);
    }

    IEnumerator UnloadMedicalScene(string _stageName)
    {
        OnSceneBeginUnload();

        var controller = InputBridge.Instance.CameraFadeController;
        controller.FadeOut();
        yield return new WaitForSeconds(3);

        yield return StartCoroutine(UnloadScene(_stageName));
        
        var startPos = GameObject.FindWithTag("SceneStartPosition");

        if (startPos)
        {
            Debug.Log("Initialize position at start position.");
            InputBridge.Instance.transform.GetChild(0).position = startPos.transform.position;
        }
        
        OnSceneUnloaded();
        UnloadTools();
        GameObject.FindWithTag("Tablet").transform.position = GameObject.Find("TabletPosition").transform.position;
        m_IsStageStared = false;
        
        controller.OnLevelFinishedLoading(0);
        
        yield return new WaitForSeconds(3);
    }

    void UnloadTools()
    {
        string[] tags = { "FractureTool", "Lantern", "Syringe", "Suction" };
        foreach (string tag in tags)
        {
            Destroy(GameObject.FindGameObjectWithTag(tag));
        }

        GameObject[] cylinders = GameObject.FindGameObjectsWithTag("Cylinder");
        foreach (GameObject t in cylinders)
        {
            Destroy(t);
        }

        CDestroyOnUnloadScene[] destroyTargets = Resources.FindObjectsOfTypeAll<CDestroyOnUnloadScene>();
        foreach (CDestroyOnUnloadScene t in destroyTargets)
        {
            Destroy(t.gameObject);
        }
    }
    #endregion

    #region UN/LOAD_SCENE
    IEnumerator LoadSceneAdditive(string _stageName)
    {
        Time.timeScale = 0;
        Vector3 gravity = Physics.gravity;
        Physics.gravity = Vector3.zero;
        
        var loadSceneAsync = SceneManager.LoadSceneAsync(_stageName, LoadSceneMode.Additive);
        while (!loadSceneAsync.isDone)
        {
            yield return null;
        }

        var newScene = SceneManager.GetSceneAt(0);
        SceneManager.SetActiveScene(newScene);

        Time.timeScale = 1;
        Physics.gravity = gravity;
    }

    IEnumerator UnloadScene(string _stageName)
    {
        Time.timeScale = 0;
        Vector3 gravity = Physics.gravity;
        Physics.gravity = Vector3.zero;

        var unloadSceneAsync = SceneManager.UnloadSceneAsync(_stageName, UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);
        while (!unloadSceneAsync.isDone)
        {
            yield return null;
        }

        var newScene = SceneManager.GetSceneAt(0);
        SceneManager.SetActiveScene(newScene);

        Time.timeScale = 1;
        Physics.gravity = gravity;
    }
    #endregion
}
