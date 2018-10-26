using UnityEngine;

public class DanMuTextUI : MonoBehaviour
{
    /// <summary>
    /// 弹幕文本.
    /// </summary>
    public UILabel m_DanMuLable;
    
    internal void Init()
    {
        if (m_DanMuLable != null)
        {
            string danMuInfo = XKGlobalData.GetInstance().m_DanMuInfo;
            if (danMuInfo != "")
            {
                m_DanMuLable.text = danMuInfo;
            }
        }
    }

    bool IsRemoveSelf = false;
    internal void RemoveSelf()
    {
        if (IsRemoveSelf == false)
        {
            IsRemoveSelf = true;
            Destroy(gameObject);
        }
    }
}
