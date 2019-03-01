using UnityEngine;
using System.Collections;

public class SSChouJiangUI : MonoBehaviour
{
    //抽奖环节：显示抽奖界面（15秒启动开奖倒计时，15秒后自动开奖），当玩家点击手柄射击键时，抽奖游戏开始，抽中或抽不中均提示玩家。
    /// <summary>
    /// 逐渐显示的时间.
    /// </summary>
    [Range(0f, 10f)]
    public float m_AlphaTime = 0.5f;
    /// <summary>
    /// 15秒倒计时.
    /// </summary>
    public SSGameNumUI m_DaoJiShiNumUI;
    /// <summary>
    /// 谢谢参与音效.
    /// </summary>
    public AudioClip m_XieXieCanYuAudio;
    /// <summary>
    /// 得奖音效.
    /// </summary>
    public AudioClip m_DeJiangAudio;
    /// <summary>
    /// 转动时的音源.
    /// </summary>
    public AudioSource m_ZhuanDongAdioSource;
    /// <summary>
    /// 结果展示的音源.
    /// </summary>
    public AudioSource m_ResultAdioSource;
    /// <summary>
    /// 抽奖动画提示UI对象.
    /// </summary>
    public GameObject m_TiShiObj;
    /// <summary>
    /// 展示抽奖结果的恭喜获得UI.
    /// </summary>
    public GameObject m_GongXiUIObj;
    /// <summary>
    /// 展示抽奖结果的谢谢参与UI.
    /// </summary>
    public GameObject m_XieXieCanYuUIObj;
    /// <summary>
    /// "找工作人员"UI.
    /// </summary>
    public GameObject m_ZhaoGongZuoRenYuanUIObj;
    /// <summary>
    /// 玩家获得分数没有达到抽奖时需要进行隐藏的对象.
    /// </summary>
    public GameObject[] m_HiddenArray;
    /// <summary>
    /// 不允许玩家进行抽经.
    /// </summary>
    public GameObject m_BuYunXuChouJiangObj;
    /// <summary>
    /// 玩家索引信息.
    /// </summary>
    PlayerEnum m_IndexPlayer = PlayerEnum.Null;
    /// <summary>
    /// 自动抽奖倒计时.
    /// </summary>
    [Range(3, 20)]
    public int m_DaoJiShiVal = 5;
    internal void Init(PlayerEnum indexPlayer, bool isCanChouJiang)
    {
        m_IndexPlayer = indexPlayer;
        CheckHiddenObjArray(isCanChouJiang);
        InitPanelAlphaToMax();
        HiddeChouJiangZhanShiUI();
        InitJiangPinUI();
        SetActivePlayerChouJiangResult(false);
        StartCoroutine(PlayChouJiangDaoJiShi());

        if (isCanChouJiang == true)
        {
            //允许抽奖.
            switch (m_IndexPlayer)
            {
                case PlayerEnum.PlayerOne:
                    {
                        InputEventCtrl.GetInstance().ClickDaoDanBtOneEvent += ClickDaoDanBtOneEvent;
                    }
                    break;
                case PlayerEnum.PlayerTwo:
                    {
                        InputEventCtrl.GetInstance().ClickDaoDanBtTwoEvent += ClickDaoDanBtTwoEvent;
                    }
                    break;
                case PlayerEnum.PlayerThree:
                    {
                        InputEventCtrl.GetInstance().ClickDaoDanBtThreeEvent += ClickDaoDanBtThreeEvent;
                    }
                    break;
            }

            if (pcvr.GetInstance().m_HongDDGamePadInterface != null)
            {
                //显示玩家微信手柄抽奖ui.
                pcvr.GetInstance().m_HongDDGamePadInterface.SendWXPadShowChouJiangUI(m_IndexPlayer);
            }
        }
        SetActiveBuYunXuChouJiangObj(isCanChouJiang);
    }

    /// <summary>
    /// 初始化逐渐显示界面功能.
    /// </summary>
    void InitPanelAlphaToMax()
    {
        UITexture uiTexture = gameObject.AddComponent<UITexture>();
        uiTexture.alpha = 0f;
        TweenAlpha tweenAlpha = gameObject.AddComponent<TweenAlpha>();
        tweenAlpha.from = 0f;
        tweenAlpha.to = 1f;
        tweenAlpha.duration = m_AlphaTime;
    }

    /// <summary>
    /// 设置是否激活不允许抽奖UI界面.
    /// </summary>
    void SetActiveBuYunXuChouJiangObj(bool isCanChouJiang)
    {
        if (m_BuYunXuChouJiangObj != null)
        {
            m_BuYunXuChouJiangObj.SetActive(isCanChouJiang);
        }
    }

    /// <summary>
    /// 检测是否隐藏对象.
    /// </summary>
    void CheckHiddenObjArray(bool isCanChouJiang)
    {
        for (int i = 0; i < m_HiddenArray.Length; i++)
        {
            if (m_HiddenArray[i] != null)
            {
                m_HiddenArray[i].SetActive(!isCanChouJiang);
            }
        }
    }

    private void ClickDaoDanBtOneEvent(pcvr.ButtonState val)
    {
        OnClickDaoDanBt(val);
    }

    private void ClickDaoDanBtTwoEvent(pcvr.ButtonState val)
    {
        OnClickDaoDanBt(val);
    }

    private void ClickDaoDanBtThreeEvent(pcvr.ButtonState val)
    {
        OnClickDaoDanBt(val);
    }

    void OnClickDaoDanBt(pcvr.ButtonState val)
    {
        if (val == pcvr.ButtonState.DOWN)
        {
            if (IsPlayChouJiangDaoJiShi == true)
            {
                //关闭抽奖界面倒计时.
                CloseChouJiangDaoJiShi();
                //初始化播放抽奖动画.
                InitPlayChouJiangAnimation();
                RemoveClickDaoDanBtEvent();
                HiddenTiShiObj();
            }
        }
    }

    /// <summary>
    /// 隐藏抽奖提示UI.
    /// </summary>
    void HiddenTiShiObj()
    {
        if (m_TiShiObj != null)
        {
            m_TiShiObj.SetActive(false);
        }
    }

    void RemoveClickDaoDanBtEvent()
    {
        switch (m_IndexPlayer)
        {
            case PlayerEnum.PlayerOne:
                {
                    InputEventCtrl.GetInstance().ClickDaoDanBtOneEvent -= ClickDaoDanBtOneEvent;
                }
                break;
            case PlayerEnum.PlayerTwo:
                {
                    InputEventCtrl.GetInstance().ClickDaoDanBtOneEvent -= ClickDaoDanBtTwoEvent;
                }
                break;
            case PlayerEnum.PlayerThree:
                {
                    InputEventCtrl.GetInstance().ClickDaoDanBtOneEvent -= ClickDaoDanBtThreeEvent;
                }
                break;
        }

        if (pcvr.GetInstance().m_HongDDGamePadInterface != null)
        {
            //隐藏玩家微信手柄抽奖ui.
            pcvr.GetInstance().m_HongDDGamePadInterface.SendWXPadHiddenChouJiangUI(m_IndexPlayer);
        }
    }

    void ShowChouJiangDaoJiShiUI(int time)
    {
        if (time >= 0 && m_DaoJiShiNumUI != null)
        {
            m_DaoJiShiNumUI.ShowNumUI(time);
        }
    }

    bool IsPlayChouJiangDaoJiShi = false;
    /// <summary>
    /// 播放抽奖界面倒计时.
    /// </summary>
    IEnumerator PlayChouJiangDaoJiShi()
    {
        if (IsPlayChouJiangDaoJiShi == true)
        {
            yield break;
        }
        IsPlayChouJiangDaoJiShi = true;

        int daoJiShiVal = m_DaoJiShiVal;
        do
        {
            ShowChouJiangDaoJiShiUI(daoJiShiVal);
            yield return new WaitForSeconds(1f);
            if (daoJiShiVal > 0)
            {
                daoJiShiVal--;
                if (daoJiShiVal <= 0)
                {
                    ShowChouJiangDaoJiShiUI(0);
                    break;
                }
            }
        }
        while (daoJiShiVal >= 0 && IsPlayChouJiangDaoJiShi == true);

        yield return new WaitForSeconds(0.2f);
        if (IsPlayChouJiangDaoJiShi == true)
        {
            //关闭抽奖界面倒计时.
            CloseChouJiangDaoJiShi();
            //初始化播放抽奖动画.
            InitPlayChouJiangAnimation();
            RemoveClickDaoDanBtEvent();
            HiddenTiShiObj();
        }
    }

    void CloseChouJiangDaoJiShi()
    {
        if (IsPlayChouJiangDaoJiShi == true)
        {
            IsPlayChouJiangDaoJiShi = false;
            if (m_DaoJiShiNumUI != null)
            {
                m_DaoJiShiNumUI.SetActive(false);
            }
        }
    }

    #region 9宫格抽奖逻辑
    /// <summary>
    /// 抽奖数据.
    /// </summary>
    [System.Serializable]
    public class ChouJiangData
    {
        /// <summary>
        /// 从左上角开始按照顺时针转动时奖品索引编号信息.
        /// </summary>
        internal int jiangPingIndex = 0;
        /// <summary>
        /// 奖品类型.
        /// </summary>
        internal JiangPinState jiangPinType = JiangPinState.XieXieCanYu;
        /// <summary>
        /// 显示奖品名称的UI.
        /// </summary>
        public UILabel jiangPinNameUI;
        /// <summary>
        /// 奖品发光图片对象.
        /// </summary>
        public GameObject jiangPinFlashObj;
        public override string ToString()
        {
            return "jiangPinType == " + jiangPinType.ToString();
        }
        public ChouJiangData(int jiangPingIndex, JiangPinState type)
        {
            this.jiangPingIndex = jiangPingIndex;
            jiangPinType = type;
        }

        internal void SetActiveFlashObj(bool isActive)
        {
            if (jiangPinFlashObj != null)
            {
                jiangPinFlashObj.SetActive(isActive);
            }
        }

        /// <summary>
        /// 设置奖品名称信息.
        /// </summary>
        internal void ShowJiangPinName()
        {
            if (jiangPinNameUI == null)
            {
                return;
            }

            string name = "";
            switch (jiangPinType)
            {
                case JiangPinState.XieXieCanYu:
                    {
                        name = "谢谢参与";
                        break;
                    }
                case JiangPinState.ZaiWanYiJu:
                    {
                        name = "再玩一局";
                        break;
                    }
                case JiangPinState.JiangPin2:
                    {
                        if (XkGameCtrl.GetInstance().m_SSShangHuInfo != null)
                        {
                            name = XkGameCtrl.GetInstance().m_SSShangHuInfo.GetShangHuMingDt(SSCaiPiaoDataManage.GameCaiPiaoData.DaiJinQuanState.ZhanCheDaiJinQuan_01).ShangHuJiangPinName;
                        }
                        break;
                    }
                case JiangPinState.JiangPin3:
                    {
                        if (XkGameCtrl.GetInstance().m_SSShangHuInfo != null)
                        {
                            name = XkGameCtrl.GetInstance().m_SSShangHuInfo.GetShangHuMingDt(SSCaiPiaoDataManage.GameCaiPiaoData.DaiJinQuanState.ZhanCheDaiJinQuan_02).ShangHuJiangPinName;
                        }
                        break;
                    }
                case JiangPinState.JiangPin4:
                    {
                        if (XkGameCtrl.GetInstance().m_SSShangHuInfo != null)
                        {
                            name = XkGameCtrl.GetInstance().m_SSShangHuInfo.GetSuiJiDaoJuShangHuInfo().ShangHuJiangPinName;
                        }
                        break;
                    }
            }

            if (name != "")
            {
                jiangPinNameUI.text = name;
            }
        }
    }
    /// <summary>
    /// 奖品布局.
    /// 0   1   0
    /// 2       3
    /// 0   4   0
    /// </summary>
    public ChouJiangData[] m_ChouJiangDtArray = new ChouJiangData[8]
    {
        new ChouJiangData(0, JiangPinState.XieXieCanYu), new ChouJiangData(1, JiangPinState.ZaiWanYiJu), new ChouJiangData(2, JiangPinState.XieXieCanYu),
        new ChouJiangData(7, JiangPinState.JiangPin2),                                                   new ChouJiangData(3, JiangPinState.JiangPin3),
        new ChouJiangData(6, JiangPinState.XieXieCanYu), new ChouJiangData(5, JiangPinState.JiangPin4),  new ChouJiangData(4, JiangPinState.XieXieCanYu)
    };

    /// <summary>
    /// 获取奖品在数据列表中的索引编号.
    /// </summary>
    int GetJiangPinIndexValue(JiangPinState type)
    {
        int indexVal = 0;
        switch (type)
        {
            case JiangPinState.JiangPin2:
                {
                    indexVal = 3;
                    break;
                }
            case JiangPinState.JiangPin3:
                {
                    indexVal = 4;
                    break;
                }
            case JiangPinState.JiangPin4:
                {
                    indexVal = 6;
                    break;
                }
            case JiangPinState.ZaiWanYiJu:
                {
                    indexVal = 1;
                    break;
                }
            case JiangPinState.XieXieCanYu:
                {
                    int randVal = Random.Range(0, 100) % 4;
                    if (randVal == 0)
                    {
                        indexVal = 0;
                    }
                    else if (randVal == 1)
                    {
                        indexVal = 2;
                    }
                    else if (randVal == 2)
                    {
                        indexVal = 5;
                    }
                    else
                    {
                        indexVal = 7;
                    }
                    break;
                }
        }
        return indexVal;
    }
    
    /// <summary>
    /// 奖品枚举.
    /// </summary>
    public enum JiangPinState
    {
        /// <summary>
        /// 谢谢参与.
        /// </summary>
        XieXieCanYu = 0,
        /// <summary>
        /// 再玩一局游戏.
        /// </summary>
        ZaiWanYiJu = 1,
        /// <summary>
        /// 奖品2/战车01.
        /// </summary>
        JiangPin2 = 2,
        /// <summary>
        /// 奖品3/战车02.
        /// </summary>
        JiangPin3 = 3,
        /// <summary>
        /// 奖品4/随机道具奖品.
        /// </summary>
        JiangPin4 = 4,
    }
    
    bool IsInitPlayChouJiangAni = false;
    /// <summary>
    /// 初始化播放抽奖动画.
    /// </summary>
    void InitPlayChouJiangAnimation()
    {
        if (IsInitPlayChouJiangAni == true)
        {
            return;
        }
        IsInitPlayChouJiangAni = true;

        CheckPlayerJiangPinData();
        //播放抽奖动画.
        StartCoroutine(PlayChouJiangAnimation());
    }

    /// <summary>
    /// 初始化奖品信息.
    /// </summary>
    void InitJiangPinUI()
    {
        for (int i = 0; i < m_ChouJiangDtArray.Length; i++)
        {
            if (m_ChouJiangDtArray[i] != null)
            {
                m_ChouJiangDtArray[i].ShowJiangPinName();
                m_ChouJiangDtArray[i].SetActiveFlashObj(false);
            }
        }
    }

    /// <summary>
    /// 奖品闪光图片转动的间隔时间.
    /// </summary>
    public float[] m_TimeChouJiangAniArray = new float[3]{0.1f, 0.4f, 0.6f};
    float GetTimeChouJiangAni(ChouJiangAniStage stage)
    {
        float time = 1f;
        int indexVal = (int)stage;
        if (indexVal >= 0 && indexVal < m_TimeChouJiangAniArray.Length)
        {
            time = m_TimeChouJiangAniArray[indexVal];
        }

        if (time < 0.03f)
        {
            time = 0.03f;
        }
        return time;
    }
    /// <summary>
    /// 抽奖动画播放阶段.
    /// </summary>
    public enum ChouJiangAniStage
    {
        Null = -1,
        Stage01 = 0,
        Stage02 = 1,
        Stage03 = 2,
    }

    /// <summary>
    /// 获取下一阶段的枚举.
    /// </summary>
    public ChouJiangAniStage GetNextChouJiangAniStage(ChouJiangAniStage stage)
    {
        ChouJiangAniStage nextStage = ChouJiangAniStage.Null;
        switch (stage)
        {
            case ChouJiangAniStage.Stage01:
                {
                    nextStage = ChouJiangAniStage.Stage02;
                    break;
                }
            case ChouJiangAniStage.Stage02:
                {
                    nextStage = ChouJiangAniStage.Stage03;
                    break;
                }
        }
        return nextStage;
    }

    /// <summary>
    /// 获取抽奖动画各个阶段需要走过多少步.
    /// </summary>
    int GetChouJiangAniMoveStep(ChouJiangAniStage type)
    {
        int step = 0;
        switch(type)
        {
            case ChouJiangAniStage.Stage01:
                {
                    step = 8 * 3;
                    break;
                }
            case ChouJiangAniStage.Stage02:
                {
                    if (m_PlayerJiangPin != null)
                    {
                        step = 8 + m_PlayerJiangPin.jiangPingIndex - 2;
                    }
                    break;
                }
            case ChouJiangAniStage.Stage03:
                {
                    step = 1;
                    break;
                }
        }
        return step;
    }

    /// <summary>
    /// 玩家所得奖品.
    /// </summary>
    ChouJiangData m_PlayerJiangPin;
    //[System.Serializable]
    //public class ZhongJiangGaiLvData
    //{
        /// <summary>
        /// 奖品2-战车01随机概率.
        /// </summary>
        //public int JiangPin02 = 50;
        /// <summary>
        /// 奖品3-战车02随机概率.
        /// </summary>
        //public int JiangPin03 = 50;
        /// <summary>
        /// 奖品4-随机道具随机概率.
        /// </summary>
        //public int JiangPin04 = 50;
        /// <summary>
        /// 再来一局游戏的随机概率.
        /// </summary>
        //public int ZaiWanYiJu = 50;
    //}
    /// <summary>
    /// 各种奖品的中奖概率.
    /// </summary>
    //public ZhongJiangGaiLvData m_ZhongJiangGaiLvDt;
    
    /// <summary>
    /// 获取某种类型奖品是否可以中奖.
    /// </summary>
    bool GetIsCanZhongJiang(JiangPinState type)
    {
        bool isZhongJiang = false;
        int randVal = Random.Range(0, 1000) % 100;
        int gaiLv = 50;
        //根据各个奖品对应的人头数进行概率数值运算.
        switch (type)
        {
            case JiangPinState.JiangPin2:
            case JiangPinState.JiangPin3:
            case JiangPinState.JiangPin4:
                {
                    //奖品2,3,4不需要在这里进行随机计算.
                    //SSHaiDiLaoBaoJiang.GetInstance().GetIsCanJiBaoNpc 奖品2,3,4用这个函数就可以进行控制了.
                    gaiLv = 100;
                    break;
                }
            case JiangPinState.ZaiWanYiJu:
                {
                    gaiLv = 100;
                    break;
                }
        }
        isZhongJiang = randVal < gaiLv ? true : false;
        return isZhongJiang;
    }

    /// <summary>
    /// 检测玩家可以得到什么奖品.
    /// </summary>
    void CheckPlayerJiangPinData()
    {
        //此处添加玩家获得什么奖品的代码.
        //当前还有战车Boss奖品没有发出的情况下,游戏画面有战车Boss时,如果该战车Boss属于不能被击爆的类型,那么此时玩家有机会随机到战车Boss奖品上.
        //当前还有战车Boss奖品没有发出的情况下,游戏画面也没有战车Boss时,那么此时玩家有机会随机到战车Boss奖品上.
        //当前还有奖品4-随机道具奖品没有发出的情况下,那么此时玩家有机会随机到奖品4上.
        //当前机位如果还没有获得免费继续游戏奖品的情况下,那么此时玩家有机会随机到免费继续游戏奖品.
        //当前玩家不能获取任何奖品的情况下,那么此时玩家只能在谢谢参与奖品里随机一个了.
        int indexJiangPin = -1;
        //是否击爆(是否已经出奖).
        bool isHaveJiBao = false;
        //bool isZhongJiang = true;
        bool isCanJiBao = false;
        //彩池金额是否充足.
        bool isCaiChiEnough = false;
        //该奖品是否可以爆奖按照人次.
        bool isDaiJinQuanBaoJiang = false;
        //按照时间计算是否可以放奖.
        bool isCanFangJiangByTime = false;
        //人数是否足够.
        //bool isEnoughPlayerNum = false;
        //海底捞菜品券游戏.
        if (SSHaiDiLaoBaoJiang.GetInstance() != null)
        {
            //奖品2-战车01奖品.
            isHaveJiBao = SSHaiDiLaoBaoJiang.GetInstance().GetIsHaveJiBaoNpc(SSCaiPiaoDataManage.GameCaiPiaoData.DaiJinQuanState.ZhanCheDaiJinQuan_01);
            //isZhongJiang = GetIsCanZhongJiang(JiangPinState.JiangPin2);
            if (XkPlayerCtrl.GetInstanceFeiJi() != null
                && XkPlayerCtrl.GetInstanceFeiJi().m_SpawnNpcManage != null
                && XkPlayerCtrl.GetInstanceFeiJi().m_SpawnNpcManage.m_CaiPiaoDataManage != null
                && XkPlayerCtrl.GetInstanceFeiJi().m_SpawnNpcManage.m_CaiPiaoDataManage.m_GameCaiPiaoData != null)
            {
                isCaiChiEnough = XkPlayerCtrl.GetInstanceFeiJi().m_SpawnNpcManage.m_CaiPiaoDataManage.m_GameCaiPiaoData.GetIsCaiChiEnough(SSCaiPiaoDataManage.GameCaiPiaoData.DaiJinQuanState.ZhanCheDaiJinQuan_01);
            }

            if (XkGameCtrl.GetInstance().m_CaiPiaoHealthDt != null
                && XkGameCtrl.GetInstance().m_CaiPiaoHealthDt.m_CurrentTotalHealthDt != null)
            {
                if (XkGameCtrl.GetInstance().m_CaiPiaoHealthDt.m_CurrentDaiJinQuanState == SSCaiPiaoDataManage.GameCaiPiaoData.DaiJinQuanState.ZhanCheDaiJinQuan_01)
                {
                    isCanJiBao = XkGameCtrl.GetInstance().m_CaiPiaoHealthDt.m_CurrentTotalHealthDt.IsCanJiBao;
                }
            }

            if (SSHaiDiLaoBaoJiang.GetInstance() != null)
            {
                isDaiJinQuanBaoJiang = SSHaiDiLaoBaoJiang.GetInstance().GetIsCanJiBaoNpc(SSCaiPiaoDataManage.GameCaiPiaoData.DaiJinQuanState.ZhanCheDaiJinQuan_01);
                //isEnoughPlayerNum = SSHaiDiLaoBaoJiang.GetInstance().GetIsEnoughPlayerNum(SSCaiPiaoDataManage.GameCaiPiaoData.DaiJinQuanState.ZhanCheDaiJinQuan_01);
                //if (isEnoughPlayerNum == true)
                //{
                    //人数足够必然放奖.
                    //isZhongJiang = true;
                //}
            }


            //if (isHaveJiBao == false && isZhongJiang == true && isCanJiBao == false && isCaiChiEnough == true && isDaiJinQuanBaoJiang == true)
            if (isHaveJiBao == false && isCanJiBao == false && isCaiChiEnough == true && isDaiJinQuanBaoJiang == true)
            {
                if (XkGameCtrl.GetInstance() != null && XkGameCtrl.GetInstance().m_SSChouJiangDt != null)
                {
                    isCanFangJiangByTime = XkGameCtrl.GetInstance().m_SSChouJiangDt.GetIsCanFangJiangByTime(JiangPinState.JiangPin2);
                }

                if (isCanFangJiangByTime == true)
                {
                    //获得奖品2-战车01奖品.
                    indexJiangPin = GetJiangPinIndexValue(JiangPinState.JiangPin2);

                    //此处添加玩家已经获得该类型奖品的代码.
                    if (SSGameLogoData.m_GameDaiJinQuanMode == SSGameLogoData.GameDaiJinQuanMode.HDL_CaiPinQuan)
                    {
                        if (SSHaiDiLaoBaoJiang.GetInstance() != null)
                        {
                            //设置已经击爆npc的数据信息.
                            SSHaiDiLaoBaoJiang.GetInstance().SetIsHaveJiBaoNpc(true, SSCaiPiaoDataManage.GameCaiPiaoData.DaiJinQuanState.ZhanCheDaiJinQuan_01);
                        }
                    }
                }
            }

            if (indexJiangPin == -1)
            {
                //奖品3-战车02奖品.
                isCanJiBao = false;
                isHaveJiBao = SSHaiDiLaoBaoJiang.GetInstance().GetIsHaveJiBaoNpc(SSCaiPiaoDataManage.GameCaiPiaoData.DaiJinQuanState.ZhanCheDaiJinQuan_02);
                //isZhongJiang = GetIsCanZhongJiang(JiangPinState.JiangPin3);
                if (XkPlayerCtrl.GetInstanceFeiJi() != null
                    && XkPlayerCtrl.GetInstanceFeiJi().m_SpawnNpcManage != null
                    && XkPlayerCtrl.GetInstanceFeiJi().m_SpawnNpcManage.m_CaiPiaoDataManage != null
                    && XkPlayerCtrl.GetInstanceFeiJi().m_SpawnNpcManage.m_CaiPiaoDataManage.m_GameCaiPiaoData != null)
                {
                    isCaiChiEnough = XkPlayerCtrl.GetInstanceFeiJi().m_SpawnNpcManage.m_CaiPiaoDataManage.m_GameCaiPiaoData.GetIsCaiChiEnough(SSCaiPiaoDataManage.GameCaiPiaoData.DaiJinQuanState.ZhanCheDaiJinQuan_02);
                }

                if (XkGameCtrl.GetInstance().m_CaiPiaoHealthDt != null
                    && XkGameCtrl.GetInstance().m_CaiPiaoHealthDt.m_CurrentTotalHealthDt != null)
                {
                    if (XkGameCtrl.GetInstance().m_CaiPiaoHealthDt.m_CurrentDaiJinQuanState == SSCaiPiaoDataManage.GameCaiPiaoData.DaiJinQuanState.ZhanCheDaiJinQuan_02)
                    {
                        isCanJiBao = XkGameCtrl.GetInstance().m_CaiPiaoHealthDt.m_CurrentTotalHealthDt.IsCanJiBao;
                    }
                }

                if (SSHaiDiLaoBaoJiang.GetInstance() != null)
                {
                    isDaiJinQuanBaoJiang = SSHaiDiLaoBaoJiang.GetInstance().GetIsCanJiBaoNpc(SSCaiPiaoDataManage.GameCaiPiaoData.DaiJinQuanState.ZhanCheDaiJinQuan_02);
                    //isEnoughPlayerNum = SSHaiDiLaoBaoJiang.GetInstance().GetIsEnoughPlayerNum(SSCaiPiaoDataManage.GameCaiPiaoData.DaiJinQuanState.ZhanCheDaiJinQuan_02);
                    //if (isEnoughPlayerNum == true)
                    //{
                        //人数足够必然放奖.
                        //isZhongJiang = true;
                    //}
                }
                
                //if (isHaveJiBao == false && isZhongJiang == true && isCanJiBao == false && isCaiChiEnough == true && isDaiJinQuanBaoJiang == true)
                if (isHaveJiBao == false && isCanJiBao == false && isCaiChiEnough == true && isDaiJinQuanBaoJiang == true)
                {
                    if (XkGameCtrl.GetInstance() != null && XkGameCtrl.GetInstance().m_SSChouJiangDt != null)
                    {
                        isCanFangJiangByTime = XkGameCtrl.GetInstance().m_SSChouJiangDt.GetIsCanFangJiangByTime(JiangPinState.JiangPin3);
                    }

                    if (isCanFangJiangByTime == true)
                    {
                        //获得奖品3-战车02奖品.
                        indexJiangPin = GetJiangPinIndexValue(JiangPinState.JiangPin3);

                        //此处添加玩家已经获得该类型奖品的代码.
                        if (SSGameLogoData.m_GameDaiJinQuanMode == SSGameLogoData.GameDaiJinQuanMode.HDL_CaiPinQuan)
                        {
                            if (SSHaiDiLaoBaoJiang.GetInstance() != null)
                            {
                                //设置已经击爆npc的数据信息.
                                SSHaiDiLaoBaoJiang.GetInstance().SetIsHaveJiBaoNpc(true, SSCaiPiaoDataManage.GameCaiPiaoData.DaiJinQuanState.ZhanCheDaiJinQuan_02);
                            }
                        }
                    }
                }
            }

            if (indexJiangPin == -1)
            {
                //奖品4-随机道具奖品.
                isHaveJiBao = SSHaiDiLaoBaoJiang.GetInstance().GetIsHaveJiBaoNpc(SSCaiPiaoDataManage.GameCaiPiaoData.DaiJinQuanState.SuiJiDaoJuDaiJinQuan);
                //isZhongJiang = GetIsCanZhongJiang(JiangPinState.JiangPin4);
                if (XkPlayerCtrl.GetInstanceFeiJi() != null
                    && XkPlayerCtrl.GetInstanceFeiJi().m_SpawnNpcManage != null
                    && XkPlayerCtrl.GetInstanceFeiJi().m_SpawnNpcManage.m_CaiPiaoDataManage != null
                    && XkPlayerCtrl.GetInstanceFeiJi().m_SpawnNpcManage.m_CaiPiaoDataManage.m_GameCaiPiaoData != null)
                {
                    isCaiChiEnough = XkPlayerCtrl.GetInstanceFeiJi().m_SpawnNpcManage.m_CaiPiaoDataManage.m_GameCaiPiaoData.GetIsCaiChiEnough(SSCaiPiaoDataManage.GameCaiPiaoData.DaiJinQuanState.SuiJiDaoJuDaiJinQuan);
                }

                if (SSHaiDiLaoBaoJiang.GetInstance() != null)
                {
                    isDaiJinQuanBaoJiang = SSHaiDiLaoBaoJiang.GetInstance().GetIsCanJiBaoNpc(SSCaiPiaoDataManage.GameCaiPiaoData.DaiJinQuanState.SuiJiDaoJuDaiJinQuan);
                    //isEnoughPlayerNum = SSHaiDiLaoBaoJiang.GetInstance().GetIsEnoughPlayerNum(SSCaiPiaoDataManage.GameCaiPiaoData.DaiJinQuanState.SuiJiDaoJuDaiJinQuan);
                    //if (isEnoughPlayerNum == true)
                    //{
                        //人数足够必然放奖.
                    //    isZhongJiang = true;
                    //}
                }

                //if (isHaveJiBao == false && isZhongJiang == true && isCaiChiEnough == true && isDaiJinQuanBaoJiang == true)
                if (isHaveJiBao == false && isCaiChiEnough == true && isDaiJinQuanBaoJiang == true)
                {
                    if (XkGameCtrl.GetInstance() != null && XkGameCtrl.GetInstance().m_SSChouJiangDt != null)
                    {
                        isCanFangJiangByTime = XkGameCtrl.GetInstance().m_SSChouJiangDt.GetIsCanFangJiangByTime(JiangPinState.JiangPin4);
                    }

                    if (isCanFangJiangByTime == true)
                    {
                        //获得奖品4-随机道具奖品.
                        indexJiangPin = GetJiangPinIndexValue(JiangPinState.JiangPin4);

                        //此处添加玩家已经获得该类型奖品的代码.
                        if (SSGameLogoData.m_GameDaiJinQuanMode == SSGameLogoData.GameDaiJinQuanMode.HDL_CaiPinQuan)
                        {
                            if (SSHaiDiLaoBaoJiang.GetInstance() != null)
                            {
                                //设置已经击爆npc的数据信息.
                                SSHaiDiLaoBaoJiang.GetInstance().SetIsHaveJiBaoNpc(true, SSCaiPiaoDataManage.GameCaiPiaoData.DaiJinQuanState.SuiJiDaoJuDaiJinQuan);
                            }
                        }
                    }
                }
            }
        }

        if (indexJiangPin == -1)
        {
            //再玩一局游戏奖品.
            //isZhongJiang = GetIsCanZhongJiang(JiangPinState.ZaiWanYiJu);
            bool isCanZaiWanYiJu = true;
            if (XkGameCtrl.GetInstance() != null)
            {
                //当前机位是否获得过再玩一局游戏奖品.
                bool isHaveZaiWanYiJu = XkGameCtrl.GetInstance().GetZaiWanYiJuPlayerInfo(m_IndexPlayer);
                if (isHaveZaiWanYiJu == true)
                {
                    //当前机位已经获得过再玩一局游戏奖品,不允许玩家连续获得再玩一局游戏奖品.
                    isCanZaiWanYiJu = false;
                    XkGameCtrl.GetInstance().SetZaiWanYiJuPlayerInfo(m_IndexPlayer, false);
                }
            }

            if (isCanZaiWanYiJu == true)
            {
                //按照人数对再玩一局游戏奖品进行判断是否可以出奖.
                if (XkGameCtrl.GetInstance() != null && XkGameCtrl.GetInstance().m_SSChouJiangDt != null)
                {
                    isCanZaiWanYiJu = XkGameCtrl.GetInstance().m_SSChouJiangDt.GetIsCanOutZaiWanYiJuJiangPin();
                    //isEnoughPlayerNum = XkGameCtrl.GetInstance().m_SSChouJiangDt.GetIsEnoughPlayerNum();
                    //if (isEnoughPlayerNum == true)
                    //{
                        //人数足够必然放奖.
                        //isZhongJiang = true;
                    //}
                }
            }

            if (XKGlobalData.GetInstance().m_SSGameXuMingData != null)
            {
                bool isCanXuMing = XKGlobalData.GetInstance().m_SSGameXuMingData.GetIsCanXuMing(m_IndexPlayer);
                if (isCanXuMing == false)
                {
                    //玩家续命次数超出,不允许发送再玩一局游戏奖品.
                    isCanZaiWanYiJu = false;
                }
            }

            //if (isZhongJiang == true && isCanZaiWanYiJu == true)
            if (isCanZaiWanYiJu == true)
            {
                if (XkGameCtrl.GetInstance() != null && XkGameCtrl.GetInstance().m_SSChouJiangDt != null)
                {
                    isCanFangJiangByTime = XkGameCtrl.GetInstance().m_SSChouJiangDt.GetIsCanFangJiangByTime(JiangPinState.ZaiWanYiJu);
                }

                if (isCanFangJiangByTime == true)
                {
                    //获得再玩一局游戏奖品.
                    indexJiangPin = GetJiangPinIndexValue(JiangPinState.ZaiWanYiJu);
                    //按照人数再玩一局游戏奖品已经爆奖.
                    if (XkGameCtrl.GetInstance() != null && XkGameCtrl.GetInstance().m_SSChouJiangDt != null)
                    {
                        XkGameCtrl.GetInstance().m_SSChouJiangDt.SetIsHaveBaoJiang(true);
                    }
                    
                    //当前机位续命一次.
                    if (XKGlobalData.GetInstance().m_SSGameXuMingData != null)
                    {
                        XKGlobalData.GetInstance().m_SSGameXuMingData.AddXuMingCount(m_IndexPlayer);
                    }
                }
            }
        }

        if (indexJiangPin == -1)
        {
            //谢谢参与.
            indexJiangPin = GetJiangPinIndexValue(JiangPinState.XieXieCanYu);
        }

        //indexJiangPin = GetJiangPinIndexValue(JiangPinState.ZaiWanYiJu); //test.
        //indexJiangPin = GetJiangPinIndexValue(JiangPinState.JiangPin3); //test.
        if (indexJiangPin < 0 || indexJiangPin >= m_ChouJiangDtArray.Length)
        {
            indexJiangPin = 0;
        }
        m_PlayerJiangPin = m_ChouJiangDtArray[indexJiangPin];
        SSDebug.Log("CheckPlayerJiangPinData -> m_PlayerJiangPin ================================== " + m_PlayerJiangPin.ToString());
    }

    /// <summary>
    /// 播放转动音效.
    /// </summary>
    void PlayZhuanDongAudio()
    {
        if (m_ZhuanDongAdioSource != null && m_ZhuanDongAdioSource.clip != null)
        {
            m_ZhuanDongAdioSource.Play();
        }
    }

    /// <summary>
    /// 获取抽奖界面每一步的间隔时间.
    /// </summary>
    float GetChouJiangAnimationStepTime(int maxStep, int curStep, ChouJiangAniStage stage)
    {
        float timeVal = 1f;
        float startTime = 0.1f;
        float endTime = 1f;
        bool isFixedTime = true;
        switch(stage)
        {
            case ChouJiangAniStage.Stage01:
            case ChouJiangAniStage.Stage02:
                {
                    startTime = GetTimeChouJiangAni(stage);
                    int nextStageIndex = (int)stage + 1;
                    endTime = GetTimeChouJiangAni((ChouJiangAniStage)nextStageIndex);
                    break;
                }
            case ChouJiangAniStage.Stage03:
                {
                    //最后阶段不用修改时间.
                    isFixedTime = false;
                    timeVal = GetTimeChouJiangAni(stage);
                    break;
                }
        }

        if (isFixedTime == true)
        {
            float unitTime = (endTime - startTime) / (maxStep + 2);
            timeVal = startTime + (unitTime * curStep);
        }
        //SSDebug.LogWarning("GetChouJiangAnimationStepTime -> maxStep == " + maxStep + ", curStep == " + curStep + ", stage == " + stage + ", timeVal == " + timeVal);
        return timeVal;
    }

    /// <summary>
    /// 播放抽奖动画.
    /// </summary>
    IEnumerator PlayChouJiangAnimation()
    {
        bool isPlayAni = true;
        ChouJiangAniStage chouJiangAniStage = ChouJiangAniStage.Stage01;
        //float timeVal = GetTimeChouJiangAni(chouJiangAniStage);
        int maxStep = GetChouJiangAniMoveStep(chouJiangAniStage);
        int stepVal = 0;
        int stepRecord = 0;
        //SSDebug.Log("PlayChouJiangAnimation -> maxStep ===================== " + maxStep + ", chouJiangAniStage ==================== " + chouJiangAniStage);
        do
        {
            if (stepVal == maxStep && chouJiangAniStage == ChouJiangAniStage.Stage03)
            {
                //抽奖动画已经播放结束.
                isPlayAni = false;
                break;
            }

            int jiangPinGeZi = 8;
            int stepTmp = stepRecord % jiangPinGeZi;
            int indexJiangPinPre = stepTmp > 0 ? (stepTmp - 1) % jiangPinGeZi : (jiangPinGeZi - 1);
            int indexJiangPin = stepTmp % jiangPinGeZi;
            stepRecord++;
            //SSDebug.Log("stepRecord ============= " + stepRecord);
            SetActiveJiangPinFlash(indexJiangPinPre, false);
            SetActiveJiangPinFlash(indexJiangPin, true);
            //音效播放.
            PlayZhuanDongAudio();
            int curStep = (stepVal + 1) >= maxStep ? maxStep + (stepVal + 1 - maxStep) : (stepVal + 1) % maxStep;
            float timeStep = GetChouJiangAnimationStepTime(maxStep, curStep, chouJiangAniStage);
            yield return new WaitForSeconds(timeStep);

            stepVal++;
            if (stepVal > maxStep)
            {
                //当前阶段步数已经走完了.
                chouJiangAniStage = GetNextChouJiangAniStage(chouJiangAniStage);
                if (chouJiangAniStage != ChouJiangAniStage.Null)
                {
                    //初始化下一阶段的数据.
                    //timeVal = GetTimeChouJiangAni(chouJiangAniStage);
                    maxStep = GetChouJiangAniMoveStep(chouJiangAniStage);
                    stepVal = 0;
                    //SSDebug.Log("PlayChouJiangAnimation -> maxStep ===================== " + maxStep + ", chouJiangAniStage ==================== " + chouJiangAniStage);
                }
            }
        }
        while (isPlayAni == true);

        //此处添加展示抽奖结果的代码.
        ShowPlayerChouJiangResult();
    }

    /// <summary>
    /// 设置奖品闪光.
    /// indexJiangPin 奖品索引编号.
    /// </summary>
    void SetActiveJiangPinFlash(int indexJiangPin, bool isActive)
    {
        if (m_ChouJiangDtArray.Length > indexJiangPin)
        {
            for (int i = 0; i < m_ChouJiangDtArray.Length; i++)
            {
                if (m_ChouJiangDtArray[i] != null && m_ChouJiangDtArray[i].jiangPingIndex == indexJiangPin)
                {
                    //SSDebug.LogWarning("SetActiveJiangPinFlash -> indexJiangPin == " + indexJiangPin + ", isActive == " + isActive + ", ChouJiangDt == " + m_ChouJiangDtArray[i].ToString());
                    m_ChouJiangDtArray[i].SetActiveFlashObj(isActive);
                    break;
                }
            }
        }
    }

    /// <summary>
    /// 展示玩家抽奖结果.
    /// </summary>
    void ShowPlayerChouJiangResult()
    {
        if (m_PlayerJiangPin == null)
        {
            SSDebug.LogWarning("ShowPlayerChouJiangResult -> m_PlayerJiangPin was null");
            return;
        }

        SSDebug.Log("ShowPlayerChouJiangResult -> playerJiangPin ==== " + m_PlayerJiangPin.ToString());
        //分发玩家的游戏奖品.
        DistributionPlayerJiangPin();
        SetActivePlayerChouJiangResult(true);
        if (SSUIRoot.GetInstance().m_GameUIManage != null)
        {
            //删除玩家游戏抽奖界面UI.
            SSUIRoot.GetInstance().m_GameUIManage.RemovePlayerChouJiangUI(m_IndexPlayer, m_TimeHiddenChouJiangResult);
        }
    }

    /// <summary>
    /// 播放抽奖结果音效.
    /// </summary>
    void PlayChouJiangResultAudio()
    {
        if (m_ResultAdioSource == null || m_PlayerJiangPin == null)
        {
            return;
        }

        AudioClip audioClip = null;
        switch (m_PlayerJiangPin.jiangPinType)
        {
            case JiangPinState.XieXieCanYu:
                {
                    audioClip = m_XieXieCanYuAudio;
                    break;
                }
            default:
                {
                    audioClip = m_DeJiangAudio;
                    break;
                }
        }

        if (audioClip != null)
        {
            m_ResultAdioSource.clip = audioClip;
            m_ResultAdioSource.Play();
        }
    }

    /// <summary>
    /// 分发玩家的游戏奖品.
    /// </summary>
    void DistributionPlayerJiangPin()
    {
        bool isSendMsg = false;
        SSCaiPiaoDataManage.GameCaiPiaoData.DeCaiState deCaiState = SSCaiPiaoDataManage.GameCaiPiaoData.DeCaiState.ZhanChe;
        SSCaiPiaoDataManage.GameCaiPiaoData.DaiJinQuanState daiJinQaunType = SSCaiPiaoDataManage.GameCaiPiaoData.DaiJinQuanState.ZhanCheDaiJinQuan_01;
        switch (m_PlayerJiangPin.jiangPinType)
        {
            case JiangPinState.XieXieCanYu:
                {
                    //感谢参与.
                    break;
                }
            case JiangPinState.JiangPin2:
                {
                    //奖品2-游戏中战车01奖品.
                    isSendMsg = true;
                    daiJinQaunType = SSCaiPiaoDataManage.GameCaiPiaoData.DaiJinQuanState.ZhanCheDaiJinQuan_01;
                    deCaiState = SSCaiPiaoDataManage.GameCaiPiaoData.DeCaiState.ZhanChe;
                    break;
                }
            case JiangPinState.JiangPin3:
                {
                    //奖品3-游戏中战车02奖品.
                    isSendMsg = true;
                    daiJinQaunType = SSCaiPiaoDataManage.GameCaiPiaoData.DaiJinQuanState.ZhanCheDaiJinQuan_02;
                    deCaiState = SSCaiPiaoDataManage.GameCaiPiaoData.DeCaiState.ZhanChe;
                    break;
                }
            case JiangPinState.JiangPin4:
                {
                    //奖品4-游戏中的随机道具奖品.
                    isSendMsg = true;
                    daiJinQaunType = SSCaiPiaoDataManage.GameCaiPiaoData.DaiJinQuanState.SuiJiDaoJuDaiJinQuan;
                    deCaiState = SSCaiPiaoDataManage.GameCaiPiaoData.DeCaiState.SuiJiDaoJu;
                    break;
                }
            case JiangPinState.ZaiWanYiJu:
                {
                    //免费再玩一局奖品.
                    break;
                }
        }

        if (isSendMsg == true)
        {
            //此处添加发送奖品代金券消息给服务器.
            if (pcvr.GetInstance().m_HongDDGamePadInterface != null)
            {
                int caiPiao = 0;
                if (XkPlayerCtrl.GetInstanceFeiJi() != null
                    && XkPlayerCtrl.GetInstanceFeiJi().m_SpawnNpcManage != null
                    && XkPlayerCtrl.GetInstanceFeiJi().m_SpawnNpcManage.m_CaiPiaoDataManage != null
                    && XkPlayerCtrl.GetInstanceFeiJi().m_SpawnNpcManage.m_CaiPiaoDataManage.m_GameCaiPiaoData != null)
                {
                    caiPiao = XkPlayerCtrl.GetInstanceFeiJi().m_SpawnNpcManage.m_CaiPiaoDataManage.m_GameCaiPiaoData.GetPrintCaiPiaoValueByDeCaiState(deCaiState,
                                  SSCaiPiaoDataManage.SuiJiDaoJuState.BaoXiang, daiJinQaunType);
                }
                pcvr.GetInstance().m_HongDDGamePadInterface.SendPostHddPlayerCouponInfoByChouJiang(m_IndexPlayer, caiPiao, daiJinQaunType);
            }
        }

        if (pcvr.GetInstance().m_HongDDGamePadInterface != null)
        {
            //此时需要对微信玩家进行的游戏时长信息发送给红点点服务器.
            pcvr.GetInstance().m_HongDDGamePadInterface.SetPlayerEndGameTime(m_IndexPlayer);
        }
    }

    bool IsRemoveSelf = false;
    internal void RemoveSelf()
    {
        if (IsRemoveSelf == false)
        {
            IsRemoveSelf = true;
            //此处添加抽奖结果展示之后的逻辑代码.
            if (m_PlayerJiangPin != null)
            {
                switch(m_PlayerJiangPin.jiangPinType)
                {
                    case JiangPinState.XieXieCanYu:
                    case JiangPinState.JiangPin2:
                    case JiangPinState.JiangPin3:
                    case JiangPinState.JiangPin4:
                        {
                            //设置玩家状态信息.
                            XkGameCtrl.SetActivePlayer(m_IndexPlayer, false);
                            //展示游戏倒计时界面.
                            DaoJiShiCtrl daoJiShiCom = DaoJiShiCtrl.GetInstance(m_IndexPlayer);
                            if (daoJiShiCom != null)
                            {
                                daoJiShiCom.StartPlayDaoJiShi();
                            }
                            break;
                        }
                    case JiangPinState.ZaiWanYiJu:
                        {
                            //免费再玩一局奖品.
                            //此处添加让玩家可以再次进行游戏的代码.
                            if (XkGameCtrl.GetInstance() != null)
                            {
                                XkGameCtrl.GetInstance().MakePlayerMianFeiZaiWanYiJu(m_IndexPlayer);
                            }
                            break;
                        }
                }
            }
            Destroy(gameObject);
        }
    }
    
    /// <summary>
    /// 多长时间之后隐藏抽奖结果.
    /// </summary>
    [Range(1f, 30f)]
    public float m_TimeHiddenChouJiangResult = 3f;
    /// <summary>
    /// 抽奖结果UI对象.
    /// </summary>
    public GameObject m_ChouJiangResulteUIObj;
    public string m_XieXieCanYu = "谢谢参与";
    public string m_ZaiWanYiJu = "再玩一局";
    /// <summary>
    /// 抽奖结果.
    /// </summary>
    public UILabel m_ResulteLabel;
    /// <summary>
    /// 设置抽奖结果是否隐藏.
    /// </summary>
    void SetActivePlayerChouJiangResult(bool isActive)
    {
        if (isActive == true)
        {
            DisplayChouJiangResult();
        }

        if (m_ChouJiangResulteUIObj != null)
        {
            m_ChouJiangResulteUIObj.SetActive(isActive);
        }

        if (m_PlayerJiangPin != null && isActive == true && m_ResulteLabel != null)
        {
            //展示中奖信息.
            string info = "";
            switch (m_PlayerJiangPin.jiangPinType)
            {
                case JiangPinState.XieXieCanYu:
                    {
                        //谢谢参与奖品.
                        info = m_XieXieCanYu;
                        break;
                    }
                case JiangPinState.JiangPin2:
                case JiangPinState.JiangPin3:
                case JiangPinState.JiangPin4:
                    {
                        if (m_PlayerJiangPin.jiangPinNameUI != null)
                        {
                            info = m_PlayerJiangPin.jiangPinNameUI.text;
                        }
                        break;
                    }
                case JiangPinState.ZaiWanYiJu:
                    {
                        //免费再玩一局奖品.
                        info = m_ZaiWanYiJu;
                        break;
                    }
            }
            m_ResulteLabel.text = info;
        }
    }

    /// <summary>
    /// 显示抽奖结果UI.
    /// </summary>
    void DisplayChouJiangResult()
    {
        if (m_PlayerJiangPin != null)
        {
            bool isZhongJiang = true;
            if (m_PlayerJiangPin.jiangPinType == JiangPinState.XieXieCanYu)
            {
                isZhongJiang = false;
            }

            if (m_GongXiUIObj != null)
            {
                m_GongXiUIObj.SetActive(isZhongJiang);
            }

            if (m_XieXieCanYuUIObj != null)
            {
                m_XieXieCanYuUIObj.SetActive(!isZhongJiang);
            }

            if (m_PlayerJiangPin.jiangPinType == JiangPinState.XieXieCanYu
                || m_PlayerJiangPin.jiangPinType == JiangPinState.ZaiWanYiJu)
            {
            }
            else
            {
                if (m_ZhaoGongZuoRenYuanUIObj != null)
                {
                    //请找工作人员兑奖.
                    m_ZhaoGongZuoRenYuanUIObj.SetActive(true);
                }
            }
        }
    }

    /// <summary>
    /// 隐藏抽奖展示结果UI.
    /// </summary>
    void HiddeChouJiangZhanShiUI()
    {
        if (m_GongXiUIObj != null)
        {
            m_GongXiUIObj.SetActive(false);
        }

        if (m_XieXieCanYuUIObj != null)
        {
            m_XieXieCanYuUIObj.SetActive(false);
        }

        if (m_ZhaoGongZuoRenYuanUIObj != null)
        {
            m_ZhaoGongZuoRenYuanUIObj.SetActive(false);
        }
    }
    #endregion
}
