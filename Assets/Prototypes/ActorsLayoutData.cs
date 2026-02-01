using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ActorsLayoutData", menuName = "Scriptable Objects/ActorsLayoutData")]
public class ActorsLayoutData : ScriptableObject
{
    public List<Entry> entries = new();

    [Serializable]
    public struct Entry
    {
        public string id;
        public Vector3 localPosition;
        public Vector3 localEuler;
    }
}