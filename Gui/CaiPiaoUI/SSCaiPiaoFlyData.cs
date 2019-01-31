using UnityEngine;

public class SSCaiPiaoFlyData : MonoBehaviour
{
    /// <summary>
    /// 彩票boss烟花粒子的高度.
    /// </summary>
    public float m_BossYanHuaOffsetPY = 2.5f;
    /// <summary>
    /// 基础彩票爆炸粒子预制.
    /// </summary>
    public GameObject m_JiChuCaiPiaoLiZiPrefab;
    /// <summary>
    /// 战车被击爆后飞出的彩票预制.
    /// </summary>
    public GameObject m_ZhanCheFlyCaiPiaoPrefab;
    /// <summary>
    /// 随机道具爆炸后飞出的彩票预制.
    /// </summary>
    public GameObject m_FlyCaiPiaoPrefab;
    /// <summary>
    /// 玩家1彩票移动的终点.
    /// </summary>
    internal Transform m_CaiPiaoEndTrP1;
    /// <summary>
    /// 玩家2彩票移动的终点.
    /// </summary>
    internal Transform m_CaiPiaoEndTrP2;
    /// <summary>
    /// 玩家3彩票移动的终点.
    /// </summary>
    internal Transform m_CaiPiaoEndTrP3;

    [System.Serializable]
    public class ZhanCheCaiPiaoFlyData
    {
        /// <summary>
        /// 几张彩票.
        /// </summary>
        public int MaxCaiPiao = 5;
        /// <summary>
        /// 彩票飞行时间.
        /// </summary>
        public float TimeFly = 1f;
        /// <summary>
        /// 彩票UI累加时长.
        /// </summary>
        public float TimeLeiJiaVal = 2f;
    }
    public ZhanCheCaiPiaoFlyData m_ZhanCheCaiPiaoFlyDt;

    [System.Serializable]
    public class SuiJiCaiPiaoFlyData
    {
        /// <summary>
        /// 几张彩票.
        /// </summary>
        public int MaxCaiPiao = 5;
        /// <summary>
        /// 彩票飞行时间.
        /// </summary>
        public float TimeFly = 1f;
        /// <summary>
        /// 彩票UI累加时长.
        /// </summary>
        public float TimeLeiJiaVal = 2f;
    }
    /// <summary>
    /// 随机道具彩票飞行数据.
    /// </summary>
    public SuiJiCaiPiaoFlyData m_SuiJiCaiPiaoFlyDt;

    [System.Serializable]
    public class ZhengChangCaiPiaoFlyData
    {
        /// <summary>
        /// 几张彩票.
        /// </summary>
        public int MaxCaiPiao = 5;
        /// <summary>
        /// 彩票飞行时间.
        /// </summary>
        public float TimeFly = 1f;
        /// <summary>
        /// 彩票UI累加时长.
        /// </summary>
        public float TimeLeiJiaVal = 2f;
    }
    /// <summary>
    /// 随机道具彩票飞行数据.
    /// </summary>
    public ZhengChangCaiPiaoFlyData m_ZhengChangCaiPiaoFlyDt;

    [System.Serializable]
    public class JPBossCaiPiaoFlyData
    {
        /// <summary>
        /// 粒子预制.
        /// </summary>
        public GameObject[] m_LiZiPrefabArray;
        /// <summary>
        /// 产生粒子时长.
        /// </summary>
        public float m_TimeLiZi;
        /// <summary>
        /// 彩票UI累加时长.
        /// </summary>
        public float TimeLeiJiaVal = 2f;
    }
    /// <summary>
    /// JPBoss彩票移动数据.
    /// </summary>
    public JPBossCaiPiaoFlyData m_JPBossCaiPiaoFlyDt;

    void Awake()
    {
        Init();
    }

    void Init()
    {
        AddCaiPiaoFlyManageDtListCom();
    }

    /// <summary>
    /// 彩票飞出数据管理组件.
    /// </summary>
    SSGameObjListManage m_CaiPiaoFlyManageDtList;

    /// <summary>
    /// 添加彩票飞出数据管理组件.
    /// </summary>
    void AddCaiPiaoFlyManageDtListCom()
    {
        if (m_CaiPiaoFlyManageDtList == null)
        {
            m_CaiPiaoFlyManageDtList = gameObject.AddComponent<SSGameObjListManage>();
        }
    }

    /// <summary>
    /// 初始化彩票飞出数据信息.
    /// </summary>
    public void InitCaiPiaoFly(Vector3 startPos, PlayerEnum indexPlayer, SSCaiPiaoDataManage.GameCaiPiaoData.DeCaiState type,
        SSCaiPiaoDataManage.GameCaiPiaoData.DaiJinQuanState daiJinQuanType = SSCaiPiaoDataManage.GameCaiPiaoData.DaiJinQuanState.ZhanCheDaiJinQuan_01)
    {
        if (m_CaiPiaoFlyManageDtList != null)
        {
            GameObject gmDataPrefab = (GameObject)Resources.Load("Prefabs/GameData/CaiPiaoFlyManageDt");
            GameObject obj = m_CaiPiaoFlyManageDtList.FindObjByPrefab(gmDataPrefab, transform);
            if (obj != null)
            {
                //SSCaiPiaoFlyManage caiPiaoFlyManage = gameObject.AddComponent<SSCaiPiaoFlyManage>();
                SSCaiPiaoFlyManage caiPiaoFlyManage = obj.GetComponent<SSCaiPiaoFlyManage>();
                if (caiPiaoFlyManage != null)
                {
                    caiPiaoFlyManage.Init(startPos, indexPlayer, type, daiJinQuanType);
                }
            }
        }
    }

    /// <summary>
    /// 初始化烟花粒子的产生.
    /// </summary>
    public void InitPlayCaiPiaoYanHua()
    {
        SSCaiPiaoYanHua yanHua = gameObject.AddComponent<SSCaiPiaoYanHua>();
        if (yanHua != null)
        {
            yanHua.Init(m_JPBossCaiPiaoFlyDt.m_TimeLiZi);
            SSUIRoot.GetInstance().m_GameUIManage.m_SSCaiPiaoYanHua = yanHua;
        }
    }
}