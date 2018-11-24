using UnityEngine;

public class SSBossXueTiaoManage : MonoBehaviour
{
    /// <summary>
    /// Boss血条控制脚本.
    /// </summary>
    public XKBossXueTiaoCtrl m_BossXueTiaoCom = null;
    // Use this for initialization
    internal void Init()
    {
        if (m_BossXueTiaoCom != null)
        {
            m_BossXueTiaoCom.Init();
        }
    }

    bool IsRemoveSelf = false;
    internal void RemoveSelf()
    {
        if (IsRemoveSelf == false)
        {
            IsRemoveSelf = true;
            if (m_BossXueTiaoCom != null)
            {
                m_BossXueTiaoCom.RemoveSelf();
                m_BossXueTiaoCom = null;
            }
            Destroy(gameObject);
        }
    }
}
