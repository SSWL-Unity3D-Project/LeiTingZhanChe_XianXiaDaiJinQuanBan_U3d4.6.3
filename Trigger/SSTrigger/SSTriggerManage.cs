using UnityEngine;

public class SSTriggerManage : MonoBehaviour
{
    #region 改变物体材质球触发器.
    /// <summary>
    /// 改变物体材质球触发器.
    /// </summary>
    SSTriggerChangeMat m_TriggerChangeMat;
    internal void SetTriggerChangeMat(SSTriggerChangeMat trigger)
    {
        if (m_TriggerChangeMat != trigger)
        {
            m_TriggerChangeMat = trigger;
        }
    }

    /// <summary>
    /// 减少物体材质球触发器玩家的进入次数.
    /// 当玩家血值耗尽时.
    /// </summary>
    internal void SubTriggerChangeMatEnterCount(PlayerEnum indexPlayer)
    {
        if (m_TriggerChangeMat != null)
        {
            m_TriggerChangeMat.SubEnterCount(indexPlayer);
        }
    }
    #endregion
}
