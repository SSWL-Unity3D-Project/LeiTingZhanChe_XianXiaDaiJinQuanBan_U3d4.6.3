﻿using UnityEngine;

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
        /// 纵向移动的JPBoss血量信息.
        /// </summary>
        public HealthData JPBossHealth;
        /// <summary>
        /// 横向移动的JPBoss血量信息.
        /// </summary>
        public HealthData JPBossHealthHengXiang;
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
    internal TotalHealthData m_CurrentTotalHealthDt;
    /// <summary>
    /// 当前游戏中存在的是战车还是JPBoss类型.
    /// </summary>
    internal SSCaiPiaoDataManage.GameCaiPiaoData.DaiJinQuanState m_CurrentDaiJinQuanState = SSCaiPiaoDataManage.GameCaiPiaoData.DaiJinQuanState.JPBossDaiJinQuan;
    /// <summary>
    /// 获取JPBoss和战车Npc的血值数据.
    /// </summary>
    internal void GetTotalHealthData(SSCaiPiaoDataManage.GameCaiPiaoData.DaiJinQuanState type)
    {
        m_CurrentDaiJinQuanState = type;
        float caiChiVal = XkPlayerCtrl.GetInstanceFeiJi().m_SpawnNpcManage.m_CaiPiaoDataManage.m_GameCaiPiaoData.GetCaiChiData(type);
        float caiChiChuPiaoTiaoJian = XkPlayerCtrl.GetInstanceFeiJi().m_SpawnNpcManage.m_CaiPiaoDataManage.m_GameCaiPiaoData.GetChuPiaoTiaoJian(type);
        //彩池是否足够.
        bool isCaiChiZuGou = caiChiVal - caiChiChuPiaoTiaoJian >= 0f ? true : false;
        //if (type == SSCaiPiaoDataManage.GameCaiPiaoData.DaiJinQuanState.JPBossDaiJinQuan)
        //{
        //    SSDebug.LogWarning("caiChiVal =========== " + caiChiVal + ", caiChiChuPiaoTiaoJian ============= " + caiChiChuPiaoTiaoJian);
        //}

        float baoJiangLv = XKGlobalData.GetInstance().GetDaiJinQuanBaoJiangLv(type);
        float randVal = Random.Range(0f, 100f) / 100f;
        //是否爆奖.
        bool isDaiJinQuanBaoJiang = randVal < baoJiangLv ? true : false;
        if (SSGameLogoData.m_GameDaiJinQuanMode == SSGameLogoData.GameDaiJinQuanMode.HDL_CaiPinQuan)
        {
            //海底捞菜品券游戏.
            if (SSHaiDiLaoBaoJiang.GetInstance() != null)
            {
                isDaiJinQuanBaoJiang = SSHaiDiLaoBaoJiang.GetInstance().GetIsCanJiBaoNpc(type);
            }
        }

        //isCaiChiZuGou = false; //test
        //SSDebug.LogWarning("isDaiJinQuanBaoJiang == " + isDaiJinQuanBaoJiang + ", randVal == " + randVal + ", baoJiangLv == " + baoJiangLv);
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
            m_CurrentTotalHealthDt = m_BaoJiangHealthDt[indexVal];
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
            m_CurrentTotalHealthDt = m_NoBaoJiangHealthDt[indexVal];
        }
        //SSDebug.Log("GetTotalHealData -> m_CurentTotalHealthDt == " + m_CurrentTotalHealthDt.ToString());
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
    /// 不能被打爆的代金券npc,血值小于0.1之后,在超出这个时间后受到玩家子弹伤害时减少一定的血值.
    /// </summary>
    [Range(0.03f, 10f)]
    public float m_TimeNoDead = 0.3f;
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
        if (m_CurrentTotalHealthDt.IsCanJiBao == true)
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
        /// <summary>
        /// 失误次数.
        /// </summary>
        internal int ShiWuCount = 0;
        internal void Reset()
        {
            BaoJiDengJi = -1;
            ShiWuCount = 0;
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
                case PlayerAmmoType.ChongJiBoAmmo:
                    {
                        if (Time.time - m_PlayerBaoJiDt[indexVal].TimeBaoJi > 0.05f)
                        {
                            //玩家导弹暴击间隔时间必须大于导弹发射的冷却时间.
                            if (npcHealth != null && npcHealth.GetIsDaiJinQuanNpc() == true)
                            {
                                //只有代金券Npc才可以被暴击.
                                if (Time.time - m_PlayerBaoJiDt[indexVal].TimeBaoJi <= m_TimeBaoJi)
                                {
                                    //暴击间隔时间必须小于设定数值.
                                    m_PlayerBaoJiDt[indexVal].ShiWuCount = 0;
                                    if (m_PlayerBaoJiDt[indexVal].BaoJiDengJi < 3)
                                    {
                                        //最高4档.
                                        m_PlayerBaoJiDt[indexVal].BaoJiDengJi++;
                                    }

                                    if (m_PlayerBaoJiDt[indexVal].BaoJiDengJi > 3)
                                    {
                                        m_PlayerBaoJiDt[indexVal].BaoJiDengJi = 3;
                                    }
                                }
                                else
                                {
                                    m_PlayerBaoJiDt[indexVal].ShiWuCount++;
                                }
                                //SSDebug.LogWarning("npcHealth ===================== " + npcHealth
                                //    + ", BaoJiDengJi ==== " + m_PlayerBaoJiDt[indexVal].BaoJiDengJi
                                //    + ", ShiWuCount ==== " + m_PlayerBaoJiDt[indexVal].ShiWuCount
                                //    + ", time ==== " + Time.time.ToString("f3"));

                                m_PlayerBaoJiDt[indexVal].TimeBaoJi = Time.time;
                                if (m_PlayerBaoJiDt[indexVal].ShiWuCount >= 1)
                                {
                                    //失误次数超过n次重置暴击等级信息.
                                    //暴击失效.
                                    m_PlayerBaoJiDt[indexVal].Reset();
                                }
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
            int indexBaoJi = baoJiDengJi;
            if (indexBaoJi > 3)
            {
                //防止暴击索引越界.
                indexBaoJi = 3;
            }

            //SSDebug.Log("GetPlayerBaoJiMaterial -> indexPlayer == " + indexPlayer + ", baoJiDengJi ============ " + baoJiDengJi);
            if (indexBaoJi >= 0)
            {
                if (m_CurrentTotalHealthDt.IsCanJiBao == true)
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

    #region 代金券npc的血条UI恢复管理
    /// <summary>
    /// 代金券npc的血条脚本.
    /// </summary>
    XKNpcHealthCtrl m_DaiJinQuanHealth;
    /// <summary>
    /// 保存代金券npc的血条脚本.
    /// </summary>
    internal void SaveDaiJinQuanHealth(XKNpcHealthCtrl health)
    {
        m_DaiJinQuanHealth = health;
    }

    /// <summary>
    /// 清理代金券npc的血值脚本.
    /// </summary>
    void CleanDaiJinQuanHealth()
    {
        if (m_DaiJinQuanHealth != null)
        {
            m_DaiJinQuanHealth = null;
        }
    }

    /// <summary>
    /// 恢复代金券npc的血值数据及UI信息.
    /// </summary>
    internal void BackDaiJinQuanNpcBlood()
    {
        if (DaoJiShiCtrl.GetIsHavePlayDaoJiShi() == true)
        {
            //有倒计时UI，则不去重置代金券血条信息.
            return;
        }

        if (m_DaiJinQuanHealth != null && m_DaiJinQuanHealth.IsDeathNpc == false)
        {
            //恢复代金券npc的血值数据及UI信息.
            m_DaiJinQuanHealth.BackDaiJinQuanNpcBlood();
            CleanDaiJinQuanHealth();
        }
    }
    #endregion
}
