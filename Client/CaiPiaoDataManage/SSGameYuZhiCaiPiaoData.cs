
/// <summary>
/// 游戏预制彩票数据.
/// </summary>
[System.Serializable]
public class SSGameYuZhiCaiPiaoData
{
    /// <summary>
    /// 预支彩票的倍率数据(该数据乘以一币兑换的彩票数).
    /// </summary>
    public int YuZhiCaiPiaoBeiLv = 100;

    float _YuZhiCaiPiaoVal = 0;
    /// <summary>
    /// 预制彩票数量 == 战车彩票数量 + JPBoss彩票数量.
    /// </summary>
    public float YuZhiCaiPiaoVal
    {
        set
        {
            XKGlobalData.GetInstance().SetYuZhiCaiChi(value);
            //_YuZhiCaiPiaoVal = value; //代金券版本关闭预支彩池.
        }
        get
        {
            return _YuZhiCaiPiaoVal;
        }
    }
    /// <summary>
    /// 初始化预制彩票.
    /// </summary>
    public void Init()
    {
        bool isUpdateYuZhiCaiChi = XKGlobalData.GetInstance().InitYuZhiCaiChi(YuZhiCaiPiaoBeiLv);
        YuZhiCaiPiaoVal = XKGlobalData.GetInstance().m_YuZhiCaiChi;
        //int coinToCaiPiao = XKGlobalData.GetInstance().m_CoinToCard;
        //YuZhiCaiPiaoVal = YuZhiCaiPiaoBeiLv * coinToCaiPiao;

        UpdateYuZhiCaiChiData();
        if (isUpdateYuZhiCaiChi)
        {
            SubZhanCheCaiPiaoVal();
            SubJPBossCaiPiaoVal();
        }
        else
        {
            XkPlayerCtrl.GetInstanceFeiJi().m_SpawnNpcManage.m_CaiPiaoDataManage.m_GameCaiPiaoData.ZhanCheDeCai_01 = XKGlobalData.GetInstance().m_ZhanCheCaiChi_01;
            XkPlayerCtrl.GetInstanceFeiJi().m_SpawnNpcManage.m_CaiPiaoDataManage.m_GameCaiPiaoData.ZhanCheDeCai_02 = XKGlobalData.GetInstance().m_ZhanCheCaiChi_02;
            XkPlayerCtrl.GetInstanceFeiJi().m_SpawnNpcManage.m_CaiPiaoDataManage.m_GameCaiPiaoData.JPBossDeCai = XKGlobalData.GetInstance().m_JPBossCaiChi;
        }
    }

    /// <summary>
    /// 获取最原始的预制彩票数量.
    /// </summary>
    float GetOldYuZhiCaiPiaoVal()
    {
        float coinToCaiPiao = XKGlobalData.GetInstance().m_CoinToCard;
        return YuZhiCaiPiaoBeiLv * coinToCaiPiao;
    }

    /// <summary>
    /// 添加预制彩票数量.
    /// </summary>
    public void AddYuZhiCaiPiao(int val)
    {
        //UnityEngine.Debug.Log("Unity: AddYuZhiCaiPiao -> val ================ " + val);

        int zhanCheCaiPiaoAdd = (int)(val * ZhanCheCaiPiaoBiLi);
        int jpBossCaiPiaoAdd = (int)(val * JPBossCaiPiaoBiLi);
        float oldYuZhiCaiPiao = GetOldYuZhiCaiPiaoVal();
        float yuZhiCaiPiaoTmp = YuZhiCaiPiaoVal;
        YuZhiCaiPiaoVal += val;
        if (YuZhiCaiPiaoVal > oldYuZhiCaiPiao)
        {
            float valAdd = oldYuZhiCaiPiao - yuZhiCaiPiaoTmp;
            zhanCheCaiPiaoAdd = (int)(valAdd * ZhanCheCaiPiaoBiLi);
            jpBossCaiPiaoAdd = (int)(valAdd * JPBossCaiPiaoBiLi);

            float superJPBossCaiPiao = YuZhiCaiPiaoVal - oldYuZhiCaiPiao;
            //superJPBoss彩票池.
            XkPlayerCtrl.GetInstanceFeiJi().m_SpawnNpcManage.m_CaiPiaoDataManage.m_SuperJPBossCaiPiaoData.SuperJPCaiPiao = superJPBossCaiPiao;
            YuZhiCaiPiaoVal = oldYuZhiCaiPiao;
        }

        if (zhanCheCaiPiaoAdd > 0)
        {
            SetZhanCheCaiPiao(ZhanCheCaiPiaoVal + zhanCheCaiPiaoAdd);
        }

        if (jpBossCaiPiaoAdd > 0)
        {
            SetJPBossCaiPiao(JPBossCaiPiaoVal + jpBossCaiPiaoAdd);
        }
        //UpdateYuZhiCaiChiData();
    }

    /// <summary>
    /// 减少预制彩票数量.
    /// </summary>
    void SubYuZhiCaiPiao(int val)
    {
        YuZhiCaiPiaoVal -= val;
    }

    /// <summary>
    /// 更新预制彩池数据.
    /// </summary>
    void UpdateYuZhiCaiChiData()
    {
        //更新战车彩池.
        SetZhanCheCaiPiao((int)(YuZhiCaiPiaoVal * ZhanCheCaiPiaoBiLi));
        //更新JPBoss彩池.
        SetJPBossCaiPiao((int)(YuZhiCaiPiaoVal * JPBossCaiPiaoBiLi));
    }

    /// <summary>
    /// 战车彩票比例.
    /// </summary>
    float ZhanCheCaiPiaoBiLi = 0.4f;
    /// <summary>
    /// 战车彩票每次取出的比例.
    /// </summary>
    float ZhanCheQuChuBiLi = 0.03f;
    int _ZhanCheCaiPiaoVal = 0;
    /// <summary>
    /// 战车彩票数量.
    /// </summary>
    public int ZhanCheCaiPiaoVal
    {
        set
        {
            _ZhanCheCaiPiaoVal = value;
        }
        get
        {
            return _ZhanCheCaiPiaoVal;
        }
    }

    /// <summary>
    /// 设置战车彩票数量.
    /// </summary>
    void SetZhanCheCaiPiao(int val)
    {
        UnityEngine.Debug.Log("Unity: SetZhanCheCaiPiao -> val ================ " + val);
        ZhanCheCaiPiaoVal = val;
    }

    /// <summary>
    /// 减少战车彩票数量,对战车彩池进行补充.
    /// </summary>
    public void SubZhanCheCaiPiaoVal()
    {
        //每次投入到战车彩池的彩票数量.
        int subVal = (int)(GetOldYuZhiCaiPiaoVal() * ZhanCheCaiPiaoBiLi * ZhanCheQuChuBiLi);
        if (ZhanCheCaiPiaoVal >= subVal)
        {
            UnityEngine.Debug.Log("SubZhanCheCaiPiaoVal -> subVal ============ " + subVal);
            //对预制彩池进行数据更新.
            SubYuZhiCaiPiao(subVal);
            SetZhanCheCaiPiao(ZhanCheCaiPiaoVal - subVal);

            if (XkPlayerCtrl.GetInstanceFeiJi() != null)
            {
                //对战车彩池进行补充.
                XkPlayerCtrl.GetInstanceFeiJi().m_SpawnNpcManage.m_CaiPiaoDataManage.m_GameCaiPiaoData.ZhanCheDeCai_01 += subVal;
            }
        }
    }
    
    /// <summary>
    /// JPBoss彩票比例.
    /// </summary>
    float JPBossCaiPiaoBiLi = 0.6f;
    /// <summary>
    /// JPBoss彩票每次取出的比例.
    /// </summary>
    float JPBossQuChuBiLi = 0.2f;
    int _JPBossCaiPiaoVal = 0;
    /// <summary>
    /// JPBoss彩票数量.
    /// </summary>
    public int JPBossCaiPiaoVal
    {
        set
        {
            _JPBossCaiPiaoVal = value;
        }
        get
        {
            return _JPBossCaiPiaoVal;
        }
    }

    /// <summary>
    /// 设置JPBoss彩票数量.
    /// </summary>
    void SetJPBossCaiPiao(int val)
    {
        UnityEngine.Debug.Log("Unity: SetJPBossCaiPiao -> val ================ " + val);
        JPBossCaiPiaoVal = val;
    }

    /// <summary>
    /// 减少JPBoss彩票数量,对JPBoss彩池进行补充.
    /// </summary>
    public void SubJPBossCaiPiaoVal()
    {
        //每次投入到JPBoss彩池的彩票数量.
        int subVal = (int)(GetOldYuZhiCaiPiaoVal() * JPBossCaiPiaoBiLi * JPBossQuChuBiLi);
        if (JPBossCaiPiaoVal >= subVal)
        {
            UnityEngine.Debug.Log("SubJPBossCaiPiaoVal -> subVal ============ " + subVal);
            //对预制彩池进行数据更新.
            SubYuZhiCaiPiao(subVal);
            SetJPBossCaiPiao(JPBossCaiPiaoVal - subVal);

            if (XkPlayerCtrl.GetInstanceFeiJi() != null)
            {
                //对JPBoss彩池进行补充.
                XkPlayerCtrl.GetInstanceFeiJi().m_SpawnNpcManage.m_CaiPiaoDataManage.m_GameCaiPiaoData.JPBossDeCai += subVal;
            }
        }
    }
}