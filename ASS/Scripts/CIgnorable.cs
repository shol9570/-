using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CIgnorable : MonoBehaviour
{
    public struct IgnoreInfo
    {
        public Collider target;
        public Collider ignored;

        public IgnoreInfo(Collider _target, Collider _ignored)
        {
            target = _target;
            ignored = _ignored;
        }
    }

    public List<IgnoreInfo> m_IgnoredInfos = new List<IgnoreInfo>();
}
