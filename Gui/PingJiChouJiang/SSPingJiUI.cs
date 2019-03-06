using UnityEngine;

public class SSPingJiUI : MonoBehaviour
{
    /// <summary>
    /// 逐渐隐藏的时间.
    /// </summary>
    [Range(0f, 10f)]
    public float m_AlphaTime = 0.5f;
    /// <summary>
    /// 多长时间之后创建抽奖UI界面.
    /// </summary>
    [Range(1f, 30f)]
    public float m_TimeCreateChouJiang = 2f;
    /// <summary>
    /// 是否创建了抽奖界面.
    /// </summary>
    bool IsCreateChouJiang = false;
    /// <summary>
    /// 玩家是否可以进入抽奖界面.
    /// </summary>
    bool IsPlayerCanChouJiang = true;
    /// <summary>
    /// 多长时间之后隐藏评级UI界面.
    /// 当玩家获得分数没有达到抽奖分值时该时间起作用,否则该时间不起作用.
    /// </summary>
    [Range(1f, 30f)]
    public float m_TimeHidden = 6f;
    float m_TimeStart = 0f;
    /// <summary>
    /// 是否展示玩家分数滚动效果.
    /// </summary>
    public bool IsDisplayFenShu = false;
    /// <summary>
    /// 玩家游戏得分UI展示.
    /// </summary>
    public SSGameNumUI m_FenShuNumUI;
    /// <summary>
    /// 分数滚动音效.
    /// </summary>
    public AudioSource m_FenShuAinAudio;
    /// <summary>
    /// 瓶机界面玩家头像UI.
    /// </summary>
    public UITexture m_PlayerHeadUI;
    /// <summary>
    /// 玩家分数滚动时长.
    /// </summary>
    public float m_TimeFenShuAni = 3f;
    /// <summary>
    /// 玩家分数滚动开始时间.
    /// </summary>
    float m_TimeFenShuStart = 0f;
    /// <summary>
    /// 玩家分数.
    /// </summary>
    int m_PlayerFenShu = 0;
    /// <summary>
    /// 分数动画是否结束.
    /// </summary>
    bool IsEndFenShuAni = false;
    /// <summary>
    /// 玩家评级UI组件.
    /// </summary>
    public UITexture m_PingJiUI;
    /// <summary>
    /// 评分等级对应的图片资源.
    /// </summary>
    public Texture[] m_PingJiImgArray = new Texture[7];
    /// <summary>
    /// 玩家评级评语UI组件.
    /// </summary>
    public UITexture m_PingJiPingYuUI;
    /// <summary>
    /// 评分等级对应的评语图片资源.
    /// </summary>
    public Texture[] m_PingJiPingYuImgArray = new Texture[7];
    /// <summary>
    /// 距抽奖UI数据.
    /// </summary>
    [System.Serializable]
    public class JuChouJiangData
    {
        /// <summary>
        /// 恭喜进入抽奖.
        /// </summary>
        public GameObject gongXiJinRuChouJiangObj;
        /// <summary>
        /// 距抽奖总集.
        /// </summary>
        public GameObject juChouJiangObj;
        /// <summary>
        /// 距抽奖UI组件.
        /// </summary>
        public UITexture juChouJiangUI;
        /// <summary>
        /// 距抽奖UI图片资源.
        /// </summary>
        public Texture[] juChouJiangImgArray = new Texture[3];
        /// <summary>
        /// 还差分数控制.
        /// </summary>
        public SSGameNumUI fenShuNumUI;
        /// <summary>
        /// 还差分数图集信息.
        /// </summary>
        public UIAtlas[] fenShuAtlasArray = new UIAtlas[3];
        /// <summary>
        /// 头像框UI组件.
        /// </summary>
        public UITexture headKuangUI;
        /// <summary>
        /// 头像框UI图片资源.
        /// </summary>
        public Texture[] headKuangImgArray = new Texture[3];
        /// <summary>
        /// 设置是否显示距抽奖界面.
        /// </summary>
        internal void SetActiveJuChouJiang(PlayerEnum indexPlayer, bool isActive)
        {
            if (isActive == true)
            {
                //玩家分数不够抽奖资格.
                int indexVal = (int)indexPlayer - 1;
                if (juChouJiangUI != null)
                {
                    if (indexVal >= 0 && indexVal < juChouJiangImgArray.Length && juChouJiangImgArray[indexVal] != null)
                    {
                        //给不同玩家设置还差多少分UI.
                        juChouJiangUI.mainTexture = juChouJiangImgArray[indexVal];
                    }
                }

                if (fenShuNumUI != null)
                {
                    if (indexVal >= 0 && indexVal < fenShuAtlasArray.Length && fenShuAtlasArray[indexVal] != null)
                    {
                        for (int i = 0; i < fenShuNumUI.m_UISpriteArray.Length; i++)
                        {
                            if (fenShuNumUI.m_UISpriteArray[i] != null)
                            {
                                //给不同玩家设置还差多少分的数字UI图集.
                                fenShuNumUI.m_UISpriteArray[i].atlas = fenShuAtlasArray[indexVal];
                            }
                        }
                    }
                    
                    //玩家最多差40000分可以获得抽奖.
                    int minChouJiangScore = 40000;
                    if (XkGameCtrl.GetInstance() != null && XkGameCtrl.GetInstance().m_PingJiData != null)
                    {
                        minChouJiangScore = XkGameCtrl.GetInstance().m_PingJiData.GetChouJiangMinScore();
                    }

                    int playerScore = XkGameCtrl.GetPlayerJiFenValue(indexPlayer);
                    //还差多少分数.
                    int haiChaScoreVal = minChouJiangScore - playerScore;
                    if (haiChaScoreVal <= 0)
                    {
                        haiChaScoreVal = 100;
                    }
                    fenShuNumUI.ShowNumUI(haiChaScoreVal);
                }
            }

            if (juChouJiangObj != null)
            {
                //距抽奖还差多少分.
                juChouJiangObj.SetActive(isActive);
            }

            if (gongXiJinRuChouJiangObj != null)
            {
                //恭喜进入抽奖.
                gongXiJinRuChouJiangObj.SetActive(!isActive);
            }

            if (headKuangUI != null)
            {
                int indexVal = (int)indexPlayer - 1;
                if (indexVal >= 0 && indexVal < headKuangImgArray.Length && headKuangImgArray[indexVal] != null)
                {
                    //玩家头像框.
                    headKuangUI.mainTexture = headKuangImgArray[indexVal];
                }
            }
        }
    }
    /// <summary>
    /// 距离抽奖还差多少分数据.
    /// </summary>
    public JuChouJiangData m_JuChouJiangDt;
    PlayerEnum m_IndexPlayer = PlayerEnum.Null;
    SSPingJiData.PingJiLevel m_PlayerPingJiLevel = SSPingJiData.PingJiLevel.D;
    internal void Init(PlayerEnum indexPlayer, int fenShu)
    {
        SetActive(false);
        m_IndexPlayer = indexPlayer;
        //m_TimeFenShuStart = m_TimeStart = Time.time;
        if (m_PingJiUI == null)
        {
            SSDebug.LogWarning("SSPingJiUI::Init -> m_PingJiUI was null");
            return;
        }

        IsCreateChouJiang = false;
        m_PlayerPingJiLevel = SSPingJiData.PingJiLevel.D;
        if (XkGameCtrl.GetInstance().m_PingJiData != null)
        {
            m_PlayerPingJiLevel = XkGameCtrl.GetInstance().m_PingJiData.GetPlayerPingJiLevel(fenShu);
        }
        else
        {
            SSDebug.LogWarning("XkGameCtrl.GetInstance().m_PingJiData was null");
        }

        //玩家还差多少分可以进行抽奖控制.
        SSPingJiData.PingJiLevel chouJiangPingJi = SSPingJiData.PingJiLevel.A;
        if (XkGameCtrl.GetInstance() != null && XkGameCtrl.GetInstance().m_PingJiData != null)
        {
            chouJiangPingJi = XkGameCtrl.GetInstance().m_PingJiData.m_ChouJiangPingJi;
        }
        //是否可以抽奖.
        bool isCanChouJiang = m_PlayerPingJiLevel < chouJiangPingJi ? false : true;
        if (m_JuChouJiangDt != null)
        {
            m_JuChouJiangDt.SetActiveJuChouJiang(indexPlayer, !isCanChouJiang);
        }

        //玩家得分控制.
        if (fenShu.ToString().Length > m_FenShuNumUI.m_UISpriteArray.Length)
        {
            fenShu = (int)Mathf.Pow(10, fenShu.ToString().Length) - 1;
        }
        m_PlayerFenShu = fenShu;
        m_FenShuAnimationData = new FenShuAnimationData(m_TimeFenShuAni, fenShu);

        int indexVal = (int)m_PlayerPingJiLevel;
        if (indexVal < m_PingJiImgArray.Length && m_PingJiImgArray[indexVal] != null)
        {
            //评级信息.
            m_PingJiUI.mainTexture = m_PingJiImgArray[indexVal];
        }
        else
        {
            SSDebug.LogWarning("indexVal or m_PingJiImgArray was wrong");
        }
        
        if (indexVal < m_PingJiPingYuImgArray.Length && m_PingJiPingYuImgArray[indexVal] != null)
        {
            //评级评语.
            m_PingJiPingYuUI.mainTexture = m_PingJiPingYuImgArray[indexVal];
        }
        else
        {
            SSDebug.LogWarning("indexVal or m_PingJiImgArray was wrong");
        }
        SetPlayerHeadImg();
    }

    /// <summary>
    /// 展示玩家分数.
    /// </summary>
    void ShowPlayerFenShu(int fenShu)
    {
        if (m_FenShuNumUI != null)
        {
            //展示玩家得分.
            m_FenShuNumUI.ShowNumUI(fenShu);
        }
    }

    /// <summary>
    /// 设置分数滚动音效开关.
    /// </summary>
    void SetIsPlayFenShuAinAudio(bool isPlay)
    {
        if (m_FenShuAinAudio != null && m_FenShuAinAudio.clip != null)
        {
            if (isPlay == true)
            {
                m_FenShuAinAudio.Play();
            }
            else
            {
                m_FenShuAinAudio.Stop();
            }
        }
    }

    /// <summary>
    /// 分数动画数据信息.
    /// </summary>
    public class FenShuAnimationData
    {
        /// <summary>
        /// 低位数字播放结束还剩多长时间之后播放下一个高位数字的动画.
        /// </summary>
        public float nextNumTime = 0.1f;
        /// <summary>
        /// 每一位数字需要转动的时间.
        /// </summary>
        internal float timeUnit = 1f;
        public FenShuAnimationData(float timeTotal, int fenShu)
        {
            int fenShuLength = fenShu.ToString().Length;
            timeUnit = timeTotal / fenShuLength;
            fenShuAniArray = new UnitFenShuAniData[fenShuLength];

            int fenShuTmp = fenShu;
            for (int i = 0; i < fenShuLength; i++)
            {
                fenShuAniArray[i] = new UnitFenShuAniData(fenShuTmp % 10);
                fenShuTmp = fenShuTmp / 10;
            }
            m_MaxFenShuLength = fenShuLength;
        }

        /// <summary>
        /// 分数单个数字动画控制列表.
        /// </summary>
        UnitFenShuAniData[] fenShuAniArray;
        /// <summary>
        /// 分数最大位数.
        /// </summary>
        int m_MaxFenShuLength = 1;
        /// <summary>
        /// 分数索引.
        /// </summary>
        int m_IndexFenShuAni = 0;

        /// <summary>
        /// 更新游戏是否切换分数索引信息.
        /// </summary>
        void UpdateChangeFenShuIndex(float dTime)
        {
            if (m_IndexFenShuAni >= m_MaxFenShuLength - 1)
            {
                return;
            }

            float timeVal = (timeUnit * (m_IndexFenShuAni + 1)) - nextNumTime;
            if (dTime >= timeVal)
            {
                //切换一次分数索引信息.
                StopMoveUnitFenShu();
            }
        }

        /// <summary>
        /// 获取分数信息.
        /// </summary>
        internal int GetFenShuValue(float dTime)
        {
            UpdateChangeFenShuIndex(dTime);

            int fenShu = 0;
            int length = m_IndexFenShuAni + 1;
            if (length > m_MaxFenShuLength)
            {
                length = m_MaxFenShuLength;
            }

            for (int i = 0; i < length; i++)
            {
                fenShu += GetFenShuUnitValue(i) * (int)Mathf.Pow(10, i);
            }
            return fenShu;
        }

        /// <summary>
        /// 获取每一位分数的数值.
        /// </summary>
        int GetFenShuUnitValue(int indexVal)
        {
            int fenShu = 0;
            if (indexVal >= 0 || indexVal < fenShuAniArray.Length)
            {
                fenShu = fenShuAniArray[indexVal].GetFenShuCount();
            }
            return fenShu;
        }

        /// <summary>
        /// 停止某一位分数的动画.
        /// 某一位数值不再滚动.
        /// </summary>
        void StopMoveUnitFenShu()
        {
            if (m_IndexFenShuAni >= 0 || m_IndexFenShuAni < fenShuAniArray.Length)
            {
                //SSDebug.LogWarning("StopMoveUnitFenShu -> m_IndexFenShuAni ================ " + m_IndexFenShuAni);
                fenShuAniArray[m_IndexFenShuAni].SetIsStop(true);
                //切换一次分数索引信息.
                m_IndexFenShuAni++;
            }
        }
    }
    /// <summary>
    /// 评级分数动画控制组件.
    /// </summary>
    FenShuAnimationData m_FenShuAnimationData;

    /// <summary>
    /// 分数单个数字动画控制.
    /// </summary>
    public class UnitFenShuAniData
    {
        /// <summary>
        /// 真实分数数字.
        /// </summary>
        int realFenShu = 0;
        /// <summary>
        /// 是否停止变化.
        /// </summary>
        bool isStop = false;
        internal void SetIsStop(bool isStop)
        {
            this.isStop = isStop;
        }

        public UnitFenShuAniData(int realFenShu)
        {
            this.realFenShu = realFenShu;
        }

        int fenShuCount = 0;
        internal int GetFenShuCount()
        {
            if (isStop == true)
            {
                return realFenShu;
            }

            int fenShu = fenShuCount % 10;
            fenShuCount++;
            return fenShu;
        }
    }

    /// <summary>
    /// 更新数字滚动.
    /// </summary>
    void UpdatePlayerFenAni()
    {
        if (IsEndFenShuAni == false)
        {
            float dTimeVal = Time.time - m_TimeFenShuStart;
            if (dTimeVal < m_TimeFenShuAni)
            {
                if (m_FenShuNumUI != null && m_FenShuNumUI.m_UISpriteArray.Length > 0)
                {
                    //int length = m_PlayerFenShu.ToString().Length;
                    //int randVal = 0;
                    //for (int i = 0; i < length; i++)
                    //{
                    //    randVal += (int)Mathf.Pow(10, i) * Random.Range(2, 9);
                    //}

                    ////SSDebug.LogWarning("UpdatePlayerFenAni -> randVal ========== " + randVal);
                    //ShowPlayerFenShu(randVal);

                    int fenShu = 0;
                    if (m_FenShuAnimationData != null)
                    {
                        fenShu = m_FenShuAnimationData.GetFenShuValue(dTimeVal);
                    }
                    //SSDebug.LogWarning("UpdatePlayerFenAni -> fenShu ========== " + fenShu);
                    ShowPlayerFenShu(fenShu);
                }
            }
            else
            {
                //动画播放结束.
                IsEndFenShuAni = true;
                ShowPlayerFenShu(m_PlayerFenShu);
                SetIsPlayFenShuAinAudio(false);
            }
        }
    }

    internal void SetActive(bool isActive)
    {
        gameObject.SetActive(isActive);
        if (isActive == true)
        {
            m_TimeFenShuStart = m_TimeStart = Time.time;
        }
    }

    bool IsRemoveSelf = false;
    internal void RemoveSelf()
    {
        if (IsRemoveSelf == false)
        {
            IsRemoveSelf = true;
            if (XkGameCtrl.GetInstance() != null)
            {
                //评级结束之后重置玩家分数信息.
                XkGameCtrl.GetInstance().ResetPlayerInfo(m_IndexPlayer);
            }
            Destroy(gameObject);
        }
    }

    private void FixedUpdate()
    {
        if (IsEndFenShuAni == false && IsDisplayFenShu == true)
        {
            UpdatePlayerFenAni();
        }
    }

    /// <summary>
    /// 是否开始淡化评级界面.
    /// </summary>
    bool IsStartDanHuaPingJi = false;
    float m_TimeDanHuaPingJi = 0f;
    /// <summary>
    /// 开始淡化隐藏评级界面.
    /// </summary>
    void StartDanHuaHiddenPingJiPanel()
    {
        if (IsStartDanHuaPingJi == true)
        {
            return;
        }
        IsStartDanHuaPingJi = true;
        m_TimeDanHuaPingJi = Time.time;
        UITexture uiTexture = gameObject.AddComponent<UITexture>();
        uiTexture.alpha = 1f;
        TweenAlpha tweenAlpha = gameObject.AddComponent<TweenAlpha>();
        tweenAlpha.from = 1f;
        tweenAlpha.to = 0f;
        tweenAlpha.duration = m_AlphaTime;
    }

    /// <summary>
    /// 检测淡化评级界面是否结束.
    /// </summary>
    void UpdateDanHuaPingJi()
    {
        if (IsStartDanHuaPingJi == false)
        {
            return;
        }

        if (Time.time - m_TimeDanHuaPingJi >= m_AlphaTime)
        {
            SetActive(false);
        }
    }

    void Update()
    {
        UpdateDanHuaPingJi();
        if (Time.time - m_TimeStart >= m_TimeCreateChouJiang && IsCreateChouJiang == false)
        {
            IsCreateChouJiang = true;
            //创建玩家抽奖界面.
            if (SSUIRoot.GetInstance().m_GameUIManage != null)
            {
                SSPingJiData.PingJiLevel chouJiangPingJi = SSPingJiData.PingJiLevel.A;
                if (XkGameCtrl.GetInstance() != null && XkGameCtrl.GetInstance().m_PingJiData != null)
                {
                    chouJiangPingJi = XkGameCtrl.GetInstance().m_PingJiData.m_ChouJiangPingJi;
                }

                //是否可以抽奖.
                IsPlayerCanChouJiang = m_PlayerPingJiLevel < chouJiangPingJi ? false : true;
                if (IsPlayerCanChouJiang == true)
                {
                    //淡化隐藏评级界面.
                    StartDanHuaHiddenPingJiPanel();
                    //允许抽奖时才可以展示抽奖界面.
                    SSUIRoot.GetInstance().m_GameUIManage.CreatPlayerChouJiangUI(m_IndexPlayer, IsPlayerCanChouJiang);
                }
            }
        }

        if (Time.time - m_TimeStart >= m_TimeHidden && IsRemoveSelf == false && IsPlayerCanChouJiang == false)
        {
            //玩家分数不足,无法进行抽奖.
            SSPingJiData.PingJiLevel chouJiangPingJi = SSPingJiData.PingJiLevel.A;
            if (XkGameCtrl.GetInstance() != null && XkGameCtrl.GetInstance().m_PingJiData != null)
            {
                chouJiangPingJi = XkGameCtrl.GetInstance().m_PingJiData.m_ChouJiangPingJi;
            }

            if (m_PlayerPingJiLevel < chouJiangPingJi)
            {
                //玩家分数不足,无法进行抽奖.
                //玩家币值是否足够.
                bool isPlayerCoinEnough = XKGlobalData.GetPlayerCoinIsEnough(m_IndexPlayer);
                if (isPlayerCoinEnough == true)
                {
                    //玩家币值充足.
                    bool isCanXuMing = true;
                    if (XKGlobalData.GetInstance().m_SSGameXuMingData != null)
                    {
                        //当前机位是否可以续命.
                        isCanXuMing = XKGlobalData.GetInstance().m_SSGameXuMingData.GetIsCanXuMing(m_IndexPlayer);
                    }

                    if (isCanXuMing == true)
                    {
                        //玩家可以续命.
                        if (XkGameCtrl.GetIsActivePlayer(m_IndexPlayer) == true)
                        {
                            //玩家首次GG之后,没有设置信息.
                            //设置玩家状态信息.
                            XkGameCtrl.SetActivePlayer(m_IndexPlayer, false);
                        }

                        //是否可以继续进行免费游戏.
                        bool isCanMianFeiPlayGame = false;
                        if (XKGlobalData.GetInstance() != null)
                        {
                            isCanMianFeiPlayGame = XKGlobalData.GetInstance().GetIsCanMianFeiPlayGame(m_IndexPlayer);
                        }

                        if (isCanMianFeiPlayGame == false)
                        {
                            //玩家不可以继续进行免费游戏.
                            //玩家币值充足,需要对微信用户进行扣费.
                            if (pcvr.GetInstance().m_HongDDGamePadInterface != null)
                            {
                                //此时需要对微信付费玩家进行红点点账户扣费.
                                pcvr.GetInstance().m_HongDDGamePadInterface.OnNeedSubPlayerMoney(m_IndexPlayer);
                            }

                            //玩家付费激活游戏.
                            if (pcvr.GetInstance() != null && pcvr.GetInstance().m_HongDDGamePadInterface != null)
                            {
                                //发送玩家付费激活游戏的登录信息给服务器.
                                pcvr.GetInstance().m_HongDDGamePadInterface.SendPlayerFuFeiActiveGameInfoToServer(m_IndexPlayer);
                            }
                        }
                        else
                        {
                            //玩家可以继续进行免费游戏.
                            if (pcvr.GetInstance() != null && pcvr.GetInstance().m_HongDDGamePadInterface != null)
                            {
                                //发送玩家首次免费游戏登录信息给服务器.
                                pcvr.GetInstance().m_HongDDGamePadInterface.SendPlayerShouCiMianFeiInfoToServer(m_IndexPlayer);
                            }
                            //减少玩家免费次数.
                            XKGlobalData.GetInstance().SubMianFeiNum(m_IndexPlayer);
                        }

                        //当前机位续命一次.
                        if (XKGlobalData.GetInstance().m_SSGameXuMingData != null)
                        {
                            XKGlobalData.GetInstance().m_SSGameXuMingData.AddXuMingCount(m_IndexPlayer);
                        }
                    }

                    if (pcvr.GetInstance().m_HongDDGamePadInterface != null)
                    {
                        //此时需要对微信玩家进行的游戏时长信息发送给红点点服务器.
                        pcvr.GetInstance().m_HongDDGamePadInterface.SetPlayerEndGameTime(m_IndexPlayer);
                    }

                    if (SSUIRoot.GetInstance().m_GameUIManage != null)
                    {
                        //删除玩家评级界面.
                        SSUIRoot.GetInstance().m_GameUIManage.RemovePlayerPingJiUI(m_IndexPlayer);
                    }

                    SSPlayerScoreManage playerScoreManage = SSPlayerScoreManage.GetInstance(m_IndexPlayer);
                    if (playerScoreManage != null)
                    {
                        //当删除玩家评级界面的同时重置距玩家还差多少分数.
                        playerScoreManage.OnRemovePlayerPingJiPanel();
                    }
                }
                else
                {
                    //玩家币值不足.
                    //玩家币值不足,需要对微信用户进行扣费.
                    if (pcvr.GetInstance().m_HongDDGamePadInterface != null)
                    {
                        if (pcvr.GetInstance().m_HongDDGamePadInterface.GetPlayerIsFuFeiActiveGame(m_IndexPlayer) == true)
                        {
                            //付费激活游戏的玩家.
                            //此时需要对微信付费玩家进行红点点账户扣费.
                            pcvr.GetInstance().m_HongDDGamePadInterface.OnNeedSubPlayerMoney(m_IndexPlayer);
                        }
                    }

                    if (pcvr.GetInstance().m_HongDDGamePadInterface != null)
                    {
                        //此时需要对微信玩家进行的游戏时长信息发送给红点点服务器.
                        pcvr.GetInstance().m_HongDDGamePadInterface.SetPlayerEndGameTime(m_IndexPlayer);
                    }

                    //设置玩家状态信息.
                    XkGameCtrl.SetActivePlayer(m_IndexPlayer, false);
                    //玩家评级过低,显示倒计时界面.
                    DaoJiShiCtrl daoJiShiCom = DaoJiShiCtrl.GetInstance(m_IndexPlayer);
                    if (daoJiShiCom != null)
                    {
                        daoJiShiCom.StartPlayDaoJiShi();
                    }

                    if (SSUIRoot.GetInstance().m_GameUIManage != null)
                    {
                        //删除玩家评级界面.
                        SSUIRoot.GetInstance().m_GameUIManage.RemovePlayerPingJiUI(m_IndexPlayer);
                    }

                    SSPlayerScoreManage playerScoreManage = SSPlayerScoreManage.GetInstance(m_IndexPlayer);
                    if (playerScoreManage != null)
                    {
                        //当删除玩家评级界面的同时重置距玩家还差多少分数.
                        playerScoreManage.OnRemovePlayerPingJiPanel();
                    }
                }

                //if (SSUIRoot.GetInstance().m_GameUIManage != null)
                //{
                //    //删除玩家游戏抽奖界面UI.
                //    SSUIRoot.GetInstance().m_GameUIManage.RemovePlayerChouJiangUI(m_IndexPlayer, 0f);
                //}
            }
            //else
            //{
            //    //玩家评级达到抽奖水平,显示抽奖界面.
            //    if (SSUIRoot.GetInstance().m_GameUIManage != null)
            //    {
            //        SSUIRoot.GetInstance().m_GameUIManage.CreatPlayerChouJiangUI(m_IndexPlayer);
            //    }
            //}

            //if (SSUIRoot.GetInstance().m_GameUIManage != null)
            //{
            //    //删除玩家评级界面.
            //    SSUIRoot.GetInstance().m_GameUIManage.RemovePlayerPingJiUI(m_IndexPlayer);
            //}
        }
    }

    /// <summary>
    /// 设置玩家微信头像.
    /// </summary>
    void SetPlayerHeadImg()
    {
        if (m_PlayerHeadUI != null)
        {
            Texture headImg = null;
            if (XueKuangCtrl.GetInstance(m_IndexPlayer) != null && XueKuangCtrl.GetInstance(m_IndexPlayer).m_WeiXinHead != null)
            {
                headImg = XueKuangCtrl.GetInstance(m_IndexPlayer).m_WeiXinHead.mainTexture;
            }

            if (headImg != null)
            {
                m_PlayerHeadUI.mainTexture = headImg;
            }
        }
    }
}
