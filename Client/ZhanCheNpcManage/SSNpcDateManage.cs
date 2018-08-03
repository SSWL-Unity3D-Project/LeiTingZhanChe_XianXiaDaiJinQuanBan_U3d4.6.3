using UnityEngine;
using System.Collections.Generic;

public class SSNpcDateManage : MonoBehaviour
{
    /// <summary>
    /// Npc数据列表.
    /// </summary>
    public List<GameObject> NpcList = new List<GameObject>();
    /// <summary>
    /// 添加npc.
    /// </summary>
    public void AddNpcToList(GameObject npc)
    {
        if (!NpcList.Contains(npc))
        {
            NpcList.Add(npc);
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
}