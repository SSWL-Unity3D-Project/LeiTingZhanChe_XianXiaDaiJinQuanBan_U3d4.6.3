
using System.Collections.Generic;

namespace Assets.XKGame.Script.GamePay
{
    /// <summary>
    /// 游戏通过微信营收和返券数据信息.
    /// 营收和返券的信息： GameData
    /// 日期        Time
    /// 营收        Revenue
    /// 返券        Rebate
    /// </summary>
    [System.Serializable]
    public class SSGameWXPayData
    {
        /// <summary>
        /// 日期信息.
        /// </summary>
        public string Time = "";
        /// <summary>
        /// 营收信息.
        /// </summary>
        public string Revenue = "0";
        /// <summary>
        /// 返券信息.
        /// </summary>
        public string Rebate = "0";
        internal void Reset()
        {
            Time = "";
            Revenue = "0";
            Rebate = "0";
        }
    }

    /// <summary>
    /// 营收和返券数据管理.
    /// </summary>
    internal class SSGameWXPayDataManage
    {
        /// <summary>
        /// 当前游戏营收信息数据.
        /// </summary>
        SSGameWXPayData m_GameWXPayDataCur = null;
        internal void Init()
        {
            if (m_GameWXPayDataCur != null)
            {
                return;
            }

            m_GameWXPayDataCur = new SSGameWXPayData();
            m_GameWXPayDataCur.Time = System.DateTime.Now.ToString("yyyy年MM月dd日");
            SSGameWXPayData[] payDataArray = GetGamePayDataInfo();
            if (payDataArray.Length > 0)
            {
                if (payDataArray[0].Time == m_GameWXPayDataCur.Time)
                {
                    m_GameWXPayDataCur.Revenue = payDataArray[0].Revenue;
                    m_GameWXPayDataCur.Rebate = payDataArray[0].Rebate;
                }
            }
            UnityEngine.Debug.Log("Init -----------> Time: " + m_GameWXPayDataCur.Time
                + ", Revenue: " + m_GameWXPayDataCur.Revenue
                + ", Rebate: " + m_GameWXPayDataCur.Rebate);
        }

        /// <summary>
        /// 记录收入信息.
        /// </summary>
        internal void WriteGamePayRevenueInfo(int value)
        {
            if (m_GameWXPayDataCur == null)
            {
                return;
            }

            string timeCur = System.DateTime.Now.ToString("yyyy年MM月dd日");
            if (timeCur != m_GameWXPayDataCur.Time)
            {
                //时间有刷新.
                //重置营收信息.
                m_GameWXPayDataCur.Reset();
                m_GameWXPayDataCur.Time = timeCur;
            }

            int rv = value + System.Convert.ToInt32(m_GameWXPayDataCur.Revenue);
            m_GameWXPayDataCur.Revenue = rv.ToString();

            SSGameWXPayDataRW payDataRW = new SSGameWXPayDataRW();
            payDataRW.WriteToFileXml(m_GameWXPayDataCur);
        }

        /// <summary>
        /// 记录支出信息.
        /// </summary>
        internal void WriteGamePayRebateInfo(int value)
        {
            if (m_GameWXPayDataCur == null)
            {
                return;
            }

            string timeCur = System.DateTime.Now.ToString("yyyy年MM月dd日");
            if (timeCur != m_GameWXPayDataCur.Time)
            {
                //时间有刷新.
                //重置营收信息.
                m_GameWXPayDataCur.Reset();
                m_GameWXPayDataCur.Time = timeCur;
            }

            int rv = value + System.Convert.ToInt32(m_GameWXPayDataCur.Rebate);
            m_GameWXPayDataCur.Rebate = rv.ToString();

            SSGameWXPayDataRW payDataRW = new SSGameWXPayDataRW();
            payDataRW.WriteToFileXml(m_GameWXPayDataCur);
        }

        /// <summary>
        /// 获取游戏营收和返券数据信息.
        /// </summary>
        internal SSGameWXPayData[] GetGamePayDataInfo()
        {
            SSGameWXPayDataRW payDataRW = new SSGameWXPayDataRW();
            return payDataRW.ReadFromFileXml();
        }
    }
}
