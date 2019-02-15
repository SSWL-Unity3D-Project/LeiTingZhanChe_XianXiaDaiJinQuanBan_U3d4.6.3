#define CREAT_NPC
#define CREAT_ZHAN_CHE_NPC
#define CREAT_BOSS_NPC
using UnityEngine;

/// <summary>
/// 控制JPBoss和战车Boss的刷怪逻辑组件.
/// </summary>
public class SpawnNpcManage : MonoBehaviour
{
    /// <summary>
    /// 彩票数据管理.
    /// </summary>
    public SSCaiPiaoDataManage m_CaiPiaoDataManage;
    public enum NpcState
    {
        /// <summary>
        /// 战车类型.
        /// </summary>
        ZhanChe = 0,
        /// <summary>
        /// JPBoss类型.
        /// </summary>
        JPBoss = 1,
        /// <summary>
        /// SuperJPBoss类型.
        /// </summary>
        SuperJPBoss = 2,
    }
    /// <summary>
    /// 产生点方位.
    /// </summary>
    public enum SpawnPointState
    {
        /// <summary>
        /// 不产生战车、JPBoss和SuperJPBoss.
        /// </summary>
        Null = -1,
        Left = 0,
        Right = 1,
        Up = 2,
        Down = 3,
    }
    /// <summary>
    /// 战车npc在需调UI上面展示的时间.
    /// </summary>
    [Range(1, 600)]
    public int m_TimeXueTiaoZhanCheNpc = 20;
    /// <summary>
    /// 战车npc从产生点运动到最后一个路径点所需要的时间.
    /// </summary>
    [Range(1, 600)]
    public int m_TimeMoveZhanCheNpc = 20;
    /// <summary>
    /// 前后刷出的JPBoss从产生点运动到最后一个路径点所需要的时间.
    /// </summary>
    [Range(1, 600)]
    public int m_TimeMoveJPBoss = 20;
    /// <summary>
    /// 左右刷出的JPBoss从产生点运动到最后一个路径点所需要的时间.
    /// </summary>
    [Range(1, 600)]
    public int m_TimeZYMoveJPBoss = 20;
    [System.Serializable]
    public class NpcData
    {
        /// <summary>
        /// 左边战车npc预制.
        /// </summary>
        public GameObject[] L_ZhanChePrefabGp;
        /// <summary>
        /// 右边战车npc预制.
        /// </summary>
        public GameObject[] R_ZhanChePrefabGp;
        /// <summary>
        /// 上边战车npc预制.
        /// </summary>
        public GameObject[] U_ZhanChePrefabGp;
        /// <summary>
        /// 下边战车npc预制.
        /// </summary>
        public GameObject[] D_ZhanChePrefabGp;
        /// <summary>
        /// JPBoss预制.
        /// </summary>
        public GameObject[] JPBossPrefabGp;
        /// <summary>
        /// 超级JPBoss预制.
        /// </summary>
        public GameObject[] SuperJPBossPrefabGp;
        /// <summary>
        /// 左边创建npc的产生点组.
        /// </summary>
        public SSCreatNpcData[] LeftSpawnPointGp;
        /// <summary>
        /// 右边创建npc的产生点组.
        /// </summary>
        public SSCreatNpcData[] RightSpawnPointGp;
        /// <summary>
        /// 上边创建npc的产生点组.
        /// </summary>
        public SSCreatNpcData[] UpSpawnPointGp;
        /// <summary>
        /// 下边创建npc的产生点组.
        /// </summary>
        public SSCreatNpcData[] DownSpawnPointGp;
        /// <summary>
        /// 左边创建Boss的产生点组.
        /// </summary>
        public SSCreatNpcData[] Boss_LeftSpawnPointGp;
        /// <summary>
        /// 右边创建Boss的产生点组.
        /// </summary>
        public SSCreatNpcData[] Boss_RightSpawnPointGp;
        /// <summary>
        /// 上边创建Boss的产生点组.
        /// </summary>
        public SSCreatNpcData[] Boss_UpSpawnPointGp;
        /// <summary>
        /// 下边创建Boss的产生点组.
        /// </summary>
        public SSCreatNpcData[] Boss_DownSpawnPointGp;
    }
    /// <summary>
    /// npc预制数据.
    /// </summary>
    public NpcData m_NpcData;

    /// <summary>
    /// 创建npc的数据.
    /// </summary>
    public class NpcSpawnData
    {
        /// <summary>
        /// npc预制.
        /// </summary>
        public GameObject NpcPrefab;
        /// <summary>
        /// npc路径.
        /// </summary>
        public NpcPathCtrl NpcPath;
        /// <summary>
        /// npc产生点组件.
        /// </summary>
        public XKSpawnNpcPoint SpawnPoint;
        internal SpawnNpcManage m_SpawnNpcManage;
        public NpcSpawnData(SpawnNpcManage spawnNpcManage)
        {
            m_SpawnNpcManage = spawnNpcManage;
        }

        /// <summary>
        /// 产生npc.
        /// </summary>
        public GameObject CreatPointNpc(SSCaiPiaoDataManage.GameCaiPiaoData.DaiJinQuanState type, SpawnPointState pointState)
        {
            if (SpawnPoint != null)
            {
                SpawnPoint.NpcObj = NpcPrefab;
                SpawnPoint.NpcPath = NpcPath.transform;
                //不进行循环产生npc.
                SpawnPoint.SpawnMaxNpc = 1;

                int daoJiShi = 0;
                switch (type)
                {
                    case SSCaiPiaoDataManage.GameCaiPiaoData.DaiJinQuanState.ZhanCheDaiJinQuan_01:
                    case SSCaiPiaoDataManage.GameCaiPiaoData.DaiJinQuanState.ZhanCheDaiJinQuan_02:
                        {
                            //战车代金券npc
                            float pathDistance = SpawnPoint.GetNpcMoveDistance();
                            float time = 0f;
                            if (m_SpawnNpcManage != null)
                            {
                                time = m_SpawnNpcManage.m_TimeMoveZhanCheNpc;
                            }

                            if (time <= 0f)
                            {
                                time = 1f;
                            }

                            if (pathDistance < 0f)
                            {
                                pathDistance = 0f;
                            }
                            //重新计算运动速度.
                            float speed = pathDistance / time;
                            speed = 3f * speed; //ITween速度有偏移量.
                            SpawnPoint.MvSpeed = speed;
                            daoJiShi = m_SpawnNpcManage.m_TimeXueTiaoZhanCheNpc;
                            //SSDebug.LogWarning("CreatPointNpc -> MvSpeed ====================================== " + SpawnPoint.MvSpeed
                            //    + ", time ==== " + Time.time);
                            break;
                        }
                    case SSCaiPiaoDataManage.GameCaiPiaoData.DaiJinQuanState.JPBossDaiJinQuan:
                        {
                            if (pointState == SpawnPointState.Down || pointState == SpawnPointState.Up)
                            {
                                daoJiShi = m_SpawnNpcManage.m_TimeMoveJPBoss;
                            }
                            else if (pointState == SpawnPointState.Left || pointState == SpawnPointState.Right)
                            {
                                daoJiShi = m_SpawnNpcManage.m_TimeZYMoveJPBoss;
                            }
                            break;
                        }
                }

                GameObject obj = SpawnPoint.CreatPointNpc(type);
                if (SSUIRoot.GetInstance().m_GameUIManage != null)
                {
                    SSUIRoot.GetInstance().m_GameUIManage.SetDaiJinQuanXuTiaoDaoJiShi(daoJiShi);
                }
                return obj;
            }
            else
            {
                Debug.LogWarning("Unity: CreatPointNpc -> SpawnPoint was null");
                return null;
            }
        }
    }

    /// <summary>
    /// 创建战车npc的状态.
    /// </summary>
    [System.Serializable]
    public class CreatZhanCheState
    {
        /// <summary>
        /// 打开左边.
        /// </summary>
        public bool IsOpenLeft = false;
        public bool IsOpenRight = false;
        public bool IsOpenUp = false;
        public bool IsOpenDown = false;

        /// <summary>
        /// 获取战车、JPBoss和SuperJPBoss的产生点方位.
        /// </summary>
        public SpawnPointState GetSpawnPointState()
        {
            if (!IsOpenLeft
                && !IsOpenRight
                && !IsOpenUp
                && !IsOpenDown)
            {
                return SpawnPointState.Null;
            }

            SpawnPointState type = SpawnPointState.Null;
            int rv = 0;
            int count = 0;
            bool isFindPointState = false;
            do
            {
                count++;
                for (int i = 0; i < 4; i++)
                {
                    if (count >= 3)
                    {
                        //超过3次检索就不再随机了.
                        rv = 1;
                    }
                    else
                    {
                        rv = Random.Range(0, 100) % 2;
                    }

                    switch (i)
                    {
                        case 0:
                            {
                                if (IsOpenLeft && rv == 1)
                                {
                                    type = SpawnPointState.Left;
                                    isFindPointState = true;
                                }
                                break;
                            }
                        case 1:
                            {
                                if (IsOpenRight && rv == 1)
                                {
                                    type = SpawnPointState.Right;
                                    isFindPointState = true;
                                }
                                break;
                            }
                        case 2:
                            {
                                if (IsOpenUp && rv == 1)
                                {
                                    type = SpawnPointState.Up;
                                    isFindPointState = true;
                                }
                                break;
                            }
                        case 3:
                            {
                                if (IsOpenDown && rv == 1)
                                {
                                    type = SpawnPointState.Down;
                                    isFindPointState = true;
                                }
                                break;
                            }
                    }

                    if (isFindPointState)
                    {
                        break;
                    }
                }
            } while (!isFindPointState);
            return type;
        }
    }
    /// <summary>
    /// 创建战车npc的状态.
    /// </summary>
    [HideInInspector]
    public CreatZhanCheState m_CreatZhanCheState;

    [System.Serializable]
    public class ZhanCheRulerData
    {
        /// <summary>
        /// 战车产生的最小间隔时间.
        /// </summary>
        public float TimeMin = 10f;
        /// <summary>
        /// 战车产生的最大间隔时间.
        /// </summary>
        public float TimeMax = 20f;
        [HideInInspector]
        public float LastTime = 0f;

        float _RandTime = 0f;
        /// <summary>
        /// JPBoss产生的间隔时间
        /// </summary>
        public float RandTime
        {
            set
            {
                _RandTime = value;
            }
            get
            {
                return _RandTime;
            }
        }

        /// <summary>
        /// 初始化.
        /// </summary>
        public void Init()
        {
            RandTime = Random.Range(TimeMin, TimeMax);
            LastTime = Time.time;
        }

        /// <summary>
        /// 信息重置.
        /// </summary>
        public void Reset()
        {
            RandTime = Random.Range(TimeMin, TimeMax);
            LastTime = Time.time;
        }

        public enum ZhanCheJiBaoState
        {
            /// <summary>
            /// 各机位投币数量相同：爆率均为30%
            /// </summary>
            State1 = 0,
            /// <summary>
            /// 各机位当前统计投币数量不同：情形1：某位最多，其它两位相同，则最多的爆率为40%，其它两位各25%
            /// </summary>
            State2 = 1,
            /// <summary>
            /// 各机位当前统计投币数量不同：情形2：三位均不同，则按大小顺讯爆率为40%、30%、20%。
            /// </summary>
            State3 = 2,
            /// <summary>
            /// 各机位当前统计投币数量不同：情形3：两位相同并多于另一位，爆率40%、40%、10%。
            /// </summary>
            State4 = 3,
        }

        [System.Serializable]
        public class ZhanCheJiBaoRuler
        {
            /// <summary>
            /// 战车击爆状态.
            /// </summary>
            public ZhanCheJiBaoState m_ZhanCheJiBaoState;
            /// <summary>
            /// 最高击爆概率.
            /// </summary>
            public float MaxJiBaoGaiLv = 0f;
            /// <summary>
            /// 中间击爆概率.
            /// </summary>
            public float CenJiBaoGaiLv = 0f;
            /// <summary>
            /// 最低击爆概率.
            /// </summary>
            public float MinJiBaoGaiLv = 0f;
            public ZhanCheJiBaoRuler(ZhanCheJiBaoState type, float max, float cen, float min)
            {
                m_ZhanCheJiBaoState = type;
                MaxJiBaoGaiLv = max;
                CenJiBaoGaiLv = cen;
                MinJiBaoGaiLv = min;
            }
        }
        /// <summary>
        /// 战车击爆规则第二阶段.
        /// </summary>
        internal ZhanCheJiBaoRuler[] m_ZhanCheJiBaoRuler = new ZhanCheJiBaoRuler[4]
        {
            new ZhanCheJiBaoRuler( ZhanCheJiBaoState.State1, 0.3f, 0.3f,  0.3f),
            new ZhanCheJiBaoRuler( ZhanCheJiBaoState.State2, 0.4f, 0.25f, 0.25f),
            new ZhanCheJiBaoRuler( ZhanCheJiBaoState.State3, 0.4f, 0.3f,  0.2f),
            new ZhanCheJiBaoRuler( ZhanCheJiBaoState.State4, 0.4f, 0.4f,  0.1f),
        };

        /// <summary>
        /// 战车击爆规则第一阶段.
        /// </summary>
        internal ZhanCheJiBaoRuler[] m_ZhanCheJiBaoRulerNew = new ZhanCheJiBaoRuler[4]
        {
            new ZhanCheJiBaoRuler( ZhanCheJiBaoState.State1, 0.23f, 0.23f,  0.235f),
            new ZhanCheJiBaoRuler( ZhanCheJiBaoState.State2, 0.3f, 0.2f,  0.2f),
            new ZhanCheJiBaoRuler( ZhanCheJiBaoState.State3, 0.30f, 0.25f, 0.15f),
            new ZhanCheJiBaoRuler( ZhanCheJiBaoState.State4, 0.25f, 0.25f,  0.20f),
        };
    }
    /// <summary>
    /// 战车创建和击爆规则数据.
    /// </summary>
    internal ZhanCheRulerData m_ZhanCheRulerData = new ZhanCheRulerData();
    
    /// <summary>
    /// 获取可以被哪个玩家击爆,通过击爆规则的产生.
    /// </summary>
    public PlayerEnum GetPlayerIndexByJiBaoGaiLv(NpcState npcType, SSCaiPiaoDataManage.GameCaiPiaoData.DaiJinQuanState daiJinQuan = SSCaiPiaoDataManage.GameCaiPiaoData.DaiJinQuanState.ZhanCheDaiJinQuan_01)
    {
        PlayerEnum index = PlayerEnum.Null;
        if (XkGameCtrl.PlayerActiveNum <= 1)
        {
            float danRenGaiLv = 0.5f;
            switch (npcType)
            {
                case NpcState.ZhanChe:
                    {
                        if (XkPlayerCtrl.GetInstanceFeiJi().m_SpawnNpcManage.m_CaiPiaoDataManage.m_GameCaiPiaoData.GetChuPiaoTiaoJianBeiShu(SSCaiPiaoDataManage.GameCaiPiaoData.DeCaiState.ZhanChe, daiJinQuan) >= 2)
                        {
                            danRenGaiLv = 0.6f;
                        }
                        break;
                    }
                case NpcState.JPBoss:
                    {
                        if (XkPlayerCtrl.GetInstanceFeiJi().m_SpawnNpcManage.m_CaiPiaoDataManage.m_GameCaiPiaoData.GetChuPiaoTiaoJianBeiShu(SSCaiPiaoDataManage.GameCaiPiaoData.DeCaiState.JPBoss, daiJinQuan) >= 2)
                        {
                            danRenGaiLv = 0.6f;
                        }
                        break;
                    }
            }

            if (Random.Range(1f, 100f) / 100f <= danRenGaiLv)
            {
                //如果是1个玩家在玩游戏,则按照这里的逻辑执行.
                index = XkGameCtrl.GetActiveOnlyOnePlayer();
            }
            return index;
        }

        SSCaiPiaoDataManage.PlayerCoinData[] coinDt = XkPlayerCtrl.GetInstanceFeiJi().m_SpawnNpcManage.m_CaiPiaoDataManage.GetSortPlayerCoinData();
        ZhanCheRulerData.ZhanCheJiBaoState type = ZhanCheRulerData.ZhanCheJiBaoState.State1;
        if (coinDt[0].XuBiVal == coinDt[1].XuBiVal && coinDt[1].XuBiVal == coinDt[2].XuBiVal)
        {
            type = ZhanCheRulerData.ZhanCheJiBaoState.State1;
        }
        else if (coinDt[0].XuBiVal > coinDt[1].XuBiVal && coinDt[1].XuBiVal == coinDt[2].XuBiVal)
        {
            type = ZhanCheRulerData.ZhanCheJiBaoState.State2;
        }
        else if (coinDt[0].XuBiVal > coinDt[1].XuBiVal && coinDt[1].XuBiVal > coinDt[2].XuBiVal)
        {
            type = ZhanCheRulerData.ZhanCheJiBaoState.State3;
        }
        else if (coinDt[0].XuBiVal == coinDt[1].XuBiVal && coinDt[1].XuBiVal > coinDt[2].XuBiVal)
        {
            type = ZhanCheRulerData.ZhanCheJiBaoState.State4;
        }

        //Debug.Log("Unity: GetPlayerIndexByJiBaoGaiLv::xuBiVal -> " + coinDt[0].XuBiVal + ", " + coinDt[1].XuBiVal + ", " + coinDt[2].XuBiVal
        //    + ", type ====== " + type);
        ZhanCheRulerData.ZhanCheJiBaoRuler ruler = m_ZhanCheRulerData.m_ZhanCheJiBaoRulerNew[(int)type];
        switch (npcType)
        {
            case NpcState.ZhanChe:
                {
                    if (XkPlayerCtrl.GetInstanceFeiJi().m_SpawnNpcManage.m_CaiPiaoDataManage.m_GameCaiPiaoData.GetChuPiaoTiaoJianBeiShu(SSCaiPiaoDataManage.GameCaiPiaoData.DeCaiState.ZhanChe, daiJinQuan) >= 2)
                    {
                        ruler = m_ZhanCheRulerData.m_ZhanCheJiBaoRuler[(int)type];
                    }
                    break;
                }
            case NpcState.JPBoss:
                {
                    if (XkPlayerCtrl.GetInstanceFeiJi().m_SpawnNpcManage.m_CaiPiaoDataManage.m_GameCaiPiaoData.GetChuPiaoTiaoJianBeiShu(SSCaiPiaoDataManage.GameCaiPiaoData.DeCaiState.JPBoss) >= 2)
                    {
                        ruler = m_ZhanCheRulerData.m_ZhanCheJiBaoRuler[(int)type];
                    }
                    break;
                }
        }

        float rv = Random.Range(0f, 100f) / 100f;
        if (rv < ruler.MaxJiBaoGaiLv)
        {
            index = coinDt[0].IndexPlayer;
        }
        else if (rv < ruler.MaxJiBaoGaiLv + ruler.CenJiBaoGaiLv)
        {
            index = coinDt[1].IndexPlayer;
        }
        else if (rv < ruler.MaxJiBaoGaiLv + ruler.CenJiBaoGaiLv + ruler.MinJiBaoGaiLv)
        {
            index = coinDt[2].IndexPlayer;
        }
        //Debug.Log("Unity: GetPlayerIndexByJiBaoGaiLv::xuBiVal -> index ============= " + index);
        return index;
    }

    [System.Serializable]
    public class JPBossRulerData
    {
        /// <summary>
        /// JPBoss产生的最小间隔时间.
        /// </summary>
        public float TimeMin = 60f;
        /// <summary>
        /// JPBoss产生的最大间隔时间.
        /// </summary>
        public float TimeMax = 70f;
        bool _IsPlayerXuBi = false;
        /// <summary>
        /// 玩家是否续币.
        /// </summary>
        public bool IsPlayerXuBi
        {
            set
            {
                _IsPlayerXuBi = value;
                if (value == true)
                {
                    //更新玩家续币的时间记录.
                    LastXuBiTime = Time.time;
                }
            }
            get
            {
                return _IsPlayerXuBi;
            }
        }
        /// <summary>
        /// 续币最小时间.
        /// </summary>
        public float TimeXuBiMin = 3f;
        /// <summary>
        /// 续币最大时间.
        /// </summary>
        public float TimeXuBiMax = 5f;
        [HideInInspector]
        public float LastTime = 0f;
        /// <summary>
        /// 玩家续币的时间记录.
        /// </summary>
        [HideInInspector]
        public float LastXuBiTime = 0f;

        float _RandTime = 0f;
        /// <summary>
        /// JPBoss产生的间隔时间
        /// </summary>
        public float RandTime
        {
            set
            {
                _RandTime = value;
            }
            get
            {
                return _RandTime;
            }
        }

        float _RandTimeXuBi = 0f;
        /// <summary>
        /// JPBoss产生的续币间隔时间
        /// </summary>
        public float RandTimeXuBi
        {
            set
            {
                _RandTimeXuBi = value;
            }
            get
            {
                return _RandTimeXuBi;
            }
        }

        /// <summary>
        /// 初始化.
        /// </summary>
        public void Init()
        {
            RandTime = Random.Range(TimeMin, TimeMax);
            RandTimeXuBi = Random.Range(TimeXuBiMin, TimeXuBiMax);
            LastTime = Time.time;
        }

        /// <summary>
        /// 信息重置.
        /// </summary>
        public void Reset()
        {
            RandTime = Random.Range(TimeMin, TimeMax);
            RandTimeXuBi = Random.Range(TimeXuBiMin, TimeXuBiMax);
            LastTime = Time.time;
            IsPlayerXuBi = false;
        }
    }
    /// <summary>
    /// JPBoss创建规则.
    /// JPBoss击爆条件和战车相同.
    /// </summary>
    internal JPBossRulerData m_JPBossRulerData = new JPBossRulerData();

    /// <summary>
    /// 战车和JPBoss的创建状态.
    /// </summary>
    public class ZhanCheJPBossData
    {
        /// <summary>
        /// 是否可以产生战车.
        /// </summary>
        public bool IsCreatZhanChe = false;
        /// <summary>
        /// 是否可以产生JPBoss.
        /// </summary>
        public bool IsCreatJPBoss = false;
        /// <summary>
        /// 是否可以产生JPBoss.
        /// </summary>
        public bool IsCreatSuperJPBoss = false;
        /// <summary>
        /// 战车数据.
        /// </summary>
        public SSNpcDateManage ZhanCheData;
        /// <summary>
        /// JPBoss数据.
        /// </summary>
        public SSNpcDateManage JPBossData;
        /// <summary>
        /// SuperJPBoss数据.
        /// </summary>
        public SSNpcDateManage SuperJPBossData;
        public void Init(GameObject obj)
        {
            if (obj != null)
            {
                ZhanCheData = obj.AddComponent<SSNpcDateManage>();
                JPBossData = obj.AddComponent<SSNpcDateManage>();
                SuperJPBossData = obj.AddComponent<SSNpcDateManage>();
            }
        }
    }
    /// <summary>
    /// 战车和JPBoss的创建状态.
    /// </summary>
    public ZhanCheJPBossData m_ZhanCheJPBossData = new ZhanCheJPBossData();

    void Awake()
    {
        m_CaiPiaoDataManage.Init();
    }

    void Start()
    {
        m_ZhanCheJPBossData.Init(gameObject);
        m_ZhanCheRulerData.Init();
        m_JPBossRulerData.Init();
    }

#if CREAT_NPC
    float m_LastUpdateTime = 0f;
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.P))
        //{
        //    CreatNpcObj(NpcState.JPBoss, m_CreatZhanCheState.GetSpawnPointState()); //test.
        //}

        if (Time.time - m_LastUpdateTime < 8f)
        {
            //冷却时间.
            //增加彩票战车和boss产生的间隔时间.
            return;
        }
        m_LastUpdateTime = Time.time;


        if (XkGameCtrl.GetInstance().m_GamePlayerAiData.IsActiveAiPlayer == true)
        {
            //没有激活玩家.
        }
        else
        {
            if (XkGameCtrl.PlayerActiveNum <= 0)
            {
                //没有玩家激活时.
                return;
            }
        }

        if (XkGameCtrl.GetInstance().IsDisplayBossDeathYanHua == true)
        {
            return;
        }

        if (XkGameCtrl.GetInstance() != null && XkGameCtrl.GetInstance().m_AiPathGroup != null
            && XkGameCtrl.GetInstance().m_AiPathGroup.m_CameraMoveType == AiPathGroupCtrl.MoveState.YuLe)
        {
            //娱乐阶段不产生npc.
            return;
        }

#if CREAT_ZHAN_CHE_NPC
        if (Time.time - m_ZhanCheRulerData.LastTime >= m_ZhanCheRulerData.RandTime)
        {
            //检测是否可以产生战车.
            if (!m_ZhanCheJPBossData.IsCreatZhanChe)
            {
                m_ZhanCheJPBossData.IsCreatZhanChe = true;
                //Debug.Log("creat zhanCheNpc, RandTime ============= " + m_ZhanCheRulerData.RandTime);
            }
        }
#endif

#if CREAT_BOSS_NPC
        if (Time.time - m_JPBossRulerData.LastTime >= m_JPBossRulerData.RandTime)
        {
            //检测是否可以产生JPBoss.
            if (!m_ZhanCheJPBossData.IsCreatJPBoss)
            {
                m_ZhanCheJPBossData.IsCreatJPBoss = true;
                //Debug.Log("creat jpBoss, RandTime ============= " + m_JPBossRulerData.RandTime);
            }
        }

        //if (m_JPBossRulerData.IsPlayerXuBi)
        //{
        //    //玩家已经续币.
        //    if (Time.time - m_JPBossRulerData.LastXuBiTime >= m_JPBossRulerData.RandTimeXuBi)
        //    {
        //        //检测是否可以产生JPBoss.
        //        if (!m_ZhanCheJPBossData.IsCreatJPBoss)
        //        {
        //            m_ZhanCheJPBossData.IsCreatJPBoss = true;
        //        }
        //    }
        //}
#endif

        if (GetIsHaveBoss() == false)
        {
            //当前游戏场景中没有任何Boss存在,才允许产生Boss.
            if (m_ZhanCheJPBossData.IsCreatSuperJPBoss)
            {
                //优先产生SuperJPBoss.
                CreatNpcObj(NpcState.SuperJPBoss, m_CreatZhanCheState.GetSpawnPointState());
            }
            else if (m_ZhanCheJPBossData.IsCreatJPBoss)
            {
                //其次产生JPBoss.
                CreatNpcObj(NpcState.JPBoss, m_CreatZhanCheState.GetSpawnPointState());
            }
            else if (m_ZhanCheJPBossData.IsCreatZhanChe)
            {
                //最后产生战车Boss.
                CreatNpcObj(NpcState.ZhanChe, m_CreatZhanCheState.GetSpawnPointState());
            }
        }
    }
#endif

    /// <summary>
    /// 获取是否有彩票boss.
    /// </summary>
    public bool GetIsHaveCaiPiaoBoss()
    {
        if (XkGameCtrl.GetInstance().IsDisplayBossDeathYanHua == true)
        {
            //正在播放boss爆炸粒子和玩家得奖烟花特效.
            return true;
        }

        GameObject npc = m_ZhanCheJPBossData.JPBossData.GetNpcByIndex(0);
        if (npc != null)
        {
            return true;
        }

        npc = m_ZhanCheJPBossData.SuperJPBossData.GetNpcByIndex(0);
        if (npc != null)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// 获取彩票战车或boss.
    /// </summary>
    public GameObject GetCaiPiaoNpc()
    {
        XKNpcMoveCtrl npc = m_ZhanCheJPBossData.ZhanCheData.GetNpcMoveComByIndex(0);
        if (npc != null)
        {
            if (npc.IsEnterCameraBox && npc.GetIsDeathNPC() == false)
            {
                return npc.gameObject;
            }
        }

        npc = m_ZhanCheJPBossData.JPBossData.GetNpcMoveComByIndex(0);
        if (npc != null)
        {
            if (npc.IsEnterCameraBox && npc.GetIsDeathNPC() == false)
            {
                return npc.gameObject;
            }
        }

        npc = m_ZhanCheJPBossData.SuperJPBossData.GetNpcMoveComByIndex(0);
        if (npc != null)
        {
            if (npc.IsEnterCameraBox && npc.GetIsDeathNPC() == false)
            {
                return npc.gameObject;
            }
        }
        return null;
    }

    public void ResetCreatNpcInfo(NpcState type)
    {
        //Debug.Log("ResetCreatNpcInfo -> type =================== " + type);
        switch (type)
        {
            case NpcState.ZhanChe:
                {
                    m_ZhanCheJPBossData.IsCreatZhanChe = false;
                    m_ZhanCheRulerData.Reset();
                    break;
                }
            case NpcState.JPBoss:
                {
                    m_ZhanCheJPBossData.IsCreatJPBoss = false;
                    m_JPBossRulerData.Reset();
                    break;
                }
            case NpcState.SuperJPBoss:
                {
                    m_ZhanCheJPBossData.IsCreatSuperJPBoss = false;
                    break;
                }
        }
    }

    /// <summary>
    /// 创建npc.
    /// npcType 产生的npc类型.
    /// pointState 产生点的方位信息.
    /// </summary>
    void CreatNpcObj(NpcState npcType, SpawnPointState pointState)
    {
        //Debug.Log("Unity: CreatNpcObj -> npcType ====== " + npcType + ", pointState ======= " + pointState);
        if (pointState == SpawnPointState.Null)
        {
            //pointState为null时不用产生战车npc.
            return;
        }

        NpcSpawnData data = GetNpcSpawnData(npcType, pointState);
        if (data != null)
        {
            SSCaiPiaoDataManage.GameCaiPiaoData.DaiJinQuanState daiJinQuanType = SSCaiPiaoDataManage.GameCaiPiaoData.DaiJinQuanState.JPBossDaiJinQuan;

            switch (npcType)
            {
                case NpcState.ZhanChe:
                    {
                        int rv = Random.Range(0, 100) % 2;
                        if (rv == 0)
                        {
                            daiJinQuanType = SSCaiPiaoDataManage.GameCaiPiaoData.DaiJinQuanState.ZhanCheDaiJinQuan_01;
                        }
                        else
                        {
                            daiJinQuanType = SSCaiPiaoDataManage.GameCaiPiaoData.DaiJinQuanState.ZhanCheDaiJinQuan_02;
                        }

                        if (XkGameCtrl.GetInstance() != null && XkGameCtrl.GetInstance().m_SSShangHuInfo != null)
                        {
                            XkGameCtrl.GetInstance().m_SSShangHuInfo.AddZhanCheShangHuMingInfo(daiJinQuanType);
                        }
                    }
                    break;
                case NpcState.JPBoss:
                    {
                        daiJinQuanType = SSCaiPiaoDataManage.GameCaiPiaoData.DaiJinQuanState.JPBossDaiJinQuan;
                        if (XkGameCtrl.GetInstance() != null && XkGameCtrl.GetInstance().m_SSShangHuInfo != null)
                        {
                            XkGameCtrl.GetInstance().m_SSShangHuInfo.GetJPBossShangHuMingInfo();
                        }
                    }
                    break;
            }

            GameObject obj = data.CreatPointNpc(daiJinQuanType, pointState);
            if (obj != null)
            {
                XKNpcMoveCtrl npcMove = null;
                switch (npcType)
                {
                    case NpcState.ZhanChe:
                        {
                            if (XkGameCtrl.GetInstance() != null && XkGameCtrl.GetInstance().m_AiPathGroup != null
                                && XkGameCtrl.GetInstance().m_AiPathGroup.m_CameraMoveType != AiPathGroupCtrl.MoveState.YuLe)
                            {
                                XkGameCtrl.GetInstance().m_AiPathGroup.SetCameraMoveType(AiPathGroupCtrl.MoveState.Default);
                            }
                            m_ZhanCheJPBossData.ZhanCheData.AddNpcToList(obj);

                            //播放boss来袭UI.
                            if (SSUIRoot.GetInstance().m_GameUIManage != null)
                            {
                                npcMove = obj.GetComponent<XKNpcMoveCtrl>();
                                if (npcMove != null)
                                {
                                    //代金券战车npc.
                                    SSUIRoot.GetInstance().m_GameUIManage.CreatBossLaiXiUI(npcType, npcMove.m_DaiJinQuanState);
                                }
                            }
                            break;
                        }
                    case NpcState.JPBoss:
                        {
                            if (XkGameCtrl.GetInstance() != null && XkGameCtrl.GetInstance().m_AiPathGroup != null
                                && XkGameCtrl.GetInstance().m_AiPathGroup.m_CameraMoveType != AiPathGroupCtrl.MoveState.YuLe)
                            {
                                XkGameCtrl.GetInstance().m_AiPathGroup.SetCameraMoveType(AiPathGroupCtrl.MoveState.Boss);
                            }
                            m_ZhanCheJPBossData.JPBossData.AddNpcToList(obj);

                            npcMove = obj.GetComponent<XKNpcMoveCtrl>();
                            if (npcMove != null)
                            {
                                switch(pointState)
                                {
                                    case SpawnPointState.Up:
                                        {
                                            npcMove.m_TriggerDir = SSTriggerCaiPiaoBossMove.TriggerDir.Qian;
                                            break;
                                        }
                                    case SpawnPointState.Down:
                                        {
                                            npcMove.m_TriggerDir = SSTriggerCaiPiaoBossMove.TriggerDir.Hou;
                                            break;
                                        }
                                    case SpawnPointState.Left:
                                        {
                                            npcMove.m_TriggerDir = SSTriggerCaiPiaoBossMove.TriggerDir.Zuo;
                                            break;
                                        }
                                    case SpawnPointState.Right:
                                        {
                                            npcMove.m_TriggerDir = SSTriggerCaiPiaoBossMove.TriggerDir.You;
                                            break;
                                        }
                                }
                                npcMove.SetJPBossHealthInfo();
                            }

                            //if (XKBossLXCtrl.GetInstance() != null)
                            //{
                            //    //播放boss来袭UI.
                            //    XKBossLXCtrl.GetInstance().StartPlayBossLaiXi();
                            //    AudioBeiJingCtrl.StopGameBeiJingAudio();
                            //}
                            //播放boss来袭UI.
                            if (SSUIRoot.GetInstance().m_GameUIManage != null)
                            {
                                SSUIRoot.GetInstance().m_GameUIManage.CreatBossLaiXiUI(npcType);
                                AudioBeiJingCtrl.StopGameBeiJingAudio();
                            }
                            break;
                        }
                    case NpcState.SuperJPBoss:
                        {
                            if (XkGameCtrl.GetInstance() != null && XkGameCtrl.GetInstance().m_AiPathGroup != null
                                && XkGameCtrl.GetInstance().m_AiPathGroup.m_CameraMoveType != AiPathGroupCtrl.MoveState.YuLe)
                            {
                                XkGameCtrl.GetInstance().m_AiPathGroup.SetCameraMoveType(AiPathGroupCtrl.MoveState.Boss);
                            }
                            m_ZhanCheJPBossData.SuperJPBossData.AddNpcToList(obj);


                            npcMove = obj.GetComponent<XKNpcMoveCtrl>();
                            if (npcMove != null)
                            {
                                switch (pointState)
                                {
                                    case SpawnPointState.Up:
                                        {
                                            npcMove.m_TriggerDir = SSTriggerCaiPiaoBossMove.TriggerDir.Qian;
                                            break;
                                        }
                                    case SpawnPointState.Down:
                                        {
                                            npcMove.m_TriggerDir = SSTriggerCaiPiaoBossMove.TriggerDir.Hou;
                                            break;
                                        }
                                    case SpawnPointState.Left:
                                        {
                                            npcMove.m_TriggerDir = SSTriggerCaiPiaoBossMove.TriggerDir.Zuo;
                                            break;
                                        }
                                    case SpawnPointState.Right:
                                        {
                                            npcMove.m_TriggerDir = SSTriggerCaiPiaoBossMove.TriggerDir.You;
                                            break;
                                        }
                                }
                            }

                            //if (XKBossLXCtrl.GetInstance() != null)
                            //{
                            //    //播放boss来袭UI.
                            //    XKBossLXCtrl.GetInstance().StartPlayBossLaiXi();
                            //    AudioBeiJingCtrl.StopGameBeiJingAudio();
                            //}

                            //播放boss来袭UI.
                            if (SSUIRoot.GetInstance().m_GameUIManage != null)
                            {
                                SSUIRoot.GetInstance().m_GameUIManage.CreatBossLaiXiUI(npcType);
                                AudioBeiJingCtrl.StopGameBeiJingAudio();
                            }
                            break;
                        }
                }
            }
        }
    }

    NpcSpawnData GetNpcSpawnData(NpcState npcType, SpawnPointState pointState)
    {
        if (pointState == SpawnPointState.Null)
        {
            //不产生战车、JPBoss和SuperJPBoss.
            return null;
        }

        NpcSpawnData data = new NpcSpawnData(this);
        //获取ncp预制.
        data.NpcPrefab = GetNpcPrefab(npcType, pointState);
        SSCreatNpcData creatNpcDt = GetCreatNpcData(npcType, pointState);
        if (creatNpcDt != null)
        {
            //获取npc路径.
            data.NpcPath = creatNpcDt.GetNpcPahtData();
            //获取产生点组件.
            data.SpawnPoint = creatNpcDt.m_SpawnPoint;
        }
        return data;
    }

    /// <summary>
    /// 获取创建npc的组件.
    /// </summary>
    SSCreatNpcData GetCreatNpcData(NpcState npcType, SpawnPointState type)
    {
        SSCreatNpcData[] comGp = null;
        switch (type)
        {
            case SpawnPointState.Left:
                {
                    if (npcType == NpcState.ZhanChe)
                    {
                        comGp = m_NpcData.LeftSpawnPointGp;
                    }
                    else
                    {
                        comGp = m_NpcData.Boss_LeftSpawnPointGp;
                    }
                    break;
                }
            case SpawnPointState.Right:
                {
                    if (npcType == NpcState.ZhanChe)
                    {
                        comGp = m_NpcData.RightSpawnPointGp;
                    }
                    else
                    {
                        comGp = m_NpcData.Boss_RightSpawnPointGp;
                    }
                    break;
                }
            case SpawnPointState.Up:
                {
                    if (npcType == NpcState.ZhanChe)
                    {
                        comGp = m_NpcData.UpSpawnPointGp;
                    }
                    else
                    {
                        comGp = m_NpcData.Boss_UpSpawnPointGp;
                    }
                    break;
                }
            case SpawnPointState.Down:
                {
                    if (npcType == NpcState.ZhanChe)
                    {
                        comGp = m_NpcData.DownSpawnPointGp;
                    }
                    else
                    {
                        comGp = m_NpcData.Boss_DownSpawnPointGp;
                    }
                    break;
                }
        }

        if (comGp == null || comGp.Length <= 0)
        {
            Debug.LogWarning("Unity: not find CreatNpcData! type ================ " + type + ", npcType == " + npcType);
            return null;
        }

        SSCreatNpcData com = null;
        int rv = Random.Range(0, 100) % comGp.Length;
        com = comGp[rv];
        if (com == null)
        {
            Debug.LogWarning("Unity: com was null! rv ============ " + rv + ", type == " + type);
        }
        return com;
    }

    /// <summary>
    /// 获取npc预制.
    /// </summary>
    GameObject GetNpcPrefab(NpcState type, SpawnPointState pointState)
    {
        GameObject[] npcPrefabGp = null;
        switch (type)
        {
            case NpcState.ZhanChe:
                {
                    if (pointState == SpawnPointState.Left)
                    {
                        npcPrefabGp = m_NpcData.L_ZhanChePrefabGp;
                    }
                    else if (pointState == SpawnPointState.Right)
                    {
                        npcPrefabGp = m_NpcData.R_ZhanChePrefabGp;
                    }
                    else if (pointState == SpawnPointState.Up)
                    {
                        npcPrefabGp = m_NpcData.U_ZhanChePrefabGp;
                    }
                    else if (pointState == SpawnPointState.Down)
                    {
                        npcPrefabGp = m_NpcData.D_ZhanChePrefabGp;
                    }
                    break;
                }
            case NpcState.JPBoss:
                {
                    npcPrefabGp = m_NpcData.JPBossPrefabGp;
                    break;
                }
            case NpcState.SuperJPBoss:
                {
                    npcPrefabGp = m_NpcData.SuperJPBossPrefabGp;
                    break;
                }
        }

        if (npcPrefabGp == null || npcPrefabGp.Length <= 0)
        {
            Debug.LogWarning("Unity: not find npc! type ================ " + type);
            return null;
        }

        GameObject npcPrefab = null;
        int rv = Random.Range(0, 100) % npcPrefabGp.Length;
        npcPrefab = npcPrefabGp[rv];
        if (npcPrefab == null)
        {
            Debug.LogWarning("Unity: npcPrefab was null! rv ============ " + rv + ", type == " + type);
        }
        return npcPrefab;
    }
    
    /// <summary>
    /// 使对象坐标贴地.
    /// </summary>
    void MakePintToLand(Transform tr)
    {
        if (tr != null)
        {
            RaycastHit hitInfo;
            Vector3 startPos = tr.position + Vector3.up * 20f;
            Vector3 forwardVal = Vector3.down;
            Physics.Raycast(startPos, forwardVal, out hitInfo, 200f, XkGameCtrl.GetInstance().LandLayer);
            if (hitInfo.collider != null)
            {
                tr.position = hitInfo.point + Vector3.up * 5f;
            }
        }
    }

    /// <summary>
    /// 使所有路径点贴地.
    /// </summary>
    void MakePathNodeToLand(Transform tr)
    {
        if (tr != null)
        {
            int max = tr.childCount;
            for (int i = 0; i < max; i++)
            {
                MakePintToLand(tr.GetChild(i));
            }
        }
    }

    /// <summary>
    /// 使产生点贴地.
    /// </summary>
    void MakeSpawnPointToLand(SSCreatNpcData dt)
    {
        if (dt != null)
        {
            MakePintToLand(dt.transform);
            int length = dt.m_NpcPathGp.Length;
            for (int i = 0; i < length; i++)
            {
                if (dt.m_NpcPathGp[i] != null)
                {
                    MakePathNodeToLand(dt.m_NpcPathGp[i].transform);
                }
            }
        }
    }

    /// <summary>
    /// 使所有产生点贴地.
    /// </summary>
    void MakeAllSpawnPonitsToLand(SSCreatNpcData[] spawnPoints)
    {
        int length = spawnPoints.Length;
        for (int i = 0; i < length; i++)
        {
            if (spawnPoints[i] != null)
            {
                MakeSpawnPointToLand(spawnPoints[i]);
            }
        }
    }

    /// <summary>
    /// 使所有创建彩票战车或boss的产生点和路径点贴地.
    /// </summary>
    public void MakeAllCreatNpcPointsToLand()
    {
        MakeAllSpawnPonitsToLand(m_NpcData.Boss_DownSpawnPointGp);
        MakeAllSpawnPonitsToLand(m_NpcData.Boss_UpSpawnPointGp);
        MakeAllSpawnPonitsToLand(m_NpcData.Boss_LeftSpawnPointGp);
        MakeAllSpawnPonitsToLand(m_NpcData.Boss_RightSpawnPointGp);
        MakeAllSpawnPonitsToLand(m_NpcData.DownSpawnPointGp);
        MakeAllSpawnPonitsToLand(m_NpcData.UpSpawnPointGp);
        MakeAllSpawnPonitsToLand(m_NpcData.LeftSpawnPointGp);
        MakeAllSpawnPonitsToLand(m_NpcData.RightSpawnPointGp);
    }

    /// <summary>
    /// 获取当前游戏场景是否有Boss存在.
    /// </summary>
    internal bool GetIsHaveBoss()
    {
        bool isHave = true;
        if (!m_ZhanCheJPBossData.ZhanCheData.GetIsHaveNpc()
            && !m_ZhanCheJPBossData.JPBossData.GetIsHaveNpc()
            && !m_ZhanCheJPBossData.SuperJPBossData.GetIsHaveNpc())
        {
            //当前没有任何Boss存在.
            if (XkGameCtrl.GetInstance().IsDisplayBossDeathYanHua == true)
            {
                //正在播放boss爆炸粒子和玩家得奖烟花特效.
            }
            else
            {
                //没有播放boss爆炸粒子和玩家得奖烟花特效.
                isHave = false;
            }
        }
        return isHave;
    }
}