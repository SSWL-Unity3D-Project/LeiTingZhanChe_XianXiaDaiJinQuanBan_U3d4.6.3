//显示玩家彩票信息.
//#define TEST_SHOW_PLAYER_CAIPIAO
//#define TEST_OUT_PRINT_CARD
//#define CREATE_SUPER_JPBOSS
using Assets.XKGame.Script.HongDDGamePad;
using System.Collections.Generic;
using UnityEngine;

public class SSCaiPiaoDataManage : SSGameMono
{
    public class GuDingBanCaiPiaoJPBossData
    {
        /// <summary>
        /// 最小启动币值.
        /// </summary>
        internal int MinCoin = 0;
        /// <summary>
        /// 最大启动币值.
        /// </summary>
        internal int MaxCoin = 0;
        /// <summary>
        /// JP大奖打爆固定放彩数量.
        /// </summary>
        internal int JPBossDeCai = 0;
        public GuDingBanCaiPiaoJPBossData(int min, int max, int caiPiaoVal)
        {
            MinCoin = min;
            MaxCoin = max;
            JPBossDeCai = caiPiaoVal;
        }
    }
    /// <summary>
    /// 固定板彩票数据.
    /// </summary>
    public class GuDingBanCaiPiaoData
    {
        /// <summary>
        /// 战车打爆固定彩票20张(战车出彩条件)
        /// 代金券20元.
        /// </summary>
        int ZhanCheDeCai_20 = 20;
        /// <summary>
        /// 战车打爆固定彩票30张(战车出彩条件)
        /// 代金券50元.
        /// </summary>
        int ZhanCheDeCai_50 = 50;
        /// <summary>
        /// JP大奖打爆固定放彩1000、2000、3000张，具体根据场地设置的几币启动及一币等于多少张彩票确定.
        /// 1——3币启动，1000张
        /// 4——5币启动，2000张
        /// 6——10币启动，3000张
        /// </summary>
        internal GuDingBanCaiPiaoJPBossData[] JPBossDeCaiData = new GuDingBanCaiPiaoJPBossData[3]
        {
            new GuDingBanCaiPiaoJPBossData(1, 3, 100),
            new GuDingBanCaiPiaoJPBossData(4, 5, 100),
            new GuDingBanCaiPiaoJPBossData(6, 10, 100),
        };

        /// <summary>
        /// 获取JPBoss出票条件.
        /// </summary>
        public int GetJPBossChuPiaoTiaoJian()
        {
            int chuPiaoVal = 0;
            int coinStart = XKGlobalData.GameNeedCoin;
            int length = JPBossDeCaiData.Length;
            for (int i = 0; i < length; i++)
            {
                if (coinStart >= JPBossDeCaiData[i].MinCoin && coinStart <= JPBossDeCaiData[i].MaxCoin)
                {
                    chuPiaoVal = JPBossDeCaiData[i].JPBossDeCai;
                    break;
                }
            }
            return chuPiaoVal;
        }

        /// <summary>
        /// 获取战车出彩条件.
        /// </summary>
        public int GetZhanCheChuCaiTiaoJian(GameCaiPiaoData.DaiJinQuanState daiJinQuan)
        {
            int chuCaiTiaoJian = 1000;
            switch (daiJinQuan)
            {
                case GameCaiPiaoData.DaiJinQuanState.ZhanCheDaiJinQuan_01:
                    {
                        chuCaiTiaoJian = ZhanCheDeCai_20;
                        break;
                    }
                case GameCaiPiaoData.DaiJinQuanState.ZhanCheDaiJinQuan_02:
                    {
                        chuCaiTiaoJian = ZhanCheDeCai_50;
                        break;
                    }
            }
            return chuCaiTiaoJian;
        }
    }
    public GuDingBanCaiPiaoData m_GuDingBanCaiPiaoData = new GuDingBanCaiPiaoData();

    /// <summary>
    /// JPBoss大奖数据.
    /// </summary>
    [System.Serializable]
    public class JPBossDaJiangData
    {
        public Transform CaiPiaoLiZiPoint;
        /// <summary>
        /// 彩票粒子预制.
        /// </summary>
        public GameObject CaiPiaoLiZiPrefab;
    }
    /// <summary>
    /// JPBoss大奖彩票粒子.
    /// </summary>
    GameObject m_JPBossDaJiangCaiPiaoLiZiObj;
    /// <summary>
    /// JPBoss大奖数据.
    /// </summary>
    public JPBossDaJiangData m_JPBossDaJiangData;
    /// <summary>
    /// 创建JPBoss大奖彩票粒子.
    /// </summary>
    public void CreatJPBossDaJiangCaiPiaoLiZi()
    {
        if (m_JPBossDaJiangCaiPiaoLiZiObj == null)
        {
            if (m_JPBossDaJiangData.CaiPiaoLiZiPrefab != null && m_JPBossDaJiangData.CaiPiaoLiZiPoint != null)
            {
                m_JPBossDaJiangCaiPiaoLiZiObj = (GameObject)Instantiate(m_JPBossDaJiangData.CaiPiaoLiZiPrefab, m_JPBossDaJiangData.CaiPiaoLiZiPoint);
            }
            else
            {
                UnityLogWarning("CreatJPBossDaJiangCaiPiaoLiZi -> CaiPiaoLiZiPrefab or CaiPiaoLiZiPoint was null!");
            }
        }
    }

    /// <summary>
    /// 删除JPBoss大奖彩票粒子.
    /// </summary>
    public void RemoveJPBossDaJiangCaiPiaoLiZi()
    {
        if (m_JPBossDaJiangCaiPiaoLiZiObj != null)
        {
            Destroy(m_JPBossDaJiangCaiPiaoLiZiObj);
        }
    }

    /// <summary>
    /// 游戏彩票数据.
    /// </summary>
    [System.Serializable]
    public class GameCaiPiaoData
    {
        float _XuBiChuPiaoLv = 0.7f;
        /// <summary>
        /// 续币出票率.
        /// </summary>
        public float XuBiChuPiaoLv
        {
            set
            {
                _XuBiChuPiaoLv = value;
            }
            get
            {
                return _XuBiChuPiaoLv;
            }
        }
        float _ZhengChangChuPiaoLv = 0f;
        /// <summary>
        /// 正常得彩出票率.
        /// </summary>
        public float ZhengChangChuPiaoLv
        {
            set
            {
                _ZhengChangChuPiaoLv = value;
            }
            get
            {
                return _ZhengChangChuPiaoLv;
            }
        }
        /// <summary>
        /// 随机道具出票率.
        /// 1元代金券.
        /// </summary>
        float SuiJiDaoJuChuPiaoLv = 0f;
        /// <summary>
        /// 战车得彩出票率.
        /// 战车代金券01低面额.
        /// </summary>
        float ZhanCheChuPiaoLv_01 = 0f;
        /// <summary>
        /// 战车得彩出票率.
        /// 战车代金券02高面额.
        /// </summary>
        float ZhanCheChuPiaoLv_02 = 0f;
        /// <summary>
        /// JPBoss出票率.
        /// 100元代金券.
        /// </summary>
        float JPBossChuPiaoLv = 1f;
        /// <summary>
        /// 更新游戏代金券出票率数据信息.
        /// </summary>
        internal void UpdateChuPiaoLvInfo(float suiJi, float zhanChe01, float zhanChe02, float jpBoss)
        {
            SuiJiDaoJuChuPiaoLv = suiJi;
            ZhanCheChuPiaoLv_01 = zhanChe01;
            ZhanCheChuPiaoLv_02 = zhanChe02;
            JPBossChuPiaoLv = jpBoss;
            SSDebug.Log("UpdateChuPiaoLvInfo -> suiJi == " + suiJi + ", zhanChe01 == " + zhanChe01
                + ", zhanChe02 == " + zhanChe02 + ", jpBoss == " + jpBoss);
        }
        /// <summary>
        /// 随机道具出票条件(游戏启动币数乘以该值).
        /// 随机道具代金券.
        /// </summary>
        internal float SuiJiDaoJuDaiJinQuan = 10f;
        /// <summary>
        /// 战车代金券01出票条件(游戏启动币数乘以该值).
        /// </summary>
        internal float ZhanCheDaiJinQuan_01 = 5f;
        /// <summary>
        /// 战车代金券02出票条件(游戏启动币数乘以该值).
        /// </summary>
        internal float ZhanCheDaiJinQuan_02 = 20f;
        /// <summary>
        /// JPBoss出票条件(游戏启动币数乘以该值).
        /// JPBoss代金券200元.
        /// </summary>
        internal float JPBossDaiJinQuan = 200f;
        /// <summary>
        /// JPBoss代金券商户支付金额.
        /// </summary>
        internal float JPBossDaiJinQuanShangHuZhiFu = 150f;
        /// <summary>
        /// 更新游戏代金券面额数据信息.
        /// </summary>
        internal void UpdateDaiJinQuanInfo(float suiJi, float zhanChe01, float zhanChe02, float jpBoss)
        {
            SuiJiDaoJuDaiJinQuan = suiJi;
            ZhanCheDaiJinQuan_01 = zhanChe01;
            ZhanCheDaiJinQuan_02 = zhanChe02;
            JPBossDaiJinQuan = jpBoss;
            SSDebug.Log("UpdateDaiJinQuanInfo -> suiJi == " + suiJi + ", zhanChe01 == " + zhanChe01
                + ", zhanChe02 == " + zhanChe02 + ", jpBoss == " + jpBoss);
        }

        /// <summary>
        /// 根据代金券分类获取出票条件.
        /// </summary>
        internal float GetChuPiaoTiaoJian(DaiJinQuanState type)
        {
            float chuPiaoTiaoJian = 0f;
            switch (type)
            {
                case DaiJinQuanState.SuiJiDaoJuDaiJinQuan:
                    {
                        chuPiaoTiaoJian = SuiJiDaoJuDaiJinQuan;
                        break;
                    }
                case DaiJinQuanState.ZhanCheDaiJinQuan_01:
                    {
                        chuPiaoTiaoJian = ZhanCheDaiJinQuan_01;
                        break;
                    }
                case DaiJinQuanState.ZhanCheDaiJinQuan_02:
                    {
                        chuPiaoTiaoJian = ZhanCheDaiJinQuan_02;
                        break;
                    }
                case DaiJinQuanState.JPBossDaiJinQuan:
                    {
                        chuPiaoTiaoJian = JPBossDaiJinQuan - JPBossDaiJinQuanShangHuZhiFu;
                        if (chuPiaoTiaoJian <= 0f)
                        {
                            //当商家支付JPBoss的代金券面额大于JPBoss代金券面额时.
                            chuPiaoTiaoJian = 10f;
                        }
                        break;
                    }
            }
            return chuPiaoTiaoJian;
        }

        /// <summary>
        /// 更新JPBoss代金券商户支付金额信息.
        /// </summary>
        internal void UpdateJPBossDaiJinQuanShangHuZhiFu(float jpBossDaiJinQuanShangHuZhiFu)
        {
            JPBossDaiJinQuanShangHuZhiFu = jpBossDaiJinQuanShangHuZhiFu;
            SSDebug.Log("UpdateDaiJinQuanInfo -> jpBossDaiJinQuanShangHuZhiFu == " + jpBossDaiJinQuanShangHuZhiFu);
        }

        /// <summary>
        /// 随机道具得彩累积数量.
        /// </summary>
        public float SuiJiDaoJuDeCai
        {
            set
            {
                XKGlobalData.GetInstance().SetDaoJuCaiChi(value);
            }
            get
            {
                return XKGlobalData.GetInstance().m_DaoJuCaiChi;
            }
        }
        float _ZhanCheDeCai_01 = 0;
        /// <summary>
        /// 战车得彩累积数量.
        /// 20元代金券累计数量.
        /// </summary>
        public float ZhanCheDeCai_01
        {
            set
            {
                XKGlobalData.GetInstance().SetZhanCheCaiChi(value, DaiJinQuanState.ZhanCheDaiJinQuan_01);
                _ZhanCheDeCai_01 = value;
            }
            get
            {
                return _ZhanCheDeCai_01;
            }
        }
        float _ZhanCheDeCai_02 = 0;
        /// <summary>
        /// 战车得彩累积数量.
        /// 50元代金券累计数量.
        /// </summary>
        public float ZhanCheDeCai_02
        {
            set
            {
                XKGlobalData.GetInstance().SetZhanCheCaiChi(value, DaiJinQuanState.ZhanCheDaiJinQuan_02);
                _ZhanCheDeCai_02 = value;
            }
            get
            {
                return _ZhanCheDeCai_02;
            }
        }
        float _JPBossDeCai = 0;
        /// <summary>
        /// JPBoss得彩累积数量.
        /// </summary>
        public float JPBossDeCai
        {
            set
            {
                XKGlobalData.GetInstance().SetJPBossCaiChi(value);
                _JPBossDeCai = value;
            }
            get
            {
                return _JPBossDeCai;
            }
        }
        /// <summary>
        /// 更新游戏代金券各项彩池的数据信息.
        /// </summary>
        internal void UpdateDaiJinQuanCaiChiInfo(float suiJi, float zhanChe01, float zhanChe02, float jpBoss)
        {
            SuiJiDaoJuDeCai = suiJi;
            ZhanCheDeCai_01 = zhanChe01;
            ZhanCheDeCai_02 = zhanChe02;
            JPBossDeCai = jpBoss;
            SSDebug.Log("UpdateDaiJinQuanCaiChiInfo -> suiJi == " + suiJi + ", zhanChe01 == " + zhanChe01
                + ", zhanChe02 == " + zhanChe02 + ", jpBoss == " + jpBoss);
        }
        
        /// <summary>
        /// 根据代金券分类获取对应的彩池数据.
        /// </summary>
        internal float GetCaiChiData(DaiJinQuanState type)
        {
            float val = 0f;
            switch (type)
            {
                case DaiJinQuanState.SuiJiDaoJuDaiJinQuan:
                    {
                        val = SuiJiDaoJuDeCai;
                        break;
                    }
                case DaiJinQuanState.ZhanCheDaiJinQuan_01:
                    {
                        val = ZhanCheDeCai_01;
                        break;
                    }
                case DaiJinQuanState.ZhanCheDaiJinQuan_02:
                    {
                        val = ZhanCheDeCai_02;
                        break;
                    }
                case DaiJinQuanState.JPBossDaiJinQuan:
                    {
                        val = JPBossDeCai;
                        break;
                    }
            }
            return val;
        }

        /// <summary>
        /// 得彩状态.
        /// </summary>
        public enum DeCaiState
        {
            /// <summary>
            /// 战车类型.
            /// </summary>
            ZhanChe = 0,
            /// <summary>
            /// 随机道具类型.
            /// </summary>
            SuiJiDaoJu = 1,
            /// <summary>
            /// JPBoss类型.
            /// </summary>
            JPBoss = 2,
            /// <summary>
            /// 普通正常得彩类型.
            /// </summary>
            ZhengChang = 3,
        }

        /// <summary>
        /// 代金券枚举.
        /// </summary>
        public enum DaiJinQuanState
        {
            /// <summary>
            /// 1元代金券.
            /// 随机道具代金券.
            /// </summary>
            SuiJiDaoJuDaiJinQuan = 1,
            /// <summary>
            /// 20元代金券.
            /// 战车代金券01.
            /// </summary>
            ZhanCheDaiJinQuan_01 = 20,
            /// <summary>
            /// 50元代金券.
            /// 战车代金券02.
            /// </summary>
            ZhanCheDaiJinQuan_02 = 50,
            /// <summary>
            /// 100元代金券.
            /// JPBoss代金券.
            /// </summary>
            JPBossDaiJinQuan = 100,
        }

        /// <summary>
        /// 分配得彩数量信息.
        /// </summary>
        public void FenPeiDeCaiVal(bool isPlayerXuBi)
        {
            float coinStart = XKGlobalData.GetInstance().m_CoinToCard * XKGlobalData.GameNeedCoin * XKGlobalData.GetInstance().m_CaiChiFanJiangLv;
            float xuBiChuPiaoLvTmp = isPlayerXuBi == true ? XuBiChuPiaoLv : 1f;
            if (isPlayerXuBi)
            {
                //玩家续币积累到预支彩票池的彩票数量.
                int jiLeiToYuZhiCaiPiaoChiVal = (int)(coinStart * XuBiChuPiaoLv);
                XkPlayerCtrl.GetInstanceFeiJi().m_SpawnNpcManage.m_CaiPiaoDataManage.m_GameYuZhiCaiPiaoData.AddYuZhiCaiPiao(jiLeiToYuZhiCaiPiaoChiVal);
            }

            coinStart = xuBiChuPiaoLvTmp * coinStart;
            ZhanCheDeCai_01 += (ZhanCheChuPiaoLv_01 * coinStart);
            ZhanCheDeCai_02 += (ZhanCheChuPiaoLv_02 * coinStart);
            float suiJiDaoJuDeCaiFenPei = SuiJiDaoJuChuPiaoLv * coinStart;
            //if (suiJiDaoJuDeCaiFenPei < 1)
            //{
            //    //至少给随机道具分配一张彩票.
            //    suiJiDaoJuDeCaiFenPei = 1;
            //}
            SuiJiDaoJuDeCai += suiJiDaoJuDeCaiFenPei;
            JPBossDeCai += (JPBossChuPiaoLv * coinStart);
            Debug.Log("Unity: FenPeiDeCaiVal -> coinStart == " + coinStart
                + ", ZhanCheDeCai_20 == " + ZhanCheDeCai_01
                + ", ZhanCheDeCai_50 == " + ZhanCheDeCai_02
                + ", SuiJiDaoJuDeCai == " + SuiJiDaoJuDeCai
                + ", JPBossDeCai == " + JPBossDeCai
                + ", isPlayerXuBi ==== " + isPlayerXuBi);
        }

        /// <summary>
        /// 减去游戏某种类型得彩累积数量.
        /// </summary>
        public void SubGameDeCaiValByDeCaiState(PlayerEnum index, DeCaiState type, SuiJiDaoJuState suiJiDaoJuType = SuiJiDaoJuState.BaoXiang, DaiJinQuanState daiJinQuan = DaiJinQuanState.ZhanCheDaiJinQuan_01)
        {
            if (XkGameCtrl.GetInstance().m_GamePlayerAiData.IsActiveAiPlayer == true)
            {
                //没有激活任何玩家.
                return;
            }

            int val = 0;
            //int coinStart = XKGlobalData.GetInstance().m_CoinToCard * XKGlobalData.GameNeedCoin;
            switch (type)
            {
                case DeCaiState.ZhanChe:
                    {
                        //if (XkGameCtrl.GetInstance().m_CaiPiaoMode == XkGameCtrl.CaiPiaoModeSuanFa.GuDing)
                        //{
                        //    val = XkPlayerCtrl.GetInstanceFeiJi().m_SpawnNpcManage.m_CaiPiaoDataManage.m_GuDingBanCaiPiaoData.ZhanCheDeCai;
                        //}
                        //else
                        //{
                        //    val = (int)(coinStart * ZhanCheChuPiaoTiaoJian);
                        //}
                        if (daiJinQuan == DaiJinQuanState.ZhanCheDaiJinQuan_02)
                        {
                            val = (int)ZhanCheDaiJinQuan_02;
                            ZhanCheDeCai_02 -= val;
                        }
                        else
                        {
                            val = (int)ZhanCheDaiJinQuan_01;
                            ZhanCheDeCai_01 -= val;
                        }
                        //从预制彩池里取彩票投入战车彩池.
                        XkPlayerCtrl.GetInstanceFeiJi().m_SpawnNpcManage.m_CaiPiaoDataManage.m_GameYuZhiCaiPiaoData.SubZhanCheCaiPiaoVal();
                        break;
                    }
                case DeCaiState.SuiJiDaoJu:
                    {
                        //随机道具.
                        float suiJiDaoJuChuPiaoLv = 0f;
                        SuiJiDaoJuData suiJiDaoJuData = XkPlayerCtrl.GetInstanceFeiJi().m_SpawnNpcManage.m_CaiPiaoDataManage.m_SuiJiDaoJuData;
                        if (suiJiDaoJuType == SuiJiDaoJuState.TouZi)
                        {
                            suiJiDaoJuChuPiaoLv = suiJiDaoJuData.TouZiDePiaoLv;
                        }
                        else
                        {
                           suiJiDaoJuChuPiaoLv = suiJiDaoJuData.BaoXiangDePiaoLv;
                        }
                        //val = (int)(coinStart * SuiJiDaoJuChuPiaoTiaoJian);

                        //应该给玩家的彩票数量.
                        //int outCaiPiao = (int)(val * suiJiDaoJuChuPiaoLv);
                        //1元代金券.
                        //outCaiPiao = 1;

                        //随机道具积累到预支彩票池的彩票数量.
                        //int jiLeiToYuZhiCaiPiaoChiVal = val - outCaiPiao;
                        //XkPlayerCtrl.GetInstanceFeiJi().m_SpawnNpcManage.m_CaiPiaoDataManage.m_GameYuZhiCaiPiaoData.AddYuZhiCaiPiao(jiLeiToYuZhiCaiPiaoChiVal);

                        //val = outCaiPiao;
                        val = (int)SuiJiDaoJuDaiJinQuan;
                        SuiJiDaoJuDeCai -= val;
                        break;
                    }
                case DeCaiState.JPBoss:
                    {
                        int subVal = 0;
                        if (XkGameCtrl.GetInstance().m_CaiPiaoMode == XkGameCtrl.CaiPiaoModeSuanFa.GuDing)
                        {
                            //val = XkPlayerCtrl.GetInstanceFeiJi().m_SpawnNpcManage.m_CaiPiaoDataManage.m_GuDingBanCaiPiaoData.GetJPBossChuPiaoTiaoJian();
                            val = (int)JPBossDaiJinQuan;
                            subVal = (int)(JPBossDaiJinQuan - JPBossDaiJinQuanShangHuZhiFu); //奖池需要减少的数值.
                            if (subVal < 0)
                            {
                                subVal = 0;
                            }
                        }
                        //else
                        //{
                        //    val = (int)(coinStart * JPBossDaiJinQuan);
                        //}
                        JPBossDeCai -= subVal;
                        //从预制彩池里取彩票投入JPBoss彩池.
                        XkPlayerCtrl.GetInstanceFeiJi().m_SpawnNpcManage.m_CaiPiaoDataManage.m_GameYuZhiCaiPiaoData.SubJPBossCaiPiaoVal();
                        break;
                    }
            }

            if (val > 0)
            {
                //此时彩票机应该给对应玩家出val张彩票.
                /*Debug.Log("Unity: SubGameDeCaiValByDeCaiState -> index ====== " + index
                    + ", chuPiaoVal ====== " + val
                    + ", type ======= " + type);*/
                XkPlayerCtrl.GetInstanceFeiJi().m_SpawnNpcManage.m_CaiPiaoDataManage.AddCaiPiaoToPlayer(index, val, type);
            }
        }

        /// <summary>
        /// 获取需要打印的彩票数量.
        /// </summary>
        public int GetPrintCaiPiaoValueByDeCaiState(DeCaiState type, SuiJiDaoJuState suiJiDaoJuType = SuiJiDaoJuState.BaoXiang, DaiJinQuanState daiJinQuan = DaiJinQuanState.ZhanCheDaiJinQuan_01)
        {
            int value = 0;
            //int coinStart = XKGlobalData.GetInstance().m_CoinToCard * XKGlobalData.GameNeedCoin;
            switch (type)
            {
                case DeCaiState.ZhanChe:
                    {
                        //if (XkGameCtrl.GetInstance().m_CaiPiaoMode == XkGameCtrl.CaiPiaoModeSuanFa.GuDing)
                        //{
                        //    value = XkPlayerCtrl.GetInstanceFeiJi().m_SpawnNpcManage.m_CaiPiaoDataManage.m_GuDingBanCaiPiaoData.ZhanCheDeCai;
                        //}
                        //else
                        //{
                        //    value = (int)(coinStart * ZhanCheChuPiaoTiaoJian);
                        //}
                        if (daiJinQuan == DaiJinQuanState.ZhanCheDaiJinQuan_02)
                        {
                            value = (int)ZhanCheDaiJinQuan_02;
                        }
                        else
                        {
                            value = (int)ZhanCheDaiJinQuan_01;
                        }
                        break;
                    }
                case DeCaiState.SuiJiDaoJu:
                    {
                        float suiJiDaoJuChuPiaoLv = 0f;
                        SuiJiDaoJuData suiJiDaoJuData = XkPlayerCtrl.GetInstanceFeiJi().m_SpawnNpcManage.m_CaiPiaoDataManage.m_SuiJiDaoJuData;
                        if (suiJiDaoJuType == SuiJiDaoJuState.TouZi)
                        {
                            suiJiDaoJuChuPiaoLv = suiJiDaoJuData.TouZiDePiaoLv;
                        }
                        else
                        {
                            suiJiDaoJuChuPiaoLv = suiJiDaoJuData.BaoXiangDePiaoLv;
                        }
                        //value = (int)(coinStart * SuiJiDaoJuChuPiaoTiaoJian);
                        //应该给玩家的彩票数量.
                        //value = (int)(value * suiJiDaoJuChuPiaoLv);
                        //1元代金券.
                        value = (int)SuiJiDaoJuDaiJinQuan;
                        break;
                    }
                case DeCaiState.JPBoss:
                    {
                        if (XkGameCtrl.GetInstance().m_CaiPiaoMode == XkGameCtrl.CaiPiaoModeSuanFa.GuDing)
                        {
                            //value = XkPlayerCtrl.GetInstanceFeiJi().m_SpawnNpcManage.m_CaiPiaoDataManage.m_GuDingBanCaiPiaoData.GetJPBossChuPiaoTiaoJian();
                            value = (int)JPBossDaiJinQuan;
                        }
                        //else
                        //{
                        //    value = (int)(coinStart * JPBossDaiJinQuan);
                        //}
                        break;
                    }
            }
            return value;
        }

        /// <summary>
        /// 判断是否达到某种得彩类型的出彩条件.
        /// </summary>
        public bool GetIsChuCaiPiaoByDeCaiState(DeCaiState type, DaiJinQuanState daiJinQuan = DaiJinQuanState.ZhanCheDaiJinQuan_01)
        {
            bool isChuPiao = false;
            //int coinToCaiPiao = XKGlobalData.GetInstance().m_CoinToCard * XKGlobalData.GameNeedCoin;
            float chuPiaoTiaoJian = 0f;
            float deCaiVal = -1f;
            switch (type)
            {
                case DeCaiState.ZhanChe:
                    {
                        if (daiJinQuan == DaiJinQuanState.ZhanCheDaiJinQuan_02)
                        {
                            chuPiaoTiaoJian = ZhanCheDaiJinQuan_02;
                            deCaiVal = ZhanCheDeCai_02;
                        }
                        else
                        {
                            chuPiaoTiaoJian = ZhanCheDaiJinQuan_01;
                            deCaiVal = ZhanCheDeCai_01;
                        }
                        break;
                    }
                case DeCaiState.SuiJiDaoJu:
                    {
                        chuPiaoTiaoJian = SuiJiDaoJuDaiJinQuan;
                        deCaiVal = SuiJiDaoJuDeCai;
                        break;
                    }
                case DeCaiState.JPBoss:
                    {
                        chuPiaoTiaoJian = JPBossDaiJinQuan;
                        deCaiVal = JPBossDeCai;
                        break;
                    }
            }

            int chuCaiVal = 0;
            if (XkGameCtrl.GetInstance().m_CaiPiaoMode == XkGameCtrl.CaiPiaoModeSuanFa.GuDing
                && XkPlayerCtrl.GetInstanceFeiJi() != null)
            {
                switch (type)
                {
                    case DeCaiState.ZhanChe:
                        {
                            chuCaiVal = XkPlayerCtrl.GetInstanceFeiJi().m_SpawnNpcManage.m_CaiPiaoDataManage.m_GuDingBanCaiPiaoData.GetZhanCheChuCaiTiaoJian(daiJinQuan);
                            break;
                        }
                    case DeCaiState.JPBoss:
                        {
                            //chuCaiVal = XkPlayerCtrl.GetInstanceFeiJi().m_SpawnNpcManage.m_CaiPiaoDataManage.m_GuDingBanCaiPiaoData.GetJPBossChuPiaoTiaoJian();
                            chuCaiVal = (int)GetChuPiaoTiaoJian(DaiJinQuanState.JPBossDaiJinQuan);
                            break;
                        }
                    case DeCaiState.SuiJiDaoJu:
                        {
                            chuCaiVal = (int)GetChuPiaoTiaoJian(DaiJinQuanState.SuiJiDaoJuDaiJinQuan);
                            break;
                        }
                }
            }

            if (deCaiVal >= chuCaiVal)
            {
                isChuPiao = true;
                //Debug.Log("Unity: GetIsChuCaiPiaoBy -> the type is can shuCaiPiao! type ============ " + type);
            }
            
            if (type == DeCaiState.SuiJiDaoJu && isChuPiao == true)
            {
                //随机道具判断爆奖率.
                float randVal = Random.Range(0f, 100f);
                if (randVal > XKGlobalData.GetInstance().SuiJiDaoJuBaoJiangLv)
                {
                    //没有随机到爆奖率区间.
                    isChuPiao = false;
                }
            }
            return isChuPiao;
        }

        /// <summary>
        /// 获取当前彩池是出票条件的几倍.
        /// </summary>
        public int GetChuPiaoTiaoJianBeiShu(DeCaiState type, DaiJinQuanState daiJinQuan = DaiJinQuanState.ZhanCheDaiJinQuan_01)
        {
            float coinToCaiPiao = XKGlobalData.GetInstance().m_CoinToCard;
            float chuPiaoTiaoJian = 0f;
            float deCaiVal = -1f;
            switch (type)
            {
                case DeCaiState.ZhanChe:
                    {
                        //chuPiaoTiaoJian = ZhanCheChuPiaoTiaoJian;
                        if (daiJinQuan == DaiJinQuanState.ZhanCheDaiJinQuan_02)
                        {
                            chuPiaoTiaoJian = ZhanCheDaiJinQuan_02;
                            deCaiVal = ZhanCheDeCai_02;
                        }
                        else
                        {
                            chuPiaoTiaoJian = ZhanCheDaiJinQuan_01;
                            deCaiVal = ZhanCheDeCai_01;
                        }
                        break;
                    }
                case DeCaiState.SuiJiDaoJu:
                    {
                        chuPiaoTiaoJian = SuiJiDaoJuDaiJinQuan;
                        deCaiVal = SuiJiDaoJuDeCai;
                        break;
                    }
                case DeCaiState.JPBoss:
                    {
                        chuPiaoTiaoJian = JPBossDaiJinQuan;
                        deCaiVal = JPBossDeCai;
                        break;
                    }
            }

            float chuCaiVal = coinToCaiPiao * chuPiaoTiaoJian;
            return (int)(deCaiVal / chuCaiVal);
        }
    }
    /// <summary>
    /// 游戏彩票数据信息.
    /// </summary>
    [HideInInspector]
    public GameCaiPiaoData m_GameCaiPiaoData = new GameCaiPiaoData();


    /// <summary>
    /// 随机道具类型.
    /// </summary>
    public enum SuiJiDaoJuState
    {
        /// <summary>
        /// 骰子.
        /// </summary>
        TouZi = 0,
        /// <summary>
        /// 宝箱.
        /// </summary>
        BaoXiang = 1,
    }

    /// <summary>
    /// 随机道具数据信息.
    /// </summary>
    [System.Serializable]
    public class SuiJiDaoJuData
    {
        float _TouZiGaiLv = 0.5f;
        /// <summary>
        /// 骰子产生的概率.
        /// </summary>
        public float TouZiGaiLv
        {
            get
            {
                return _TouZiGaiLv;
            }
        }
        /// <summary>
        /// 骰子在随机道具里的得票率.
        /// </summary>
        internal float TouZiDePiaoLv = 0.6f;
        /// <summary>
        /// 宝箱在随机道具里的得票率.
        /// </summary>
        internal float BaoXiangDePiaoLv = 0.8f;
        /// <summary>
        /// 骰子道具预制.
        /// </summary>
        public GameObject TouZiPrefab;
        /// <summary>
        /// 宝箱道具预制.
        /// </summary>
        public GameObject BaoXiangPrefab;
    }
    /// <summary>
    /// 随机道具数据信息.
    /// </summary>
    public SuiJiDaoJuData m_SuiJiDaoJuData = new SuiJiDaoJuData();
    /// <summary>
    /// 游戏预支彩票数据.
    /// </summary>
    internal SSGameYuZhiCaiPiaoData m_GameYuZhiCaiPiaoData = new SSGameYuZhiCaiPiaoData();

    /// <summary>
    /// 获取随机道具预制.
    /// </summary>
    public GameObject GetSuiJiDaoJuPrefab(PlayerEnum index)
    {
        SuiJiDaoJuState type = SuiJiDaoJuState.TouZi;
        GameObject obj = null;
        //SSCaiPiaoSuiJiDaoJu suiJiDaoJu = null;
        float rv = UnityEngine.Random.Range(0, 100) / 100f;
        if (rv < m_SuiJiDaoJuData.TouZiGaiLv)
        {
            obj = m_SuiJiDaoJuData.TouZiPrefab;
            //if (obj != null)
            //{
            //    suiJiDaoJu = obj.GetComponent<SSCaiPiaoSuiJiDaoJu>();
            //    if (suiJiDaoJu != null)
            //    {
            //        suiJiDaoJu.DaoJuType = SuiJiDaoJuState.TouZi;
            //    }
            //    else
            //    {
            //        UnityLogWarning("GetSuiJiDaoJuPrefab -> suiJiDaoJu was null..........");
            //    }
            //}
        }
        else
        {
            type = SuiJiDaoJuState.BaoXiang;
            obj = m_SuiJiDaoJuData.BaoXiangPrefab;
            //if (obj != null)
            //{
            //    suiJiDaoJu = obj.GetComponent<SSCaiPiaoSuiJiDaoJu>();
            //    if (suiJiDaoJu != null)
            //    {
            //        suiJiDaoJu.DaoJuType = SuiJiDaoJuState.BaoXiang;
            //    }
            //    else
            //    {
            //        UnityLogWarning("GetSuiJiDaoJuPrefab -> suiJiDaoJu was null..........");
            //    }
            //}
        }

        m_GameCaiPiaoData.SubGameDeCaiValByDeCaiState(index, GameCaiPiaoData.DeCaiState.SuiJiDaoJu, type);
        return obj;
    }

    [System.Serializable]
    public class PlayerActiveTimeData
    {
        /// <summary>
        /// 游戏激活时间等级.
        /// </summary>
        public int TimeLevel = 0;
        /// <summary>
        /// 增加伤害百分比.
        /// </summary>
        public float DamageAdd = 0f;
        public PlayerActiveTimeData(int time, float damage)
        {
            TimeLevel = time;
            DamageAdd = damage;
        }
    }
    /// <summary>
    /// 玩家激活游戏时长等级信息.
    /// m_PlayerActiveTimeData.TimeLevel从低到高.
    /// </summary>
    internal PlayerActiveTimeData[] m_PlayerActiveTimeData = new PlayerActiveTimeData[3]
    {
        new PlayerActiveTimeData(90, 0.3f),
        new PlayerActiveTimeData(120, 0.5f),
        new PlayerActiveTimeData(180, 0.8f),
    };

    /// <summary>
    /// 正常得彩数据信息.
    /// </summary>
    [System.Serializable]
    public class ZhengChangDeCaiData
    {
        /// <summary>
        /// 游戏激活时长.
        /// </summary>
        public float TimeVal = 0f;
        /// <summary>
        /// 获得正常彩票的比例.
        /// </summary>
        public float DeCaiBiLi = 0f;
        public ZhengChangDeCaiData(float time, float biLi)
        {
            TimeVal = time;
            DeCaiBiLi = biLi;
        }
    }
    internal ZhengChangDeCaiData[] m_ZhengChangDeCaiData = new ZhengChangDeCaiData[3]
    {
            new ZhengChangDeCaiData(90f, 0.8f),
            new ZhengChangDeCaiData(120f, 0.9f),
            new ZhengChangDeCaiData(9999f, 1f),
    };

    /// <summary>
    /// 获取对玩家增加的伤害数值.
    /// </summary>
    public float GetAddDamageToPlayer(PlayerEnum index)
    {
        int indexVal = (int)index - 1;
        if (indexVal < 0 || indexVal >= m_PlayerCoinData.Length)
        {
            return 0f;
        }

        float damageVal = 0f;
        float timeVal = Time.time - m_PlayerCoinData[indexVal].TimeActive;
        for (int i = m_PlayerActiveTimeData.Length - 1; i > -1; i--)
        {
            if (m_PlayerActiveTimeData[i].TimeLevel <= timeVal)
            {
                //玩家激活游戏时长大于等于当前等级.
                damageVal = m_PlayerActiveTimeData[i].DamageAdd;
                break;
            }
        }
        return damageVal;
    }

    /// <summary>
    /// 玩家续币数据信息.
    /// </summary>
    public class PlayerCoinData
    {
        /// <summary>
        /// 续币数量.
        /// </summary>
        public int XuBiVal = 0;
        /// <summary>
        /// 游戏激活时间记录.
        /// </summary>
        public float TimeActive = 0f;
        int _ZhengChangDeCai = 0;
        /// <summary>
        /// 玩家正常德彩数量.
        /// </summary>
        public int ZhengChangDeCai
        {
            set
            {
                _ZhengChangDeCai = value;
            }
            get
            {
                return _ZhengChangDeCai;
            }
        }

        /// <summary>
        /// 重置正常得彩信息.
        /// </summary>
        public void ResetZhengChangDeCai(PlayerEnum index)
        {
            int indexVal = (int)index - 1;
            if (indexVal < 0 || indexVal >= XkPlayerCtrl.GetInstanceFeiJi().m_SpawnNpcManage.m_CaiPiaoDataManage.m_PlayerCoinData.Length)
            {
                return;
            }
            
            float timeVal = Time.time - XkPlayerCtrl.GetInstanceFeiJi().m_SpawnNpcManage.m_CaiPiaoDataManage.m_PlayerCoinData[indexVal].TimeActive;
            int deCaiVal = ZhengChangDeCai;
            ZhengChangDeCaiData[] data = XkPlayerCtrl.GetInstanceFeiJi().m_SpawnNpcManage.m_CaiPiaoDataManage.m_ZhengChangDeCaiData;
            float deCaiBiLi = 0;
            for (int i = 0; i < data.Length; i++)
            {
                if (timeVal <= data[i].TimeVal)
                {
                    deCaiBiLi = data[i].DeCaiBiLi;
                    break;
                }
            }

            deCaiVal = (int)(ZhengChangDeCai * deCaiBiLi);
            //回收彩票.
            int huiShouCaiPiao = ZhengChangDeCai - deCaiVal;
            XkPlayerCtrl.GetInstanceFeiJi().m_SpawnNpcManage.m_CaiPiaoDataManage.m_GameYuZhiCaiPiaoData.AddYuZhiCaiPiao(huiShouCaiPiao);
            ZhengChangDeCai = deCaiVal;

            if (deCaiVal > 0)
            {
                //这个时候应该打印出玩家的正常产得彩数量.
                Debug.Log("Unity: ResetZhengChangDeCai -> index ========== " + index + ", ZhengChangDeCai ==== " + ZhengChangDeCai + ", deCaiBiLi == " + deCaiBiLi);
                //显示玩家剩余彩票成就UI.
                SSUIRoot.GetInstance().m_GameUIManage.CreatePlayerCaiPiaoChengJiu(index, deCaiVal);
                //XkPlayerCtrl.GetInstanceFeiJi().m_SpawnNpcManage.m_CaiPiaoDataManage.AddCaiPiaoToPlayer(index, deCaiVal, GameCaiPiaoData.DeCaiState.ZhengChang);
            }
            ZhengChangDeCai = 0;
        }

        /// <summary>
        /// 增加正常得彩数量.
        /// </summary>
        public void AddPlayerZhengChangDeCai(bool isPlayerXuBi)
        {
            int deCaiVal = 0;
            float xuBiChuPiaoLvTmp = 1f;
            float zhengChangChuPiaoLvTmp = 0f;
            float coinStart = XKGlobalData.GetInstance().m_CoinToCard;
            if (isPlayerXuBi)
            {
                xuBiChuPiaoLvTmp = XkPlayerCtrl.GetInstanceFeiJi().m_SpawnNpcManage.m_CaiPiaoDataManage.m_GameCaiPiaoData.XuBiChuPiaoLv;
            }
            zhengChangChuPiaoLvTmp = XkPlayerCtrl.GetInstanceFeiJi().m_SpawnNpcManage.m_CaiPiaoDataManage.m_GameCaiPiaoData.ZhengChangChuPiaoLv;
            XkPlayerCtrl.GetInstanceFeiJi().m_SpawnNpcManage.m_CaiPiaoDataManage.m_GameCaiPiaoData.FenPeiDeCaiVal(isPlayerXuBi);

            deCaiVal = (int)(coinStart * xuBiChuPiaoLvTmp * zhengChangChuPiaoLvTmp);
            ZhengChangDeCai += deCaiVal;
            Debug.Log("Unity: AddPlayerZhengChangDeCai -> ZhengChangDeCai ==== " + ZhengChangDeCai + ", indexPlayer == " + IndexPlayer);
            
            if (XkGameCtrl.GetInstance().m_PlayerJiChuCaiPiaoData != null)
            {
                //添加基础得彩数据.
                XkGameCtrl.GetInstance().m_PlayerJiChuCaiPiaoData.AddPlayerJiChuCaiPiao(IndexPlayer, deCaiVal);
            }
        }

        /// <summary>
        /// 减少正常得彩数量.
        /// </summary>
        public void SubZhengChangDeCai(int val)
        {
            if (ZhengChangDeCai >= val)
            {
                ZhengChangDeCai -= val;
            }
            else
            {
                val = ZhengChangDeCai;
                ZhengChangDeCai = 0;
            }
            Debug.Log("Unity: SubZhengChangDeCai -> ZhengChangDeCai ================= " + ZhengChangDeCai + ", indexPlayer == " + IndexPlayer);

            if (XkPlayerCtrl.GetInstanceFeiJi().m_SpawnNpcManage.m_CaiPiaoDataManage != null && val > 0)
            {
                XkPlayerCtrl.GetInstanceFeiJi().m_SpawnNpcManage.m_CaiPiaoDataManage.AddCaiPiaoToPlayer(IndexPlayer, val, GameCaiPiaoData.DeCaiState.ZhengChang);
            }
        }

        /// <summary>
        /// 玩家索引.
        /// </summary>
        public PlayerEnum IndexPlayer = PlayerEnum.Null;
        public PlayerCoinData(PlayerEnum index)
        {
            IndexPlayer = index;
        }
    }
    /// <summary>
    /// 玩家续币信息.
    /// </summary>
    public PlayerCoinData[] m_PlayerCoinData = new PlayerCoinData[3];

    /// <summary>
    /// 玩家续币数量比较器.
    /// </summary>
    int PlayerCoinDataSortByXuBiVal(PlayerCoinData x, PlayerCoinData y)//排序器  
    {
        if (x == null)
        {
            if (y == null)
            {
                return 0;
            }
            return 1;
        }

        if (y == null)
        {
            return -1;
        }

        int retval = y.XuBiVal.CompareTo(x.XuBiVal);
        return retval;
    }

    /// <summary>
    /// 获取对玩家续币信息排序后的数据列表.
    /// </summary>
    public PlayerCoinData[] GetSortPlayerCoinData()
    {
        List<PlayerCoinData> listDt = new List<PlayerCoinData>(m_PlayerCoinData);
        listDt.Sort(PlayerCoinDataSortByXuBiVal);
        return listDt.ToArray();
    }

    /// <summary>
    /// 初始化.
    /// </summary>
    public void Init()
    {
        PlayerEnum index = PlayerEnum.Null;
        for (int i = 0; i < m_PlayerCoinData.Length; i++)
        {
            index = (PlayerEnum)(i + 1);
            m_PlayerCoinData[i] = new PlayerCoinData(index);
        }

        m_GameYuZhiCaiPiaoData.Init();
        PcvrComInputEvent.GetInstance().OnCaiPiaJiWuPiaoEvent += OnCaiPiaJiWuPiaoEvent;
        PcvrComInputEvent.GetInstance().OnCaiPiaJiChuPiaoEvent += OnCaiPiaJiChuPiaoEvent;
    }

    /// <summary>
    /// 彩票机无票.
    /// </summary>
    private void OnCaiPiaJiWuPiaoEvent(pcvrTXManage.CaiPiaoJi val)
    {
        int indexVal = (int)val;
        if (indexVal < 0 || indexVal > 2)
        {
            Debug.LogWarning("OnCaiPiaJiWuPiaoEvent -> indexVal was wrong! indexVal ==== " + indexVal);
            return;
        }

        PlayerEnum indexPlayer = (PlayerEnum)(indexVal + 1);
        if (SSUIRoot.GetInstance().m_GameUIManage != null)
        {
            SSUIRoot.GetInstance().m_GameUIManage.CreatCaiPiaoBuZuPanel(indexPlayer);
        }
    }

    /// <summary>
    /// 彩票机出票.
    /// </summary>
    private void OnCaiPiaJiChuPiaoEvent(pcvrTXManage.CaiPiaoJi val)
    {
        int indexVal = (int)val;
        if (indexVal < 0 || indexVal > 2)
        {
            Debug.LogWarning("OnCaiPiaJiChuPiaoEvent -> indexVal was wrong! indexVal ==== " + indexVal);
            return;
        }

        PlayerEnum indexPlayer = (PlayerEnum)(indexVal + 1);
        SubPlayerCaiPiao(indexPlayer, 1);
    }

    /// <summary>
    /// 设置玩家游戏激活时间信息.
    /// </summary>
    public void SetPlayerCoinTimeActive(PlayerEnum index)
    {
        int indexVal = (int)index - 1;
        if (indexVal < 0 || indexVal >= m_PlayerCoinData.Length)
        {
            return;
        }
        m_PlayerCoinData[indexVal].TimeActive = Time.time;
        Debug.Log("Unity: SetPlayerCoinTimeActive -> index == " + index + ", time == " + Time.time);
    }

    /// <summary>
    /// 添加玩家续币数量.
    /// </summary>
    public void AddPlayerXuBiVal(PlayerEnum index)
    {
        int indexVal = (int)index - 1;
        if (indexVal < 0 || indexVal >= m_PlayerCoinData.Length)
        {
            return;
        }

        //玩家进行了续币激活游戏操作.
        //设置JPBoss的玩家续币状态.
        XkPlayerCtrl.GetInstanceFeiJi().m_SpawnNpcManage.m_JPBossRulerData.IsPlayerXuBi = true;

        int coinStart = XKGlobalData.GameNeedCoin;
        m_PlayerCoinData[indexVal].XuBiVal += coinStart;
        Debug.Log("Unity: AddPlayerXuBiVal -> index == " + index + ", coinVal ==== " + m_PlayerCoinData[indexVal].XuBiVal);
    }

    /// <summary>
    /// 添加玩家正常得彩数据.
    /// </summary>
    public void AddPlayerZhengChangDeCai(PlayerEnum index, bool isPlayerXuBi)
    {
        if (pcvr.GetInstance().m_HongDDGamePadInterface != null)
        {
            HongDDGamePad.GamePlayerData data = pcvr.GetInstance().m_HongDDGamePadInterface.GetGamePlayerData(index);
            if (data != null && data.IsMianFeiTiYanPlayer == true)
            {
                //微信端免费体验游戏玩家不进行奖池累加.
                return;
            }
        }

        int indexVal = (int)index - 1;
        if (indexVal < 0 || indexVal >= m_PlayerCoinData.Length)
        {
            return;
        }

        m_PlayerCoinData[indexVal].AddPlayerZhengChangDeCai(isPlayerXuBi);
    }

    /// <summary>
    /// 减少玩家正常得彩数据.
    /// </summary>
    public void SubPlayerZhengChangDeCai(PlayerEnum indexPlayer, int val)
    {
        int indexVal = (int)indexPlayer - 1;
        if (indexVal < 0 || indexVal >= m_PlayerCoinData.Length)
        {
            return;
        }

        m_PlayerCoinData[indexVal].SubZhengChangDeCai(val);
    }


    /// <summary>
    /// 重置玩家续币数量.
    /// </summary>
    public void ResetPlayerXuBiInfo(PlayerEnum index)
    {
        int indexVal = (int)index - 1;
        if (indexVal < 0 || indexVal >= m_PlayerCoinData.Length)
        {
            return;
        }
        m_PlayerCoinData[indexVal].XuBiVal = 0;
        Debug.Log("Unity: ResetPlayerXuBiInfo -> index == " + index + ", coinVal ==== " + m_PlayerCoinData[indexVal].XuBiVal);

        //重置正常得彩数据.
        m_PlayerCoinData[indexVal].ResetZhengChangDeCai(index);
    }

    /// <summary>
    /// SuperJPBoss彩票数据.
    /// </summary>
    [System.Serializable]
    public class SuperJPBossCaiPiaoData
    {
        /// <summary>
        /// 彩票倍率基数.
        /// </summary>
        public int CaiPiaoBeiLvJiShu = 25;
        /// <summary>
        /// 爆彩条件.
        /// </summary>
        public int BaoCaiTiaoJian = 125;
        /// <summary>
        /// 爆彩数量.
        /// </summary>
        public int BaoCaiShuLiang = 150;
        public SuperJPBossCaiPiaoData(int jiShu, int tiaoJian, int shuLiang)
        {
            CaiPiaoBeiLvJiShu = jiShu;
            BaoCaiTiaoJian = tiaoJian;
            BaoCaiShuLiang = shuLiang;
        }
        /// <summary>
        /// 彩票基数计数.
        /// </summary>
        float CaiPiaoJiShuCount = 1f;
        float _SuperJPCaiPiao = 0f;
        /// <summary>
        /// 超级JPBoss彩票数.
        /// </summary>
        public float SuperJPCaiPiao
        {
            get
            {
                return _SuperJPCaiPiao;
            }
            set
            {
                _SuperJPCaiPiao = value;
                float coinToCaiPiao = XKGlobalData.GetInstance().m_CoinToCard;
                float caiPiaoVal = CaiPiaoJiShuCount * CaiPiaoBeiLvJiShu * coinToCaiPiao;
                //Debug.Log("Unity: SuperJPCaiPiao =============== " + SuperJPCaiPiao
                //    + ", CaiPiaoJiShuCount == " + CaiPiaoJiShuCount
                //    + ", CaiPiaoBeiLvJiShu == " + CaiPiaoBeiLvJiShu
                //    + ", coinToCaiPiao == " + coinToCaiPiao
                //    + ", caiPiaoVal == " + caiPiaoVal);
                if (value >= caiPiaoVal)
                {
                    //可以产生SuperJPBoss了.
                    CaiPiaoJiShuCount = (value / (CaiPiaoBeiLvJiShu * coinToCaiPiao)) + 1f;
#if CREATE_SUPER_JPBOSS
                    if (XkPlayerCtrl.GetInstanceFeiJi().m_SpawnNpcManage.m_ZhanCheJPBossData.IsCreatSuperJPBoss == false)
                    {
                        Debug.Log("Unity: game can create superJPBoss...................");
                        XkPlayerCtrl.GetInstanceFeiJi().m_SpawnNpcManage.m_ZhanCheJPBossData.IsCreatSuperJPBoss = true;
                    }
#endif
                }
            }
        }
    }
    /// <summary>
    /// SuperJPBoss彩票数据.
    /// </summary>
    internal SuperJPBossCaiPiaoData m_SuperJPBossCaiPiaoData = new SuperJPBossCaiPiaoData(25, 125, 150);

    /// <summary>
    /// 游戏需要打印给玩家的彩票数据信息.
    /// </summary>
    public class PcvrPrintCaiPiaoData
    {
        PlayerEnum IndexPlayer;
        int _CaiPiaoVal = 0;
        /// <summary>
        /// 彩票数量.
        /// </summary>
        internal int CaiPiaoVal
        {
            set
            {
                _CaiPiaoVal = value;
                if (SSUIRoot.GetInstance().m_GameUIManage != null)
                {
                    //显示玩家彩票数量.
                    SSUIRoot.GetInstance().m_GameUIManage.ShowPlayerCaiPiaoInfo(IndexPlayer, _CaiPiaoVal + CaiPiaoValCache);
                }

                if (IsDaJiangCaiPiao == true && _CaiPiaoVal + CaiPiaoValCache <= 0)
                {
                    IsDaJiangCaiPiao = false;
                    //删除彩票大奖UI界面.
                    if (SSUIRoot.GetInstance().m_GameUIManage != null)
                    {
                        SSUIRoot.GetInstance().m_GameUIManage.RemoveCaiPiaoDaJiangPanel();
                    }
                }
            }
            get
            {
                return _CaiPiaoVal;
            }
        }

        /// <summary>
        /// 是否得到JPBoss大奖.
        /// </summary>
        internal bool IsDaJiangCaiPiao = false;

        /// <summary>
        /// 是否正在打印彩票.
        /// </summary>
        internal bool IsPrintCaiPiao = false;
        /// <summary>
        /// 彩票数据缓存信息.
        /// 当彩票机处于打印彩票时,新加进来的彩票数暂时存入缓冲区数据里,等彩票机打印完当前
        /// 彩票后,再去检查缓冲区的彩票数,有数据则继续打印缓冲区彩票并将缓冲区彩票清空,没有
        /// 数据则不进行任何操作.
        /// </summary>
        internal int CaiPiaoValCache = 0;

        /// <summary>
        /// 清理彩票数据.
        /// </summary>
        public void ClearCaiPiaoData()
        {
            IsPrintCaiPiao = false;
            CaiPiaoValCache = 0;
            CaiPiaoVal = 0;
        }

        public PcvrPrintCaiPiaoData(PlayerEnum indexPlayerVal)
        {
            IndexPlayer = indexPlayerVal;
        }
    }
    /// <summary>
    /// 3个玩家的彩票数据信息.
    /// </summary>
    internal PcvrPrintCaiPiaoData[] m_PcvrPrintCaiPiaoData = new PcvrPrintCaiPiaoData[3]
    {
        new PcvrPrintCaiPiaoData(PlayerEnum.PlayerOne),
        new PcvrPrintCaiPiaoData(PlayerEnum.PlayerTwo),
        new PcvrPrintCaiPiaoData(PlayerEnum.PlayerThree),
    };

    /// <summary>
    /// 添加彩票给玩家.
    /// </summary>
    internal void AddCaiPiaoToPlayer(PlayerEnum indexPlayer, int caiPiao, GameCaiPiaoData.DeCaiState type, bool isPlayCaiPiaoNumAni = true)
    {
        int index = (int)indexPlayer - 1;
        if (index < 0 || index > 2)
        {
            UnityLogWarning("AddCaiPiaoToPlayer -> index was wrong! index ==== " + index);
            return;
        }

        UnityLog("AddCaiPiaoToPlayer -> indexPlayer == " + indexPlayer + ", caiPiao == " + caiPiao + ", type == " + type);
        if (type == GameCaiPiaoData.DeCaiState.JPBoss)
        {
            //产生彩票大奖UI界面.
            if (SSUIRoot.GetInstance().m_GameUIManage != null)
            {
                SSUIRoot.GetInstance().m_GameUIManage.CreatCaiPiaoDaJiangPanel(indexPlayer, caiPiao);
            }
            //if (m_PcvrPrintCaiPiaoData[index].IsDaJiangCaiPiao == false)
            //{
            //    m_PcvrPrintCaiPiaoData[index].IsDaJiangCaiPiao = true;
            //    //产生彩票大奖UI界面.
            //    if (SSUIRoot.GetInstance().m_GameUIManage != null)
            //    {
            //        SSUIRoot.GetInstance().m_GameUIManage.CreatCaiPiaoDaJiangPanel(indexPlayer, caiPiao);
            //    }
            //}
        }
        else if (type == GameCaiPiaoData.DeCaiState.ZhanChe)
        {
            //产生彩票小奖UI界面.
            if (SSUIRoot.GetInstance().m_GameUIManage != null)
            {
                SSUIRoot.GetInstance().m_GameUIManage.CreatCaiPiaoXiaoJiangPanel(indexPlayer, caiPiao);
            }
        }
        
        //if (pcvr.GetInstance().m_HongDDGamePadInterface.GetHongDDGamePadWXPay() != null)
        //{
        //    //将玩家得到的代金券信息发送到服务器.
        //    string args = "index == " + indexPlayer + ", caiPiao == " + caiPiao;
        //    pcvr.GetInstance().m_HongDDGamePadInterface.GetHongDDGamePadWXPay().CToS_OnPlayerReceiveDaiJinQuan(args);
        //}

        if (pcvr.GetInstance().m_HongDDGamePadInterface != null)
        {
            //将玩家得到的代金券信息发送到服务器.
            string args = "index == " + indexPlayer + ", caiPiao == " + caiPiao;
            pcvr.GetInstance().m_HongDDGamePadInterface.SendPostHddPlayerCouponInfo(indexPlayer, caiPiao);
        }

        if (XKGlobalData.GetInstance().m_GameWXPayDataManage != null)
        {
            //记录游戏支出的代金券信息.
            XKGlobalData.GetInstance().m_GameWXPayDataManage.WriteGamePayRebateInfo(caiPiao);
        }

        XKGlobalData.GetInstance().SetTotalOutPrintCards(XKGlobalData.GetInstance().m_TotalOutPrintCards + caiPiao);
        if (m_PcvrPrintCaiPiaoData[index].IsPrintCaiPiao)
        {
            //当前机位正在打印彩票.
            //将新得到的彩票存入缓冲区.
            m_PcvrPrintCaiPiaoData[index].CaiPiaoValCache += caiPiao;
            if (isPlayCaiPiaoNumAni == true)
            {
                //有播放彩票数字动画,等动画播完在更新彩票数字UI.
            }
            else
            {
                //没有播放彩票数字动画,直接更新彩票数字UI.
                if (SSUIRoot.GetInstance().m_GameUIManage != null)
                {
                    //显示玩家彩票数量.
                    SSUIRoot.GetInstance().m_GameUIManage.ShowPlayerCaiPiaoInfo(indexPlayer,
                        m_PcvrPrintCaiPiaoData[index].CaiPiaoVal + m_PcvrPrintCaiPiaoData[index].CaiPiaoValCache);
                }
            }
            return;
        }

        m_PcvrPrintCaiPiaoData[index].IsPrintCaiPiao = true;
        m_PcvrPrintCaiPiaoData[index].CaiPiaoVal += caiPiao;

        //if (isPlayCaiPiaoNumAni == true)
        //{
        //    //有播放彩票数字动画,等动画播完在更新彩票数字UI.
        //}
        //else
        //{
        //    //没有播放彩票数字动画,直接更新彩票数字UI.
        //    if (SSUIRoot.GetInstance().m_GameUIManage != null)
        //    {
        //        //显示玩家彩票数量.
        //        SSUIRoot.GetInstance().m_GameUIManage.ShowPlayerCaiPiaoInfo(indexPlayer,
        //            m_PcvrPrintCaiPiaoData[index].CaiPiaoVal + m_PcvrPrintCaiPiaoData[index].CaiPiaoValCache);
        //    }
        //}

        Debug.Log("AddCaiPiaoToPlayer ->CaiPiaoVal ===== " + m_PcvrPrintCaiPiaoData[index].CaiPiaoVal
            + ", addCaiPiao ====== " + caiPiao
            + ", coinToCaiPiao ==== " + XKGlobalData.GetInstance().m_CoinToCard);

        //这里添加pcvr打印彩票的消息.
        pcvr.GetInstance().StartPrintPlayerCaiPiao(indexPlayer, caiPiao);
    }

    /// <summary>
    /// 开始打印缓冲区彩票.
    /// </summary>
    void StartPrintCaiPiaoCache(PlayerEnum indexPlayer)
    {
        int index = (int)indexPlayer - 1;
        if (index < 0 || index > 2)
        {
            UnityLogWarning("AddCaiPiaoToPlayer -> index was wrong! index ==== " + index);
            return;
        }

        if (m_PcvrPrintCaiPiaoData[index].IsPrintCaiPiao)
        {
            //当前机位正在打印彩票.
            return;
        }
        m_PcvrPrintCaiPiaoData[index].IsPrintCaiPiao = true;

        int caiPiao = m_PcvrPrintCaiPiaoData[index].CaiPiaoValCache;
        //清空缓冲区彩票数量,避免UI显示彩票数会显示为缓冲区彩票数的2倍.
        m_PcvrPrintCaiPiaoData[index].CaiPiaoValCache = 0;
        //重置彩票数量信息.
        m_PcvrPrintCaiPiaoData[index].CaiPiaoVal = caiPiao;
        //UnityLog("StartPrintCaiPiaoCache -> indexPlayer ====== " + indexPlayer + ", caiPiao ==== " + caiPiao);

        //这里添加pcvr打印彩票的消息.
        pcvr.GetInstance().StartPrintPlayerCaiPiao(indexPlayer, caiPiao);
    }
    
    /// <summary>
    ///  减少玩家彩票.
    /// </summary>
    internal void SubPlayerCaiPiao(PlayerEnum indexPlayer, int caiPiao)
    {
        int index = (int)indexPlayer - 1;
        if (index < 0 || index > 2)
        {
            UnityLogWarning("SubPlayerCaiPiao -> index was wrong! index ==== " + index);
            return;
        }

        if (m_PcvrPrintCaiPiaoData[index].CaiPiaoVal >= caiPiao)
        {
            m_PcvrPrintCaiPiaoData[index].CaiPiaoVal -= caiPiao;
        }
        else
        {
            m_PcvrPrintCaiPiaoData[index].CaiPiaoVal = 0;
        }

        if (m_PcvrPrintCaiPiaoData[index].CaiPiaoVal <= 0)
        {
            //彩票已经打印完成.
            m_PcvrPrintCaiPiaoData[index].IsPrintCaiPiao = false;
            if (m_PcvrPrintCaiPiaoData[index].CaiPiaoValCache > 0)
            {
                //开始打印缓冲区彩票.
                StartPrintCaiPiaoCache(indexPlayer);
            }
        }
        //Debug.Log("SubPlayerCaiPiao ->CaiPiaoVal ===== " + m_PcvrPrintCaiPiaoData[index].CaiPiaoVal
        //    + ", CaiPiaoValCache ==== " + m_PcvrPrintCaiPiaoData[index].CaiPiaoValCache
        //    + ", subCaiPiao ====== " + caiPiao);

        if (SSUIRoot.GetInstance().m_GameUIManage != null)
        {
            //删除彩票不足UI界面.
            SSUIRoot.GetInstance().m_GameUIManage.RemoveCaiPiaoBuZuPanel(indexPlayer, false);
        }
    }

    /// <summary>
    /// 获取玩家当前的彩票数量.
    /// </summary>
    internal int GetPlayerCaiPiaoVal(PlayerEnum indexPlayer)
    {
        int indexVal = (int)indexPlayer;
        if (indexVal < 1 || indexVal > 3)
        {
            Debug.LogWarning("GetPlayerCaiPiaoVal -> indexVal was wrong! indexVal ==== " + indexVal);
            return 0;
        }

        if (m_PcvrPrintCaiPiaoData.Length < indexVal)
        {
            Debug.LogWarning("GetPlayerCaiPiaoVal -> m_PcvrPrintCaiPiaoData was wrong! indexVal ==== " + indexVal);
            return 0;
        }
        return m_PcvrPrintCaiPiaoData[indexVal - 1].CaiPiaoVal;
    }

    /// <summary>
    /// 清理所有玩家的彩票数据信息.
    /// </summary>
    internal void ClearAllPlayerCaiPiaoData()
    {
        ClearPlayerCaiPiaoData(PlayerEnum.PlayerOne);
        ClearPlayerCaiPiaoData(PlayerEnum.PlayerTwo);
        ClearPlayerCaiPiaoData(PlayerEnum.PlayerThree);
    }

    /// <summary>
    /// 工作人员清理彩票不足机台彩票数据.
    /// 清理玩家彩票数据信息.
    /// </summary>
    internal void ClearPlayerCaiPiaoData(PlayerEnum indexPlayer)
    {
        int index = (int)indexPlayer - 1;
        if (index < 0 || index > 2)
        {
            UnityLogWarning("ClearPlayerCaiPiaoData -> index was wrong! index ==== " + index);
            return;
        }
        UnityLog("ClearPlayerCaiPiaoData -> indexPlayer ======= " + indexPlayer);
        //清理玩家彩票数据.
        m_PcvrPrintCaiPiaoData[index].ClearCaiPiaoData();
        //清理玩家pcvr彩票数据.
        pcvr.GetInstance().ClearCaiPiaoData(indexPlayer);
    }

#if TEST_OUT_PRINT_CARD && UNITY_EDITOR
    void Update()
    {
        if (IsTestPrintCard == false)
        {
            return;
        }

        if (Input.GetKeyUp(KeyCode.G))
        {
            //填充彩票.
            TestAddCard();
        }
        TestUpdataPrintCard();
    }

    float TestLastPrintCardTime = 0f;
    int TestCardNum = 0;
    public bool IsTestPrintCard = true;
    void TestAddCard()
    {
        if (TestCardNum <= 0)
        {
            TestCardNum = 10;
        }
    }

    void TestUpdataPrintCard()
    {
        if (TestCardNum <= 0)
        {
            //没有彩票了.
            return;
        }

        if (m_PcvrPrintCaiPiaoData[0].CaiPiaoVal <= 0)
        {
            return;
        }

        if (Time.time - TestLastPrintCardTime >= 0.5f)
        {
            TestLastPrintCardTime = Time.time;
            PcvrComInputEvent.GetInstance().OnCaiPiaJiChuPiao(pcvrTXManage.CaiPiaoJi.Num01);
            TestCardNum--;
            if (TestCardNum <= 0)
            {
                //没有彩票了.
                PcvrComInputEvent.GetInstance().OnCaiPiaJiWuPiao(pcvrTXManage.CaiPiaoJi.Num01);
            }
        }
    }
#endif

#if TEST_SHOW_PLAYER_CAIPIAO
    private void OnGUI()
    {
        string info = "CaiPiaoP1: " + m_PcvrPrintCaiPiaoData[0].CaiPiaoVal + ", CaiPiaoCacheP1: " + m_PcvrPrintCaiPiaoData[0].CaiPiaoValCache
            + ", CaiPiaoP2: " + m_PcvrPrintCaiPiaoData[1].CaiPiaoVal + ", CaiPiaoCacheP2: " + m_PcvrPrintCaiPiaoData[1].CaiPiaoValCache
            + ", CaiPiaoP3: " + m_PcvrPrintCaiPiaoData[2].CaiPiaoVal + ", CaiPiaoCacheP3: " + m_PcvrPrintCaiPiaoData[2].CaiPiaoValCache;
        Rect rectVal = new Rect(15f, 15f, Screen.width - 30f, 25f);
        GUI.Box(rectVal, "");
        GUI.Label(rectVal, info);
        
        info = "CaiPiJiP1: ";
        byte[] buffer = MyCOMDevice.ComThreadClass.ReadByteMsg;
        //UpdateCaiPiaoJiInfo(buffer[44], buffer[15], buffer[16]);
        pcvrTXManage.CaiPiaoPrintState type = (pcvrTXManage.CaiPiaoPrintState)buffer[44];
        switch (type)
        {
            case pcvrTXManage.CaiPiaoPrintState.WuXiao:
                {
                    info += "无效";
                    break;
                }
            case pcvrTXManage.CaiPiaoPrintState.Failed:
                {
                    info += "失败";
                    break;
                }
            case pcvrTXManage.CaiPiaoPrintState.Succeed:
                {
                    info += "成功";
                    break;
                }
        }
        
        info += ", CaiPiJiP2: ";
        type = (pcvrTXManage.CaiPiaoPrintState)buffer[15];
        switch (type)
        {
            case pcvrTXManage.CaiPiaoPrintState.WuXiao:
                {
                    info += "无效";
                    break;
                }
            case pcvrTXManage.CaiPiaoPrintState.Failed:
                {
                    info += "失败";
                    break;
                }
            case pcvrTXManage.CaiPiaoPrintState.Succeed:
                {
                    info += "成功";
                    break;
                }
        }

        info += ", CaiPiJiP3: ";
        type = (pcvrTXManage.CaiPiaoPrintState)buffer[16];
        switch (type)
        {
            case pcvrTXManage.CaiPiaoPrintState.WuXiao:
                {
                    info += "无效";
                    break;
                }
            case pcvrTXManage.CaiPiaoPrintState.Failed:
                {
                    info += "失败";
                    break;
                }
            case pcvrTXManage.CaiPiaoPrintState.Succeed:
                {
                    info += "成功";
                    break;
                }
        }
        rectVal = new Rect(15f, 45f, Screen.width - 30f, 25f);
        GUI.Box(rectVal, "");
        GUI.Label(rectVal, info);

        info = "PcvrCaiPiaoP1: " + pcvr.GetInstance().mPcvrTXManage.CaiPiaoCountPrint[0].ToString()
            + ", PcvrCaiPiaoP2: " + pcvr.GetInstance().mPcvrTXManage.CaiPiaoCountPrint[1].ToString()
            + ", PcvrCaiPiaoP3: " + pcvr.GetInstance().mPcvrTXManage.CaiPiaoCountPrint[2].ToString();
        rectVal = new Rect(15f, 75f, Screen.width - 30f, 25f);
        GUI.Box(rectVal, "");
        GUI.Label(rectVal, info);

        info = "PcvrCaiPiaoPrintFailedP1: " + pcvr.GetInstance().mPcvrTXManage.CaiPiaoPrintFailedCount[0].ToString()
            + ", PcvrCaiPiaoPrintFailedP2: " + pcvr.GetInstance().mPcvrTXManage.CaiPiaoPrintFailedCount[1].ToString()
            + ", PcvrCaiPiaoPrintFailedP3: " + pcvr.GetInstance().mPcvrTXManage.CaiPiaoPrintFailedCount[2].ToString();
        rectVal = new Rect(15f, 105f, Screen.width - 30f, 25f);
        GUI.Box(rectVal, "");
        GUI.Label(rectVal, info);
    }
#endif
}