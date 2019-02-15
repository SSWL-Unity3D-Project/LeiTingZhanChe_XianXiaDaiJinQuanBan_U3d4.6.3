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
        UpdateShangJiaDanMuInfo();
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
                //SSDebug.Log("UpdateDanMuInfo -> danMuInfo ===================== " + danMuInfo);
                m_DanMuLable.text = danMuInfo;
            }
        }
    }

    /// <summary>
    /// 更新商家名称文本信息Lable.
    /// </summary>
    internal void UpdateShangJiaDanMuInfo()
    {
        if (XkGameCtrl.GetInstance() != null && XkGameCtrl.GetInstance().m_SSShangHuInfo != null)
        {
            SSShangHuInfo.ShangHuData[] shangHuInfoArray = XkGameCtrl.GetInstance().m_SSShangHuInfo.m_ShangHuDanMuDt;
            for (int i = 0; i < m_ShangJiNameLable.Length; i++)
            {
                if (m_ShangJiNameLable[i] != null)
                {
                    string shangHuDanMu = "盛世网络50元";
                    if (shangHuInfoArray[i] != null)
                    {
                        shangHuDanMu = shangHuInfoArray[i].ShangHuDanMuInfo;
                    }

                    if (shangHuDanMu.Length > 9)
                    {
                        //最多9个字.
                        shangHuDanMu = shangHuDanMu.Substring(0, 9);
                    }
                    m_ShangJiNameLable[i].text = shangHuDanMu;
                    //SSDebug.Log("UpdateShangJiaNameInfo -> shangJiaName[" + i + "] ===================== " + m_ShangJiNameLable[i].text);
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
