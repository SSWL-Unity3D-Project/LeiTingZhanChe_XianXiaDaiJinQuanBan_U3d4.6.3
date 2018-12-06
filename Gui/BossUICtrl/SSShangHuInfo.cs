using UnityEngine;

/// <summary>
/// 游戏商户配置信息.
/// </summary>
public class SSShangHuInfo : MonoBehaviour
{
    [System.Serializable]
    public class ShangHuData
    {
        /// <summary>
        /// 商户名列表信息索引.
        /// </summary>
        internal int IndexShangHu = 0;
        /// <summary>
        /// 商户名信息.
        /// 最多5个字.
        /// </summary>
        public string ShangHuMing = "";
    }
    /// <summary>
    /// 商户配置信息.
    /// 最多4个商户数据信息.
    /// </summary>
    public ShangHuData[] m_ShangHuDt = new ShangHuData[4];
    /// <summary>
    /// 商户名列表信息索引.
    /// </summary>
    int m_IndexShangHu = 0;

    internal void Init()
    {
        for (int i = 0; i < m_ShangHuDt.Length; i++)
        {
            m_ShangHuDt[i].IndexShangHu = i;
            SSDebug.Log("Init -> ShangHuMing[" + i + "] ===== " + m_ShangHuDt[i].ShangHuMing);
        }
    }

    /// <summary>
    /// 更新游戏商户数据信息.
    /// </summary>
    internal void UpdateShangHuInfo(string[] shangHuInfoArray)
    {
        for (int i = 0; i < m_ShangHuDt.Length; i++)
        {
            m_ShangHuDt[i].ShangHuMing = shangHuInfoArray[i];
            SSDebug.Log("UpdateShangHuInfo -> ShangHuMing[" + i + "] ===== " + shangHuInfoArray[i]);
        }

        if (SSUIRoot.GetInstance().m_GameUIManage != null
            && SSUIRoot.GetInstance().m_GameUIManage.m_DanMuTextUI != null)
        {
            //更新游戏弹幕的商户名信息.
            SSUIRoot.GetInstance().m_GameUIManage.m_DanMuTextUI.UpdateShangJiaNameInfo();
        }
    }

    /// <summary>
    /// 获取代金券npc的商户名信息.
    /// </summary>
    internal ShangHuData GetShangHuMingInfo()
    {
        int indexVal = m_IndexShangHu;
        m_IndexShangHu++;
        if (m_IndexShangHu >= m_ShangHuDt.Length)
        {
            m_IndexShangHu = 0;
        }
        return m_ShangHuDt[indexVal];
    }
}
