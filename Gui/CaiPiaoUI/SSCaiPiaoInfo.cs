using UnityEngine;

public class SSCaiPiaoInfo : SSGameMono
{
    /// <summary>
    /// 正在出票UI.
    /// </summary>
    public GameObject m_ZhengZaiChuPiaoUI;
    /// <summary>
    /// 设置正在出票UI的显示状态.
    /// </summary>
    public void SetActiveZhengZaiChuPiao(bool isActive)
    {
        if (m_ZhengZaiChuPiaoUI != null)
        {
            m_ZhengZaiChuPiaoUI.SetActive(isActive);
        }
        else
        {
            UnityLogWarning("SetActiveZhengZaiChuPiao -> m_ZhengZaiChuPiaoUI was null!");
        }
    }
}