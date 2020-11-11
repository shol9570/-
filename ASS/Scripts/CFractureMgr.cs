using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CFractureMgr : MonoBehaviour
{
    public GameObject[] m_FractureFactories;

    /// <summary>
    /// Create fractures without overlapping.
    /// </summary>
    /// <param name="_target"></param>
    /// <param name="_count"></param>
    /// <param name="_decals"></param>
    /// <param name="_points"></param>
    /// <param name="_fractureTypes"></param>
    public void CreateFractures(CAnimalStatus _target, int _count, List<CFractureDecal> _decals, string[] _points, int[] _fractureTypes = null)
    {
        if ((_points != null && _points.Length != _count) || (_fractureTypes != null && _fractureTypes.Length != _count))
        {
            Debug.LogError("[CFractureMgr] Number of count and length of _points or _fractureTypes are not EQUAL.");
            return;
        }

        if (_points == null)
            CreateFractures(_target, _count, _decals, _points, _fractureTypes);
        else
        {
            int[] convert = new int[_points.Length];
            for (int i = 0; i < convert.Length; i++)
            {
                convert[i] = GetPointWithName(_target, _points[i]);
            }

            CreateFractures(_target, _count, _decals, convert, _fractureTypes);
        }
    }

    /// <summary>
    /// Create fractures without overlapping.
    /// </summary>
    /// <param name="_target"></param>
    /// <param name="_count"></param>
    /// <param name="_decals"></param>
    /// <param name="_points"></param>
    /// <param name="_fractureTypes"></param>
    public void CreateFractures(CAnimalStatus _target, int _count, List<CFractureDecal> _decals, int[] _points = null, int[] _fractureTypes = null)
    {
        if ((_points != null && _points.Length != _count) || (_fractureTypes != null && _fractureTypes.Length != _count))
        {
            Debug.LogError("[CFractureMgr] Number of count and length of _points or _fractureTypes are not EQUAL.");
            return;
        }

        List<int> createdPoints = new List<int>();
        for (int i = 0; i < _count; i++)
        {
            int point, type;
            do
            {
                point = _points == null ? Random.Range(0, _target.m_FracturePoints.Length) : _points[i];
            } while (createdPoints.Contains(point));
            type = _fractureTypes == null ? -1 : _fractureTypes[i];
            
            CreateFracture(_target, point, _decals, type);
            createdPoints.Add(point);
        }
    }
    
    /// <summary>
    /// Create fracture object at point.
    /// </summary>
    /// <param name="_target"></param>
    /// <param name="_point">It will create fracture at random point if the value is null.</param>
    /// <param name="_decals"></param>
    /// <param name="_fractureType">m_FractureFactories index that will be created. It will randomly choose one if the value is -1.</param>
    public void CreateFracture(CAnimalStatus _target, string _point, List<CFractureDecal> _decals, int _fractureType = -1)
    {
        if (_target.m_FracturePoints == null || _target.m_FracturePoints.Length <= 0)
            Debug.LogError("[CFractureMgr] No fracture point.");
        if (m_FractureFactories == null || m_FractureFactories.Length <= 0)
            Debug.LogError("[CFractureMgr] No fracture factories.");
        if (_point == null) CreateFracture(_target, Random.Range(0, _target.m_FracturePoints.Length), _decals, _fractureType);
        else CreateFracture(_target, GetPointWithName(_target, _point), _decals, _fractureType);
    }
    
    /// <summary>
    /// Create fracture object at point.
    /// </summary>
    /// <param name="_target"></param>
    /// <param name="_point">It will create fracture at random point if the value is -1.</param>
    /// <param name="_decals"></param>
    /// <param name="_fractureType">m_FractureFactories index that will be created. It will randomly choose one if the value is -1.</param>
    public void CreateFracture(CAnimalStatus _target, int _point, List<CFractureDecal> _decals, int _fractureType = -1)
    {
        if (_point == -1) _point = Random.Range(0, _target.m_FracturePoints.Length);
        if (_fractureType == -1) _fractureType = Random.Range(0, m_FractureFactories.Length);

        CFracturePoint point = _target.m_FracturePoints[_point];
        GameObject fracture = Instantiate(m_FractureFactories[_fractureType], point.transform.position, point.transform.rotation, point.transform);
        CFractureDecal decal = fracture.GetComponent<CFractureDecal>();
        decal.Subordinated = _target;
        decal.SetProjectDirection(-fracture.transform.forward);
        decal.Project(point.m_Target);
        _decals.Add(decal);
    }

    /// <summary>
    /// Get point index with name.
    /// </summary>
    /// /// <param name="_target"></param>
    /// <param name="_name">Fracture point object name</param>
    /// <returns>Return m_FracturePoints array index which has equal name. It will return -1 if there is no equal name in m_FracturePoints</returns>
    int GetPointWithName(CAnimalStatus _target, string _name)
    {
        if (_target.m_FracturePoints == null || _target.m_FracturePoints.Length <= 0)
            Debug.LogError("[CFractureMgr] No fracture point.");
        for (int i = 0; i < _target.m_FracturePoints.Length; i++)
        {
            if (_target.m_FracturePoints[i].name == _name) return i;
        }

        return -1;
    }

    public void ClearFractures(List<CFractureDecal> _decals)
    {
        for (int i = 0; i < _decals.Count; i++)
        {
            GameObject decal = _decals[i].gameObject;
            _decals.Remove(_decals[i]);
            Destroy(decal);
            break;
        }
    }
}