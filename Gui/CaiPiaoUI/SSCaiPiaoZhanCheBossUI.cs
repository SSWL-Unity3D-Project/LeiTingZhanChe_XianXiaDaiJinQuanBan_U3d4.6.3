using System.Collections;
using UnityEngine;

/// <summary>
/// 战车Boss彩票转盘UI.
/// 该对象必须挂上DestroyThisTimed.
/// </summary>
//[RequireComponent(typeof(DestroyThisTimed))]
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
    //public GameObject m_CaiPiaoInfoParent;
    public float m_TimePlay = 3f;
    /// <summary>
    /// 显示爆炸.
    /// </summary>
    public float m_TimeShowExp = 2f;
    public Animator m_Animator;
    PlayerEnum m_IndexPlayer;
    Vector3 m_StartPos;
    SSCaiPiaoDataManage.GameCaiPiaoData.DeCaiState m_DeCaiState;
    GameObject m_ExplosionPrefab;
    GameObject m_ExplosionPoint;
    public void Init(PlayerEnum indexPlayer, int caiPiaoNum, Vector3 pos, SSCaiPiaoDataManage.GameCaiPiaoData.DeCaiState type, GameObject exp)
    {
        m_IndexPlayer = indexPlayer;
        m_CaiPiaoNum = caiPiaoNum;
        m_DeCaiState = type;
        m_StartPos = pos;
        m_ExplosionPrefab = exp;

        if (m_ExplosionPoint == null)
        {
            m_ExplosionPoint = new GameObject();
        }
        m_ExplosionPoint.transform.position = pos;
        if (Camera.main != null)
        {
            m_ExplosionPoint.transform.SetParent(Camera.main.transform);
        }

        Vector3 posUI = XkGameCtrl.GetInstance().GetWorldObjToScreenPos(pos);
        transform.localPosition = posUI;
        ShowCaiPiaoInfo();

        if (type == SSCaiPiaoDataManage.GameCaiPiaoData.DeCaiState.JPBoss)
        {
            //开启镜头微动.
            XkGameCtrl.GetInstance().IsDisplayBossDeathYanHua = true;
        }

        if (m_Animator != null)
        {
            IsCloseZhuanPanAni = false;
            m_Animator.enabled = true;
            //m_Animator.enabled = false; //test
        }
        SetActive(true);
        StartCoroutine(DelayShowCaiPiaoInfo());
    }

    bool IsCloseZhuanPanAni = false;
    IEnumerator DelayShowCaiPiaoInfo()
    {
        yield return new WaitForSeconds(m_TimeShowExp);
        ShowCaiPiaoZhanCheBossFlyCaiPiao(m_DeCaiState, m_IndexPlayer, m_StartPos);

        yield return new WaitForSeconds(m_TimePlay);
        IsCloseZhuanPanAni = true;
        if (m_Animator != null)
        {
            m_Animator.enabled = false;
        }
        //Destroy(gameObject);
        //SetActive(false);
    }

    private void Update()
    {
        if (Time.frameCount % 3 == 0)
        {
            if (m_Animator != null)
            {
                if (IsCloseZhuanPanAni == true)
                {
                    //隐藏彩票转盘.
                    SetActive(false);
                }
            }
        }
    }

    internal void SetActive(bool isActive)
    {
        gameObject.SetActive(isActive);
    }

    /// <summary>
    /// 显示战车和boss的飞行彩票.
    /// </summary>
    void ShowCaiPiaoZhanCheBossFlyCaiPiao(SSCaiPiaoDataManage.GameCaiPiaoData.DeCaiState deCaiType, PlayerEnum indexPlayer, Vector3 startPos)
    {
        //Debug.LogWarning("Unity: ShowCaiPiaoZhanCheBossFlyCaiPiao -> deCaiType ========= " + deCaiType);
        if (m_ExplosionPrefab != null)
        {
            if (m_ExplosionPoint != null)
            {
                startPos = m_ExplosionPoint.transform.position;
                Destroy(m_ExplosionPoint);
            }

            if (deCaiType == SSCaiPiaoDataManage.GameCaiPiaoData.DeCaiState.ZhanChe)
            {
                //只给战车产生爆炸粒子.
                GameObject objExplode = (GameObject)Instantiate(m_ExplosionPrefab, startPos, Quaternion.identity);
                objExplode.transform.parent = XkGameCtrl.NpcAmmoArray;
                XkGameCtrl.CheckObjDestroyThisTimed(objExplode);

                SSCaiPiaoLiZiManage caiPiaoLiZi = objExplode.GetComponent<SSCaiPiaoLiZiManage>();
                if (caiPiaoLiZi != null)
                {
                    caiPiaoLiZi.ShowNumUI(m_CaiPiaoNum, indexPlayer);
                }
                else
                {
                    Debug.LogWarning("CheckNpcDeathExplode -> caiPiaoLiZi was null.................");
                }
            }
        }

        if (deCaiType == SSCaiPiaoDataManage.GameCaiPiaoData.DeCaiState.ZhanChe)
        {
            if (XkGameCtrl.GetInstance().m_CaiPiaoFlyData != null)
            {
                //初始化飞出的彩票逻辑.
				XkGameCtrl.GetInstance().m_CaiPiaoFlyData.InitCaiPiaoFly(startPos, indexPlayer, SSCaiPiaoDataManage.GameCaiPiaoData.DeCaiState.ZhanChe);
            }
            else
            {
                Debug.LogWarning("CreatLiZi -> m_CaiPiaoFlyData was null............");
            }
        }
        else if (deCaiType == SSCaiPiaoDataManage.GameCaiPiaoData.DeCaiState.JPBoss)
        {
            if (SSUIRoot.GetInstance().m_GameUIManage != null)
            {
                SSUIRoot.GetInstance().m_GameUIManage.InitCaiPiaoAnimation(XkGameCtrl.GetInstance().m_CaiPiaoFlyData.m_JPBossCaiPiaoFlyDt.TimeLeiJiaVal, indexPlayer);
            }

            if (XkGameCtrl.GetInstance().m_CaiPiaoFlyData != null)
            {
                //初始化烟花粒子的产生.
                XkGameCtrl.GetInstance().m_CaiPiaoFlyData.InitPlayCaiPiaoYanHua();
            }
            else
            {
                Debug.LogWarning("CreatLiZi -> m_CaiPiaoFlyData was null............");
            }
        }
    }

    /// <summary>
    /// 显示彩票信息.
    /// </summary>
    void ShowCaiPiaoInfo()
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

        //if (m_CaiPiaoInfoParent != null)
        //{
        //    m_CaiPiaoInfoParent.SetActive(true);
        //}

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
            if (m_CaiPIaoSprite[i] == null)
            {
                break;
            }

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