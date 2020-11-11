using System.Collections;
using System.Collections.Generic;
using BNG;
using UnityEngine;

public class CTutorial : CStageEvent
{
    public GameObject m_Lantern;
    public GameObject m_Suction;
    public GameObject m_FractureTool;
    public GameObject m_Syringe;
    public GameObject m_ToolPlate;
    public GameObject m_Tablet;
    public GameObject m_Player;
    public GameObject m_TargetAnimal;
    public CToolSpawner m_ToolSpawner;
    public GameObject m_StatusDisplay;
    public Grabbable m_SuctionSnapZone;
    public Grabbable m_SyringeSnapZone;
    public Grabbable m_FractureToolSnapZone;
    public Grabbable m_LanternSnapZone;
    public Button m_ToolSpawner_Next;
    public Button m_ToolSpawner_Previous;
    public Button m_ToolSpawner_Spawn;
    public Button m_EndButton;
    public GameObject[] m_Arrows;

    private bool m_TutorialStarted = false;

    private void Update()
    {
        if (!m_TutorialStarted) return;
        if (m_TargetAnimal != null)
        {
            if (m_TargetAnimal.GetComponent<CAnimalStatus>().ConditionFigure < 0.3f)
            {
                m_TargetAnimal.GetComponent<CAnimalStatus>().ConditionFigure = 0.3f;
            }
        }
    }

    public override void SceneLoadedEvent()
    {
        base.SceneLoadedEvent();
        StartCoroutine(Tutorial());
    }

    private void DisableAllToolsGrabbables()
    {
        //Disable tools
        m_Lantern.GetComponent<Grabbable>().enabled = false;
        m_Suction.GetComponent<Grabbable>().enabled = false;
        m_FractureTool.GetComponent<Grabbable>().enabled = false;
        m_Syringe.GetComponent<Grabbable>().enabled = false;
        GameObject[] cylinders = GameObject.FindGameObjectsWithTag("Cylinder");
        foreach (GameObject cylinder in cylinders)
        {
            cylinder.GetComponent<Grabbable>().enabled = false;
        }
        //Disable plate grabbable
        Grabbable[] plateGrabbables = m_ToolPlate.GetComponentsInChildren<Grabbable>();
        foreach (Grabbable g in plateGrabbables)
        {
            g.enabled = false;
        }
        //Disable button
        m_ToolSpawner_Next.enabled = false;
        m_ToolSpawner_Previous.enabled = false;
        m_ToolSpawner_Spawn.enabled = false;
        m_EndButton.enabled = false;

        //Disable items of tool spawner
        for (int i = 0; i < m_ToolSpawner.m_SpawnList.Length; i++)
        {
            m_ToolSpawner.m_SpawnList[i].isAble = false;
        }

        //Inactive arrows
        for (int i = 0; i < m_Arrows.Length; i++)
        {
            m_Arrows[i].SetActive(false);
        }
    }
    
    /*
     * 1. 태블릿 소환
     * 2. 태블릿에서 스테이지 설명 확인할 수 있다고 알려줌
     * 3. 체온에 대해서 알려줌
     * 4. 해열제 소환 법 알려줌
     * 5. 주사기 사용 법 알려줌
     * 6. 체온 맞추라고 지시
     * 7. 체온이 너무 낮아지면 안 된다고 알려줌
     * 8. 랜턴 사용 법 알려줌
     * 9. 강아지를 비춰보게 지시
     * 10. 석션 사용 법 알려줌
     * 11. 염증 제거하게 지시
     * 12. 골절 치료 도구 알려줌
     * 13. 골절 치료하게 지시
     * 14. 염증 항생제, 골절 항생제 주입 지시
     * 15. 결과창 확인 지시
     * 16. 로비로 돌아가기
     */
    private IEnumerator Tutorial()
    {
        if (m_TutorialStarted) yield break;
        m_TutorialStarted = true;

        m_Tablet = GameObject.FindWithTag("Tablet");
        m_Player = GameObject.FindWithTag("Player").transform.GetChild(0).gameObject;
        m_TargetAnimal = GameObject.FindWithTag("TargetAnimal");
        DisableAllToolsGrabbables();

        yield return StartCoroutine(Subtitle("ASS(Animal Surgery Simulator)를 플레이 해주셔서 감사합니다.", 5f, "Sounds/Narration/tutorial_00"));
        yield return StartCoroutine(Subtitle("ASS에서는 동물들이 질병에 따라 겪는 증상과 해결 방법을 학습 할 수 있고, 내부를 해부학적으로 탐구해볼 수 있습니다.", 11f, "Sounds/Narration/tutorial_01"));
        yield return StartCoroutine(Subtitle("지금부터 ASS에서의 진단과 진료 방법에 대해서 설명해드리도록 하겠습니다.", 7f, "Sounds/Narration/tutorial_02"));
        yield return StartCoroutine(Subtitle("가장 먼저 왼쪽 손목에 있는 버튼을 누르면 태블릿을 불러올 수 있습니다.", 6f, "Sounds/Narration/tutorial_03"));

        while(Vector3.Distance(m_Player.transform.position, m_Tablet.transform.position) > 1f)
        {
            yield return null;
        }

        yield return StartCoroutine(Subtitle("잘 하셨습니다.", 3f, "Sounds/Narration/tutorial_04"));
        yield return StartCoroutine(Subtitle("태블릿의 스테이지 메뉴에서는 스테이지의 정보를 알 수 있습니다.", 5f, "Sounds/Narration/tutorial_05"));
        yield return StartCoroutine(Subtitle("태블릿의 백과사전에서는 관련 정보를 얻을 수 있습니다.", 5f, "Sounds/Narration/tutorial_06"));
        yield return StartCoroutine(Subtitle("그럼 이제 진료 방법에 대해 알려드리겠습니다.", 4f, "Sounds/Narration/tutorial_07"));
        m_Arrows[0].SetActive(true);
        yield return StartCoroutine(Subtitle("화살표가 가르키는 곳으로 가주세요.", 5f, "Sounds/Narration/tutorial_08"));

        while (Vector3.Distance(m_Player.transform.position, m_StatusDisplay.transform.position) > 2f) {
            yield return null;
        }

        m_TargetAnimal.GetComponent<CAnimalStatus>().IncreaseTemperature(2);
        yield return StartCoroutine(Subtitle("좋습니다.", 3f, "Sounds/Narration/tutorial_09"));
        yield return StartCoroutine(Subtitle("이건 상태 표시기입니다.", 3f, "Sounds/Narration/tutorial_10"));
        yield return StartCoroutine(Subtitle("치료 받는 동물의 상태를 나타내는데", 4f, "Sounds/Narration/tutorial_11"));
        yield return StartCoroutine(Subtitle("상단 게이지는 치료율로 당신은 이걸 100%로 만들어야 합니다.", 6f, "Sounds/Narration/tutorial_12"));
        yield return StartCoroutine(Subtitle("하단 게이지는 동물 컨디션으로 0%가 되면 치료에 실패합니다.", 6f, "Sounds/Narration/tutorial_13"));
        yield return StartCoroutine(Subtitle("중앙에는 동물의 체온을 표시합니다.", 4f, "Sounds/Narration/tutorial_14"));
        yield return StartCoroutine(Subtitle("강아지의 평균 체온은 38.4도인데 이 강아지는 체온이 높군요.", 6.5f, "Sounds/Narration/tutorial_15"));
        yield return StartCoroutine(Subtitle("감기와 같은 질병에 걸리지 않았다면 체온은 서서히 정상 체온으로 돌아옵니다.", 6.5f, "Sounds/Narration/tutorial_16"));
        yield return StartCoroutine(Subtitle("빠르게 체온을 낮추기 위해서는 주사기를 이용해 해열제를 주사하면 됩니다.", 6.5f, "Sounds/Narration/tutorial_17"));
        m_Arrows[0].SetActive(false);
        m_Arrows[1].SetActive(true);
        m_Syringe.GetComponent<Grabbable>().enabled = true;
        Grabbable[] grabs = m_Syringe.GetComponentsInChildren<Grabbable>();
        for(int i = 0; i < grabs.Length; i++)
        {
            grabs[i].enabled = true;
        }
        m_SyringeSnapZone.enabled = true;
        yield return StartCoroutine(Subtitle("그럼 화살표에 표시된 위치의 주사기를 집어주세요.", 5f, "Sounds/Narration/tutorial_18"));

        Grabbable syringeGrabbable = m_Syringe.GetComponent<Grabbable>();
        while(syringeGrabbable.GetPrimaryGrabber() == null)
        {
            yield return null;
        }

        m_Arrows[1].SetActive(false);
        yield return StartCoroutine(Subtitle("주사기는 약물을 주입해주지 않으면 사용이 불가능합니다.", 6f, "Sounds/Narration/tutorial_19"));
        m_Arrows[2].SetActive(true);
        yield return StartCoroutine(Subtitle("약물을 구하기 위해 화살표 위치로 가세요.", 5f, "Sounds/Narration/tutorial_20"));

        while(Vector3.Distance(m_Player.transform.position, m_ToolSpawner.transform.position) > 2f)
        {
            yield return null;
        }

        yield return StartCoroutine(Subtitle("이건 도구 프린터입니다.", 4.5f, "Sounds/Narration/tutorial_21"));
        yield return StartCoroutine(Subtitle("빨간 버튼을 누르면 프린트할 도구를 전환합니다.", 5f, "Sounds/Narration/tutorial_22"));
        yield return StartCoroutine(Subtitle("초록 버튼을 누르면 도구를 프린트합니다.", 5f, "Sounds/Narration/tutorial_23"));
        yield return StartCoroutine(Subtitle("주사기와 해열제 소환을 해제해드렸습니다.", 4.5f, "Sounds/Narration/tutorial_24"));
        m_ToolSpawner_Spawn.enabled = true;
        m_ToolSpawner_Next.enabled = true;
        m_ToolSpawner_Previous.enabled = true;
        m_ToolSpawner.m_SpawnList[2].isAble = true;
        m_ToolSpawner.m_SpawnList[6].isAble = true;
        m_Arrows[2].SetActive(false);
        yield return StartCoroutine(Subtitle("해열제를 소환한 뒤 주사기에 장착해주세요.", 5f, "Sounds/Narration/tutorial_25"));
        yield return StartCoroutine(Subtitle("주사기의 고리부분에 약병을 가져다 놓으면 주사기에 장착됩니다.", 6.5f, "Sounds/Narration/tutorial_26"));

        while(m_Syringe.GetComponent<CSyringeTool>().m_Cylinder == null)
        {
            yield return null;
        }

        yield return StartCoroutine(Subtitle("잘 하셨습니다.", 4f, "Sounds/Narration/tutorial_04"));
        m_Arrows[3].SetActive(true);
        yield return StartCoroutine(Subtitle("강아지의 특정 부위에 주사기를 가져다대면 주사기가 강아지에게 꽂힙니다.", 7f, "Sounds/Narration/tutorial_27"));
        yield return StartCoroutine(Subtitle("강아지에게 주사기가 꽂힌 상태에서 트리거를 당겨주세요.", 6.5f, "Sounds/Narration/tutorial_28"));
        yield return StartCoroutine(Subtitle("주사기에 꽂힌 약병 안의 내용물이 모두 떨어졌을 경우 새로운 약병으로 교체해줘야 합니다.", 7.5f, "Sounds/Narration/tutorial_29"));
        yield return StartCoroutine(Subtitle("이제 주사기를 강아지에게 사용해 열을 낮춰주세요.", 5f, "Sounds/Narration/tutorial_30"));

        while(m_TargetAnimal.GetComponent<CAnimalStatus>().TemperatureFigure > 38.5f)
        {
            yield return null;
        }

        yield return new WaitForSeconds(2f);

        m_Arrows[3].SetActive(false);
        if(m_TargetAnimal.GetComponent<CAnimalStatus>().TemperatureFigure < 37.9f)
        {
            yield return StartCoroutine(Subtitle("체온을 너무 많이 낮췄습니다.", 4f, "Sounds/Narration/tutorial_31"));
            yield return StartCoroutine(Subtitle("강아지에게 특별한 경우가 없다면 비정상적으로 낮아진 체온은 점차 상승합니다.", 8f, "Sounds/Narration/tutorial_32"));
        }
        else
        {
            yield return StartCoroutine(Subtitle("잘 하셨습니다.", 5f, "Sounds/Narration/tutorial_04"));
        }
        yield return StartCoroutine(Subtitle("이제 강아지의 몸 속을 보도록 하겠습니다.", 5f, "Sounds/Narration/tutorial_33"));
        m_ToolSpawner.m_SpawnList[1].isAble = true;
        m_LanternSnapZone.enabled = true;
        m_Lantern.GetComponent<Grabbable>().enabled = true;
        m_Arrows[4].SetActive(true);
        yield return StartCoroutine(Subtitle("표시된 위치로 가서 라이트를 얻으세요.", 6f, "Sounds/Narration/tutorial_34"));

        Grabbable lanternGrabbable = m_Lantern.GetComponent<Grabbable>();
        while(lanternGrabbable.GetPrimaryGrabber() == null)
        {
            yield return null;
        }

        m_Arrows[4].SetActive(false);
        yield return StartCoroutine(Subtitle("좋습니다.", 4f, "Sounds/Narration/tutorial_09"));
        yield return StartCoroutine(Subtitle("라이트를 든 상태에서 들고 있는 컨트롤러의 트리거 버튼을 누르면 키거나 끌 수 있습니다.", 7f, "Sounds/Narration/tutorial_35"));
        yield return StartCoroutine(Subtitle("라이트를 켜보세요.", 3f, "Sounds/Narration/tutorial_36"));

        while (!m_Lantern.GetComponent<CLantern>().Switch)
        {
            yield return null;
        }

        m_Arrows[3].SetActive(true);
        yield return StartCoroutine(Subtitle("이제 라이트가 켜진 상태에서 강아지에게 가져다 대보세요.", 6f, "Sounds/Narration/tutorial_37"));
        
        while(m_Lantern.GetComponent<CLantern>().XRAYEDOBJ == null)
        {
            yield return null;
        }

        m_Arrows[3].SetActive(false);

        yield return StartCoroutine(Subtitle("아주 좋습니다.", 4f, "Sounds/Narration/tutorial_38"));
        yield return StartCoroutine(Subtitle("강아지의 몸 속을 보면 깨진 뼈와 살구색 구체의 염증들이 보일겁니다.", 6.5f, "Sounds/Narration/tutorial_39"));
        yield return StartCoroutine(Subtitle("치료율을 높히기 위해서는 이것들을 제거해야 합니다.", 5f, "Sounds/Narration/tutorial_40"));
        yield return StartCoroutine(Subtitle("염증을 먼저 제거해보도록 합시다.", 4.5f, "Sounds/Narration/tutorial_41"));
        m_Arrows[5].SetActive(true);
        m_Suction.GetComponent<Grabbable>().enabled = true;
        m_SuctionSnapZone.enabled = true;
        m_ToolSpawner.m_SpawnList[0].isAble = true;
        yield return StartCoroutine(Subtitle("표시된 위치로 이동해 석션을 집도록 합시다.", 4.5f, "Sounds/Narration/tutorial_42"));

        Grabbable suctionGrabbable = m_Suction.GetComponent<Grabbable>();
        while(suctionGrabbable.GetPrimaryGrabber() == null)
        {
            yield return null;
        }

        m_Arrows[5].SetActive(false);
        yield return StartCoroutine(Subtitle("이제 곧 잘 하시는군요.", 4f, "Sounds/Narration/tutorial_43"));
        yield return StartCoroutine(Subtitle("석션을 염증이 있는 곳에 가져가 댄 상태에서 트리거를 당기면 염증을 빨아들일 수 있습니다.", 7.5f, "Sounds/Narration/tutorial_44"));
        m_Arrows[3].SetActive(true);
        yield return StartCoroutine(Subtitle("염증을 모두 제거해주세요.", 4f, "Sounds/Narration/tutorial_45"));

        while(m_TargetAnimal.GetComponent<CAnimalStatus>().m_Inflamed.Count > 0)
        {
            yield return null;
        }

        m_Arrows[3].SetActive(false);
        yield return StartCoroutine(Subtitle("완벽합니다!", 4f, "Sounds/Narration/tutorial_46"));
        yield return StartCoroutine(Subtitle("이젠 골절을 제거해보도록 할까요.", 5f, "Sounds/Narration/tutorial_47"));
        m_Arrows[6].SetActive(true);
        m_FractureTool.GetComponent<Grabbable>().enabled = true;
        m_FractureToolSnapZone.enabled = true;
        m_ToolSpawner.m_SpawnList[3].isAble = true;
        yield return StartCoroutine(Subtitle("표시된 위치로 가 골절 접합기를 집어보세요.", 5f, "Sounds/Narration/tutorial_48"));

        Grabbable fractureToolGrabbable = m_FractureTool.GetComponent<Grabbable>();
        while(fractureToolGrabbable.GetPrimaryGrabber() == null)
        {
            yield return null;
        }

        m_Arrows[6].SetActive(false);
        yield return StartCoroutine(Subtitle("이젠 너무나도 쉬웠을거라 예상합니다.", 5f, "Sounds/Narration/tutorial_49"));
        m_Arrows[3].SetActive(true);
        yield return StartCoroutine(Subtitle("골절 접합기를 골절 위치에 가져다 대고 트리거를 계속 당기세요.", 6.5f, "Sounds/Narration/tutorial_50"));
        
        while(m_TargetAnimal.GetComponent<CAnimalStatus>().m_Fractures.Count > 0)
        {
            yield return null;
        }

        m_Arrows[3].SetActive(false);
        yield return StartCoroutine(Subtitle("첫 시도에 금방 배우시는군요!", 4f, "Sounds/Narration/tutorial_51"));
        yield return StartCoroutine(Subtitle("이제 보이는 모든 치료가 끝났습니다.", 4.5f, "Sounds/Narration/tutorial_52"));
        yield return StartCoroutine(Subtitle("하지만 여기서 끝이 아닙니다.", 4f, "Sounds/Narration/tutorial_53"));
        yield return StartCoroutine(Subtitle("각각 골절과 염증이 동반된 문제는 항생제를 투여해야 완전히 치료가 됩니다.", 7.5f, "Sounds/Narration/tutorial_54"));
        m_Arrows[2].SetActive(true);
        m_Arrows[3].SetActive(true);
        m_ToolSpawner.m_SpawnList[4].isAble = true;
        m_ToolSpawner.m_SpawnList[5].isAble = true;
        yield return StartCoroutine(Subtitle("골수염 항생제와 소염제 생성을 활성화하였습니다.", 5.5f, "Sounds/Narration/tutorial_55"));
        yield return StartCoroutine(Subtitle("해열제를 투여했을 때와 같이 골수염 항생제와 소염제를 투여하세요.", 6.5f, "Sounds/Narration/tutorial_56"));
        yield return StartCoroutine(Subtitle("완전하게 치료되면 이펙트가 생성되면서 치료율이 올라갑니다.", 6f, "Sounds/Narration/tutorial_57"));

        CAnimalStatus status = m_TargetAnimal.GetComponent<CAnimalStatus>();
        while(status.m_FractureAntiCoeff > 0f || status.m_InflammationAntiCoeff > 0f)
        {
            yield return null;
        }

        m_Arrows[2].SetActive(false);
        m_Arrows[3].SetActive(false);
        yield return StartCoroutine(Subtitle("모두 잘 따라하셨습니다!", 4f, "Sounds/Narration/tutorial_58"));
        yield return StartCoroutine(Subtitle("이제 결과를 확인해보도록 할까요?", 4.5f, "Sounds/Narration/tutorial_59"));
        m_Arrows[0].SetActive(true);
        m_EndButton.enabled = true;
        yield return StartCoroutine(Subtitle("처음에 확인했던 상태 표시기로 가 빨간 버튼을 누르세요.", 6f, "Sounds/Narration/tutorial_60"));

        while (!m_StatusDisplay.GetComponent<CStatusDisplayCtrl>().m_ResultUI.activeSelf)
        {
            yield return null;
        }

        m_EndButton.enabled = false;
        yield return StartCoroutine(Subtitle("결과창에서는 결과에 따라 최대 하트 3개가 부여됩니다.", 6f, "Sounds/Narration/tutorial_61"));
        yield return StartCoroutine(Subtitle("받는 하트의 개수는 치료율과 동물의 컨디션에 따라 달라집니다.", 6f, "Sounds/Narration/tutorial_62"));
        yield return StartCoroutine(Subtitle("다시 한번 더 빨간 버튼을 누르면 로비로 돌아가게 됩니다.", 5.5f, "Sounds/Narration/tutorial_63"));
        yield return StartCoroutine(Subtitle("여기까지 긴 튜토리얼을 하느라 고생하셨습니다.", 4f, "Sounds/Narration/tutorial_64"));
        yield return StartCoroutine(Subtitle("부디 많은 동물들을 구해주세요!", 4f, "Sounds/Narration/tutorial_65"));

        m_EndButton.enabled = true;
    }

    private IEnumerator Subtitle(string _msg, float _time, string _audioPath = "")
    {
        if (_audioPath == "")
        {
            GameManager.Manager.subtitle.AddSubtitle(_msg, _time);
        }
        else
        {
            AudioClip clip = Resources.Load<AudioClip>(_audioPath);
            GameManager.Manager.subtitle.AddSubtitle(_msg, _time, clip);
        }
        yield return new WaitForSeconds(_time);
    }
}
