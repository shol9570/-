using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using BNG;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CToolSpawner : MonoBehaviour
{
    #region STRUCT
    [System.Serializable]
    public struct SpawnList
    {
        public GameObject target;
        public GameObject previewObj;
        public string name;
        public bool isNewOne;
        public int spawnLimitCount;
        public bool isAble;

        public SpawnList(GameObject _target, GameObject _previewObj, string _name, bool _isNewOne = false, int _spawnLimitCount = 0, bool _isAble = true)
        {
            target = _target;
            previewObj = _previewObj;
            name = _name;
            isNewOne = _isNewOne;
            spawnLimitCount = _spawnLimitCount;
            isAble = _isAble;
        }
    }
    #endregion

    public SpawnList[] m_SpawnList;
    public AudioClip m_SpawnSound;
    public AudioClip m_SpawnErrorSound;
    public AudioSource m_SpawnSoundSource;
    public Transform m_SpawnPoint;
    
    #region DISPLAY

    [Space] public TextMeshProUGUI m_ObjectNameTxt;
    private GameObject m_CurrentDisplayedObj;
    public Material m_HologramMat;
    public Material m_SpawnEffectMat;
    #endregion

    [HideInInspector] public bool m_IsPlayerDetected = false;

    private int m_Current = 0; //Seleted object index in list
    private bool m_IsSpawning = false;
    private float m_LastPressTime = 0f;
    private List<GameObject[]> m_ObjectPool;
    private int[] m_LastObjectPoolIndex;

    void Start()
    {
        DisplaySettingByIndex(m_Current);
        m_SpawnPoint.gameObject.SetActive(false);
        ObjectPooling();
        //OnPlayerDetected();
    }

    //Object pooling
    void ObjectPooling()
    {
        m_ObjectPool = new List<GameObject[]>();
        m_LastObjectPoolIndex = new int[m_SpawnList.Length];
        for (int i = 0; i < m_SpawnList.Length; i++)
        {
            GameObject[] pool;
            if (m_SpawnList[i].isNewOne)
            {
                pool = new GameObject[m_SpawnList[i].spawnLimitCount + 1];
                for (int j = 0; j < m_SpawnList[i].spawnLimitCount + 1; j++)
                {
                    GameObject target = Instantiate(m_SpawnList[i].target);
                    pool[j] = target;
                    target.SetActive(false);
                }
            }
            else
            {
                pool = new GameObject[0];
            }
            m_ObjectPool.Add(pool);
        }
    }

    void Update()
    {
        if (m_IsPlayerDetected && !m_IsSpawning) DisplayHologram();
    }

    void DisplayHologram()
    {
        m_SpawnPoint.gameObject.SetActive(true);
        m_CurrentDisplayedObj.transform.Rotate(0f, 180f * Time.deltaTime, 0f, Space.World);
    }

    void DisplaySettingByIndex(int _idx)
    {
        if (m_CurrentDisplayedObj != null) Destroy(m_CurrentDisplayedObj);
        
        //Create hologramObject
        m_CurrentDisplayedObj = Instantiate(m_SpawnList[_idx].previewObj, m_SpawnPoint);
        MeshRenderer[] renderers = m_CurrentDisplayedObj.GetComponentsInChildren<MeshRenderer>();
        for (int i = 0; i < renderers.Length; i++)
        {
            Material[] replacedMats = new Material[renderers[i].materials.Length];
            for (int j = 0; j < replacedMats.Length; j++)
            {
                replacedMats[j] = m_HologramMat;
            }
            renderers[i].materials = replacedMats;
        }
        
        //Display name
        m_ObjectNameTxt.text = m_SpawnList[_idx].name;
    }

    void AddListIndex(int _i)
    {
        m_Current += _i;
        if (m_Current >= m_SpawnList.Length) m_Current -= m_SpawnList.Length;
        if (m_Current < 0) m_Current += m_SpawnList.Length;
    }

    public void OnPlayerDetected()
    {
        m_IsPlayerDetected = true;
        m_SpawnPoint.gameObject.SetActive(true);
        m_ObjectNameTxt.enabled = true;
    }

    public void OnPlayerMissed()
    {
        m_IsPlayerDetected = false;
        m_SpawnPoint.gameObject.SetActive(false);
        m_ObjectNameTxt.enabled = false;
    }

    public void OnNextButtonDown()
    {
        //Return when object is spawning
        if (m_IsSpawning) return;
        
        //Time limit
        if (Time.time - m_LastPressTime < 0.3f) return;
        m_LastPressTime = Time.time;
        
        //print("Next button down");
        AddListIndex(1);
        DisplaySettingByIndex(m_Current);
    }

    public void OnNextButtonUp()
    {
        //Return when object is spawning
        if (m_IsSpawning) return;

        //Time limit
        if (Time.time - m_LastPressTime < 0.3f) return;
        m_LastPressTime = Time.time;

        //print("Next button up");
    }

    public void OnPreviousButtonDown()
    {
        //Return when object is spawning
        if (m_IsSpawning) return;

        //Time limit
        if (Time.time - m_LastPressTime < 0.3f) return;
        m_LastPressTime = Time.time;

        //print("Previous button down");
        AddListIndex(-1);
        DisplaySettingByIndex(m_Current);
    }

    public void OnPreviousButtonUp()
    {
        //Return when object is spawning
        if (m_IsSpawning) return;

        //Time limit
        if (Time.time - m_LastPressTime < 0.3f) return;
        m_LastPressTime = Time.time;

        //print("Previous button up");
    }

    public void OnSpawnButtonDown()
    {
        //Return when object is spawning
        if (m_IsSpawning) return;

        //Time limit
        if (Time.time - m_LastPressTime < 0.3f) return;
        m_LastPressTime = Time.time;

        //print("Spawn button down");
        if (!m_IsSpawning) StartCoroutine(SpawnObject());
    }

    public void OnSpawnButtonUp()
    {
        //Return when object is spawning
        if (m_IsSpawning) return;

        //Time limit
        if (Time.time - m_LastPressTime < 0.3f) return;
        m_LastPressTime = Time.time;

        //print("Spawn button up");
    }

    IEnumerator SpawnObject()
    {
        if (!m_SpawnList[m_Current].isAble || (m_SpawnList[m_Current].target.transform.parent != null && !m_SpawnList[m_Current].target.transform.parent.name.Contains("Suction Plate (Tool set)")))
        {
            m_SpawnSoundSource.PlayOneShot(m_SpawnErrorSound);
            yield break;
        }
        
        m_IsSpawning = true;
        
        //Play spawn sound
        m_SpawnSoundSource.PlayOneShot(m_SpawnSound, 1f);
        
        //If is creating new one
        if (m_SpawnList[m_Current].isNewOne)
        {
            #region ANIMATION

            int activatingObj = 0;
            for (int i = 0; i < m_ObjectPool[m_Current].Length; i++)
            {
                if (m_ObjectPool[m_Current][i].active) activatingObj++;
            }
            
            int lostTargetIndex = m_LastObjectPoolIndex[m_Current] + 1;
            if (lostTargetIndex >= m_ObjectPool[m_Current].Length) lostTargetIndex = 0;
            GameObject lostTarget = m_ObjectPool[m_Current][lostTargetIndex];

            CRespawnable respawnable = m_CurrentDisplayedObj.GetComponent<CRespawnable>();
            CRespawnable lostTargetRespawnable = lostTarget.GetComponent<CRespawnable>();
            if (respawnable == null || lostTargetRespawnable == null)
            {
                Debug.LogError("No CRespawnable");
                yield break;
            }
            
            MeshRenderer[] targetRenderers = respawnable.m_Models;
            MeshRenderer[] lostTargetRenderers = lostTargetRespawnable.m_Models;
            List<Material[]> targetOrinMats = new List<Material[]>();
            List<Material[]> lostTargetOrinMats = new List<Material[]>();
            for (int i = 0; i < targetRenderers.Length; i++)
            {
                Material[] orinMats = targetRenderers[i].materials;
                targetOrinMats.Add(orinMats);
                Material[] replacedMats = new Material[orinMats.Length + 1];
                for (int j = 0; j < orinMats.Length; j++)
                {
                    replacedMats[j] = orinMats[j];
                }

                replacedMats[orinMats.Length] = m_SpawnEffectMat;
                targetRenderers[i].materials = replacedMats;
                if(respawnable.m_OrinTexture != null) targetRenderers[i].materials[orinMats.Length].mainTexture = respawnable.m_OrinTexture;
                targetRenderers[i].materials[orinMats.Length].color = respawnable.m_OrinColor;
            }

            if (lostTarget.active)
            {
                for (int i = 0; i < lostTargetRenderers.Length; i++)
                {
                    Material[] orinMats = lostTargetRenderers[i].materials;
                    lostTargetOrinMats.Add(orinMats);
                    Material[] targetReplacedMats = new Material[1];
                    targetReplacedMats[0] = m_SpawnEffectMat;
                    lostTargetRenderers[i].materials = targetReplacedMats;
                    if (lostTargetRespawnable.m_OrinTexture != null)
                        lostTargetRenderers[i].materials[0].mainTexture = lostTargetRespawnable.m_OrinTexture;
                    lostTargetRenderers[i].materials[0].color = lostTargetRespawnable.m_OrinColor;
                }
            }

            float blend = 0;
            while (blend < 1)
            {
                blend += Time.deltaTime * 2f;
                blend = Mathf.Min(1f, blend);
                for (int i = 0; i < targetRenderers.Length; i++)
                {
                    targetRenderers[i].materials[targetRenderers[i].materials.Length - 1].SetFloat("_Blend", blend);
                }

                if (lostTarget.active)
                {
                    for (int i = 0; i < lostTargetRenderers.Length; i++)
                    {
                        lostTargetRenderers[i].material.SetFloat("_Blend", 1f - blend);
                        print(lostTargetRenderers[i].material.GetFloat("_Blend"));
                    }
                }

                yield return null;
            }
            #endregion
            
            //Replace hologram materials to original material and hide
            for (int i = 0; i < targetRenderers.Length; i++)
            {
                targetRenderers[i].materials = targetOrinMats[i];
                targetRenderers[i].enabled = false;
            }
            
            //Disable old spawned object and replace materials to original material
            if (lostTarget.active)
            {
                for (int i = 0; i < lostTargetRenderers.Length; i++)
                {
                    lostTargetRenderers[i].materials = lostTargetOrinMats[i];
                }
            }
            lostTarget.SetActive(false); //Disable old spawned object
            
            //Create
            GameObject spawned = m_ObjectPool[m_Current][m_LastObjectPoolIndex[m_Current]];
            spawned.transform.position = m_CurrentDisplayedObj.transform.position;
            spawned.transform.rotation = m_CurrentDisplayedObj.transform.rotation;
            spawned.GetComponentInChildren<CLiquid>().m_LiquidRemain = 1f;
            spawned.SetActive(true);
            
            //Add index
            m_LastObjectPoolIndex[m_Current] += 1;
            if (m_LastObjectPoolIndex[m_Current] >= m_ObjectPool[m_Current].Length)
                m_LastObjectPoolIndex[m_Current] = 0;

            //Unhide preview after 2 seconds
            yield return new WaitForSeconds(2f);
            for (int i = 0; i < targetRenderers.Length; i++)
            {
                targetRenderers[i].enabled = true;
            }
        }
        //if is respawning old one
        else
        {
            GameObject spawnTarget = m_SpawnList[m_Current].target;
            //Cancel if object is grabbed
            if (spawnTarget.transform.parent != null)
            {
                if (spawnTarget.transform.parent.GetComponent<Grabber>() != null) yield break;
            }
            
            #region ANIMATION
            //Disalbe target collision & gravity
            Rigidbody rigi = spawnTarget.GetComponent<Rigidbody>();
            if (rigi)
            {
                rigi.useGravity = false;
                rigi.isKinematic = true;
            }

            CRespawnable holoRespawnable = m_CurrentDisplayedObj.GetComponent<CRespawnable>();
            if (holoRespawnable == null)
            {
                Debug.LogError("No CRespawnable");
                yield break;
            }

            CRespawnable targetRespawnable = spawnTarget.GetComponent<CRespawnable>();
            if (targetRespawnable == null)
            {
                Debug.LogError("No CRespawnable");
                yield break;
            }

            MeshRenderer[] holoRenderers = holoRespawnable.m_Models;
            MeshRenderer[] targetRenderers = targetRespawnable.m_Models;
            List<Material[]> holoOrinMats = new List<Material[]>();
            List<Material[]> targetOrinMats = new List<Material[]>();
            for (int i = 0; i < holoRenderers.Length; i++)
            {
                Material[] orinMats = holoRenderers[i].materials;
                holoOrinMats.Add(orinMats);
                Material[] holoReplacedMats = new Material[orinMats.Length + 1];
                for (int j = 0; j < orinMats.Length; j++)
                {
                    holoReplacedMats[j] = orinMats[j];
                }

                holoReplacedMats[orinMats.Length] = m_SpawnEffectMat;
                holoRenderers[i].materials = holoReplacedMats;
                if(holoRespawnable.m_OrinTexture != null) holoRenderers[i].materials[orinMats.Length].mainTexture = holoRespawnable.m_OrinTexture;
                holoRenderers[i].materials[orinMats.Length].color = holoRespawnable.m_OrinColor;
            }
            for (int i = 0; i < targetRenderers.Length; i++)
            {
                Material[] orinMats = targetRenderers[i].materials;
                targetOrinMats.Add(orinMats);
                Material[] targetReplacedMats = new Material[1];
                targetReplacedMats[0] = m_SpawnEffectMat;
                targetRenderers[i].materials = targetReplacedMats;
                if(targetRespawnable.m_OrinTexture != null) targetRenderers[i].materials[0].mainTexture = targetRespawnable.m_OrinTexture;
                targetRenderers[i].materials[0].color = targetRespawnable.m_OrinColor;
            }
            
            float blend = 0;
            while (blend < 1)
            {
                blend += Time.deltaTime * 2f;
                blend = Mathf.Min(1f, blend);
                for (int i = 0; i < holoRenderers.Length; i++)
                {
                    holoRenderers[i].materials[holoRenderers[i].materials.Length - 1].SetFloat("_Blend", blend);
                }
                for (int i = 0; i < targetRenderers.Length; i++)
                {
                    targetRenderers[i].material.SetFloat("_Blend", 1f - blend);
                }
                yield return null;
            }
            
            //enabled target collision & gravity
            if (rigi)
            {
                rigi.useGravity = true;
                rigi.isKinematic = false;
            }
            #endregion

            //Replace hologram materials to original material and hide
            for (int i = 0; i < holoRenderers.Length; i++)
            {
                holoRenderers[i].materials = holoOrinMats[i];
                holoRenderers[i].enabled = false;
            }
            for (int i = 0; i < targetRenderers.Length; i++)
            {
                targetRenderers[i].materials = targetOrinMats[i];
            }
            
            spawnTarget.transform.position = m_CurrentDisplayedObj.transform.position;
            spawnTarget.transform.rotation = m_CurrentDisplayedObj.transform.rotation;

            //Unhide after 2 seconds
            yield return new WaitForSeconds(2f);
            for (int i = 0; i < targetRenderers.Length; i++)
            {
                holoRenderers[i].enabled = true;
            }
        }
        yield return null;
        m_IsSpawning = false;
    }
}
