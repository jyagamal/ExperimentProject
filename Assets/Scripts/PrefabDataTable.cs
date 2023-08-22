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
    public void AddPrefab(GameObject prefab, Sprite icon)
    {
        PrefabData data = new PrefabData();
        data.prefab = prefab;
        data.icon = icon;

        dataList.Add(data);
    }

    public void Clear()
    {
        dataList.Clear();
    }

    public List<PrefabData> dataList = new List<PrefabData>();
}
