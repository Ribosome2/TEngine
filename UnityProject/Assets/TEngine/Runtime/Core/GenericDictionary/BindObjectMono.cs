using System.Text.RegularExpressions;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BindObjectMono : MonoBehaviour
{
    public GenericDictionary<string, GameObject> objDict;

#if UNITY_EDITOR

    public string ViewScripts ;

    public void BindObject(bool includeInactive)
    {
        objDict.Clear();
        FindTransform(transform, includeInactive);
    }

    public List<GameObject> GetGOListForEditor()
    {
        List<GameObject> list = new List<GameObject>();
        foreach (var kv in objDict)
        {
            list.Add(kv.Value);
        }
        return list;
    }

    private void FindTransform(Transform parent, bool includeInactive)
    {
        for (int i = 0; i < parent.childCount; ++i)
        {
            var temp = parent.GetChild(i);
            if (!includeInactive && !temp.gameObject.activeInHierarchy)
                continue;

            if (Regex.IsMatch(temp.name, "^m_[A-Za-z0-9_]+$"))
            {
                if (!objDict.ContainsKey(temp.name))
                {
                    objDict.Add(temp.name, temp.gameObject);
                }
                else
                {
                    Debug.LogError("重复的节点名字：" + temp.name + "，请检查！");
                }
                   
            }

            if (temp.childCount > 0 && !temp.GetComponent<BindObjectMono>())
                FindTransform(temp, includeInactive);
        }
    }

    public void DeleteRayCast()
    {
        var arr = gameObject.GetComponentsInChildren<Image>();
        foreach (var img in arr)
        {
            img.raycastTarget = false;
        }
    }

#endif

    //这个函数好像就是不能传T为GameObject的，是因为GameObject不算component?
    public T Get<T>(string objName)
    {
        return objDict.TryGetValue(objName, out var obj) ? obj.GetComponent<T>() : default;
    }
    
    public GameObject GetGO(string objName)
    {
        if (objDict.TryGetValue(objName, out var obj))
        {
            return obj;
        }

        return null;
    }
}
