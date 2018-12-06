using UnityEngine;

public class DanMuTextUI : MonoBehaviour
{
    /// <summary>
    /// 弹幕文本.
    /// </summary>
    public UILabel m_DanMuLable;
    /// <summary>
    /// 商家名称文本信息Lable.
    /// </summary>
    public UILabel[] m_ShangJiNameLable = new UILabel[4];

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
        UpdateShangJiaNameInfo();
    }

    /// <summary>
    /// 更新机台弹幕信息.
    /// </summary>
    internal void UpdateDanMuInfo()
    {
        if (m_DanMuLable != null)
        {
            string danMuInfo = XKGlobalData.GetInstance().m_DanMuInfo;
            if (danMuInfo != "")
            {
                SSDebug.Log("UpdateDanMuInfo -> danMuInfo ===================== " + danMuInfo);
                m_DanMuLable.text = danMuInfo;
            }
        }
    }

    /// <summary>
    /// 更新商家名称文本信息Lable.
    /// </summary>
    internal void UpdateShangJiaNameInfo()
    {
        if (XkGameCtrl.GetInstance() != null && XkGameCtrl.GetInstance().m_SSShangHuInfo != null)
        {
            SSShangHuInfo.ShangHuData[] shangHuInfoArray = XkGameCtrl.GetInstance().m_SSShangHuInfo.m_ShangHuDt;
            for (int i = 0; i < m_ShangJiNameLable.Length; i++)
            {
                if (m_ShangJiNameLable[i] != null)
                {
                    string shangHuName = "海底捞";
                    if (shangHuInfoArray[i] != null)
                    {
                        shangHuName = shangHuInfoArray[i].ShangHuMing;
                    }

                    if (shangHuName.Length > 5)
                    {
                        //最多5个字.
                        shangHuName = shangHuName.Substring(0, 5);
                    }
                    m_ShangJiNameLable[i].text = shangHuName;
                    SSDebug.Log("UpdateShangJiaNameInfo -> shangJiaName[" + i + "] ===================== " + m_ShangJiNameLable[i].text);
                }
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
