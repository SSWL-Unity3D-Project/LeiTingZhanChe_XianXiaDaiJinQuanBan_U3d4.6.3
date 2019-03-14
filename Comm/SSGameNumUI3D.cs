using UnityEngine;
using System.Collections.Generic;

public class SSGameNumUI3D : MonoBehaviour
{
    [System.Serializable]
    public class FixedUiPosData
    {
        /// <summary>
        /// 是否修改UI信息的x轴坐标.
        /// </summary>
        public bool IsFixPosX = false;
        /// <summary>
        /// 数字UI组件的父级.
        /// </summary>
        public Transform UINumParent;
        /// <summary>
        /// UI坐标x轴偏移量.
        /// m_PosXArray[0]   --- 最小数据的x轴坐标.
        /// m_PosXArray[max] --- 最大数据的x轴坐标.
        /// </summary>
        public int[] m_PosXArray;
    }
    /// <summary>
    /// 修改UI坐标数据信息.
    /// </summary>
    public FixedUiPosData m_FixedUiPosDt;
    /// <summary>
    /// 数字UI是否为从高位到低位填充.
    /// 默认值为从高位到低位填充.
    /// 如果IsUIGaoToDiWei==false则需要动态将其数字UI进行翻转.
    /// </summary>
    public bool IsUIGaoToDiWei = true;
    /// <summary>
    /// 数字UI数据.
    /// </summary>
    [System.Serializable]
    public class NumUIData
    {
        /// <summary>
        /// 数字UI材质渲染组件.
        /// </summary>
        public MeshRenderer m_UIMesh;
        /// <summary>
        /// 数字UI材质球组件.
        /// </summary>
        public Material m_UIMat;
        /// <summary>
        /// 展示数字.
        /// </summary>
        internal void ShowNum(int num, Texture[] textureArray)
        {
            if (m_UIMat != null)
            {
                if (num < textureArray.Length && textureArray[num] != null)
                {
                    m_UIMat.mainTexture = textureArray[num];
                }
            }

            if (m_UIMesh != null)
            {
                m_UIMesh.enabled = true;
            }
        }

        /// <summary>
        /// 隐藏数字.
        /// </summary>
        internal void HiddenNum()
        {
            if (m_UIMesh != null)
            {
                m_UIMesh.enabled = false;
            }
        }
    }
    /// <summary>
    /// 数字UI材质球组件列表.
    /// m_UIDtArray[0]   - 最高位.
    /// m_UIDtArray[max] - 最低位.
    /// </summary>
    public NumUIData[] m_UIDtArray;
    /// <summary>
    /// 数字UI图片列表.
    /// m_UINumTextureArray[0]  - 0.
    /// m_UINumTextureArray[10] - 9.
    /// </summary>
    public Texture[] m_UINumTextureArray = new Texture[10];
    /// <summary>
    /// 是否隐藏高位数字的0.
    /// </summary>
    public bool IsHiddenGaoWeiNumZero = true;
    bool IsInit = false;
    /// <summary>
    /// 初始化.
    /// </summary>
    void Init()
    {
        if (IsInit == true)
        {
            return;
        }
        IsInit = true;

        if (IsUIGaoToDiWei == false)
        {
            //如果IsUIGaoToDiWei==false则需要动态将其数字UI进行翻转.
            List<NumUIData> listUI = new List<NumUIData>(m_UIDtArray);
            listUI.Reverse();
            m_UIDtArray = listUI.ToArray();
        }
    }

    /// <summary>
    /// 显示UI数量信息.
    /// </summary>
    internal void ShowNumUI(int num)
    {
        if (IsInit == false)
        {
            Init();
        }

        string numStr = num.ToString();
        if (m_FixedUiPosDt != null && m_FixedUiPosDt.IsFixPosX)
        {
            if (m_FixedUiPosDt.UINumParent != null)
            {
                int len = numStr.Length;
                if (m_FixedUiPosDt.m_PosXArray.Length >= len)
                {
                    //动态修改UI数据的父级坐标.
                    Vector3 posTmp = m_FixedUiPosDt.UINumParent.localPosition;
                    posTmp.x = m_FixedUiPosDt.m_PosXArray[len - 1];
                    m_FixedUiPosDt.UINumParent.localPosition = posTmp;
                }
            }
        }

        int max = m_UIDtArray.Length;
        int numVal = num;
        int valTmp = 0;
        int powVal = 0;
        for (int i = 0; i < max; i++)
        {
            if (max - i > numStr.Length && IsHiddenGaoWeiNumZero)
            {
                //隐藏数据高位的0.
                m_UIDtArray[i].HiddenNum();
            }
            else
            {
                powVal = (int)Mathf.Pow(10, max - i - 1);
                valTmp = numVal / powVal;
                //UnityLog("ShowNumUI -> valTmp ====== " + valTmp);
                m_UIDtArray[i].ShowNum(valTmp, m_UINumTextureArray);
                numVal -= valTmp * powVal;
            }
        }
    }

    internal void SetActive(bool isActive)
    {
        gameObject.SetActive(isActive);
    }
}
