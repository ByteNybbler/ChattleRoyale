// Author(s): Paul Calande
// A serializable Dictionary of integers mapped to Text references.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DictionaryIntToText : MonoBehaviour
{
    [System.Serializable]
    public class Pair : KeyValueArrayPair<int, Text> { }
    [System.Serializable]
    public class Array : KeyValueArray<int, Text, Pair> { }

    [SerializeField]
    [Tooltip("Integer mapped to Text reference.")]
    Array array;

    public bool TryGetValue(int key, out Text value)
    {
        return array.TryGetValue(key, out value);
    }
}