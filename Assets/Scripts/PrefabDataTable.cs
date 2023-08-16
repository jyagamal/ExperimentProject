using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PrefabData 
{
    public Sprite icon;
    public GameObject prefab;
}

[CreateAssetMenu(fileName = "PrefabDataTable", menuName = "Scriptableobject/PrefabDataTable")]
public class PrefabDataTable : ScriptableObject
{
    public List<PrefabData> dataList = new List<PrefabData>();
}
