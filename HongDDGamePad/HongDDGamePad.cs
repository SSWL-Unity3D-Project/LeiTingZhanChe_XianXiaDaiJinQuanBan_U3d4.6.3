#define TEST_HDD_ZHIFU_PLATFORM
//#define USE_WX_GAME_PAD_ACTIVE_PLAYER
using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace Assets.XKGame.Script.HongDDGamePad
{
    /// <summary>
    /// 红点点微信虚拟游戏手柄控制单元.
    /// </summary>
    public class HongDDGamePad : MonoBehaviour
    {
        #region 红点点微信虚拟游戏手柄控制单元
        /// <summary>
        /// 是否为红点点微信手柄操作模式.
        /// </summary>
        private bool IsHongDDShouBing
        {
            get { return pcvr.IsHongDDShouBing; }
        }
        /// <summary>
        /// 微信头像url.
        /// </summary>
        internal string[] m_PlayerHeadUrl = new string[4];
        /// <summary>
        /// 微信小程序虚拟手柄.
        /// </summary>
        internal SSBoxPostNet.WeiXinShouBingEnum m_WXShouBingType
        {
            get
            {
                SSBoxPostNet.WeiXinShouBingEnum type = SSBoxPostNet.WeiXinShouBingEnum.XiaoChengXu;
                /*if (m_TVGamePayType == SSGamePayUICtrl.TVGamePayState.DianXinApk)
                {
                    type = SSBoxPostNet.WeiXinShouBingEnum.XiaoChengXu;
                }*/
                return type;
            }
        }

        public enum WeiXinShouBingEnum
        {
            /// <summary>
            /// 线上电视版本.
            /// 应用于家庭客厅的电视游戏.
            /// 支持遥控器操作.
            /// </summary>
            XianShangTVBan = 0,
            /// <summary>
            /// 线下电视版本.
            /// 应用于商场、影院、超市或机场等公共场所.
            /// 不支持电视遥控器操作.
            /// </summary>
            XianXiaTVBan = 1,
        }
        /// <summary>
        /// 微信手柄枚举类型.
        /// </summary>
        WeiXinShouBingEnum m_WeiXinShouBinType = WeiXinShouBingEnum.XianXiaTVBan;
        /// <summary>
        /// 二维码生成脚本.
        /// </summary>
        internal BarcodeCam m_BarcodeCam;

        /// <summary>
        /// BoxPostNet控制组件.
        /// </summary>
        internal SSBoxPostNet m_SSBoxPostNet;

        /// <summary>
        /// 红点点微信虚拟游戏手柄微信支付控制组件.
        /// </summary>
        internal HongDDGamePadWXPay m_HongDDGamePadWXPay = null;
        
        /// <summary>
        /// 初始化.
        /// </summary>
        internal void Init()
        {
            InitInfo();
        }

        void InitInfo()
        {
            for (int i = 0; i < m_GmWXLoginDt.Length; i++)
            {
                m_GmWXLoginDt[i] = new GameWeiXinLoginData();
            }

            if (IsHongDDShouBing == true)
            {
                //创建游戏盒子登陆控制组件.
                GameObject websocketObj = SpawnWebSocketBox(transform);
                m_SSBoxPostNet = websocketObj.GetComponent<SSBoxPostNet>();
                m_SSBoxPostNet.Init();
                m_SSBoxPostNet.OnReceivedWXPlayerHddPayData += OnReceivedWXPlayerHddPayData;
                m_BarcodeCam = gameObject.AddComponent<BarcodeCam>();
            }
            
            if (IsHongDDShouBing == true)
            {
                if (m_WeiXinShouBinType == WeiXinShouBingEnum.XianXiaTVBan)
                {
                    //线下电视版本.
                    m_HongDDGamePadWXPay = new HongDDGamePadWXPay();
                    if (m_HongDDGamePadWXPay != null)
                    {
                        //向服务器请求游戏的后台配置信息.
                        m_HongDDGamePadWXPay.CToS_RequestGameConfigInfo("");
                    }
                }
            }

            if (IsHongDDShouBing == true)
            {
                if (m_SSBoxPostNet != null)
                {
                    WebSocketSimpet webSocketSimpet = m_SSBoxPostNet.m_WebSocketSimpet;
                    if (webSocketSimpet != null)
                    {
                        Debug.Log("Unity:" + "add webSocketSimpet event...");
                        webSocketSimpet.OnEventPlayerLoginBox += OnEventPlayerLoginBox;
                        webSocketSimpet.OnEventPlayerExitBox += OnEventPlayerExitBox;
                        webSocketSimpet.OnEventDirectionAngle += OnEventDirectionAngle;
                        webSocketSimpet.OnEventActionOperation += OnEventActionOperation;
                    }
                    else
                    {
                        Debug.Log("Unity:" + "webSocketSimpet is null!");
                    }
                }
            }
        }
        
        /// <summary>
        /// 产生Websocket预制.
        /// </summary>
        internal GameObject SpawnWebSocketBox(Transform tr)
        {
            Debug.Log("Unity:" + "SpawnWebSocketBox...");
            GameObject obj = null;
            GameObject gmDataPrefab = (GameObject)Resources.Load("Prefabs/Server/_WebSocketBox");
            if (gmDataPrefab != null)
            {
                obj = (GameObject)Instantiate(gmDataPrefab);
                obj.transform.SetParent(tr);
            }
            else
            {
                Debug.LogWarning("Unity: SpawnWebSocketBox -> gmDataPrefab was null!");
            }
            return obj;
        }

        /// <summary>
        /// 添加遥控器按键信息响应事件.
        /// </summary>
        internal void AddTVYaoKongBtEvent()
        {
            if (m_WeiXinShouBinType != WeiXinShouBingEnum.XianShangTVBan)
            {
                //不是线上家庭电视游戏版本.
                return;
            }

            if (pcvr.bIsHardWare == true)
            {
                //硬件版本.
                return;
            }
            //InputEventCtrl.GetInstance().ClickTVYaoKongEnterBtEvent += ClickTVYaoKongEnterBtEvent;
            //InputEventCtrl.GetInstance().ClickTVYaoKongUpBtEvent += ClickTVYaoKongUpBtEvent;
            //InputEventCtrl.GetInstance().ClickTVYaoKongDownBtEvent += ClickTVYaoKongDownBtEvent;
            //InputEventCtrl.GetInstance().ClickTVYaoKongLeftBtEvent += ClickTVYaoKongLeftBtEvent;
            //InputEventCtrl.GetInstance().ClickTVYaoKongRightBtEvent += ClickTVYaoKongRightBtEvent;
        }

        private void ClickTVYaoKongUpBtEvent(pcvr.ButtonState val)
        {
            if (m_GmTVLoginDt != null)
            {
                int index = m_GmTVLoginDt.Index;
                InputEventCtrl.GetInstance().OnClickFangXiangUBt(index, val);

                if (val == pcvr.ButtonState.DOWN)
                {
                    InputEventCtrl.GetInstance().OnClickFireBt(index, pcvr.ButtonState.DOWN);
                    InputEventCtrl.GetInstance().OnClickDaoDanBt(index, pcvr.ButtonState.DOWN);
                }
            }
        }

        private void ClickTVYaoKongDownBtEvent(pcvr.ButtonState val)
        {
            if (m_GmTVLoginDt != null)
            {
                int index = m_GmTVLoginDt.Index;
                InputEventCtrl.GetInstance().OnClickFangXiangDBt(index, val);

                if (val == pcvr.ButtonState.DOWN)
                {
                    InputEventCtrl.GetInstance().OnClickFireBt(index, pcvr.ButtonState.DOWN);
                    InputEventCtrl.GetInstance().OnClickDaoDanBt(index, pcvr.ButtonState.DOWN);
                }
            }
        }

        private void ClickTVYaoKongLeftBtEvent(pcvr.ButtonState val)
        {
            if (m_GmTVLoginDt != null)
            {
                int index = m_GmTVLoginDt.Index;
                InputEventCtrl.GetInstance().OnClickFangXiangLBt(index, val);

                if (val == pcvr.ButtonState.DOWN)
                {
                    InputEventCtrl.GetInstance().OnClickFireBt(index, pcvr.ButtonState.DOWN);
                    InputEventCtrl.GetInstance().OnClickDaoDanBt(index, pcvr.ButtonState.DOWN);
                }
            }
        }

        private void ClickTVYaoKongRightBtEvent(pcvr.ButtonState val)
        {
            if (m_GmTVLoginDt != null)
            {
                int index = m_GmTVLoginDt.Index;
                InputEventCtrl.GetInstance().OnClickFangXiangRBt(index, val);

                if (val == pcvr.ButtonState.DOWN)
                {
                    InputEventCtrl.GetInstance().OnClickFireBt(index, pcvr.ButtonState.DOWN);
                    InputEventCtrl.GetInstance().OnClickDaoDanBt(index, pcvr.ButtonState.DOWN);
                }
            }
        }

        [Serializable]
        public class GameWeiXinLoginData
        {
            /// <summary>
            /// 是否登陆微信手柄.
            /// </summary>
            public bool IsLoginWX = false;
            /// <summary>
            /// 是否激活游戏.
            /// </summary>
            public bool IsActiveGame = false;
            /// <summary>
            /// 游戏外设操作设备.
            /// </summary>
            public GamePadType m_GamePadType = GamePadType.Null;
            internal void Reset()
            {
                IsLoginWX = false;
                IsActiveGame = false;
                m_GamePadType = GamePadType.Null;
            }
        }
        /// <summary>
        /// 游戏微信手柄登陆信息.
        /// </summary>
        public GameWeiXinLoginData[] m_GmWXLoginDt = new GameWeiXinLoginData[3];

        public enum GamePadType
        {
            Null,
            /// <summary>
            /// 电视遥控器.
            /// </summary>
            TV_YaoKongQi,
            /// <summary>
            /// 微信虚拟手柄.
            /// </summary>
            WeiXin_ShouBing,
        }

        /// <summary>
        /// 电视遥控器激活玩家时的数据信息.
        /// </summary>
        [System.Serializable]
        public class TVYaoKongPlayerData
        {
            /// <summary>
            /// 最后一个血值耗尽的玩家索引信息.
            /// </summary>
            public int Index = -1;
            /// <summary>
            /// 玩家Id.
            /// </summary>
            public int m_UserId = 0;
            /// <summary>
            /// 游戏外设操作设备.
            /// </summary>
            public GamePadType m_GamePadType = GamePadType.Null;
            public TVYaoKongPlayerData(int indexVal, GamePadType pad, int userId)
            {
                Index = indexVal;
                m_GamePadType = pad;
                m_UserId = userId;
            }

            public void Reset()
            {
                Index = -1;
                m_GamePadType = GamePadType.Null;
            }
        }
        /// <summary>
        /// 遥控器确定键激活玩家数据列表.
        /// 按照谁最后挂掉优先激活谁的顺序排列.
        /// </summary>
        List<TVYaoKongPlayerData> m_TVYaoKongPlayerDt = new List<TVYaoKongPlayerData>();
        /// <summary>
        /// 游戏电视遥控器登陆的玩家信息.
        /// </summary>
        private TVYaoKongPlayerData m_GmTVLoginDt;

        void ClickTVYaoKongEnterBtEvent(pcvr.ButtonState val)
        {
            if (val == pcvr.ButtonState.UP)
            {
                Debug.Log("Unity: pcvr -> ClickTVYaoKongEnterBtEvent...");
                int count = m_TVYaoKongPlayerDt.Count;
                if (count > 0)
                {
                    TVYaoKongPlayerData playerDt = m_TVYaoKongPlayerDt[count - 1];
                    int indexPlayer = playerDt.Index;
                    //清理最后一个血值耗尽的玩家信息.
                    m_TVYaoKongPlayerDt.RemoveAt(count - 1);

                    if (indexPlayer > -1 && indexPlayer < 4)
                    {
                        switch (playerDt.m_GamePadType)
                        {
                            case GamePadType.TV_YaoKongQi:
                                {
                                    if (m_GmWXLoginDt[indexPlayer].IsLoginWX)
                                    {
                                        if (!m_GmWXLoginDt[indexPlayer].IsActiveGame)
                                        {
                                            Debug.Log("Unity: click TVYaoKong EnterBt -> active TV_YaoKongQi " + indexPlayer + " player!");
                                            m_GmWXLoginDt[indexPlayer].IsActiveGame = true;
                                            InputEventCtrl.GetInstance().OnClickGameStartBt(indexPlayer);
                                        }
                                    }
                                    break;
                                }
                            case GamePadType.WeiXin_ShouBing:
                                {
                                    if (m_GmWXLoginDt[indexPlayer].IsLoginWX)
                                    {
                                        if (!m_GmWXLoginDt[indexPlayer].IsActiveGame)
                                        {
                                            Debug.Log("Unity: click TVYaoKong EnterBt -> active " + indexPlayer + " player!");
                                            m_GmWXLoginDt[indexPlayer].IsActiveGame = true;
                                            InputEventCtrl.GetInstance().OnClickGameStartBt(indexPlayer);
                                        }
                                    }
                                    break;
                                }
                        }
                    }
                }
                else
                {
                    if (SSUIRoot.GetInstance().m_ExitUICom == null)
                    {
                        //没有退出游戏界面出现时,可以用遥控器进入游戏.
                        if (m_GmTVLoginDt == null)
                        {
                            //遥控器激活玩家.
                            int index = GetActivePlayerIndex();
                            if (index < m_IndexPlayerActiveGameState.Length && index > -1)
                            {
                                Debug.Log("Unity: click TVYaoKong EnterBt -> --> active TV_YaoKongQi " + index + " player!");
                                m_GmWXLoginDt[index].IsLoginWX = true;
                                m_GmWXLoginDt[index].IsActiveGame = true;
                                m_GmWXLoginDt[index].m_GamePadType = GamePadType.TV_YaoKongQi;
                                m_PlayerHeadUrl[index] = "";
                                m_GmTVLoginDt = new TVYaoKongPlayerData(index, GamePadType.TV_YaoKongQi, 0);
                                InputEventCtrl.GetInstance().OnClickGameStartBt(index);
                            }
                        }
                    }
                }
            }
        }

        [Serializable]
        public class GamePlayerData
        {
            /// <summary>
            /// 玩家在游戏中的索引.
            /// </summary>
            public int Index = -1;
            /// <summary>
            /// 是否退出微信.
            /// </summary>
            public bool IsExitWeiXin = false;
            /// <summary>
            /// 是否为免费体验游戏的玩家.
            /// </summary>
            public bool IsMianFeiTiYanPlayer = false;
            /// <summary>
            /// 是否在获取微信玩家的红点点游戏账户数据.
            /// </summary>
            public bool IsGetWXPlayerHddPayData = false;
            /// <summary>
            /// 玩家的微信数据信息.
            /// </summary>
            public WebSocketSimpet.PlayerWeiXinData m_PlayerWeiXinData;
        }
        public List<GamePlayerData> m_GamePlayerData = new List<GamePlayerData>();

        /// <summary>
        /// 查找玩家微信游戏数据.
        /// </summary>
        GamePlayerData FindGamePlayerData(int userId)
        {
            GamePlayerData playerDt = m_GamePlayerData.Find((dt) => {
                return dt.m_PlayerWeiXinData.userId.Equals(userId);
            });
            return playerDt;
        }

        /// <summary>
        /// 查找玩家微信游戏数据.
        /// </summary>
        internal GamePlayerData FindGamePlayerData(PlayerEnum indexPlayer)
        {
            int indexVal = (int)indexPlayer - 1;
            GamePlayerData playerDt = m_GamePlayerData.Find((dt) => {
                return dt.Index.Equals(indexVal);
            });
            return playerDt;
        }

        /// <summary>
        /// 玩家激活游戏状态.
        /// </summary>
        enum PlayerActiveState
        {
            WeiJiHuo = 0, //未激活.
            JiHuo = 1, //激活.
        }

        /// <summary>
        /// 红点点微信手柄发射按键.
        /// </summary>
        enum PlayerShouBingFireBt
        {
            fireXDown,
            fireYDown,
            fireADown,
            fireBDown,
            fireXUp,
            fireYUp,
            fireAUp,
            fireBUp,
        }

        /// <summary>
        /// 红点点微信手柄方向.
        /// </summary>
        enum PlayerShouBingDir
        {
            up = 0, //没有操作方向盘(手指离开摇杆).
            DirLeft = 1,
            DirLeftDown = 2,
            DirDown = 3,
            DirRightDown = 4,
            DirRight = 5,
            DirRightUp = 6,
            DirUp = 7,
            DirLeftUp = 8,
        }

        /// <summary>
        /// 4个玩家激活游戏的列表状态(0 未激活, 1 激活).
        /// </summary>
        public byte[] m_IndexPlayerActiveGameState = new byte[3];
        internal void SetIndexPlayerActiveGameState(int index, byte activeState)
        {
            SSDebug.Log("SetIndexPlayerActiveGameState -> index ================= " + index
                + ", activeState =============== " + activeState);
            m_IndexPlayerActiveGameState[index] = activeState;
            if (activeState == (int)PlayerActiveState.WeiJiHuo)
            {
                int userId = 0;
                bool isExitWeiXin = false;
                GamePlayerData playerDt = m_GamePlayerData.Find((dt) => { return dt.Index.Equals(index); });
                if (playerDt != null)
                {
                    if (playerDt.m_PlayerWeiXinData != null)
                    {
                        userId = playerDt.m_PlayerWeiXinData.userId;
                        
                        if (playerDt.IsMianFeiTiYanPlayer == false)
                        {
                            //付费玩家进行扣费.
                            //扣除玩家红点点游戏账户金币.
                            SubWXPlayerHddPayData(userId);
                        }
                        else
                        {
                            //强制将玩家状态修改为付费玩家.
                            playerDt.IsMianFeiTiYanPlayer = false;
                        }
                    }

                    isExitWeiXin = playerDt.IsExitWeiXin;
                    if (playerDt.IsExitWeiXin == true)
                    {
                        //玩家已经退出微信.
                        Debug.Log("Unity:" + "player have exit weiXin! clean the player data...");
                        int coin = XKGlobalData.GetInstance().GetCoinPlayer((PlayerEnum)(playerDt.Index + 1));
                        if (coin < XKGlobalData.GameNeedCoin)
                        {
                            m_GamePlayerData.Remove(playerDt);
                        }
                    }
                    else
                    {
                        int coin = XKGlobalData.GetInstance().GetCoinPlayer((PlayerEnum)(playerDt.Index + 1));
                        if (coin < XKGlobalData.GameNeedCoin)
                        {
                            //玩家血值耗尽应该续费了,找到玩家数据.
                            Debug.Log("Unity:" + "player should buy game coin!");
                            SendWXPadShowTopUpPanel(userId);
                        }
                    }
                }

                m_GmWXLoginDt[index].IsActiveGame = false;
                if (m_GmWXLoginDt[index].IsLoginWX)
                {
                    Debug.Log("Unity:" + "player m_GamePadType ==  " + m_GmWXLoginDt[index].m_GamePadType);
                    switch (m_GmWXLoginDt[index].m_GamePadType)
                    {
                        case GamePadType.WeiXin_ShouBing:
                            {
                                //微信手柄玩家血值耗尽了.
                                if (isExitWeiXin == false)
                                {
                                    //没有退出微信进行添加.
                                    m_TVYaoKongPlayerDt.Add(new TVYaoKongPlayerData(index, GamePadType.WeiXin_ShouBing, userId));
                                }
                                break;
                            }
                        case GamePadType.TV_YaoKongQi:
                            {
                                //电视遥控器玩家血值耗尽了.
                                m_TVYaoKongPlayerDt.Add(new TVYaoKongPlayerData(index, GamePadType.TV_YaoKongQi, 0));
                                if (m_GmTVLoginDt != null)
                                {
                                    //关闭玩家发射子弹的按键消息.
                                    int indexVal = m_GmTVLoginDt.Index;
                                    InputEventCtrl.GetInstance().OnClickDaoDanBt(indexVal, pcvr.ButtonState.UP);
                                    InputEventCtrl.GetInstance().OnClickFireBt(indexVal, pcvr.ButtonState.UP);
                                }
                                break;
                            }
                    }

                    if (isExitWeiXin == true)
                    {
                        m_GmWXLoginDt[playerDt.Index].IsLoginWX = false;
                    }
                }
            }
            else
            {
                if (!IsHongDDShouBing)
                {
                    //软件版本测试用,模拟微信手柄登陆.
                    m_GmWXLoginDt[index].IsLoginWX = true;
                    m_GmWXLoginDt[index].m_GamePadType = GamePadType.Null;
                    Debug.Log("Unity: SetIndexPlayerActiveGameState -> index == " + index);
                }
            }
        }

        /// <summary>
        /// 获取未激活玩家的索引. returnVal == -1 -> 所有玩家都处于激活状态.
        /// </summary>
        int GetActivePlayerIndex()
        {
            int indexPlayer = -1;
            for (int i = 0; i < m_IndexPlayerActiveGameState.Length; i++)
            {
                //Debug.Log("Unity: ActiveGame == " + m_IndexPlayerActiveGameState[i] + ", IsLoginWX == " + m_GmWXLoginDt[i].IsLoginWX);
                if (m_IndexPlayerActiveGameState[i] == 0)
                {
                    if (!m_GmWXLoginDt[i].IsLoginWX)
                    {
                        //未激活且未登陆过微信手柄的玩家索引.
                        indexPlayer = i;
                        break;
                    }
                }
            }
            return indexPlayer;
        }

        /// <summary>
        /// 获取游戏当前是否还有空余机位.
        /// </summary>
        bool GetIsHaveEmptyJiWei()
        {
            int indexPlayer = GetActivePlayerIndex();
            if (indexPlayer > -1 && indexPlayer < m_IndexPlayerActiveGameState.Length)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 点击微信游戏虚拟手柄上的按键事件.
        /// 主要用于玩家血值耗尽后的再次复活功能.
        /// </summary>
        void OnClickWXGamePadBt(pcvr.ButtonState val, int userId)
        {
            if (val == pcvr.ButtonState.DOWN)
            {
                Debug.Log("Unity: pcvr -> OnClickWXGamePadBt...");
                int count = m_TVYaoKongPlayerDt.Count;
                for (int i = 0; i < count; i++)
                {
                    TVYaoKongPlayerData playerDt = m_TVYaoKongPlayerDt[i];
                    if (playerDt != null && playerDt.m_UserId == userId)
                    {
                        int indexPlayer = playerDt.Index;
                        //清理最后一个血值耗尽的玩家信息.
                        m_TVYaoKongPlayerDt.RemoveAt(i);

                        if (indexPlayer > -1 && indexPlayer < 4)
                        {
                            switch (playerDt.m_GamePadType)
                            {
                                case GamePadType.WeiXin_ShouBing:
                                    {
                                        if (m_GmWXLoginDt[indexPlayer].IsLoginWX)
                                        {
                                            if (!m_GmWXLoginDt[indexPlayer].IsActiveGame)
                                            {
                                                Debug.Log("Unity: click WXGamePad EnterBt -> active " + indexPlayer + " player!");
                                                CoinPlayerCtrl playerCoinCom = CoinPlayerCtrl.GetInstance((PlayerEnum)(indexPlayer + 1));
                                                if (playerCoinCom != null)
                                                {
                                                    playerCoinCom.SetActiveMianFeiTiYanUI(false);
                                                }
                                                AddWeiXinGameCoinToPlayer((PlayerEnum)(indexPlayer + 1), 10);
                                                m_GmWXLoginDt[indexPlayer].IsActiveGame = true;
                                                InputEventCtrl.GetInstance().OnClickGameStartBt(indexPlayer);
                                            }
                                        }
                                        break;
                                    }
                            }
                        }
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// 发射按键响应.
        /// </summary>
        private void OnEventActionOperation(string val, int userId)
        {
            //Debug.Log("Unity:"+"pcvr::OnEventActionOperation -> userId " + userId + ", val " + val);
            GamePlayerData playerDt = m_GamePlayerData.Find((dt) => { return dt.m_PlayerWeiXinData.userId.Equals(userId); });
            if (playerDt != null && playerDt.Index > -1 && playerDt.Index < 4)
            {
                //Debug.Log("Unity:"+"OnEventActionOperation -> playerIndex == " + playerDt.Index);
                playerDt.IsExitWeiXin = false;
                if (m_IndexPlayerActiveGameState[playerDt.Index] == (int)PlayerActiveState.JiHuo)
                {
                    //处于激活状态的玩家才可以进行游戏操作.
                    if (val == PlayerShouBingFireBt.fireADown.ToString()
                        || val == PlayerShouBingFireBt.fireXDown.ToString())
                    {
                        InputEventCtrl.GetInstance().OnClickFireBt(playerDt.Index, pcvr.ButtonState.DOWN);
                        //InputEventCtrl.GetInstance().OnClickDaoDanBt(playerDt.Index, pcvr.ButtonState.DOWN);
                    }

                    if (val == PlayerShouBingFireBt.fireAUp.ToString()
                        || val == PlayerShouBingFireBt.fireXUp.ToString())
                    {
                        InputEventCtrl.GetInstance().OnClickFireBt(playerDt.Index, pcvr.ButtonState.UP);
                        //InputEventCtrl.GetInstance().OnClickDaoDanBt(playerDt.Index, pcvr.ButtonState.UP);
                    }

                    if (val == PlayerShouBingFireBt.fireBDown.ToString()
                        || val == PlayerShouBingFireBt.fireYDown.ToString())
                    {
                        //InputEventCtrl.GetInstance().OnClickFireBt(playerDt.Index, pcvr.ButtonState.DOWN);
                        InputEventCtrl.GetInstance().OnClickDaoDanBt(playerDt.Index, pcvr.ButtonState.DOWN);
                    }

                    if (val == PlayerShouBingFireBt.fireBUp.ToString()
                        || val == PlayerShouBingFireBt.fireYUp.ToString())
                    {
                        //InputEventCtrl.GetInstance().OnClickFireBt(playerDt.Index, pcvr.ButtonState.UP);
                        InputEventCtrl.GetInstance().OnClickDaoDanBt(playerDt.Index, pcvr.ButtonState.UP);
                    }
                }
                else
                {
#if USE_WX_GAME_PAD_ACTIVE_PLAYER
                    //test 通过微信游戏手柄的按键消息来激活对应玩家的游戏.
                    //处于没有激活状态的玩家才可以进行游戏操作.
                    if (val == PlayerShouBingFireBt.fireADown.ToString()
                        || val == PlayerShouBingFireBt.fireXDown.ToString())
                    {
                        OnClickWXGamePadBt(pcvr.ButtonState.DOWN, userId);
                        InputEventCtrl.GetInstance().OnClickFireBt(playerDt.Index, pcvr.ButtonState.DOWN);
                        InputEventCtrl.GetInstance().OnClickDaoDanBt(playerDt.Index, pcvr.ButtonState.DOWN);
                    }

                    if (val == PlayerShouBingFireBt.fireBDown.ToString()
                        || val == PlayerShouBingFireBt.fireYDown.ToString())
                    {
                        OnClickWXGamePadBt(pcvr.ButtonState.DOWN, userId);
                        InputEventCtrl.GetInstance().OnClickFireBt(playerDt.Index, pcvr.ButtonState.DOWN);
                        InputEventCtrl.GetInstance().OnClickDaoDanBt(playerDt.Index, pcvr.ButtonState.DOWN);
                    }
                    //test
#else
                    //玩家币值不够,需要拉起微信游戏手柄的充值界面.

                    if (val == PlayerShouBingFireBt.fireADown.ToString()
                        || val == PlayerShouBingFireBt.fireXDown.ToString())
                    {
                        SendWXPadShowTopUpPanel(userId);
                    }

                    if (val == PlayerShouBingFireBt.fireBDown.ToString()
                        || val == PlayerShouBingFireBt.fireYDown.ToString())
                    {
                        SendWXPadShowTopUpPanel(userId);
                    }
#endif
                }
            }
        }

        /// <summary>
        /// 手柄方向信息.
        /// </summary>
        private void OnEventDirectionAngle(string dirValStr, int userId)
        {
            //Debug.Log("Unity:"+"pcvr::OnEventDirectionAngle -> userId " + userId + ", val " + val);
            GamePlayerData playerDt = m_GamePlayerData.Find((dt) => { return dt.m_PlayerWeiXinData.userId.Equals(userId); });
            if (playerDt != null && playerDt.Index > -1 && playerDt.Index < 4)
            {
                //Debug.Log("Unity:"+"OnEventDirectionAngle -> playerIndex == " + playerDt.Index);
                playerDt.IsExitWeiXin = false;
                if (m_IndexPlayerActiveGameState[playerDt.Index] == (int)PlayerActiveState.JiHuo)
                {
                    //处于激活状态的玩家才可以进行游戏操作.
                    int dirVal = 0;
                    if (dirValStr == PlayerShouBingDir.up.ToString())
                    {
                        //玩家手指离开摇杆(摇杆回中了).
                    }
                    else
                    {
                        int val = Convert.ToInt32(dirValStr);
                        if (val >= -22.5f && val < 22.5f)
                        {
                            dirVal = 1;
                        }
                        else if (val >= -67.5f && val < -22.5f)
                        {
                            dirVal = 2;
                        }
                        else if (val >= -112.5f && val < -67.5f)
                        {
                            dirVal = 3;
                        }
                        else if (val >= -157.5f && val < -112.5f)
                        {
                            dirVal = 4;
                        }
                        else if ((val >= -180f && val < -157.5f)
                            || (val <= 180f && val > 157.5f))
                        {
                            dirVal = 5;
                        }
                        else if (val > 112.5f && val <= 157.5f)
                        {
                            dirVal = 6;
                        }
                        else if (val > 67.5f && val <= 112.5f)
                        {
                            dirVal = 7;
                        }
                        else if (val >= 22.5f && val <= 67.5f)
                        {
                            dirVal = 8;
                        }
                    }

                    PlayerShouBingDir dirState = (PlayerShouBingDir)dirVal;
                    //if (dirState != PlayerShouBingDir.up)
                    //{
                    //    //自动回中摇杆.
                    //    StopCoroutine(DelayResetPlayerShouBingDir(playerDt.Index));
                    //    StartCoroutine(DelayResetPlayerShouBingDir(playerDt.Index));
                    //}

                    switch (dirState)
                    {
                        case PlayerShouBingDir.DirLeft:
                            {
                                InputEventCtrl.GetInstance().OnClickFangXiangUBt(playerDt.Index, pcvr.ButtonState.UP);
                                InputEventCtrl.GetInstance().OnClickFangXiangDBt(playerDt.Index, pcvr.ButtonState.UP);
                                InputEventCtrl.GetInstance().OnClickFangXiangLBt(playerDt.Index, pcvr.ButtonState.DOWN);
                                InputEventCtrl.GetInstance().OnClickFangXiangRBt(playerDt.Index, pcvr.ButtonState.UP);
                                break;
                            }
                        case PlayerShouBingDir.DirLeftDown:
                            {
                                InputEventCtrl.GetInstance().OnClickFangXiangUBt(playerDt.Index, pcvr.ButtonState.UP);
                                InputEventCtrl.GetInstance().OnClickFangXiangDBt(playerDt.Index, pcvr.ButtonState.DOWN);
                                InputEventCtrl.GetInstance().OnClickFangXiangLBt(playerDt.Index, pcvr.ButtonState.DOWN);
                                InputEventCtrl.GetInstance().OnClickFangXiangRBt(playerDt.Index, pcvr.ButtonState.UP);
                                break;
                            }
                        case PlayerShouBingDir.DirDown:
                            {
                                InputEventCtrl.GetInstance().OnClickFangXiangUBt(playerDt.Index, pcvr.ButtonState.UP);
                                InputEventCtrl.GetInstance().OnClickFangXiangDBt(playerDt.Index, pcvr.ButtonState.DOWN);
                                InputEventCtrl.GetInstance().OnClickFangXiangLBt(playerDt.Index, pcvr.ButtonState.UP);
                                InputEventCtrl.GetInstance().OnClickFangXiangRBt(playerDt.Index, pcvr.ButtonState.UP);
                                break;
                            }
                        case PlayerShouBingDir.DirRightDown:
                            {
                                InputEventCtrl.GetInstance().OnClickFangXiangUBt(playerDt.Index, pcvr.ButtonState.UP);
                                InputEventCtrl.GetInstance().OnClickFangXiangDBt(playerDt.Index, pcvr.ButtonState.DOWN);
                                InputEventCtrl.GetInstance().OnClickFangXiangLBt(playerDt.Index, pcvr.ButtonState.UP);
                                InputEventCtrl.GetInstance().OnClickFangXiangRBt(playerDt.Index, pcvr.ButtonState.DOWN);
                                break;
                            }
                        case PlayerShouBingDir.DirRight:
                            {
                                InputEventCtrl.GetInstance().OnClickFangXiangUBt(playerDt.Index, pcvr.ButtonState.UP);
                                InputEventCtrl.GetInstance().OnClickFangXiangDBt(playerDt.Index, pcvr.ButtonState.UP);
                                InputEventCtrl.GetInstance().OnClickFangXiangLBt(playerDt.Index, pcvr.ButtonState.UP);
                                InputEventCtrl.GetInstance().OnClickFangXiangRBt(playerDt.Index, pcvr.ButtonState.DOWN);
                                break;
                            }
                        case PlayerShouBingDir.DirRightUp:
                            {
                                InputEventCtrl.GetInstance().OnClickFangXiangUBt(playerDt.Index, pcvr.ButtonState.DOWN);
                                InputEventCtrl.GetInstance().OnClickFangXiangDBt(playerDt.Index, pcvr.ButtonState.UP);
                                InputEventCtrl.GetInstance().OnClickFangXiangLBt(playerDt.Index, pcvr.ButtonState.UP);
                                InputEventCtrl.GetInstance().OnClickFangXiangRBt(playerDt.Index, pcvr.ButtonState.DOWN);
                                break;
                            }
                        case PlayerShouBingDir.DirUp:
                            {
                                InputEventCtrl.GetInstance().OnClickFangXiangUBt(playerDt.Index, pcvr.ButtonState.DOWN);
                                InputEventCtrl.GetInstance().OnClickFangXiangDBt(playerDt.Index, pcvr.ButtonState.UP);
                                InputEventCtrl.GetInstance().OnClickFangXiangLBt(playerDt.Index, pcvr.ButtonState.UP);
                                InputEventCtrl.GetInstance().OnClickFangXiangRBt(playerDt.Index, pcvr.ButtonState.UP);
                                break;
                            }
                        case PlayerShouBingDir.DirLeftUp:
                            {
                                InputEventCtrl.GetInstance().OnClickFangXiangUBt(playerDt.Index, pcvr.ButtonState.DOWN);
                                InputEventCtrl.GetInstance().OnClickFangXiangDBt(playerDt.Index, pcvr.ButtonState.UP);
                                InputEventCtrl.GetInstance().OnClickFangXiangLBt(playerDt.Index, pcvr.ButtonState.DOWN);
                                InputEventCtrl.GetInstance().OnClickFangXiangRBt(playerDt.Index, pcvr.ButtonState.UP);
                                break;
                            }
                        case PlayerShouBingDir.up:
                            {
                                InputEventCtrl.GetInstance().OnClickFangXiangUBt(playerDt.Index, pcvr.ButtonState.UP);
                                InputEventCtrl.GetInstance().OnClickFangXiangDBt(playerDt.Index, pcvr.ButtonState.UP);
                                InputEventCtrl.GetInstance().OnClickFangXiangLBt(playerDt.Index, pcvr.ButtonState.UP);
                                InputEventCtrl.GetInstance().OnClickFangXiangRBt(playerDt.Index, pcvr.ButtonState.UP);
                                break;
                            }
                    }
                }
            }
        }

        IEnumerator DelayResetPlayerShouBingDir(int index)
        {
            yield return new WaitForSeconds(1f);
            InputEventCtrl.GetInstance().OnClickFangXiangUBt(index, pcvr.ButtonState.UP);
            InputEventCtrl.GetInstance().OnClickFangXiangDBt(index, pcvr.ButtonState.UP);
            InputEventCtrl.GetInstance().OnClickFangXiangLBt(index, pcvr.ButtonState.UP);
            InputEventCtrl.GetInstance().OnClickFangXiangRBt(index, pcvr.ButtonState.UP);
        }

        /// <summary>
        /// 当玩家游戏倒计时结束,清理玩家的游戏微信数据.
        /// 主要目的是想让新来的玩家可以立马进入游戏.
        /// </summary>
        internal void OnPlayerGameDaoJiShiOver(PlayerEnum indexPlayer)
        {
            int indexVal = (int)indexPlayer - 1;
            if (indexVal > -1 && indexVal < m_IndexPlayerActiveGameState.Length)
            {
                if (m_IndexPlayerActiveGameState[indexVal] == (int)PlayerActiveState.JiHuo)
                {
                    //玩家已经激活游戏.
                    return;
                }
            }

            GamePlayerData playerData = FindGamePlayerData(indexPlayer);
            if (playerData != null)
            {
                SSDebug.Log("OnPlayerGameDaoJiShiOver -> indexPlayer ==== " + indexPlayer
                    + ", userId ============ " + playerData.m_PlayerWeiXinData.userId);
                //清理微信数据.
                m_GmWXLoginDt[playerData.Index].Reset();
                //删除微信手柄玩家的数据信息.
                RemoveWeiXinPadPlayerData(playerData.m_PlayerWeiXinData.userId);

                //删除轮询检测玩家账户的数据.
                //RemoveLoopGetWXHddPayData(playerData.m_PlayerWeiXinData.userId);
            }
        }

        private void OnEventPlayerExitBox(int userId)
        {
            Debug.Log("Unity:" + "OnEventPlayerExitBox -> userId " + userId);
            GamePlayerData playerDt = m_GamePlayerData.Find((dt) => { return dt.m_PlayerWeiXinData.userId.Equals(userId); });
            if (playerDt != null)
            {
                playerDt.IsExitWeiXin = true;
                if (playerDt.Index > -1 && playerDt.Index < 4)
                {
                    if (m_IndexPlayerActiveGameState[playerDt.Index] == (int)PlayerActiveState.WeiJiHuo)
                    {
                        //删除轮询检测玩家账户的数据.
                        RemoveLoopGetWXHddPayData(userId);

                        //玩家血值耗尽,清理玩家微信数据.
                        m_GamePlayerData.Remove(playerDt);
                        m_GmWXLoginDt[playerDt.Index].IsLoginWX = false;
                        for (int i = 0; i < m_TVYaoKongPlayerDt.Count; i++)
                        {
                            if (m_TVYaoKongPlayerDt[i] != null && m_TVYaoKongPlayerDt[i].m_UserId == userId)
                            {
                                m_TVYaoKongPlayerDt.Remove(m_TVYaoKongPlayerDt[i]);
                                break;
                            }
                        }
                        Debug.Log("Unity:" + "OnEventPlayerExitBox -> clear player weiXin data...");
                    }
                }
            }
        }

        /// <summary>
        /// 删除微信手柄玩家的数据信息.
        /// </summary>
        internal void RemoveWeiXinPadPlayerData(int userId)
        {
            Debug.Log("Unity:" + "RemoveWeiXinPadPlayerData -> userId " + userId);
            GamePlayerData playerDt = m_GamePlayerData.Find((dt) => { return dt.m_PlayerWeiXinData.userId.Equals(userId); });
            if (playerDt != null)
            {
                PlayerEnum indexPlayer = (PlayerEnum)(playerDt.Index + 1);
                DaoJiShiCtrl daoJiShiCom = DaoJiShiCtrl.GetInstance(indexPlayer);
                if (daoJiShiCom != null)
                {
                    daoJiShiCom.WXPlayerStopGameDaoJiShi();
                }
                m_GamePlayerData.Remove(playerDt);
            }
        }

        internal void ClearGameWeiXinData()
        {
            Debug.Log("Unity: ClearGameWeiXinData...");
            if (m_GamePlayerData != null)
            {
                m_GamePlayerData.Clear();
            }

            for (int i = 0; i < m_GmWXLoginDt.Length; i++)
            {
                if (m_GmWXLoginDt[i] != null)
                {
                    m_GmWXLoginDt[i].IsLoginWX = false;
                    m_GmWXLoginDt[i].IsActiveGame = false;
                    m_GmWXLoginDt[i].m_GamePadType = GamePadType.Null;
                }
            }

            if (m_TVYaoKongPlayerDt != null)
            {
                m_TVYaoKongPlayerDt.Clear();
            }

            if (m_GmTVLoginDt != null)
            {
                m_GmTVLoginDt.Reset();
                m_GmTVLoginDt = null;
            }
        }

        private void OnEventPlayerLoginBox(WebSocketSimpet.PlayerWeiXinData val)
        {
            Debug.Log("Unity:" + "pcvr::OnEventPlayerLoginBox -> userName " + val.userName + ", userId " + val.userId);
            GamePlayerData playerDt = m_GamePlayerData.Find((dt) => {
                if (dt.m_PlayerWeiXinData != null)
                {
                    return dt.m_PlayerWeiXinData.userId.Equals(val.userId);
                }
                return dt.m_PlayerWeiXinData.Equals(val);
            });
            
            //发送微信玩家获得的游戏代金券信息给服务器.
            //SendPostHddPlayerCouponInfo(val.userId, 1);//test.
            
            int indexPlayer = -1;
            bool isGamePlayerFull = false; //游戏激活人数是否已经满了.
            bool isActivePlayer = false;
            if (playerDt == null)
            {
                indexPlayer = GetActivePlayerIndex();
                if (indexPlayer > -1 && indexPlayer < m_IndexPlayerActiveGameState.Length)
                {
                    Debug.Log("Unity:" + "Active player, indexPlayer == " + indexPlayer);
                    playerDt = new GamePlayerData();
                    playerDt.m_PlayerWeiXinData = val;
                    playerDt.Index = indexPlayer;
                    m_GamePlayerData.Add(playerDt);
                    isActivePlayer = true;
                    m_GmWXLoginDt[indexPlayer].IsLoginWX = true;
                    m_GmWXLoginDt[indexPlayer].m_GamePadType = GamePadType.WeiXin_ShouBing;
                }
                else
                {
                    //游戏激活人数已满.
                    Debug.Log("Unity:" + "have not empty player!");
                    isGamePlayerFull = true;
                }
            }
            else
            {
                Debug.Log("Unity:" + "player have active game!");
                playerDt.IsExitWeiXin = false;
                if (playerDt.Index > -1 && playerDt.Index < 4)
                {
                    PlayerActiveState playerSt = (PlayerActiveState)m_IndexPlayerActiveGameState[playerDt.Index];
                    switch (playerSt)
                    {
                        case PlayerActiveState.WeiJiHuo:
                            {
                                isActivePlayer = true;
                                indexPlayer = playerDt.Index;
                                m_GmWXLoginDt[indexPlayer].IsLoginWX = true;
                                m_GmWXLoginDt[indexPlayer].m_GamePadType = GamePadType.WeiXin_ShouBing;
                                break;
                            }
                        case PlayerActiveState.JiHuo:
                            {
                                GamePlayerData playerTmpDt = m_GamePlayerData.Find((dt) =>
                                {
                                    return dt.Index.Equals(playerDt.Index);
                                });

                                if (playerTmpDt != null
                                    && playerTmpDt.m_PlayerWeiXinData != null
                                    && val.userId == playerTmpDt.m_PlayerWeiXinData.userId)
                                {
                                    Debug.Log("Unity: " + playerTmpDt.m_PlayerWeiXinData.userName + " have active " + " player!");
                                }
                                else
                                {
                                    indexPlayer = GetActivePlayerIndex();
                                    if (indexPlayer > -1 && indexPlayer < m_IndexPlayerActiveGameState.Length)
                                    {
                                        Debug.Log("Unity: player active -> indexPlayer == " + indexPlayer);
                                        isActivePlayer = true;
                                        playerDt.Index = indexPlayer;
                                        m_GmWXLoginDt[indexPlayer].IsLoginWX = true;
                                        m_GmWXLoginDt[indexPlayer].m_GamePadType = GamePadType.WeiXin_ShouBing;
                                    }
                                    else
                                    {
                                        //游戏激活人数已满.
                                        Debug.Log("Unity:" + " ---> have not empty player!");
                                        isGamePlayerFull = true;
                                    }
                                }
                                break;
                            }
                    }
                }
            }

            if (isGamePlayerFull == true)
            {
                //if (m_HongDDGamePadWXPay != null)
                //{
                //    //游戏激活人数已满.
                //    m_HongDDGamePadWXPay.CToS_GamePlayerIsFull("");
                //}
                //游戏激活人数已满.
                SendWXPadGamePlayerFull(val.userId);
            }

            if (isActivePlayer)
            {
                if (m_HongDDGamePadWXPay != null)
                {
                    //m_HongDDGamePadWXPay.m_GameConfigData.MianFeiShiWanCount = 1; //免费试玩次数.
                    if (m_HongDDGamePadWXPay.m_GameConfigData.MianFeiShiWanCount > 0)
                    {
                        if (m_HongDDGamePadWXPay.CheckPlayerIsCanFreePlayGame(val.userId) == true)
                        {
                            //该玩家可以免费试玩游戏.
                            //给玩家添加一个微信游戏币.
                            AddWeiXinGameCoinToPlayer((PlayerEnum)(indexPlayer + 1), m_HongDDGamePadWXPay.m_GameConfigData.MianFeiShiWanCount);
                            //直接打开微信小程序游戏手柄.
                            m_HongDDGamePadWXPay.CToS_GameIsCanFreePlay("");

                            CoinPlayerCtrl playerCoinCom = CoinPlayerCtrl.GetInstance((PlayerEnum)(indexPlayer + 1));
                            if (playerCoinCom != null)
                            {
                                playerCoinCom.SetActiveMianFeiTiYanUI(true);
                            }
                            //免费体验游戏的玩家.
                            playerDt.IsMianFeiTiYanPlayer = true;
                        }
                        else
                        {
                            //该玩家不可以试玩游戏.
                            //拉起玩家手机微信小程序支付界面.
                            m_HongDDGamePadWXPay.CToS_GameIsCanFreePlay("");
                            //测试:直接给玩家2个游戏币.
                            //AddWeiXinGameCoinToPlayer((PlayerEnum)(indexPlayer + 1), 2); //test.
                            
                            //获取微信玩家的红点点游戏账户数据.
                            GetWXPlayerHddPayData(val.userId);
                            playerDt.IsMianFeiTiYanPlayer = false;
                            playerDt.IsGetWXPlayerHddPayData = true;
                        }
                    }
                    else
                    {
                        //拉起玩家手机微信小程序支付界面.
                        m_HongDDGamePadWXPay.CToS_GameIsCanFreePlay("");

                        //获取微信玩家的红点点游戏账户数据.
                        GetWXPlayerHddPayData(val.userId);
                        playerDt.IsMianFeiTiYanPlayer = false;
                        playerDt.IsGetWXPlayerHddPayData = true;
                    }
                }
                
                m_GmWXLoginDt[indexPlayer].IsActiveGame = true;
                m_PlayerHeadUrl[indexPlayer] = playerDt.m_PlayerWeiXinData.headUrl;
                InputEventCtrl.GetInstance().OnClickGameStartBt(indexPlayer);
            }
        }

        /// <summary>
        /// 添加微信玩家游戏币.
        /// </summary>
        internal void AddWeiXinGameCoinToPlayer(int userId, int coin)
        {
            //test start
            //if (m_GamePlayerData.Count > 0
            //    && m_GamePlayerData[0] != null
            //    && m_GamePlayerData[0].m_PlayerWeiXinData != null)
            //{
            //    userId = m_GamePlayerData[0].m_PlayerWeiXinData.userId; //test
            //}
            //test end

            GamePlayerData playerData = null;
            int length = m_GamePlayerData.Count;
            for (int i = 0; i < length; i++)
            {
                if (m_GamePlayerData[i] != null
                    && m_GamePlayerData[i].m_PlayerWeiXinData != null
                    && m_GamePlayerData[i].m_PlayerWeiXinData.userId == userId)
                {
                    playerData = m_GamePlayerData[i];
                    break;
                }
            }

            if (playerData != null)
            {
                //查找到游戏玩家信息.
                int index = playerData.Index;
                if (index > -1 && index < 4)
                {
                    PlayerEnum indexPlayer = (PlayerEnum)(index + 1);
                    CoinPlayerCtrl playerCoinCom = CoinPlayerCtrl.GetInstance(indexPlayer);
                    if (playerCoinCom != null)
                    {
                        playerCoinCom.SetActiveMianFeiTiYanUI(false);
                    }
                    AddWeiXinGameCoinToPlayer(indexPlayer, coin);
                }
            }
        }

        /// <summary>
        /// 添加微信游戏币给玩家.
        /// </summary>
        void AddWeiXinGameCoinToPlayer(PlayerEnum playerIndex, int coin)
        {
            Debug.Log("Unity: AddGameCoinToPlayer -> playerIndex === " + playerIndex + ", coin ===== " + coin);
            if (XKGlobalData.GetInstance().m_GameWXPayDataManage != null)
            {
                XKGlobalData.GetInstance().m_GameWXPayDataManage.WriteGamePayRevenueInfo(coin);
            }

            bool isMianFeiTiYanPlayer = false;
            GamePlayerData data = FindGamePlayerData(playerIndex);
            if (data != null)
            {
                isMianFeiTiYanPlayer = data.IsMianFeiTiYanPlayer;
            }

            int coinTotal = 0;
            switch (playerIndex)
            {
                case PlayerEnum.PlayerOne:
                    {
                        if (isMianFeiTiYanPlayer == false)
                        {
                            XKGlobalData.CoinPlayerOne += coin;
                            coinTotal = XKGlobalData.CoinPlayerOne;
                        }
                        else
                        {
                            coinTotal = XKGlobalData.CoinPlayerOne + coin;
                        }
                        XKGlobalData.SetCoinPlayerOne(coinTotal);
                        break;
                    }
                case PlayerEnum.PlayerTwo:
                    {
                        if (isMianFeiTiYanPlayer == false)
                        {
                            XKGlobalData.CoinPlayerTwo += coin;
                            coinTotal = XKGlobalData.CoinPlayerTwo;
                        }
                        else
                        {
                            coinTotal = XKGlobalData.CoinPlayerTwo + coin;
                        }
                        XKGlobalData.SetCoinPlayerTwo(coinTotal);
                        break;
                    }
                case PlayerEnum.PlayerThree:
                    {
                        if (isMianFeiTiYanPlayer == false)
                        {
                            XKGlobalData.CoinPlayerThree += coin;
                            coinTotal = XKGlobalData.CoinPlayerThree;
                        }
                        else
                        {
                            coinTotal = XKGlobalData.CoinPlayerThree + coin;
                        }
                        XKGlobalData.SetCoinPlayerThree(coinTotal);
                        break;
                    }
                case PlayerEnum.PlayerFour:
                    {
                        if (isMianFeiTiYanPlayer == false)
                        {
                            XKGlobalData.CoinPlayerFour += coin;
                            coinTotal = XKGlobalData.CoinPlayerFour;
                        }
                        else
                        {
                            coinTotal = XKGlobalData.CoinPlayerFour + coin;
                        }
                        XKGlobalData.SetCoinPlayerFour(coinTotal);
                        break;
                    }
            }

            if (coinTotal >= XKGlobalData.GameNeedCoin)
            {
                //玩家币值足够,则开启游戏.
                InputEventCtrl.GetInstance().OnClickGameStartBt((int)playerIndex - 1);
            }
        }
        #endregion
        
        #region 微信游戏手柄玩家红点点账户信息管理

#if TEST_HDD_ZHIFU_PLATFORM
        //测试红点点支付平台逻辑.
        /// <summary>
        /// 游戏币和红点点金币的转换率.
        /// 1币 == 2元人民币 == 200分.
        /// 游戏币转换为红点点账户金币（单位为分）.
        /// </summary>
        int m_GameCoinToMoney = 1;
#else
        /// <summary>
        /// 游戏币和红点点金币的转换率.
        /// 1币 == 2元人民币 == 200分.
        /// 游戏币转换为红点点账户金币（单位为分）.
        /// </summary>
        int m_GameCoinToMoney = 200;
#endif

        /// <summary>
        /// 更新游戏一币等于多少人民币的信息.
        /// </summary>
        internal void UpdateGameCoinToMoney(int args)
        {
            m_GameCoinToMoney = args;
            XKGlobalData.GetInstance().m_CoinToCard = args;
            SSDebug.Log("UpdateGameCoinToMoney -> m_GameCoinToMoney ====================== " + m_GameCoinToMoney);
        }

        /// <summary>
        /// 循环检测获取微信玩家在红点点平台的账户信息.
        /// </summary>
        [Serializable]
        public class LoopGetWXHddPayData
        {
            /// <summary>
            /// 用户Id.
            /// </summary>
            public int userId = 0;
            /// <summary>
            /// 玩家微信数据.
            /// </summary>
            public GamePlayerData m_GamePlayerData = null;
            public LoopGetWXHddPayData(int userIdVal)
            {
                userId = userIdVal;
            }
            public LoopGetWXHddPayData(int userIdVal, GamePlayerData playerDt)
            {
                userId = userIdVal;
                m_GamePlayerData = playerDt;
            }
        }
        List<LoopGetWXHddPayData> m_LoopGetWXHddPayDataList = new List<LoopGetWXHddPayData>();
        /// <summary>
        /// 查找数据.
        /// </summary>
        LoopGetWXHddPayData FindLoopGetWXHddPayData(int userId)
        {
            LoopGetWXHddPayData data = m_LoopGetWXHddPayDataList.Find((dt) => {
                return dt.userId.Equals(userId);
            });
            return data;
        }

        /// <summary>
        /// 添加数据.
        /// </summary>
        void AddLoopGetWXHddPayData(int userId)
        {
            if (FindLoopGetWXHddPayData(userId) == null)
            {
                //缓存玩家微信数据m_GamePlayerData
                GamePlayerData playerData = FindGamePlayerData(userId);
                m_LoopGetWXHddPayDataList.Add(new LoopGetWXHddPayData(userId, playerData));
            }
        }

        /// <summary>
        /// 删除数据.
        /// 当玩家游戏账户金额足够开启游戏时删除数据.
        /// 当玩家已经退出微信游戏手柄时删除数据.
        /// </summary>
        void RemoveLoopGetWXHddPayData(int userId)
        {
            LoopGetWXHddPayData data = m_LoopGetWXHddPayDataList.Find((dt) => {
                return dt.userId.Equals(userId);
            });

            if (data != null)
            {
                m_LoopGetWXHddPayDataList.Remove(data);
                data = null;
            }
        }

        /// <summary>
        /// 循环检测玩家的充值信息.
        /// </summary>
        IEnumerator LoopGetWXPlayerHddPayData(int userId)
        {
            LoopGetWXHddPayData data = FindLoopGetWXHddPayData(userId);
            if (data == null)
            {
                AddLoopGetWXHddPayData(userId);
            }
            else
            {
                Debug.Log("LoopGetWXPlayerHddPayData -> playerData have been to inserted!");
                yield break;
            }

            float time = Time.time;
            do
            {
                if (Time.time - time >= 120f)
                {
                    //轮询检测超时,认为玩家已经不再继续游戏了.
                    //删除数据.
                    RemoveLoopGetWXHddPayData(userId);
                    yield break;
                }

                yield return new WaitForSeconds(3f);
                data = FindLoopGetWXHddPayData(userId);
                if (data != null)
                {
                    //获取玩家充值信息.
                    GetWXPlayerHddPayData(userId);
                }
                else
                {
                    //获取到玩家的有效充值数据信息了(充值金额大于等于游戏启动金额),或者玩家已经退出微信游戏手柄.
                    yield break;
                }
            } while (data != null);
        }

        /// <summary>
        /// 每次玩家登陆成功后，都需要获取玩家的账户信息.
        /// 获取微信玩家红点点游戏账户数据.
        /// </summary>
        void GetWXPlayerHddPayData(int userId)
        {
            if (m_SSBoxPostNet != null
                && m_SSBoxPostNet.m_GamePayPlatform == SSBoxPostNet.GamePayPlatform.HongDianDian)
            {
                m_SSBoxPostNet.HttpSendGetPlayerPayData(userId);
            }
        }

        /// <summary>
        /// 收到微信玩家的红点点游戏账户数据.
        /// </summary>
        void OnReceivedWXPlayerHddPayData(int userId, int money)
        {
            Debug.Log("OnReceivedWXPlayerHddPayData -> userId == " + userId + ", money == " + money);
            if (money < m_GameCoinToMoney)
            {
                //玩家红点点游戏平台的金额不足,应该去进行充值.
                GamePlayerData data = FindGamePlayerData(userId);
                if (data != null && data.IsGetWXPlayerHddPayData == true)
                {
                    //玩家登陆微信游戏手柄后,游戏币值不足,需要进行充值.
                    data.IsGetWXPlayerHddPayData = false;
                    //玩家币值不够,需要拉起微信游戏手柄的充值界面.
                    SendWXPadShowTopUpPanel(userId);
                }
            }
            else
            {
                GamePlayerData playerDt = FindGamePlayerData(userId);
                if (playerDt != null)
                {
                    SSDebug.Log("Active player, userId =============== " + userId);
                    m_GmWXLoginDt[playerDt.Index].IsActiveGame = true;
                    //在微信玩家列表信息中找到了玩家信息.
                    //说明玩家是在游戏倒计时结束前成功续费的.
                    //当前玩家的红点点游戏金额兑换为游戏币.
                    int coin = money / m_GameCoinToMoney;
                    if (coin > 10)
                    {
                        //最多给玩家显示9次复活信息.
                        coin = 10;
                    }
                    AddWeiXinGameCoinToPlayer(userId, coin);
                }
                else
                {
                    //在微信玩家列表信息中没有找到玩家信息.
                    //说明玩家是在游戏倒计时结束之后成功交费的.
                    bool isHaveEmptyJiWei = GetIsHaveEmptyJiWei();
                    if (isHaveEmptyJiWei == true)
                    {
                        //空余机位索引.
                        int indexPlayer = GetActivePlayerIndex();
                        SSDebug.Log("Active player, indexPlayer == " + indexPlayer + ", userId =============== " + userId);
                        LoopGetWXHddPayData playerWxHddPayDt = FindLoopGetWXHddPayData(userId);
                        if (playerWxHddPayDt != null && playerWxHddPayDt.m_GamePlayerData != null)
                        {
                            playerDt = playerWxHddPayDt.m_GamePlayerData;
                            //playerDt.m_PlayerWeiXinData = val;
                            playerDt.Index = indexPlayer;
                            m_GamePlayerData.Add(playerDt);
                            //isActivePlayer = true;
                            m_GmWXLoginDt[indexPlayer].IsLoginWX = true;
                            m_GmWXLoginDt[indexPlayer].m_GamePadType = GamePadType.WeiXin_ShouBing;

                            m_GmWXLoginDt[indexPlayer].IsActiveGame = true;
                            m_PlayerHeadUrl[indexPlayer] = playerDt.m_PlayerWeiXinData.headUrl;

                            //当前有空余机位可以进行游戏.
                            //当前玩家的红点点游戏金额兑换为游戏币.
                            int coin = money / m_GameCoinToMoney;
                            if (coin > 10)
                            {
                                //最多给玩家显示9次复活信息.
                                coin = 10;
                            }
                            AddWeiXinGameCoinToPlayer(userId, coin);
                        }
                        else
                        {
                            //超出了轮询检测付费消息时间.
                            SSDebug.Log("out time loop check player pay data! userId ============== " + userId);
                        }
                    }
                    else
                    {
                        //没有空余机位时,需要提示玩家"当前游戏人数已满".
                        SendWXPadGamePlayerFull(userId);
                    }
                }

                //删除轮询检测玩家账户的数据.
                RemoveLoopGetWXHddPayData(userId);
            }
        }

        /// <summary>
        /// 扣除微信游戏手柄玩家的代币.
        /// </summary>
        void SubWXPlayerHddPayData(int userId)
        {
            //Debug.Log("Unity: SubWXPlayerHddPayData -> userId ============================ " + userId);
            if (m_SSBoxPostNet != null
                && m_SSBoxPostNet.m_GamePayPlatform == SSBoxPostNet.GamePayPlatform.HongDianDian)
            {
                int subCoin = 1; //减去1个游戏币(1元1个游戏币).
                int money = subCoin * m_GameCoinToMoney; //扣除的红点点账户金币.
                SendSubWXPlayerHddPayData(userId, money);
            }
        }

        /// <summary>
        /// 发送扣除微信玩家的红点点游戏账户消费信息.
        /// </summary>
        void SendSubWXPlayerHddPayData(int userId, int money)
        {
            if (m_SSBoxPostNet != null
                && m_SSBoxPostNet.m_GamePayPlatform == SSBoxPostNet.GamePayPlatform.HongDianDian)
            {
                m_SSBoxPostNet.HttpSendPostHddSubPlayerMoney(userId, money);
            }
        }

        /// <summary>
        /// 发送打开微信游戏手柄充值界面.
        /// </summary>
        void SendWXPadShowTopUpPanel(int userId)
        {
            if (m_SSBoxPostNet != null && m_SSBoxPostNet.m_WebSocketSimpet != null
                && m_SSBoxPostNet.m_GamePayPlatform == SSBoxPostNet.GamePayPlatform.HongDianDian)
            {
                m_SSBoxPostNet.m_WebSocketSimpet.NetSendWeiXinPadShowTopUpPanel(userId);
            }
            StartCoroutine(LoopGetWXPlayerHddPayData(userId));
        }

        /// <summary>
        /// 发送玩家获取商家代金券的信息给服务器.
        /// indexPlayer玩家索引.
        /// money代金券金额(元).
        /// </summary>
        internal void SendPostHddPlayerCouponInfo(PlayerEnum indexPlayer, int money)
        {
            GamePlayerData data = FindGamePlayerData(indexPlayer);
            if (data != null && data.m_PlayerWeiXinData != null)
            {
                int userId = data.m_PlayerWeiXinData.userId;
                Debug.Log("SendPostHddPlayerCouponInfo -> userId ==== " + userId + ", money ==== " + money);
                SendPostHddPlayerCouponInfo(userId, money);
            }
        }

        /// <summary>
        /// 发送玩家获取商家代金券的信息给服务器.
        /// userId玩家id.
        /// account代金券金额(元).
        /// </summary>
        void SendPostHddPlayerCouponInfo(int userId, int account)
        {
            if (m_SSBoxPostNet != null && m_SSBoxPostNet.m_BoxLoginData != null)
            {
                //boxId 游戏盒子Id.最终应该为商家id(有的商家可能是连锁店).
                string boxId = m_SSBoxPostNet.m_BoxLoginData.boxNumber;
                m_SSBoxPostNet.HttpSendPostHddPlayerCouponInfo(userId, account, boxId);
            }
        }

        /// <summary>
        /// 发送游戏当前处于满员状态消息给微信游戏手柄.
        /// </summary>
        void SendWXPadGamePlayerFull(int userId)
        {
            if (m_SSBoxPostNet != null && m_SSBoxPostNet.m_WebSocketSimpet != null
                && m_SSBoxPostNet.m_GamePayPlatform == SSBoxPostNet.GamePayPlatform.HongDianDian)
            {
                m_SSBoxPostNet.m_WebSocketSimpet.NetSendWeiXinPadGamePlayerFull(userId);
            }
        }

        /// <summary>
        /// 更新免费试玩信息.
        /// </summary>
        internal void UpdateMianFeiCountInfo(int args)
        {
            if (m_HongDDGamePadWXPay != null)
            {
                m_HongDDGamePadWXPay.SToC_ReceiveGameConfigInfo(args);
            }
        }
        #endregion

        //private void Update()
        //{
        //    if (Input.GetKeyUp(KeyCode.P))
        //    {
        //        for (int i = 0; i < m_GamePlayerData.Count; i++)
        //        {
        //            if (m_GamePlayerData[i] != null && m_GamePlayerData[i].m_PlayerWeiXinData != null)
        //            {
        //                int userId = m_GamePlayerData[i].m_PlayerWeiXinData.userId;
        //                //SendWXPadShowTopUpPanel(userId); //test.
        //                SendWXPadGamePlayerFull(userId); //test.
        //            }
        //        }
        //    }
        //}
    }
}
