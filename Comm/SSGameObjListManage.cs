using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 多个重复相同对象数据链表管理组件.
/// </summary>
public class SSGameObjListManage : SSGameMono
{
    /// <summary>
    /// 对象列表.
    /// </summary>
    List<GameObject> m_ObjList = new List<GameObject>();
    public GameObject FindObjByPrefab(GameObject prefab, Transform parent, Transform trPosRot = null)
    {
        GameObject obj = null;
        int length = m_ObjList.Count;
        for (int i = 0; i < length; i++)
        {
            if (m_ObjList[i] != null && m_ObjList[i].activeInHierarchy == false)
            {
                //在链表中找到已经休眠的对象.
                obj = m_ObjList[i];
                obj.SetActive(true);
                break;
            }
        }

        if (obj == null)
        {
            //没有找到处于休眠状态的对象.
            if (prefab != null)
            {
                //创建新的对象.
                obj = (GameObject)Instantiate(prefab, parent, trPosRot);
                AddObjToList(obj);
            }
            else
            {
                UnityLogWarning("FindObjByPrefab -> prefab was null!");
            }
        }
        return obj;
    }

    /// <summary>
    /// 添加obj到链表.
    /// </summary>
    void AddObjToList(GameObject obj)
    {
        if (m_ObjList != null)
        {
            if (m_ObjList.Contains(obj) == false)
            {
                m_ObjList.Add(obj);
            }
        }
    }

    /// <summary>
    /// 析构函数.
    /// </summary>
    ~SSGameObjListManage()
    {
        //UnityLog("~SSGameObjListManage.............");
        if (m_ObjList != null)
        {
            m_ObjList.Clear();
            m_ObjList = null;
        }
    }
}