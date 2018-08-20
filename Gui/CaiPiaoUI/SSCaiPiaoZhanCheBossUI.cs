using System.Collections;
using UnityEngine;

/// <summary>
/// 战车Boss彩票转盘UI.
/// 该对象必须挂上DestroyThisTimed.
/// </summary>
[RequireComponent(typeof(DestroyThisTimed))]
public class SSCaiPiaoZhanCheBossUI : SSGameMono
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
    /// 修改彩票UI坐标数据信息.
    /// </summary>
    public FixedUiPosData m_CaiPiaoUiPosDt;
    int m_CaiPiaoNum;
    /// <summary>
    /// 彩票数字.
    /// </summary>
    public UISprite[] m_CaiPIaoSprite;
    /// <summary>
    /// 玩家彩票数字图集.
    /// </summary>
    public UIAtlas[] m_UIAtlas = new UIAtlas[3];
    /// <summary>
    /// 彩票信息.
    /// </summary>
    public GameObject m_CaiPiaoInfoParent;
    public float m_TimePlay = 3f;
    public Animator m_Animator;
    PlayerEnum m_IndexPlayer;
    public void Init(PlayerEnum indexPlayer, int caiPiaoNum, Vector3 pos)
    {
        m_IndexPlayer = indexPlayer;
        m_CaiPiaoNum = caiPiaoNum;
        if (m_CaiPiaoInfoParent != null)
        {
            m_CaiPiaoInfoParent.SetActive(false);
        }
        transform.localPosition = pos;
        StartCoroutine(DelayShowCaiPiaoInfo());
    }

    IEnumerator DelayShowCaiPiaoInfo()
    {
        yield return new WaitForSeconds(m_TimePlay);
        m_Animator.enabled = false;
        OnEndAnimation();
    }

    /// <summary>
    /// 动画结束.
    /// </summary>
    public void OnEndAnimation()
    {
        int indexVal = (int)m_IndexPlayer - 1;
        if (indexVal >= 0 && indexVal <= 2)
        {
            ShowCaiPiaoVal(m_CaiPiaoNum, m_UIAtlas[indexVal]);
        }
    }

    void ShowCaiPiaoVal(int caiPiaoVal, UIAtlas fenShuAtlas)
    {
        if (caiPiaoVal <= 0)
        {
            return;
        }

        if (m_CaiPiaoInfoParent != null)
        {
            m_CaiPiaoInfoParent.SetActive(true);
        }

        string numStr = caiPiaoVal.ToString();
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

        if (m_CaiPiaoUiPosDt != null && m_CaiPiaoUiPosDt.IsFixPosX)
        {
            if (m_CaiPiaoUiPosDt.UIParent != null)
            {
                int len = numStr.Length;
                if (m_CaiPiaoUiPosDt.m_PosXArray.Length >= len)
                {
                    //动态修改UI数据的父级坐标.
                    Vector3 posTmp = m_CaiPiaoUiPosDt.UIParent.localPosition;
                    posTmp.x = m_CaiPiaoUiPosDt.m_PosXArray[len - 1];
                    m_CaiPiaoUiPosDt.UIParent.localPosition = posTmp;
                }
            }
        }

        int max = m_CaiPIaoSprite.Length;
        int numVal = caiPiaoVal;
        int valTmp = 0;
        int powVal = 0;
        bool isShowZero = false;
        for (int i = 0; i < max; i++)
        {
            powVal = (int)Mathf.Pow(10, max - i - 1);
            valTmp = numVal / powVal;
            m_CaiPIaoSprite[i].enabled = true;
            if (fenShuAtlas != null)
            {
                m_CaiPIaoSprite[i].atlas = fenShuAtlas;
            }

            if (!isShowZero)
            {
                if (valTmp != 0)
                {
                    isShowZero = true;
                }
                else
                {
                    m_CaiPIaoSprite[i].enabled = false;
                }
            }
            m_CaiPIaoSprite[i].spriteName = valTmp.ToString();
            numVal -= valTmp * powVal;
        }
    }
}