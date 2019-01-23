using UnityEngine;

public class SSCaiPiaoNpcUI : MonoBehaviour
{
    [System.Serializable]
    public class FixedUiPosData
    {
        /// <summary>
        /// 是否修改UI信息的x轴坐标.
        /// </summary>
        public bool IsFixPosX = false;
        /// <summary>
        /// 数字UI精灵组件的父级.
        /// </summary>
        public Transform UIParent;
        /// <summary>
        /// UI坐标x轴偏移量.
        /// m_PosXArray[0]   --- 最小数据的x轴坐标.
        /// m_PosXArray[max] --- 最大数据的x轴坐标.
        /// </summary>
        public float[] m_PosXArray;
    }
    /// <summary>
    /// 修改UI坐标数据信息.
    /// </summary>
    public FixedUiPosData m_FixedUiPosDt;
    /// <summary>
    /// 数字UI材质球组件.
    /// m_NumMatArray[0]   - 最高位.
    /// m_NumMatArray[max] - 最低位.
    /// </summary>
    public Renderer[] m_NumRenderArray;

    /// <summary>
    /// 是否隐藏高位数字的0.
    /// </summary>
    public bool IsHiddenGaoWeiNumZero = false;
    /// <summary>
    /// 彩票信息的父级.
    /// </summary>
    public Transform m_CaiPiaoInfoParent;
    /// <summary>
    /// npc血量控制脚本.
    /// </summary>
    XKNpcHealthCtrl m_NpcHealthCom;
    void Update()
    {
        if (m_NpcHealthCom != null && m_NpcHealthCom.IsDeathNpc)
        {
            return;
        }

        if (m_CaiPiaoInfoParent != null)
        {
            if (Camera.main != null)
            {
                Vector3 forwardVal = Camera.main.transform.forward;
                forwardVal.y = 0f;
                m_CaiPiaoInfoParent.forward = forwardVal.normalized;
            }
        }
    }

    /// <summary>
    /// 隐藏数字信息.
    /// </summary>
    internal void HiddenNumUI()
    {
        if (m_CaiPiaoInfoParent != null)
        {
            m_CaiPiaoInfoParent.gameObject.SetActive(false);
        }
        enabled = false;
    }

    /// <summary>
    /// 显示爆炸粒子的彩票UI数量信息.
    /// </summary>
    internal void ShowNumUI(SSCaiPiaoDataManage.GameCaiPiaoData.DeCaiState deCaiType, XKNpcHealthCtrl healthCom)
    {
        SetShangJiaInfo(deCaiType);
        if (m_CaiPiaoInfoParent != null)
        {
            m_CaiPiaoInfoParent.gameObject.SetActive(true);
        }
        enabled = true;
        m_NpcHealthCom = healthCom;

        SSCaiPiaoDataManage.GameCaiPiaoData.DaiJinQuanState daiJiQuan = healthCom.NpcScript.m_DaiJinQuanState;
        int num = XkPlayerCtrl.GetInstanceFeiJi().m_SpawnNpcManage.m_CaiPiaoDataManage.m_GameCaiPiaoData.GetPrintCaiPiaoValueByDeCaiState(deCaiType,
            SSCaiPiaoDataManage.SuiJiDaoJuState.BaoXiang, daiJiQuan);

        string numStr = num.ToString();
        if (m_FixedUiPosDt != null && m_FixedUiPosDt.IsFixPosX)
        {
            if (m_FixedUiPosDt.UIParent != null)
            {
                int len = numStr.Length;
                if (m_FixedUiPosDt.m_PosXArray.Length >= len)
                {
                    //动态修改UI数据的父级坐标.
                    Vector3 posTmp = m_FixedUiPosDt.UIParent.localPosition;
                    posTmp.x = m_FixedUiPosDt.m_PosXArray[len - 1];
                    m_FixedUiPosDt.UIParent.localPosition = posTmp;
                }
            }
        }
        
        int max = m_NumRenderArray.Length;
        int numVal = num;
        int valTmp = 0;
        int powVal = 0;
        for (int i = 0; i < max; i++)
        {
            if (max - i > numStr.Length && IsHiddenGaoWeiNumZero)
            {
                //隐藏数据高位的0.
                m_NumRenderArray[i].material = XkGameCtrl.GetInstance().m_CaiPiaoLiZiNumATouMing;
            }
            else
            {
                //m_UISpriteArray[i].enabled = true;
                powVal = (int)Mathf.Pow(10, max - i - 1);
                valTmp = numVal / powVal;
                //UnityLog("ShowNumUI -> valTmp ====== " + valTmp);
                m_NumRenderArray[i].material = XkGameCtrl.GetInstance().m_NpcCaiPiaoNumArray[valTmp];
                //m_UISpriteArray[i].spriteName = valTmp.ToString();
                numVal -= valTmp * powVal;
            }
        }
    }
    
    //*************************************************************************************************************//
    /// <summary>
    /// 商户名称信息.
    /// </summary>
    public TextMesh m_ShangJiaInfoLb;
    /// <summary>
    /// 设置商户名称信息.
    /// </summary>
    void SetShangJiaInfo(SSCaiPiaoDataManage.GameCaiPiaoData.DeCaiState deCaiType)
    {
        if (m_ShangJiaInfoLb != null)
        {
            string shangHuInfo = "盛世网络";
            if (XkGameCtrl.GetInstance().m_SSShangHuInfo != null)
            {
                if (deCaiType == SSCaiPiaoDataManage.GameCaiPiaoData.DeCaiState.JPBoss)
                {
                    shangHuInfo = XkGameCtrl.GetInstance().m_SSShangHuInfo.GetJPBossShangHuMingDt();
                }
                else
                {
                    shangHuInfo = XkGameCtrl.GetInstance().m_SSShangHuInfo.GetShangHuMingDt().ShangHuMing;
                }
            }

            if (shangHuInfo.Length > 5)
            {
                //最多支持5个字.
                shangHuInfo = shangHuInfo.Substring(0, 5);
            }
            m_ShangJiaInfoLb.text = shangHuInfo;
        }
    }
}