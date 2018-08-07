using UnityEngine;

/// <summary>
/// 游戏中UI数字信息控制组件.
/// </summary>
public class SSGameNumUI : SSGameMono
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
        public Transform UISpriteParent;
        /// <summary>
        /// UI坐标x轴偏移量.
        /// m_PosXArray[max] --- 最大数据的x轴坐标.
        /// m_PosXArray[min] --- 最小数据的x轴坐标.
        /// </summary>
        public int[] m_PosXArray;
    }
    /// <summary>
    /// 修改UI坐标数据信息.
    /// </summary>
    public FixedUiPosData m_FixedUiPosDt;
    /// <summary>
    /// 数字UI精灵组件.
    /// </summary>
    public UISprite[] m_UISpriteArray;
    /// <summary>
    /// 显示UI数量信息.
    /// </summary>
    internal void ShowNumUI(int num)
    {
        if (m_FixedUiPosDt != null && m_FixedUiPosDt.IsFixPosX)
        {
            if (m_FixedUiPosDt.UISpriteParent != null)
            {
                string numStr = num.ToString();
                int len = numStr.Length;
                if (m_FixedUiPosDt.m_PosXArray.Length >= len)
                {
                    //动态修改UI数据的父级坐标.
                    Vector3 posTmp = m_FixedUiPosDt.UISpriteParent.localPosition;
                    posTmp.x = m_FixedUiPosDt.m_PosXArray[len - 1];
                    m_FixedUiPosDt.UISpriteParent.localPosition = posTmp;
                }
            }
        }

        int max = m_UISpriteArray.Length;
        int numVal = num;
        int valTmp = 0;
        int powVal = 0;
        for (int i = 0; i < max; i++)
        {
            powVal = (int)Mathf.Pow(10, max - i - 1);
            valTmp = numVal / powVal;
            //UnityLog("ShowNumUI -> valTmp ====== " + valTmp);
            m_UISpriteArray[i].spriteName = valTmp.ToString();
            numVal -= valTmp * powVal;
        }
    }
}