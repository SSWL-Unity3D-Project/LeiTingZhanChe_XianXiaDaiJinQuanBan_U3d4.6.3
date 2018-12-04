using UnityEngine;

public class SSCaiPiaoHealthData : MonoBehaviour
{
    #region 代金券npc血值数据管理.
    [System.Serializable]
    public class HealthData
    {
        /// <summary>
        /// MaxPuTongAmmo[0] -> 单人模式下.
        /// MaxPuTongAmmo[1] -> 双人模式下.
        /// MaxPuTongAmmo[2] -> 三人模式下.
        /// MaxPuTongAmmo[3] -> 四人模式下.
        /// </summary>
        //[Range(1, 10000000)]
        public int[] MaxPuTongAmmo = { 1, 1, 1, 1 };
        public override string ToString()
        {
            string info = "MaxPuTongAmmo[0] == " + MaxPuTongAmmo[0]
                + ", MaxPuTongAmmo[1] == " + MaxPuTongAmmo[1]
                + ", MaxPuTongAmmo[2] == " + MaxPuTongAmmo[2]
                + ", MaxPuTongAmmo[3] == " + MaxPuTongAmmo[3];
            return info;
        }
    }
    
    [System.Serializable]
    public class TotalHealthData
    {
        /// <summary>
        /// 是否可以被击爆.
        /// </summary>
        internal bool IsCanJiBao = false;
        /// <summary>
        /// UI血量显示比例.
        /// </summary>
        [Range(0.01f, 1f)]
        public float UIHealthPer = 0.5f;
        /// <summary>
        /// JPBoss血量信息.
        /// </summary>
        public HealthData JPBossHealth;
        /// <summary>
        /// 战车Npc血量信息.
        /// </summary>
        public HealthData ZhanCheHealth;
        public override string ToString()
        {
            string info = "IsCanJiBao == " + IsCanJiBao + ", UIHealthPer == " + UIHealthPer
                + ", JPBossHealth -> " + JPBossHealth.ToString()
                + ", ZhanCheHealth -> " + ZhanCheHealth.ToString();
            return info;
        }
    }
    /// <summary>
    /// 可以爆奖的血量控制数据.
    /// </summary>
    public TotalHealthData[] m_BaoJiangHealthDt = new TotalHealthData[3];
    /// <summary>
    /// 不可以爆奖的血量控制数据.
    /// </summary>
    public TotalHealthData[] m_NoBaoJiangHealthDt = new TotalHealthData[3];
    internal void Init()
    {
        for (int i = 0; i < m_BaoJiangHealthDt.Length; i++)
        {
            //可以爆奖的血量控制数据.
            m_BaoJiangHealthDt[i].IsCanJiBao = true;
        }

        for (int i = 0; i < m_NoBaoJiangHealthDt.Length; i++)
        {
            //不可以爆奖的血量控制数据.
            m_NoBaoJiangHealthDt[i].IsCanJiBao = false;
        }

        for (int i = 0; i < m_PlayerBaoJiDt.Length; i++)
        {
            //初始化玩家暴击数据.
            m_PlayerBaoJiDt[i] = new PlayerBaoJiData();
        }
    }

    /// <summary>
    /// 当前采用的彩票npc血量控制数据.
    /// </summary>
    internal TotalHealthData m_CurentTotalHealthDt;
    /// <summary>
    /// 获取JPBoss和战车Npc的血值数据.
    /// </summary>
    internal void GetTotalHealData(SSCaiPiaoDataManage.GameCaiPiaoData.DaiJinQuanState type)
    {
        float caiChiVal = XkPlayerCtrl.GetInstanceFeiJi().m_SpawnNpcManage.m_CaiPiaoDataManage.m_GameCaiPiaoData.GetCaiChiData(type);
        float caiChiChuPiaoTiaoJian = XkPlayerCtrl.GetInstanceFeiJi().m_SpawnNpcManage.m_CaiPiaoDataManage.m_GameCaiPiaoData.GetChuPiaoTiaoJian(type);
        //彩池是否足够.
        bool isCaiChiZuGou = caiChiVal - caiChiChuPiaoTiaoJian >= 0f ? true : false;

        float baoJiangLv = XKGlobalData.GetInstance().GetDaiJinQuanBaoJiangLv(type);
        float randVal = Random.Range(0f, 100f) / 100f;
        //是否爆奖.
        bool isDaiJinQuanBaoJiang = randVal < baoJiangLv ? true : false;
        if (isCaiChiZuGou == true && isDaiJinQuanBaoJiang == true)
        {
            //可以爆奖.
            int indexVal = Random.Range(0, 100) % m_BaoJiangHealthDt.Length;
            if (m_TestBaoJiangData != null)
            {
                if (m_TestBaoJiangData.IsOpenTest == true)
                {
                    if (m_TestBaoJiangData.IndexHealth >= 0 && m_TestBaoJiangData.IndexHealth <= 2)
                    {
                        //测试数据信息.
                        indexVal = m_TestBaoJiangData.IndexHealth;
                    }
                }
            }
            m_CurentTotalHealthDt = m_BaoJiangHealthDt[indexVal];
        }
        else
        {
            //不可以爆奖.
            int indexVal = Random.Range(0, 100) % m_NoBaoJiangHealthDt.Length;
            if (m_TestBaoJiangData != null)
            {
                if (m_TestBaoJiangData.IsOpenTest == true)
                {
                    if (m_TestBaoJiangData.IndexHealth >= 0 && m_TestBaoJiangData.IndexHealth <= 2)
                    {
                        //测试数据信息.
                        indexVal = m_TestBaoJiangData.IndexHealth;
                    }
                }
            }
            m_CurentTotalHealthDt = m_NoBaoJiangHealthDt[indexVal];
        }
        SSDebug.Log("GetTotalHealData -> m_CurentTotalHealthDt == " + m_CurentTotalHealthDt.ToString());
    }
    #endregion

    #region 玩家对代金券npc暴击的数据管理
    /// <summary>
    /// 暴击对代金券npc造成的伤害.
    /// </summary>
    [System.Serializable]
    public class BaoJiNpcDamage
    {
        /// <summary>
        /// 暴击伤害.
        /// </summary>
        public int[] BaoJiDamage = new int[4] { 1, 2, 3, 4 };
    }
    /// <summary>
    /// 可以被击爆的npc造成的暴击伤害.
    /// </summary>
    public BaoJiNpcDamage m_BaoJiDamage_01;
    /// <summary>
    /// 不可以被击爆的npc造成的暴击伤害.
    /// </summary>
    public BaoJiNpcDamage m_BaoJiDamage_02;
    /// <summary>
    /// 暴击的最小间隔时间.
    /// </summary>
    [Range(0.03f, 10f)]
    public float m_TimeBaoJi = 0.5f;
    /// <summary>
    /// 获取暴击伤害.
    /// </summary>
    /// <returns></returns>
    internal int GetBaoJiDamage(PlayerEnum indexPlayer)
    {
        //获取玩家的暴击等级.
        int baoJiDengJi = GetPlayerBaoJiDengJi(indexPlayer);
        int indexVal = Mathf.Clamp(baoJiDengJi - 1, -1, 3);
        if (indexVal <= -1)
        {
            return 0;
        }

        int baoJiDamageVal = 0;
        if (m_CurentTotalHealthDt.IsCanJiBao == true)
        {
            //可以被击爆.
            baoJiDamageVal = m_BaoJiDamage_01.BaoJiDamage[indexVal];
        }
        else
        {
            //不可以被击爆.
            baoJiDamageVal = m_BaoJiDamage_02.BaoJiDamage[indexVal];
        }
        return baoJiDamageVal;
    }
    
    /// <summary>
    /// 玩家暴击数据信息.
    /// </summary>
    public class PlayerBaoJiData
    {
        /// <summary>
        /// 暴击等级.
        /// </summary>
        internal int BaoJiDengJi = -1;
        /// <summary>
        /// 暴击时间记录.
        /// </summary>
        internal float TimeBaoJi = 0f;
        internal void Reset()
        {
            BaoJiDengJi = -1;
            TimeBaoJi = Time.time;
        }
    }
    PlayerBaoJiData[] m_PlayerBaoJiDt = new PlayerBaoJiData[3];
    internal void CheckPlayerBaoJiDengJi(PlayerAmmoType ammoType, PlayerEnum indexPlayer, XKNpcHealthCtrl npcHealth)
    {
        int indexVal = (int)indexPlayer - 1;
        if (indexVal >= 0 && indexVal <= 3)
        {
            if (m_TestBaoJiangData != null)
            {
                if (m_TestBaoJiangData.IsOpenTest == true)
                {
                    if (m_TestBaoJiangData.IndexBaoJi >= 0 && m_TestBaoJiangData.IndexBaoJi <= 3)
                    {
                        //测试暴击数据信息.
                        indexVal = m_TestBaoJiangData.IndexBaoJi;
                        m_PlayerBaoJiDt[indexVal].BaoJiDengJi = indexVal;
                        return;
                    }
                }
            }

            switch (ammoType)
            {
                case PlayerAmmoType.DaoDanAmmo:
                case PlayerAmmoType.PaiJiPaoAmmo:
                case PlayerAmmoType.ChuanTouAmmo:
                case PlayerAmmoType.SanDanAmmo:
                    {
                        if (Time.time - m_PlayerBaoJiDt[indexVal].TimeBaoJi > XKPlayerGlobalDt.GetInstance().TimeShouDongDaoDan * 0.3f)
                        {
                            //玩家导弹暴击间隔时间必须大于导弹发射的冷却时间.
                            if (Time.time - m_PlayerBaoJiDt[indexVal].TimeBaoJi <= m_TimeBaoJi)
                            {
                                //暴击间隔时间必须小于设定数值.
                                if (npcHealth != null && npcHealth.GetIsDaiJinQuanNpc() == true)
                                {
                                    //只有代金券Npc才可以被暴击.
                                    if (m_PlayerBaoJiDt[indexVal].BaoJiDengJi < 3)
                                    {
                                        //最高4档.
                                        m_PlayerBaoJiDt[indexVal].BaoJiDengJi++;
                                    }
                                    m_PlayerBaoJiDt[indexVal].TimeBaoJi = Time.time;
                                    SSDebug.Log("CheckPlayerBaoJiDengJi -> indexPlayer == " + indexPlayer
                                        + ", BaoJiDengJi == " + m_PlayerBaoJiDt[indexVal].BaoJiDengJi);
                                }
                            }
                            else
                            {
                                //暴击失效.
                                m_PlayerBaoJiDt[indexVal].Reset();
                            }
                        }
                        break;
                    }
            }
        }
    }

    /// <summary>
    /// 获取玩家的暴击等级信息.
    /// </summary>
    internal int GetPlayerBaoJiDengJi(PlayerEnum indexPlayer)
    {
        int indexVal = (int)indexPlayer - 1;
        if (m_TestBaoJiangData != null)
        {
            if (m_TestBaoJiangData.IsOpenTest == true)
            {
                if (m_TestBaoJiangData.IndexBaoJi >= 0 && m_TestBaoJiangData.IndexBaoJi <= 3)
                {
                    //测试暴击数据信息.
                    indexVal = m_TestBaoJiangData.IndexBaoJi;
                }
            }
        }

        if (indexVal >= 0 && indexVal <= 3)
        {
            return m_PlayerBaoJiDt[indexVal].BaoJiDengJi;
        }
        return 0;
    }

    /// <summary>
    /// 暴击粒子材质球.
    /// </summary>
    [System.Serializable]
    public class BaoJiLiZiMaterial
    {
        /// <summary>
        /// 不同暴击对应的粒子材质球.
        /// </summary>
        public Material[] BaoJiMaterial = new Material[4];
    }

    /// <summary>
    /// 玩家暴击粒子材质球.
    /// </summary>
    [System.Serializable]
    public class PlayerBaoJiLiZiMaterial
    {
        /// <summary>
        /// 可以被玩家击爆的暴击效果对应的粒子材质球.
        /// </summary>
        public BaoJiLiZiMaterial BaoJiMaterial_01;
        /// <summary>
        /// 不可以被玩家击爆的暴击效果对应的粒子材质球.
        /// </summary>
        public BaoJiLiZiMaterial BaoJiMaterial_02;
    }
    public PlayerBaoJiLiZiMaterial[] m_PlayerBaoJiLiZiMaterial = new PlayerBaoJiLiZiMaterial[3];
    public Material GetPlayerBaoJiMaterial(PlayerEnum indexPlayer, int baoJiDengJi)
    {
        Material mat = null;
        int indexVal = (int)indexPlayer - 1;
        if (indexVal >= 0 && indexVal <= m_PlayerBaoJiLiZiMaterial.Length)
        {
            int indexBaoJi = baoJiDengJi - 1;
            if (indexBaoJi > 3)
            {
                //防止暴击索引越界.
                indexBaoJi = 3;
            }

            if (indexBaoJi >= 0)
            {
                if (m_CurentTotalHealthDt.IsCanJiBao == true)
                {
                    //可以被击爆.
                    mat = m_PlayerBaoJiLiZiMaterial[indexVal].BaoJiMaterial_01.BaoJiMaterial[indexBaoJi];
                }
                else
                {
                    //不可以被击爆.
                    mat = m_PlayerBaoJiLiZiMaterial[indexVal].BaoJiMaterial_02.BaoJiMaterial[indexBaoJi];
                }
            }
        }
        return mat;
    }
    #endregion

    #region 爆奖数据测试.
    [System.Serializable]
    public class TestBaoJiangData
    {
        /// <summary>
        /// 是否打开测试.
        /// </summary>
        public bool IsOpenTest = false;
        /// <summary>
        /// 爆奖或不爆奖时,代金券npc的血条索引.
        /// IndexHealth = -1 ---> 采用系统随机数据.
        /// IndexHealth = [0, 2] ---> 采用该处配置的数据.
        /// </summary>
        [Range(-1, 2)]
        public int IndexHealth = -1;

        /// <summary>
        /// 暴击效果时指定暴击数据的索引.
        /// IndexBaoJi = -1 ---> 采用系统数据.
        /// IndexBaoJi = [0, 3] ---> 采用该处配置的数据.
        /// </summary>
        [Range(-1, 3)]
        public int IndexBaoJi = -1;
        /// <summary>
        /// 测试将所有玩家的子弹在玩家1的开火点发出.
        /// </summary>
        public bool IsTestPlayerAmmo = false;
    }
    /// <summary>
    /// 测试游戏暴击或血值数据.
    /// 代金券npc是否可以击爆在爆奖率数据进行调控.
    /// </summary>
    public TestBaoJiangData m_TestBaoJiangData;
    #endregion
}
