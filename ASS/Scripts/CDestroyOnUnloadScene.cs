using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CDestroyOnUnloadScene : CStageEvent
{
    public override void SceneUnloadedEvent()
    {
        Destroy(this.gameObject);
    }
}
