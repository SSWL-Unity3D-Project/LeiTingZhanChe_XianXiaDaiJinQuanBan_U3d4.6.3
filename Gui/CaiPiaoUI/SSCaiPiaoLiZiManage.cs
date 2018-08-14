using UnityEngine;

public class SSCaiPiaoLiZiManage : MonoBehaviour
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
        public int[] m_PosXArray;
    }
    /// <summary>
    /// 修改UI坐标数据信息.
    /// </summary>
    public FixedUiPosData m_FixedUiPosDt;
    /// <summary>
    /// 数字UI精灵组件.
    /// m_NumTextureArray[0]   - 数字0.
    /// m_NumTextureArray[max] - 数字9.
    /// </summary>
    Material[] m_NumMatArray;
    /// <summary>
    /// 数字UI材质球组件.
    /// m_NumMatArray[0]   - 最高位.
    /// m_NumMatArray[max] - 最低位.
    /// </summary>
    public ParticleSystem[] m_NumParticleArray;

    /// <summary>
    /// 是否隐藏高位数字的0.
    /// </summary>
    public bool IsHiddenGaoWeiNumZero = true;
    /// <summary>
    /// 显示UI数量信息.
    /// </summary>
    internal void ShowNumUI(int num, PlayerEnum indexPlayer)
    {
        if (indexPlayer == PlayerEnum.Null)
        {
            return;
        }

        if (Camera.main != null)
        {
            Vector3 forwardVal = Camera.main.transform.forward;
            forwardVal.y = 0f;
            transform.forward = forwardVal.normalized;
//            Vector3 angle = transform.localEulerAngles;
//            angle.z += 180f;
//            transform.localEulerAngles = angle;
        }

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

        switch(indexPlayer)
        {
            case PlayerEnum.PlayerOne:
                {
                    m_NumMatArray = XkGameCtrl.GetInstance().m_CaiPiaoLiZiNumArrayP1;
                    break;
                }
            case PlayerEnum.PlayerTwo:
                {
                    m_NumMatArray = XkGameCtrl.GetInstance().m_CaiPiaoLiZiNumArrayP2;
                    break;
                }
            case PlayerEnum.PlayerThree:
                {
                    m_NumMatArray = XkGameCtrl.GetInstance().m_CaiPiaoLiZiNumArrayP3;
                    break;
                }
        }

        int max = m_NumParticleArray.Length;
        int numVal = num;
        int valTmp = 0;
        int powVal = 0;
        for (int i = 0; i < max; i++)
        {
            if (max - i > numStr.Length && IsHiddenGaoWeiNumZero)
            {
                //隐藏数据高位的0.
                m_NumParticleArray[i].renderer.material = XkGameCtrl.GetInstance().m_CaiPiaoLiZiNumATouMing;
            }
            else
            {
                //m_UISpriteArray[i].enabled = true;
                powVal = (int)Mathf.Pow(10, max - i - 1);
                valTmp = numVal / powVal;
                //UnityLog("ShowNumUI -> valTmp ====== " + valTmp);
                m_NumParticleArray[i].renderer.material = m_NumMatArray[valTmp];
                //m_UISpriteArray[i].spriteName = valTmp.ToString();
                numVal -= valTmp * powVal;
            }
        }
    }
}