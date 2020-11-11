using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CHelper
{
    /// <summary>
    /// Get array of component T in childrens with tag.
    /// </summary>
    /// <param name="_target">Root transform.</param>
    /// <param name="_tag">Tag name.</param>
    /// <typeparam name="T">Component type.</typeparam>
    /// <returns></returns>
    public static T[] GetComponentsInChildrensWithTag<T>(Transform _target, string _tag) where T : Component
    {
        List<T> results = new List<T>();
        T t = _target.GetComponent<T>();
        if (t != null && _target.CompareTag(_tag)) results.Add(t);
        if (_target.childCount > 0)
        {
            for (int i = 0; i < _target.childCount; i++)
            {
                T[] ts = GetComponentsInChildrensWithTag<T>(_target.GetChild(i), _tag);
                results.AddRange(ts);
            }
        }

        return results.ToArray();
    }
}
