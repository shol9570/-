using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IgnoreInfo = CIgnorable.IgnoreInfo;

public class CIgnoreCollisionArea : MonoBehaviour
{
    public CLantern m_Lantern;

    private List<CIgnorable> m_Ignorables = new List<CIgnorable>();

    private void OnTriggerEnter(Collider coll)
    {
        //If triggered object is not Tool
        if (coll.gameObject.layer != LayerMask.NameToLayer("Tool")) return;
        if (coll.isTrigger) return;
        if (m_Lantern.XRAYEDOBJ == null) return;
        //print("in " + coll.name);
        IgnoreObjectCollision(m_Lantern.XRAYEDOBJ, coll);
    }

    private void OnTriggerExit(Collider coll)
    {
        //If triggered object is not Tool
        if (coll.gameObject.layer != LayerMask.NameToLayer("Tool")) return;
        if (coll.isTrigger) return;
        //print("out " + coll.name);
        UnignoreObjectCollistion(coll);
    }

    void IgnoreObjectCollision(GameObject _target, Collider _ignored)
    {
        //print("Ignore " + _ignored.name);
        CIgnorable ignorable = _ignored.GetComponent<CIgnorable>();
        if (!ignorable) ignorable = _ignored.gameObject.AddComponent<CIgnorable>(); //If no CIgnorable component, add component.
        Collider[] targetColls = _target.GetComponentsInChildren<Collider>();
        for (int i = 0; i < targetColls.Length; i++)
        {
            //print(targetColls[i].name);
            Physics.IgnoreCollision(targetColls[i], _ignored, true);
            IgnoreInfo info = new IgnoreInfo(targetColls[i], _ignored);
            ignorable.m_IgnoredInfos.Add(info);
        }
    }

    void UnignoreObjectCollistion(Collider _ignored)
    {
        //print("UnIgnore " + _ignored.name);
        CIgnorable ignorable = _ignored.GetComponent<CIgnorable>();
        if (!ignorable) return; //If no CIgnorable component, exit this function.
        foreach (IgnoreInfo i in ignorable.m_IgnoredInfos)
        {
            if (i.ignored != _ignored) return;
            Physics.IgnoreCollision(i.target, i.ignored, false);
            ignorable.m_IgnoredInfos.Remove(i);
            break;
        }
        if(ignorable.m_IgnoredInfos.Count == 0) m_Ignorables.Remove(ignorable);
    }

    public void ResetIgnoredObjectCollision()
    {
        //print("Reset");
        foreach (CIgnorable ignorable in m_Ignorables)
        {
            foreach (IgnoreInfo i in ignorable.m_IgnoredInfos)
            {
                Physics.IgnoreCollision(i.target, i.ignored, false);
                ignorable.m_IgnoredInfos.Remove(i);
                break;
            }

            m_Ignorables.Remove(ignorable);
            break;
        }
    }
}
