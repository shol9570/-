using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CylinderType
{
    None,
    FractureAnti,
    InflammationAnti,
    FeverReducer,
    ColdAnti,
    Anthelmintic,
    DHPPL
}

public class CCylinderType : MonoBehaviour
{
    public CylinderType m_AntibioticsType;
}
