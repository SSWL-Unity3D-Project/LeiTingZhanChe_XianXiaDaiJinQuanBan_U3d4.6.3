//#define CREATE_SUPER_JPBOSS
using System.Collections.Generic;
using UnityEngine;

public class SSCaiPiaoDataManage : SSGameMono
{
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
        float _ZhengChangChuPiaoLv = 0.4f;
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
        /// 战车得彩出票率.
        /// </summary>
        float ZhanCheChuPiaoLv = 0.3f;
        /// <summary>
        /// 随机道具出票率.
        /// </summary>
        float SuiJiDaoJuChuPiaoLv = 0.05f;
        /// <summary>
        /// JPBoss出票率.
        /// </summary>
        float JPBossChuPiaoLv = 0.25f;
        /// <summary>
        /// 战车出票条件(游戏启动币数乘以该值).
        /// </summary>
        float ZhanCheChuPiaoTiaoJian = 10f;
        /// <summary>
        /// 随机道具出票条件(游戏启动币数乘以该值).
        /// </summary>
        float SuiJiDaoJuChuPiaoTiaoJian = 2.5f;
        /// <summary>
        /// JPBoss出票条件(游戏启动币数乘以该值).
        /// </summary>
        float JPBossChuPiaoTiaoJian = 50f;
        int _ZhanCheDeCai = 0;
        /// <summary>
        /// 战车得彩累积数量.
        /// </summary>
        public int ZhanCheDeCai
        {
            set
            {
                _ZhanCheDeCai = value;
            }
            get
            {
                return _ZhanCheDeCai;
            }
        }
        int _SuiJiDaoJuDeCai = 0;
        /// <summary>
        /// 随机道具得彩累积数量.
        /// </summary>
        public int SuiJiDaoJuDeCai
        {
            set
            {
                _SuiJiDaoJuDeCai = value;
            }
            get
            {
                return _SuiJiDaoJuDeCai;
            }
        }
        int _JPBossDeCai = 0;
        /// <summary>
        /// JPBoss得彩累积数量.
        /// </summary>
        public int JPBossDeCai
        {
            set
            {
                _JPBossDeCai = value;
            }
            get
            {
                return _JPBossDeCai;
            }
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
        /// 分配得彩数量信息.
        /// </summary>
        public void FenPeiDeCaiVal(bool isPlayerXuBi)
        {
            int coinStart = XKGlobalData.GetInstance().m_CoinToCaiPiao * XKGlobalData.GameNeedCoin;
            float xuBiChuPiaoLvTmp = isPlayerXuBi == true ? XuBiChuPiaoLv : 1f;
            if (isPlayerXuBi)
            {
                //玩家续币积累到预支彩票池的彩票数量.
                int jiLeiToYuZhiCaiPiaoChiVal = (int)(coinStart * XuBiChuPiaoLv);
                XkPlayerCtrl.GetInstanceFeiJi().m_SpawnNpcManage.m_CaiPiaoDataManage.m_GameYuZhiCaiPiaoData.AddYuZhiCaiPiao(jiLeiToYuZhiCaiPiaoChiVal);
            }

            coinStart = (int)(coinStart * xuBiChuPiaoLvTmp);
            ZhanCheDeCai += (int)(coinStart * ZhanCheChuPiaoLv);
            SuiJiDaoJuDeCai += (int)(coinStart * SuiJiDaoJuChuPiaoLv);
            JPBossDeCai += (int)(coinStart * JPBossChuPiaoLv);
            Debug.Log("Unity: FenPeiDeCaiVal -> coinStart == " + coinStart
                + ", ZhanCheDeCai == " + ZhanCheDeCai
                + ", SuiJiDaoJuDeCai == " + SuiJiDaoJuDeCai
                + ", JPBossDeCai == " + JPBossDeCai
                + ", isPlayerXuBi ==== " + isPlayerXuBi);
        }

        /// <summary>
        /// 减去游戏某种类型得彩累积数量.
        /// </summary>
        public void SubGameDeCaiValByDeCaiState(PlayerEnum index, DeCaiState type, SuiJiDaoJuState suiJiDaoJuType = SuiJiDaoJuState.BaoXiang)
        {
            int val = 0;
            int coinStart = XKGlobalData.GetInstance().m_CoinToCaiPiao * XKGlobalData.GameNeedCoin;
            switch (type)
            {
                case DeCaiState.ZhanChe:
                    {
                        val = (int)(coinStart * ZhanCheChuPiaoTiaoJian);
                        ZhanCheDeCai -= val;
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
                        val = (int)(coinStart * SuiJiDaoJuChuPiaoTiaoJian);

                        //应该给玩家的彩票数量.
                        int outCaiPiao = (int)(val * suiJiDaoJuChuPiaoLv);

                        //随机道具积累到预支彩票池的彩票数量.
                        int jiLeiToYuZhiCaiPiaoChiVal = val - outCaiPiao;
                        XkPlayerCtrl.GetInstanceFeiJi().m_SpawnNpcManage.m_CaiPiaoDataManage.m_GameYuZhiCaiPiaoData.AddYuZhiCaiPiao(jiLeiToYuZhiCaiPiaoChiVal);

                        val = outCaiPiao;
                        SuiJiDaoJuDeCai -= val;
                        break;
                    }
                case DeCaiState.JPBoss:
                    {
                        val = (int)(coinStart * JPBossChuPiaoTiaoJian);
                        JPBossDeCai -= val;
                        //从预制彩池里取彩票投入JPBoss彩池.
                        XkPlayerCtrl.GetInstanceFeiJi().m_SpawnNpcManage.m_CaiPiaoDataManage.m_GameYuZhiCaiPiaoData.SubJPBossCaiPiaoVal();
                        break;
                    }
            }

            if (val > 0)
            {
                //此时彩票机应该给对应玩家出val张彩票.
                Debug.Log("Unity: SubGameDeCaiValByDeCaiState -> index ====== " + index
                    + ", chuPiaoVal ====== " + val
                    + ", type ======= " + type);
                XkPlayerCtrl.GetInstanceFeiJi().m_SpawnNpcManage.m_CaiPiaoDataManage.AddCaiPiaoToPlayer(index, val, type);
            }
        }

        /// <summary>
        /// 判断是否达到某种得彩类型的出彩条件.
        /// </summary>
        public bool GetIsChuCaiPiaoByDeCaiState(DeCaiState type)
        {
            bool isChuPiao = false;
            int coinToCaiPiao = XKGlobalData.GetInstance().m_CoinToCaiPiao * XKGlobalData.GameNeedCoin;
            float chuPiaoTiaoJian = 0f;
            int deCaiVal = -1;
            switch (type)
            {
                case DeCaiState.ZhanChe:
                    {
                        chuPiaoTiaoJian = ZhanCheChuPiaoTiaoJian;
                        deCaiVal = ZhanCheDeCai;
                        break;
                    }
                case DeCaiState.SuiJiDaoJu:
                    {
                        chuPiaoTiaoJian = SuiJiDaoJuChuPiaoTiaoJian;
                        deCaiVal = SuiJiDaoJuDeCai;
                        break;
                    }
                case DeCaiState.JPBoss:
                    {
                        chuPiaoTiaoJian = JPBossChuPiaoTiaoJian;
                        deCaiVal = JPBossDeCai;
                        break;
                    }
            }

            int chuCaiVal = (int)(coinToCaiPiao * chuPiaoTiaoJian);
            if (deCaiVal >= chuCaiVal)
            {
                isChuPiao = true;
                Debug.Log("Unity: GetIsChuCaiPiaoBy -> the type is can shuCaiPiao! type ============ " + type);
            }
            return isChuPiao;
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
        float rv = Random.Range(0, 100) / 100f;
        if (rv < m_SuiJiDaoJuData.TouZiGaiLv)
        {
            obj = m_SuiJiDaoJuData.TouZiPrefab;
        }
        else
        {
            type = SuiJiDaoJuState.BaoXiang;
            obj = m_SuiJiDaoJuData.BaoXiangPrefab;
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

            //这个时候应该打印出玩家的正常产得彩数量.
            Debug.Log("Unity: ResetZhengChangDeCai -> index ========== " + index + ", ZhengChangDeCai ==== " + ZhengChangDeCai + ", deCaiBiLi == " + deCaiBiLi);
            XkPlayerCtrl.GetInstanceFeiJi().m_SpawnNpcManage.m_CaiPiaoDataManage.AddCaiPiaoToPlayer(index, deCaiVal, GameCaiPiaoData.DeCaiState.ZhengChang);
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
            int coinStart = XKGlobalData.GetInstance().m_CoinToCaiPiao * XKGlobalData.GameNeedCoin;
            if (isPlayerXuBi)
            {
                xuBiChuPiaoLvTmp = XkPlayerCtrl.GetInstanceFeiJi().m_SpawnNpcManage.m_CaiPiaoDataManage.m_GameCaiPiaoData.XuBiChuPiaoLv;
            }
            zhengChangChuPiaoLvTmp = XkPlayerCtrl.GetInstanceFeiJi().m_SpawnNpcManage.m_CaiPiaoDataManage.m_GameCaiPiaoData.ZhengChangChuPiaoLv;
            XkPlayerCtrl.GetInstanceFeiJi().m_SpawnNpcManage.m_CaiPiaoDataManage.m_GameCaiPiaoData.FenPeiDeCaiVal(isPlayerXuBi);

            deCaiVal = (int)(coinStart * xuBiChuPiaoLvTmp * zhengChangChuPiaoLvTmp);
            ZhengChangDeCai += deCaiVal;
            Debug.Log("Unity: AddPlayerZhengChangDeCai -> ZhengChangDeCai ==== " + ZhengChangDeCai);
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
        int indexVal = (int)index - 1;
        if (indexVal < 0 || indexVal >= m_PlayerCoinData.Length)
        {
            return;
        }

        m_PlayerCoinData[indexVal].AddPlayerZhengChangDeCai(isPlayerXuBi);
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
        int CaiPiaoJiShuCount = 1;
        int _SuperJPCaiPiao = 0;
        /// <summary>
        /// 超级JPBoss彩票数.
        /// </summary>
        public int SuperJPCaiPiao
        {
            get
            {
                return _SuperJPCaiPiao;
            }
            set
            {
                _SuperJPCaiPiao = value;
                int coinToCaiPiao = XKGlobalData.GetInstance().m_CoinToCaiPiao;
                int caiPiaoVal = CaiPiaoJiShuCount * CaiPiaoBeiLvJiShu * coinToCaiPiao;
                Debug.Log("Unity: SuperJPCaiPiao =============== " + SuperJPCaiPiao
                    + ", CaiPiaoJiShuCount == " + CaiPiaoJiShuCount
                    + ", CaiPiaoBeiLvJiShu == " + CaiPiaoBeiLvJiShu
                    + ", coinToCaiPiao == " + coinToCaiPiao
                    + ", caiPiaoVal == " + caiPiaoVal);
                if (value >= caiPiaoVal)
                {
                    //可以产生SuperJPBoss了.
                    CaiPiaoJiShuCount = (value / (CaiPiaoBeiLvJiShu * coinToCaiPiao)) + 1;
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
                    SSUIRoot.GetInstance().m_GameUIManage.ShowPlayerCaiPiaoInfo(IndexPlayer, _CaiPiaoVal);
                }

                if (IsDaJiangCaiPiao == true && _CaiPiaoVal <= 0)
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
    internal void AddCaiPiaoToPlayer(PlayerEnum indexPlayer, int caiPiao, GameCaiPiaoData.DeCaiState type)
    {
        int index = (int)indexPlayer - 1;
        if (index < 0 || index > 2)
        {
            UnityLogWarning("AddCaiPiaoToPlayer -> index was wrong! index ==== " + index);
            return;
        }

        if (type == GameCaiPiaoData.DeCaiState.JPBoss)
        {
            if (m_PcvrPrintCaiPiaoData[index].IsDaJiangCaiPiao == false)
            {
                m_PcvrPrintCaiPiaoData[index].IsDaJiangCaiPiao = true;
                //产生彩票大奖UI界面.
                if (SSUIRoot.GetInstance().m_GameUIManage != null)
                {
                    SSUIRoot.GetInstance().m_GameUIManage.CreatCaiPiaoDaJiangPanel(indexPlayer, caiPiao);
                }
            }
        }

        if (m_PcvrPrintCaiPiaoData[index].IsPrintCaiPiao)
        {
            //当前机位正在打印彩票.
            //将新得到的彩票存入缓冲区.
            m_PcvrPrintCaiPiaoData[index].CaiPiaoValCache += caiPiao;
            return;
        }
        m_PcvrPrintCaiPiaoData[index].IsPrintCaiPiao = true;
        m_PcvrPrintCaiPiaoData[index].CaiPiaoVal += caiPiao;
        Debug.Log("AddCaiPiaoToPlayer ->CaiPiaoVal ===== " + m_PcvrPrintCaiPiaoData[index].CaiPiaoVal
            + ", addCaiPiao ====== " + caiPiao
            + ", coinToCaiPiao ==== " + XKGlobalData.GetInstance().m_CoinToCaiPiao);

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
        //这里添加pcvr打印彩票的消息.
        pcvr.GetInstance().StartPrintPlayerCaiPiao(indexPlayer, caiPiao);
        m_PcvrPrintCaiPiaoData[index].CaiPiaoValCache = 0;
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

        if (m_PcvrPrintCaiPiaoData[index].CaiPiaoVal <= 0
            && m_PcvrPrintCaiPiaoData[index].CaiPiaoValCache > 0)
        {
            //开始打印缓冲区彩票.
            m_PcvrPrintCaiPiaoData[index].IsPrintCaiPiao = false;
            StartPrintCaiPiaoCache(indexPlayer);
        }
        Debug.Log("SubPlayerCaiPiao ->CaiPiaoVal ===== " + m_PcvrPrintCaiPiaoData[index].CaiPiaoVal
            + ", addCaiPiao ====== " + caiPiao);

        if (SSUIRoot.GetInstance().m_GameUIManage != null)
        {
            //删除彩票不足UI界面.
            SSUIRoot.GetInstance().m_GameUIManage.RemoveCaiPiaoBuZuPanel(indexPlayer, false);
        }
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
        //清理玩家彩票数据.
        m_PcvrPrintCaiPiaoData[index].ClearCaiPiaoData();
        //清理玩家pcvr彩票数据.
        pcvr.GetInstance().ClearCaiPiaoData(indexPlayer);
    }
}