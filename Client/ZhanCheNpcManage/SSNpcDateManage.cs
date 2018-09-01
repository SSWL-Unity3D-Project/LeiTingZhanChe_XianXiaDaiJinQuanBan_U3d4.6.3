using UnityEngine;
using System.Collections.Generic;

public class SSNpcDateManage : MonoBehaviour
{
    /// <summary>
    /// Npc数据列表.
    /// </summary>
    public List<GameObject> NpcList = new List<GameObject>();
    internal List<XKNpcMoveCtrl> NpcMoveList = new List<XKNpcMoveCtrl>();
    /// <summary>
    /// 添加npc.
    /// </summary>
    public void AddNpcToList(GameObject npc)
    {
        if (!NpcList.Contains(npc))
        {
            NpcList.Add(npc);
            XKNpcMoveCtrl npcMoveCom = npc.GetComponent<XKNpcMoveCtrl>();
            if (npcMoveCom != null)
            {
                if (NpcMoveList.Contains(npcMoveCom) == false)
                {
                    NpcMoveList.Add(npcMoveCom);
                }
            }
        }
    }

    /// <summary>
    /// 删除npc.
    /// </summary>
    public bool RemoveNpcFromList(GameObject npc)
    {
        if (NpcList.Contains(npc))
        {
            NpcList.Remove(npc);
            XKNpcMoveCtrl npcMoveCom = npc.GetComponent<XKNpcMoveCtrl>();
            if (npcMoveCom != null)
            {
                if (NpcMoveList.Contains(npcMoveCom) == true)
                {
                    NpcMoveList.Remove(npcMoveCom);
                }
            }
            return true;
        }
        return false;
    }

    /// <summary>
    /// 获取npc列表中是否还有npc.
    /// </summary>
    public bool GetIsHaveNpc()
    {
        return NpcList.Count > 0 ? true : false;
    }

    /// <summary>
    /// 通过索引获取npc.
    /// </summary>
    public GameObject GetNpcByIndex(int index)
    {
        GameObject obj = null;
        if (NpcList.Count > index)
        {
            obj = NpcList[index];
        }
        return obj;
    }

    /// <summary>
    /// 获取ncpMove组件.
    /// </summary>
    public XKNpcMoveCtrl GetNpcMoveComByIndex(int index)
    {
        XKNpcMoveCtrl com = null;
        if (NpcMoveList.Count > index)
        {
            com = NpcMoveList[index];
        }
        return com;
    }
}