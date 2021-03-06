﻿#define USE_HDD_PAD_BT_ACTIVE_PLAYER //使用红点点手柄按键消息激活游戏主角
//#define TEST_DEBUG_PLAYER_PAD_INFO
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
        /// 重置微信头像地址信息.
        /// </summary>
        internal void ResetPlayerHeadUrl(int indexVal)
        {
            if (indexVal > -1 && indexVal < m_PlayerHeadUrl.Length)
            {
                m_PlayerHeadUrl[indexVal] = "";
            }
        }
        /// <summary>
        /// 设置玩家微信头像url.
        /// </summary>
        void SetPlayerHeadUrl(int indexVal, string url)
        {
            if (indexVal > -1 && indexVal < m_PlayerHeadUrl.Length)
            {
                m_PlayerHeadUrl[indexVal] = url;
            }
        }
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
            InitPlayerPlayGameTimeData();
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
                SSBoxPostNet.OnReceivedSendPostHddSubPlayerMoneyEvent += OnReceivedSendPostHddSubPlayerMoneyEvent;
                
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
                        m_HongDDGamePadWXPay.Init();
                        //向服务器请求游戏的后台配置信息.
                        //m_HongDDGamePadWXPay.CToS_RequestGameConfigInfo("");
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
            //UpdateGameCoinToMoney(1); //test
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
        void SetGmWXloginDtGamePadType(int indexVal, GamePadType type)
        {
            if (indexVal >= 0 && indexVal < m_GmWXLoginDt.Length)
            {
                m_GmWXLoginDt[indexVal].m_GamePadType = type;
            }
        }

        void SetGmWXloginDtIsActiveGame(int indexVal, bool isActive)
        {
            if (indexVal >= 0 && indexVal < m_GmWXLoginDt.Length)
            {
                m_GmWXLoginDt[indexVal].IsActiveGame = isActive;
            }
        }

        void SetGmWXloginDtIsLoginWX(int indexVal, bool isLogin)
        {
            if (indexVal >= 0 && indexVal < m_GmWXLoginDt.Length)
            {
                m_GmWXLoginDt[indexVal].IsLoginWX = isLogin;
            }
        }

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

        //void ClickTVYaoKongEnterBtEvent(pcvr.ButtonState val)
        //{
        //    if (val == pcvr.ButtonState.UP)
        //    {
        //        Debug.Log("Unity: pcvr -> ClickTVYaoKongEnterBtEvent...");
        //        int count = m_TVYaoKongPlayerDt.Count;
        //        if (count > 0)
        //        {
        //            TVYaoKongPlayerData playerDt = m_TVYaoKongPlayerDt[count - 1];
        //            int indexPlayer = playerDt.Index;
        //            //清理最后一个血值耗尽的玩家信息.
        //            m_TVYaoKongPlayerDt.RemoveAt(count - 1);

        //            if (indexPlayer > -1 && indexPlayer < 4)
        //            {
        //                switch (playerDt.m_GamePadType)
        //                {
        //                    case GamePadType.TV_YaoKongQi:
        //                        {
        //                            if (m_GmWXLoginDt[indexPlayer].IsLoginWX)
        //                            {
        //                                if (!m_GmWXLoginDt[indexPlayer].IsActiveGame)
        //                                {
        //                                    Debug.Log("Unity: click TVYaoKong EnterBt -> active TV_YaoKongQi " + indexPlayer + " player!");
        //                                    m_GmWXLoginDt[indexPlayer].IsActiveGame = true;
        //                                    InputEventCtrl.GetInstance().OnClickGameStartBt(indexPlayer);
        //                                }
        //                            }
        //                            break;
        //                        }
        //                    case GamePadType.WeiXin_ShouBing:
        //                        {
        //                            if (m_GmWXLoginDt[indexPlayer].IsLoginWX)
        //                            {
        //                                if (!m_GmWXLoginDt[indexPlayer].IsActiveGame)
        //                                {
        //                                    Debug.Log("Unity: click TVYaoKong EnterBt -> active " + indexPlayer + " player!");
        //                                    m_GmWXLoginDt[indexPlayer].IsActiveGame = true;
        //                                    InputEventCtrl.GetInstance().OnClickGameStartBt(indexPlayer);
        //                                }
        //                            }
        //                            break;
        //                        }
        //                }
        //            }
        //        }
        //        else
        //        {
        //            if (SSUIRoot.GetInstance().m_ExitUICom == null)
        //            {
        //                //没有退出游戏界面出现时,可以用遥控器进入游戏.
        //                if (m_GmTVLoginDt == null)
        //                {
        //                    //遥控器激活玩家.
        //                    int index = GetActivePlayerIndex();
        //                    if (index < m_IndexPlayerActiveGameState.Length && index > -1)
        //                    {
        //                        Debug.Log("Unity: click TVYaoKong EnterBt -> --> active TV_YaoKongQi " + index + " player!");
        //                        m_GmWXLoginDt[index].IsLoginWX = true;
        //                        m_GmWXLoginDt[index].IsActiveGame = true;
        //                        m_GmWXLoginDt[index].m_GamePadType = GamePadType.TV_YaoKongQi;
        //                        ResetPlayerHeadUrl(index);
        //                        m_GmTVLoginDt = new TVYaoKongPlayerData(index, GamePadType.TV_YaoKongQi, 0);
        //                        InputEventCtrl.GetInstance().OnClickGameStartBt(index);
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}

        [Serializable]
        public class GamePlayerData
        {
            /// <summary>
            /// 玩家在游戏中的索引.
            /// </summary>
            public int Index = -1;
            internal void SetIndex(int Index)
            {
                this.Index = Index;
            }
            /// <summary>
            /// 是否退出微信.
            /// </summary>
            public bool IsExitWeiXin = false;
            /// <summary>
            /// 是否为免费体验游戏的玩家.
            /// </summary>
            public bool IsMianFeiTiYanPlayer = false;
            /// <summary>
            /// 设置是否为免费玩家.
            /// </summary>
            internal void SetIsMianFeiTiYanPlayer(bool isMianFeiTiYan)
            {
                IsMianFeiTiYanPlayer = isMianFeiTiYan;
                //SSDebug.LogWarning("SetIsMianFeiTiYanPlayer -> isMianFeiTiYan =========================== " + isMianFeiTiYan
                //    + ", Index ==== " + Index);
            }
            /// <summary>
            /// 是否在获取微信玩家的红点点游戏账户数据.
            /// </summary>
            public bool IsGetWXPlayerHddPayData = false;
            /// <summary>
            /// 玩家的微信数据信息.
            /// </summary>
            public WebSocketSimpet.PlayerWeiXinData m_PlayerWeiXinData;
        }
        /// <summary>
        /// 该数据列表只允许保存登录游戏后在m_GamePlayerData中找不到的玩家信息.
        /// </summary>
        public List<GamePlayerData> m_LoginGamePlayerData = new List<GamePlayerData>();
        /// <summary>
        /// 查找登录游戏的玩家微信数据.
        /// </summary>
        GamePlayerData FindLoginGamePlayerData(int userId)
        {
            if (m_LoginGamePlayerData == null)
            {
                return null;
            }

            GamePlayerData playerDt = m_LoginGamePlayerData.Find((dt) => {
                return dt.m_PlayerWeiXinData.userId.Equals(userId);
            });
            return playerDt;
        }

        /// <summary>
        /// 添加登录游戏的玩家微信数据.
        /// </summary>
        void AddLoginGamePlayerData(GamePlayerData playerDt)
        {
            if (m_LoginGamePlayerData == null)
            {
                return;
            }

            if (playerDt != null && playerDt.m_PlayerWeiXinData != null)
            {
                if (FindLoginGamePlayerData(playerDt.m_PlayerWeiXinData.userId) == null)
                {
                    //按照玩家userId没有找到玩家信息时才允许添加玩家数据信息.
                    m_LoginGamePlayerData.Add(playerDt);
                }
            }
        }

        /// <summary>
        /// 删除登录游戏的玩家微信数据.
        /// </summary>
        internal void RemoveLoginGamePlayerData(int userId)
        {
            GamePlayerData playerDt = FindLoginGamePlayerData(userId);
            if (playerDt != null)
            {
                m_LoginGamePlayerData.Remove(playerDt);
            }
        }

        /// <summary>
        /// 微信玩家数据列表.
        /// 该数据列表只允许存储玩游戏的玩家信息,只登录而没有激活游戏的微信玩家数据不允许在该列表中保存.
        /// 微信玩家激活游戏后必须将微信玩家数据保存到该列表中.
        /// 微信玩家GG并且付费超时之后必须将该微信玩家的数据清除.
        /// </summary>
        public List<GamePlayerData> m_GamePlayerData = new List<GamePlayerData>();
        /// <summary>
        /// 查找玩家微信游戏数据.
        /// </summary>
        GamePlayerData FindGamePlayerData(int userId)
        {
            //GamePlayerData playerDt = m_GamePlayerData.Find((dt) => {
            //    return dt.m_PlayerWeiXinData.userId.Equals(userId);
            //});
            //return playerDt;
            return FindGamePlayerDataByDictionary(userId);
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
        /// 添加玩家微信数据信息.
        /// </summary>
        void AddGamePlayerData(GamePlayerData playerDt)
        {
            if (playerDt != null && m_GamePlayerData != null && m_GamePlayerData.Contains(playerDt) == false)
            {
                m_GamePlayerData.Add(playerDt);
                if (playerDt.m_PlayerWeiXinData != null)
                {
                    //在字典中添加玩家数据信息.
                    AddGamePlayerDataByDictionary(playerDt);
                }
            }
        }

        /// <summary>
        /// 删除玩家微信数据信息.
        /// </summary>
        internal void RemoveGamePlayerData(int userId)
        {
            GamePlayerData playerDt = FindGamePlayerData(userId);
            if (playerDt != null)
            {
                m_GamePlayerData.Remove(playerDt);
                if (playerDt.m_PlayerWeiXinData != null)
                {
                    //在字典中删除玩家数据信息.
                    RemoveGamePlayerDataByDictionary(playerDt.m_PlayerWeiXinData.userId);
                }
            }
        }

        /// <summary>
        /// 删除玩家微信数据信息.
        /// </summary>
        internal void RemoveGamePlayerData(PlayerEnum indexPlayer)
        {
            //重置微信头像url信息.
            ResetPlayerHeadUrl((int)indexPlayer - 1);
            GamePlayerData playerDt = FindGamePlayerData(indexPlayer);
            if (playerDt != null && playerDt.m_PlayerWeiXinData != null)
            {
                //清理微信数据.
                RemoveGamePlayerData(playerDt.m_PlayerWeiXinData.userId);
            }
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
            startGameBtUp,
            startGameBtDown,
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
        /// 当玩家币值充足不用展示评级UI时进入此函数对玩家进行扣费.
        /// 当展示玩家评级UI时进入此函数对玩家进行扣费.
        /// 此时需要对微信付费玩家进行红点点账户扣费.
        /// </summary>
        internal void OnNeedSubPlayerMoney(PlayerEnum indexPlayer)
        {
            int index = (int)indexPlayer - 1;
            if (index < 0 || index >= m_IndexPlayerActiveGameState.Length)
            {
                return;
            }

            //是否可以继续免费玩游戏.
            bool isCanMianFeiPlayGame = false;
            if (XKGlobalData.GetInstance() != null)
            {
                isCanMianFeiPlayGame = XKGlobalData.GetInstance().GetIsCanMianFeiPlayGame(indexPlayer);
                //if (isCanMianFeiPlayGame == true)
                //{
                    //XKGlobalData.GetInstance().SubMianFeiNum(indexPlayer);
                //}
            }

            if (isCanMianFeiPlayGame == true)
            {
                //玩家可以继续免费玩游戏,因为玩家的首次免费次数还没有用完.
                //所以不用对玩家进行扣费.
                return;
            }

            //SSDebug.Log("OnNeedSubPlayerMoney -> indexPlayer =============================== " + indexPlayer);
            //SSDebug.LogWarning("OnNeedSubPlayerMoney -> indexPlayer =============================== " + indexPlayer);
            GamePlayerData playerDt = FindGamePlayerData(indexPlayer);
            if (playerDt != null)
            {
                if (playerDt.m_PlayerWeiXinData != null)
                {
                    int userId = playerDt.m_PlayerWeiXinData.userId;
                    if (playerDt.IsMianFeiTiYanPlayer == false)
                    {
                        //付费玩家进行扣费.
                        //扣除玩家红点点游戏账户金币.
                        SubWXPlayerHddPayData(userId);
                    }
                }
            }

            //设置玩家结束游戏的时间.
            //SetPlayerEndGameTime(indexPlayer);
        }

        /// <summary>
        /// 获取玩家是否为付费激活游戏用户.
        /// </summary>
        internal bool GetPlayerIsFuFeiActiveGame(PlayerEnum indexPlayer)
        {
            bool isFuFeiPlayer = false;
            int index = (int)indexPlayer - 1;
            if (index < 0 || index >= m_IndexPlayerActiveGameState.Length)
            {
                return isFuFeiPlayer;
            }

            GamePlayerData playerDt = FindGamePlayerData(indexPlayer);
            if (playerDt != null)
            {
                if (playerDt.m_PlayerWeiXinData != null)
                {
                    isFuFeiPlayer = !playerDt.IsMianFeiTiYanPlayer;
                }
            }
            //SSDebug.LogWarning("GetPlayerIsFuFeiActiveGame -> indexPlayer =============================== " + indexPlayer + ", isFuFeiPlayer == " + isFuFeiPlayer);
            return isFuFeiPlayer;
        }

        /// <summary>
        /// 4个玩家激活游戏的列表状态(0 未激活, 1 激活).
        /// </summary>
        public byte[] m_IndexPlayerActiveGameState = new byte[3];
        /// <summary>
        /// 玩家进行游戏的时间.
        /// </summary>
        public class PlayerPlayGameTimeData
        {
            /// <summary>
            /// 开始游戏时间.
            /// </summary>
            float startGameTime = 0f;
            /// <summary>
            /// 结束游戏时间.
            /// </summary>
            float endGameTime = 0f;
            internal void SetStartGameTime(float time)
            {
                startGameTime = time;
            }
            internal void SetEndGameTime(float time)
            {
                endGameTime = time;
            }
            internal int GetPlayGameTime()
            {
                int time = 30;
                if (endGameTime > startGameTime)
                {
                    time = (int)(endGameTime - startGameTime) + 6;
                }
                return time;
            }
        }
        /// <summary>
        /// 玩家进行游戏的时间数据表.
        /// </summary>
        PlayerPlayGameTimeData[] m_PlayerPlayGameTimeDtArray = new PlayerPlayGameTimeData[3];
        /// <summary>
        /// 初始化.
        /// </summary>
        void InitPlayerPlayGameTimeData()
        {
            for (int i = 0; i < m_PlayerPlayGameTimeDtArray.Length; i++)
            {
                m_PlayerPlayGameTimeDtArray[i] = new PlayerPlayGameTimeData();
            }
        }
        /// <summary>
        /// 设置玩家开始游戏时间.
        /// </summary>
        void SetPlayerStartGameTime(PlayerEnum indexPlayer)
        {
            int indexVal = (int)indexPlayer - 1;
            if (indexVal < 0 || indexVal >= m_PlayerPlayGameTimeDtArray.Length)
            {
                return;
            }
            m_PlayerPlayGameTimeDtArray[indexVal].SetStartGameTime(Time.time);
            //SSDebug.LogWarning("SetPlayerStartGameTime -> indexPlayer === " + indexPlayer + ", time === " + Time.time.ToString("f2"));
        }
        /// <summary>
        /// 设置玩家结束游戏时间.
        /// </summary>
        internal void SetPlayerEndGameTime(PlayerEnum indexPlayer)
        {
            int indexVal = (int)indexPlayer - 1;
            if (indexVal < 0 || indexVal >= m_PlayerPlayGameTimeDtArray.Length)
            {
                return;
            }
            m_PlayerPlayGameTimeDtArray[indexVal].SetEndGameTime(Time.time);
            //SSDebug.LogWarning("SetPlayerEndGameTime -> indexPlayer === " + indexPlayer + ", time === " + Time.time.ToString("f2"));

            int gameTime = GetPlayerPlayGameTime(indexPlayer);
            //发送玩家游戏时长信息到红点点服务器.
            SendPlayerPlayGameTimeToServer(indexPlayer, gameTime);
        }

        /// <summary>
        /// 发送玩家游戏时长信息到红点点服务器.
        /// </summary>
        void SendPlayerPlayGameTimeToServer(PlayerEnum indexPlayer, int time)
        {
            GamePlayerData playerDt = FindGamePlayerData(indexPlayer);
            //记录玩家登陆游戏的信息.
            if (m_SSBoxPostNet != null && playerDt != null && playerDt.m_PlayerWeiXinData != null)
            {
                int userId = playerDt.m_PlayerWeiXinData.userId;
                m_SSBoxPostNet.HttpSendPostUserPlayGameTimeInfo(userId, time);
            }
        }

        /// <summary>
        /// 获取玩家完了多长时间游戏的信息.
        /// </summary>
        int GetPlayerPlayGameTime(PlayerEnum indexPlayer)
        {
            int indexVal = (int)indexPlayer - 1;
            if (indexVal < 0 || indexVal >= m_PlayerPlayGameTimeDtArray.Length)
            {
                return 30;
            }
            return m_PlayerPlayGameTimeDtArray[indexVal].GetPlayGameTime();
        }

        /// <summary>
        /// 设置玩家激活游戏状态信息.
        /// </summary>
        internal void SetIndexPlayerActiveGameState(int index, byte activeState)
        {
            //PlayerActiveState activeEnum = (PlayerActiveState)activeState;
            //SSDebug.Log("SetIndexPlayerActiveGameState -> index ======= " + index + ", activeState ======= " + activeEnum);
            //SSDebug.LogWarning("SetIndexPlayerActiveGameState -> index ======= " + index + ", activeState ======= " + activeEnum);

            PlayerEnum indexPlayer = (PlayerEnum)(index + 1);
            XKPlayerMoveCtrl playerCom = XKPlayerMoveCtrl.GetInstance(indexPlayer);
            if (playerCom != null)
            {
                if (XkPlayerCtrl.GetInstanceFeiJi() != null)
                {
                    XkPlayerCtrl.GetInstanceFeiJi().RemoveGetZhanCheDaiJinQuanPlayer(playerCom);
                }
            }

            ClearPlayerBtInfo(indexPlayer);
            m_IndexPlayerActiveGameState[index] = activeState;
            if (activeState == (int)PlayerActiveState.WeiJiHuo)
            {
                int userId = 0;
                bool isExitWeiXin = false;
                GamePlayerData playerDt = FindGamePlayerData(indexPlayer);
                if (playerDt != null)
                {
                    if (playerDt.m_PlayerWeiXinData != null)
                    {
                        userId = playerDt.m_PlayerWeiXinData.userId;
                        
                        if (playerDt.IsMianFeiTiYanPlayer == false)
                        {
                            //付费玩家进行扣费.
                            //扣除玩家红点点游戏账户金币.
                            //SubWXPlayerHddPayData(userId);
                        }
                        else
                        {
                            //是否可以继续免费玩游戏.
                            bool isCanMianFeiPlayGame = false;
                            if (XKGlobalData.GetInstance() != null)
                            {
                                isCanMianFeiPlayGame = XKGlobalData.GetInstance().GetIsCanMianFeiPlayGame(indexPlayer);
                            }

                            if (isCanMianFeiPlayGame == false)
                            {
                                //玩家不可以进行免费玩游戏.
                                //强制将玩家状态修改为付费玩家.
                                playerDt.SetIsMianFeiTiYanPlayer(false);
                            }
                        }

                        if (m_HongDDGamePadWXPay != null)
                        {
                            m_HongDDGamePadWXPay.SetFreePlayGamePlayerInfoTime(userId);
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
                            RemoveGamePlayerData(userId);
                        }
                    }
                    else
                    {
                        int coin = XKGlobalData.GetInstance().GetCoinPlayer((PlayerEnum)(playerDt.Index + 1));
                        if (coin < XKGlobalData.GameNeedCoin)
                        {
                            //玩家血值耗尽应该续费了,找到玩家数据.
                            bool isCanXuMing = false;
                            if (XKGlobalData.GetInstance().m_SSGameXuMingData != null)
                            {
                                isCanXuMing = XKGlobalData.GetInstance().m_SSGameXuMingData.GetIsCanXuMing((PlayerEnum)(playerDt.Index + 1));
                            }

                            if (isCanXuMing == false)
                            {
                                //玩家续命次数超出,不允许拉起付费.
                            }
                            else
                            {
                                Debug.Log("Unity:" + "player should buy game coin!");
                                SendWXPadShowTopUpPanel(userId);
                            }
                        }
                    }
                }
                
                SetGmWXloginDtIsActiveGame(index, false);
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
                        SetGmWXloginDtIsLoginWX(index, false);
                    }
                }
            }
            else
            {
                if (!IsHongDDShouBing)
                {
                    //软件版本测试用,模拟微信手柄登陆.
                    SetGmWXloginDtGamePadType(index, GamePadType.Null);
                    SetGmWXloginDtIsLoginWX(index, true);
                    //Debug.Log("Unity: SetIndexPlayerActiveGameState -> index == " + index);
                }
                //设置玩家开始游戏的时间.
                SetPlayerStartGameTime(indexPlayer);
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
                if (m_IndexPlayerActiveGameState[i] == (int)PlayerActiveState.WeiJiHuo)
                {
                    DaoJiShiCtrl daoJiShiCom = DaoJiShiCtrl.GetInstance((PlayerEnum)(i + 1));
                    if (daoJiShiCom != null && daoJiShiCom.IsPlayDaoJishi == false)
                    {
                        //未激活的机位索引,并且该机为当前没有播放倒计时.
                        indexPlayer = i;
                        break;
                    }
                }
            }
            return indexPlayer;
        }

        /// <summary>
        /// 玩家付费之后获取激活玩家机位的索引.
        /// </summary>
        int GetActivePlayerIndexAfterPay()
        {
            int indexPlayer = -1;
            for (int i = 0; i < m_IndexPlayerActiveGameState.Length; i++)
            {
                //SSDebug.Log("ActiveGame == " + m_IndexPlayerActiveGameState[i]);
                if (m_IndexPlayerActiveGameState[i] == (int)PlayerActiveState.WeiJiHuo)
                {
                    DaoJiShiCtrl daoJiShiCom = DaoJiShiCtrl.GetInstance((PlayerEnum)(i + 1));
                    if (daoJiShiCom != null && daoJiShiCom.IsPlayDaoJishi == false)
                    {
                        //未激活的机位索引,并且该机为当前没有播放倒计时.
                        indexPlayer = i;
                        break;
                    }
                }
            }
            return indexPlayer;
        }
        
        /// <summary>
        /// 获取可以激活玩家枚举信息.
        /// </summary>
        PlayerEnum GetCanActivePlayerEnum()
        {
            PlayerEnum indexPlayerEnum = PlayerEnum.Null;
            for (int i = 0; i < m_IndexPlayerActiveGameState.Length; i++)
            {
                //SSDebug.Log("ActiveGame == " + m_IndexPlayerActiveGameState[i]);
                if (m_IndexPlayerActiveGameState[i] == (int)PlayerActiveState.WeiJiHuo)
                {
                    PlayerEnum indexPlayer = (PlayerEnum)(i + 1);
                    DaoJiShiCtrl daoJiShiCom = DaoJiShiCtrl.GetInstance(indexPlayer);
                    if (daoJiShiCom != null && daoJiShiCom.IsPlayDaoJishi == false)
                    {
                        //未激活的机位索引,并且该机为当前没有播放倒计时.
                        indexPlayerEnum = indexPlayer;
                        break;
                    }
                }
            }
            return indexPlayerEnum;
        }

        /// <summary>
        /// 获取游戏当前是否还有空余机位.
        /// </summary>
        bool CheckIsSendGameFullMsg(int userId)
        {
            GamePlayerData playerDt = FindGamePlayerData(userId);
            if (playerDt != null)
            {
                if (playerDt.Index > -1 && playerDt.Index < m_IndexPlayerActiveGameState.Length)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                int indexPlayer = GetActivePlayerIndexAfterPay();
                if (indexPlayer > -1 && indexPlayer < m_IndexPlayerActiveGameState.Length)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        /// <summary>
        /// 获取游戏当前是否还有空余机位.
        /// </summary>
        bool GetIsHaveEmptyJiWei()
        {
            int indexPlayer = GetActivePlayerIndexAfterPay();
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
        //void OnClickWXGamePadBt(pcvr.ButtonState val, int userId)
        //{
        //    if (val == pcvr.ButtonState.DOWN)
        //    {
        //        Debug.Log("Unity: pcvr -> OnClickWXGamePadBt...");
        //        int count = m_TVYaoKongPlayerDt.Count;
        //        for (int i = 0; i < count; i++)
        //        {
        //            TVYaoKongPlayerData playerDt = m_TVYaoKongPlayerDt[i];
        //            if (playerDt != null && playerDt.m_UserId == userId)
        //            {
        //                int indexPlayer = playerDt.Index;
        //                //清理最后一个血值耗尽的玩家信息.
        //                m_TVYaoKongPlayerDt.RemoveAt(i);

        //                if (indexPlayer > -1 && indexPlayer < 4)
        //                {
        //                    switch (playerDt.m_GamePadType)
        //                    {
        //                        case GamePadType.WeiXin_ShouBing:
        //                            {
        //                                if (m_GmWXLoginDt[indexPlayer].IsLoginWX)
        //                                {
        //                                    if (!m_GmWXLoginDt[indexPlayer].IsActiveGame)
        //                                    {
        //                                        Debug.Log("Unity: click WXGamePad EnterBt -> active " + indexPlayer + " player!");
        //                                        CoinPlayerCtrl playerCoinCom = CoinPlayerCtrl.GetInstance((PlayerEnum)(indexPlayer + 1));
        //                                        if (playerCoinCom != null)
        //                                        {
        //                                            playerCoinCom.SetActiveMianFeiTiYanUI(false);
        //                                        }
        //                                        AddWeiXinGameCoinToPlayer((PlayerEnum)(indexPlayer + 1), 10);
        //                                        m_GmWXLoginDt[indexPlayer].IsActiveGame = true;
        //                                        InputEventCtrl.GetInstance().OnClickGameStartBt(indexPlayer);
        //                                    }
        //                                }
        //                                break;
        //                            }
        //                    }
        //                }
        //                break;
        //            }
        //        }
        //    }
        //}

#if USE_HDD_PAD_BT_ACTIVE_PLAYER //使用红点点手柄按键消息激活游戏主角
            
        /// <summary>
        /// 创建泛型哈希表,Key类型为int,Value类型为GamePlayerData.
        /// </summary>
        Dictionary<int, GamePlayerData> m_PlayerDataDictionary = new Dictionary<int, GamePlayerData>();
        /// <summary>
        /// 查找玩家信息.
        /// </summary>
        GamePlayerData FindGamePlayerDataByDictionary(int userId)
        {
            GamePlayerData playerDt = null;
            if (m_PlayerDataDictionary != null && m_PlayerDataDictionary.ContainsKey(userId) == true)
            {
                playerDt = m_PlayerDataDictionary[userId];
            }
            return playerDt;
        }

        /// <summary>
        /// 添加玩家数据.
        /// </summary>
        void AddGamePlayerDataByDictionary(GamePlayerData playerDt)
        {
            if (m_PlayerDataDictionary != null && playerDt != null && playerDt.m_PlayerWeiXinData != null)
            {
                if (m_PlayerDataDictionary.ContainsKey(playerDt.m_PlayerWeiXinData.userId) == false)
                {
                    m_PlayerDataDictionary.Add(playerDt.m_PlayerWeiXinData.userId, playerDt);
                }
            }
        }
        
        /// <summary>
        /// 删除玩家数据.
        /// </summary>
        void RemoveGamePlayerDataByDictionary(int userId)
        {
            if (m_PlayerDataDictionary != null)
            {
                if (m_PlayerDataDictionary.ContainsKey(userId) == true)
                {
                    m_PlayerDataDictionary.Remove(userId);
                }
            }
        }

        /// <summary>
        /// 发射按键响应.
        /// </summary>
        private void OnEventActionOperation(string val, int userId)
        {
            //bool isHaveEmptyJiWei = GetIsHaveEmptyJiWei();
            //if (isHaveEmptyJiWei == false)
            //{
            //    //当前没有空余机位.
            //    SendWXPadGamePlayerFull(userId);
            //    return;
            //}

            //Debug.Log("Unity:"+"pcvr::OnEventActionOperation -> userId " + userId + ", val " + val);
            //GamePlayerData playerDt = FindGamePlayerData(userId);
            GamePlayerData playerDt = FindGamePlayerDataByDictionary(userId);
            if (playerDt == null)
            {
                //在玩家微信数据列表中找不到该玩家的信息.
                bool isHaveEmptyJiWei = GetIsHaveEmptyJiWei();
                if (isHaveEmptyJiWei == false)
                {
                    //如果当前没有空余机位则发送当前机位已满的消息给手柄。
                    //当前没有空余机位.
                    //游戏激活人数已满.
                    SendWXPadGamePlayerFull(userId);
                }
                else
                {
                    //如果有空余机位而且可以进行免费试玩则游戏激活该玩家。
                    //当前机位有空余机位.
                    //在玩家登录数据列表中查找玩家信息.
                    GamePlayerData loginPlayerDt = FindLoginGamePlayerData(userId);
                    if (loginPlayerDt == null)
                    {
                        //玩家必须经过一次登录,否则不允许激活玩家.
                        return;
                    }

                    if (m_HongDDGamePadWXPay == null)
                    {
                        return;
                    }

                    //必须是收到开始按键按下消息才允许激活免费试玩游戏玩家.
                    if (val == PlayerShouBingFireBt.startGameBtDown.ToString())
                    //if (val == PlayerShouBingFireBt.startGameBtDown.ToString() //test
                    //    || val == PlayerShouBingFireBt.fireBDown.ToString()   //test
                    //    || val == PlayerShouBingFireBt.fireYDown.ToString()) //test
                    {
                        if (loginPlayerDt.IsMianFeiTiYanPlayer == false)
                        {
                            //该玩家在登陆后被标记为需要付费的玩家,所以这里不允许激活该玩家.
                            return;
                        }

                        //m_HongDDGamePadWXPay.m_GameConfigData.MianFeiShiWanCount = 1; //免费试玩次数.
                        if (m_HongDDGamePadWXPay.m_GameConfigData.MianFeiShiWanCount > 0)
                        {
                            //SSDebug.LogWarning("******************** userId ==================== " + userId);
                            if (m_HongDDGamePadWXPay.CheckPlayerIsCanFreePlayGame(userId) == true)
                            {
                                //该玩家可以免费试玩游戏.
                                //给玩家添加一个微信游戏币.
                                PlayerEnum playerEnum = GetCanActivePlayerEnum();
                                //SSDebug.LogWarning("******************** playerEnum ==================== " + playerEnum);
                                int indexPlayer = (int)playerEnum - 1;
                                AddWeiXinGameCoinToPlayer(playerEnum, m_HongDDGamePadWXPay.m_GameConfigData.MianFeiShiWanCount);

                                if (XKGlobalData.GetInstance() != null)
                                {
                                    //设置玩家免费次数.
                                    XKGlobalData.GetInstance().SetMianFeiNum(playerEnum, m_HongDDGamePadWXPay.m_GameConfigData.MianFeiShiWanCount);
                                }

                                //SSDebug.LogWarning("******************** activePlayer ==================== " + playerEnum);
                                //在登录玩家列表中找到了该玩家的信息.
                                //免费体验游戏的玩家.
                                loginPlayerDt.SetIsMianFeiTiYanPlayer(true);
                                //设置玩家索引信息.
                                loginPlayerDt.SetIndex(indexPlayer);
                                AddGamePlayerData(loginPlayerDt);

                                //记录玩家登陆游戏的信息.
                                if (m_SSBoxPostNet != null && loginPlayerDt.m_PlayerWeiXinData != null)
                                {
                                    //首次免费玩家.
                                    m_SSBoxPostNet.HttpSendPostUserLoginInfo(userId, loginPlayerDt.m_PlayerWeiXinData.userName, SSBoxPostNet.FuFeiState.ShouCiMianFei);
                                }

                                SetGmWXloginDtIsActiveGame(indexPlayer, true);
                                if (loginPlayerDt.m_PlayerWeiXinData != null)
                                {
                                    SetPlayerHeadUrl(indexPlayer, loginPlayerDt.m_PlayerWeiXinData.headUrl);
                                }
                                InputEventCtrl.GetInstance().OnClickGameStartBt(indexPlayer);

                                //激活玩家之后需要删除玩家登陆数据信息.
                                RemoveLoginGamePlayerData(userId);
                                
                                //发送手柄隐藏开始按键的消息.
                                SendWXPadHiddenStartBt(userId);
                                return;
                            }
                        }
                    }
                    
                    if (val == PlayerShouBingFireBt.fireBDown.ToString()
                        || val == PlayerShouBingFireBt.fireYDown.ToString())
                    {
                        if (loginPlayerDt.IsMianFeiTiYanPlayer == false)
                        {
                            //该玩家在登录后被标记为需要付费的玩家,这里认为玩家主动放弃支付.
                            //发送踢人消息.
                            SendWXPadPlayerCloseConnect(userId);
                            //踢出玩家之后需要删除玩家登录数据信息.
                            RemoveLoginGamePlayerData(userId);
                            //删除轮询检测玩家账户的数据.
                            RemoveLoopGetWXHddPayData(userId);
                            return;
                        }
                    }
                }
                return;
            }

            //在玩家微信数据列表中可以找到该玩家的信息.
            if (playerDt.Index > -1 && playerDt.Index < m_IndexPlayerActiveGameState.Length)
            {
                //Debug.Log("Unity:"+"OnEventActionOperation -> playerIndex == " + playerDt.Index);
                playerDt.IsExitWeiXin = false;
                if (m_IndexPlayerActiveGameState[playerDt.Index] == (int)PlayerActiveState.JiHuo)
                {
                    //处于激活状态的玩家才可以进行游戏操作.
//                    if (val == PlayerShouBingFireBt.fireADown.ToString()
//                        || val == PlayerShouBingFireBt.fireXDown.ToString())
//                    {
//                        //InputEventCtrl.GetInstance().OnClickFireBt(playerDt.Index, pcvr.ButtonState.DOWN);
//                        InputEventCtrl.GetInstance().OnClickDaoDanBt(playerDt.Index, pcvr.ButtonState.DOWN);
//#if TEST_DEBUG_PLAYER_PAD_INFO
//                        OnReceivePlayerPadBtInfo(PadBtState.PuTong, playerDt.Index, pcvr.ButtonState.DOWN);
//#endif
//                    }

//                    if (val == PlayerShouBingFireBt.fireAUp.ToString()
//                        || val == PlayerShouBingFireBt.fireXUp.ToString())
//                    {
//                        //InputEventCtrl.GetInstance().OnClickFireBt(playerDt.Index, pcvr.ButtonState.UP);
//                        InputEventCtrl.GetInstance().OnClickDaoDanBt(playerDt.Index, pcvr.ButtonState.UP);
//#if TEST_DEBUG_PLAYER_PAD_INFO
//                        OnReceivePlayerPadBtInfo(PadBtState.PuTong, playerDt.Index, pcvr.ButtonState.UP);
//#endif
//                    }

                    if (val == PlayerShouBingFireBt.fireBDown.ToString()
                        || val == PlayerShouBingFireBt.fireYDown.ToString())
                    {
                        //InputEventCtrl.GetInstance().OnClickFireBt(playerDt.Index, pcvr.ButtonState.DOWN);
                        InputEventCtrl.GetInstance().OnClickDaoDanBt(playerDt.Index, pcvr.ButtonState.DOWN);
#if TEST_DEBUG_PLAYER_PAD_INFO
                        OnReceivePlayerPadBtInfo(PadBtState.DaoDan, playerDt.Index, pcvr.ButtonState.DOWN);
#endif
                    }
                    else if (val == PlayerShouBingFireBt.fireBUp.ToString()
                        || val == PlayerShouBingFireBt.fireYUp.ToString())
                    {
                        //InputEventCtrl.GetInstance().OnClickFireBt(playerDt.Index, pcvr.ButtonState.UP);
                        InputEventCtrl.GetInstance().OnClickDaoDanBt(playerDt.Index, pcvr.ButtonState.UP);
#if TEST_DEBUG_PLAYER_PAD_INFO
                        OnReceivePlayerPadBtInfo(PadBtState.DaoDan, playerDt.Index, pcvr.ButtonState.UP);
#endif
                    }

                    //玩家有操作手柄按键.
                    InputEventCtrl.GetInstance().OnPlayerDoPadButton((PlayerEnum)(playerDt.Index + 1));
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
                    bool isSendMsg = false;
                    //玩家币值不够,需要拉起微信游戏手柄的充值界面.
                    //if (val == PlayerShouBingFireBt.fireADown.ToString()
                    //    || val == PlayerShouBingFireBt.fireXDown.ToString())
                    //{
                    //    //SendWXPadShowTopUpPanel(userId);
                    //    isSendMsg = true;
                    //}
                    //else if (val == PlayerShouBingFireBt.fireBDown.ToString()
                    if (val == PlayerShouBingFireBt.fireBDown.ToString()
                        || val == PlayerShouBingFireBt.fireYDown.ToString())
                    {
                        //SendWXPadShowTopUpPanel(userId);
                        isSendMsg = true;
                    }

                    if (isSendMsg == true)
                    {
                        //发送消息.
                        PlayerEnum indexPlayer = (PlayerEnum)(playerDt.Index + 1);
                        DaoJiShiCtrl daoJiShiCom = DaoJiShiCtrl.GetInstance(indexPlayer);
                        if (daoJiShiCom != null)
                        {
                            if (daoJiShiCom.GetIsPlayDaoJishi() == true && daoJiShiCom.DaoJiShiCount > 8)
                            {
                                //倒计时刚开始显示,还没有到8时,不进行消息发送.
                                isSendMsg = false;
                            }

                            if (isSendMsg == true)
                            {
                                //玩家主动放弃支付.
                                if (daoJiShiCom.GetIsPlayDaoJishi() == true)
                                {
                                    //游戏结束倒计时播放中
                                    daoJiShiCom.WXPlayerStopGameDaoJiShi();
                                }
                                else
                                {
                                    //游戏结束倒计时没有播放时.
                                    OnPlayerGameDaoJiShiOver(indexPlayer);
                                }
                                SendWXPadPlayerCloseConnect(userId);
                            }
                        }
                    }
#endif
                }
            }
        }
#endif

        /// <summary>
        /// 清理玩家的按键和移动信息.
        /// </summary>
        void ClearPlayerBtInfo(PlayerEnum indexPlayer)
        {
            int indexVal = (int)indexPlayer - 1;
            if (indexVal >= 0 && indexVal <= 4)
            {
                InputEventCtrl.GetInstance().OnClickFangXiangUBt(indexVal, pcvr.ButtonState.UP);
                InputEventCtrl.GetInstance().OnClickFangXiangDBt(indexVal, pcvr.ButtonState.UP);
                InputEventCtrl.GetInstance().OnClickFangXiangLBt(indexVal, pcvr.ButtonState.UP);
                InputEventCtrl.GetInstance().OnClickFangXiangRBt(indexVal, pcvr.ButtonState.UP);
                InputEventCtrl.GetInstance().OnClickDaoDanBt(indexVal, pcvr.ButtonState.UP);
                InputEventCtrl.GetInstance().OnClickFireBt(indexVal, pcvr.ButtonState.UP);
            }
        }

        /// <summary>
        /// 手柄方向信息.
        /// </summary>
        private void OnEventDirectionAngle(string dirValStr, int userId)
        {
            //bool isHaveEmptyJiWei = GetIsHaveEmptyJiWei();
            //if (isHaveEmptyJiWei == false)
            //{
            //    //当前没有空余机位.
            //    SendWXPadGamePlayerFull(userId);
            //    return;
            //}

            //Debug.Log("Unity:"+"pcvr::OnEventDirectionAngle -> userId " + userId + ", val " + val);
            GamePlayerData playerDt = FindGamePlayerData(userId);
            if (playerDt != null && playerDt.Index > -1 && playerDt.Index < m_IndexPlayerActiveGameState.Length)
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
#if TEST_DEBUG_PLAYER_PAD_INFO
                        OnReceivePlayerPadDirInfo(playerDt.Index, 0);
#endif
                    }
                    else
                    {
#if TEST_DEBUG_PLAYER_PAD_INFO
                        OnReceivePlayerPadDirInfo(playerDt.Index, Convert.ToInt32(dirValStr));
#endif
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

                    //玩家有操作方向.
                    InputEventCtrl.GetInstance().OnPlayerDoPadDirection((PlayerEnum)(playerDt.Index + 1));
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

#if TEST_DEBUG_PLAYER_PAD_INFO
        /// <summary>
        /// 手柄方向信息.
        /// </summary>
        int[] m_PlayerPadDirArray = new int[3];
        /// <summary>
        /// 手柄导弹按键信息.
        /// </summary>
        pcvr.ButtonState[] m_PlayerPadDaoDanBtArray = new pcvr.ButtonState[3];
        /// <summary>
        /// 手柄普通子弹按键信息.
        /// </summary>
        pcvr.ButtonState[] m_PlayerPadPuTongBtArray = new pcvr.ButtonState[3];
        public enum PadBtState
        {
            /// <summary>
            /// 普通子弹.
            /// </summary>
            PuTong = 0,
            /// <summary>
            /// 导弹.
            /// </summary>
            DaoDan = 1,
        }
        /// <summary>
        /// 收到微信手柄方向信息.
        /// </summary>
        void OnReceivePlayerPadDirInfo(int indexVal, int dir)
        {
            if (indexVal > -1 && indexVal < m_PlayerPadDirArray.Length)
            {
                m_PlayerPadDirArray[indexVal] = dir;
            }
        }
        /// <summary>
        /// 收到手柄按键信息.
        /// </summary>
        void OnReceivePlayerPadBtInfo(PadBtState padBt, int indexVal, pcvr.ButtonState btState)
        {
            if (indexVal > -1 && indexVal < 3)
            {
                switch(padBt)
                {
                    case PadBtState.PuTong:
                        {
                            m_PlayerPadPuTongBtArray[indexVal] = btState;
                            break;
                        }
                    case PadBtState.DaoDan:
                        {
                            m_PlayerPadDaoDanBtArray[indexVal] = btState;
                            break;
                        }
                }
            }
        }
        void OnGUI()
        {
            GUI.Box(new Rect(15f, 40f, 280f, 75f), "");
            string info = "PadP1 -> Dir: " + m_PlayerPadDirArray[0] + ", DaoDanBt: " + (int)m_PlayerPadDaoDanBtArray[0]
                + ", PuTongBt: " + (int)m_PlayerPadPuTongBtArray[0];
            GUI.Label(new Rect(15f, 40f, 280f, 25f), info);
            info = "PadP2 -> Dir: " + m_PlayerPadDirArray[1] + ", DaoDanBt: " + (int)m_PlayerPadDaoDanBtArray[1]
                + ", PuTongBt: " + (int)m_PlayerPadPuTongBtArray[1];
            GUI.Label(new Rect(15f, 65f, 280f, 25f), info);
            info = "PadP3 -> Dir: " + m_PlayerPadDirArray[2] + ", DaoDanBt: " + (int)m_PlayerPadDaoDanBtArray[2]
                + ", PuTongBt: " + (int)m_PlayerPadPuTongBtArray[2];
            GUI.Label(new Rect(15f, 90f, 280f, 25f), info);
        }
#endif

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

            //清理微信数据.
            m_GmWXLoginDt[indexVal].Reset();
            GamePlayerData playerData = FindGamePlayerData(indexPlayer);
            if (playerData != null)
            {
                SSDebug.Log("OnPlayerGameDaoJiShiOver -> indexPlayer ==== " + indexPlayer
                    + ", userId ============ " + playerData.m_PlayerWeiXinData.userId);
                //删除微信手柄玩家的数据信息.
                RemoveWeiXinPadPlayerData(playerData.m_PlayerWeiXinData.userId);

                //删除轮询检测玩家账户的数据.
                //RemoveLoopGetWXHddPayData(playerData.m_PlayerWeiXinData.userId);
            }
        }


        public class GameExitPadPlayerData
        {
            /// <summary>
            /// 玩家激活游戏时的索引缓存.
            /// </summary>
            public int index = -1;
            public int userId = 0;
            public float timeLast = 0f;
            public GameExitPadPlayerData(int userId, int index, float timeLast)
            {
                this.userId = userId;
                this.index = index;
                this.timeLast = timeLast;
            }
            public override string ToString()
            {
                return "userId ==== " + userId + ", timeLast ==== " + timeLast.ToString("f2");
            }
        }
        /// <summary>
        /// 退出微信手柄的玩家数据信息.
        /// </summary>
        List<GameExitPadPlayerData> m_PlayerExitPadList = new List<GameExitPadPlayerData>();

        GameExitPadPlayerData FindExitPadPlayerData(int userId)
        {
            return m_PlayerExitPadList.Find((dt) => { return dt.userId.Equals(userId); });
        }

        void AddExitPadPlayerData(GameExitPadPlayerData playerDt)
        {
            if (m_PlayerExitPadList.Contains(playerDt) == false)
            {
                m_PlayerExitPadList.Add(playerDt);
                SSDebug.Log("AddExitPadPlayerData -> playerDt == " + playerDt.ToString());
            }
        }

        /// <summary>
        /// 返回玩家之前激活游戏机位的索引信息.
        /// </summary>
        int RemoveExitPadPlayerData(int userId)
        {
            int indexVal = -1;
            GameExitPadPlayerData playerDt = FindExitPadPlayerData(userId);
            if (playerDt != null)
            {
                indexVal = playerDt.index;
                m_PlayerExitPadList.Remove(playerDt);
                SSDebug.Log("RemoveExitPadPlayerData -> playerDt == " + playerDt.ToString() + ", indexVal == " + indexVal);
            }
            return indexVal;
        }

        private void OnEventPlayerExitBox(int userId)
        {
            Debug.Log("Unity:" + "OnEventPlayerExitBox -> userId " + userId);
            int indexVal = -1;
            GamePlayerData playerDt = FindGamePlayerData(userId);
            if (playerDt != null)
            {
                indexVal = playerDt.Index;
            }

            if (FindExitPadPlayerData(userId) == null)
            {
                AddExitPadPlayerData(new GameExitPadPlayerData(userId, indexVal, Time.time));
            }

            if (playerDt != null)
            {
                playerDt.IsExitWeiXin = true;
                if (playerDt.Index > -1 && playerDt.Index < m_IndexPlayerActiveGameState.Length)
                {
                    if (m_IndexPlayerActiveGameState[playerDt.Index] == (int)PlayerActiveState.WeiJiHuo)
                    {
                        //删除轮询检测玩家账户的数据.
                        RemoveLoopGetWXHddPayData(userId);

                        //玩家血值耗尽,清理玩家微信数据.
                        RemoveGamePlayerData(userId);
                        SetGmWXloginDtIsLoginWX(playerDt.Index, false);
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
            //GamePlayerData playerDt = m_GamePlayerData.Find((dt) => { return dt.m_PlayerWeiXinData.userId.Equals(userId); });
            GamePlayerData playerDt = FindGamePlayerData(userId);
            if (playerDt != null)
            {
                PlayerEnum indexPlayer = (PlayerEnum)(playerDt.Index + 1);
                DaoJiShiCtrl daoJiShiCom = DaoJiShiCtrl.GetInstance(indexPlayer);
                if (daoJiShiCom != null)
                {
                    daoJiShiCom.WXPlayerStopGameDaoJiShi();
                }
                RemoveGamePlayerData(userId);
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
                    m_GmWXLoginDt[i].Reset();
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

#if USE_HDD_PAD_BT_ACTIVE_PLAYER //使用红点点手柄按键消息激活游戏主角
        private void OnEventPlayerLoginBox(WebSocketSimpet.PlayerWeiXinData weiXinDt)
        {
            if (weiXinDt == null)
            {
                return;
            }

            Debug.Log("Unity:" + "pcvr::OnEventPlayerLoginBox -> userName " + weiXinDt.userName + ", userId " + weiXinDt.userId
                + ", time == " + DateTime.Now.ToString());
            GameExitPadPlayerData playerExitPadDt = FindExitPadPlayerData(weiXinDt.userId);
            if (playerExitPadDt != null)
            {
                //用于玩家新进入后激活游戏机位的数据.
                RemoveExitPadPlayerData(weiXinDt.userId);
                //float timeLast = playerExitPadDt.timeLast;
                if (Time.time - playerExitPadDt.timeLast < 1.5f)
                {
                    SSDebug.LogWarning("OnEventPlayerLoginBox -> this playerPad have been yaRu shouji houTai......");
                    return;
                }
            }

            GamePlayerData playerDt = FindGamePlayerData(weiXinDt.userId);
            if (playerDt != null)
            {
                //如果玩游戏的玩家信息中有当前登陆的用户信息则不用进行任何操作.
                return;
            }

            bool isHaveEmptyJiWei = GetIsHaveEmptyJiWei();
            if (isHaveEmptyJiWei == false)
            {
                //如果当前没有空余机位则发送当前机位已满的消息给手柄。
                //当前机位没有空余机位.
                //游戏激活人数已满.
                SendWXPadGamePlayerFull(weiXinDt.userId);
                return;
            }
            else
            {
                //如果当前有空余机位但是玩家不能进行免费试玩需要发送付费消息给手柄.
                //当前机位有空余机位.
                if (m_HongDDGamePadWXPay != null)
                {
                    //是否为免费体验玩家.
                    bool isMianFeiTiYanPlayer = false;
                    bool isSendDisplayFuFeiPanel = false;
                    //m_HongDDGamePadWXPay.m_GameConfigData.MianFeiShiWanCount = 1; //免费试玩次数.
                    if (m_HongDDGamePadWXPay.m_GameConfigData.MianFeiShiWanCount > 0)
                    {
                        if (m_HongDDGamePadWXPay.CheckLoginPlayerIsCanFreePlayGame(weiXinDt.userId) == true)
                        {
                            //该玩家可以免费试玩游戏.
                            //给玩家添加一个微信游戏币.
                            //发送手柄显示开始按键的消息.
                            SendWXPadShowStartBt(weiXinDt.userId);
                            isMianFeiTiYanPlayer = true;

                            //AddWeiXinGameCoinToPlayer((PlayerEnum)(indexPlayer + 1), m_HongDDGamePadWXPay.m_GameConfigData.MianFeiShiWanCount);

                            //CoinPlayerCtrl playerCoinCom = CoinPlayerCtrl.GetInstance((PlayerEnum)(indexPlayer + 1));
                            //if (playerCoinCom != null)
                            //{
                            //    playerCoinCom.SetActiveMianFeiTiYanUI(true);
                            //}
                            //免费体验游戏的玩家.
                            //playerDt.IsMianFeiTiYanPlayer = true;

                            //记录玩家登陆游戏的信息.
                            //if (m_SSBoxPostNet != null && val != null)
                            //{
                            //首次免费玩家.
                            //    m_SSBoxPostNet.HttpSendPostUserLoginInfo(val.userId, val.userName, SSBoxPostNet.FuFeiState.ShouCiMianFei);
                            //}
                        }
                        else
                        {
                            //该玩家不可以试玩游戏.
                            //测试:直接给玩家2个游戏币.
                            //AddWeiXinGameCoinToPlayer((PlayerEnum)(indexPlayer + 1), 2); //test.

                            //获取微信玩家的红点点游戏账户数据.
                            //GetWXPlayerHddPayData(val.userId);
                            //playerDt.IsMianFeiTiYanPlayer = false;
                            //playerDt.IsGetWXPlayerHddPayData = true;

                            //如果当前有空余机位但是玩家不能进行免费试玩需要发送付费消息给手柄.
                            isSendDisplayFuFeiPanel = true;
                        }
                    }
                    else
                    {
                        //获取微信玩家的红点点游戏账户数据.
                        //GetWXPlayerHddPayData(val.userId);
                        //playerDt.IsMianFeiTiYanPlayer = false;
                        //playerDt.IsGetWXPlayerHddPayData = true;

                        //如果当前有空余机位但是玩家不能进行免费试玩需要发送付费消息给手柄.
                        isSendDisplayFuFeiPanel = true;
                    }

                    if (isSendDisplayFuFeiPanel == true)
                    {
                        //发送付费消息给手柄.
                        SendWXPadShowTopUpPanel(weiXinDt.userId);
                    }

                    GamePlayerData loginPlayerDt = FindLoginGamePlayerData(weiXinDt.userId);
                    if (loginPlayerDt != null)
                    {
                        //在已经登录的玩家数据列表中找到该玩家信息.
                        //需要将玩家数据信息设置为免费试玩玩家,便于玩家点击开始游戏按键后激活该玩家.
                        loginPlayerDt.IsMianFeiTiYanPlayer = isMianFeiTiYanPlayer;
                    }
                    else
                    {
                        //在正在进行游戏的玩家微信数据中没有找到该玩家信息.
                        //在已经登录的玩家数据列表中没有找到该玩家信息.
                        //添加该玩家信息到登录游戏的玩家数据列表中.
                        loginPlayerDt = new GamePlayerData();
                        //设置是否为免费体验玩家.
                        loginPlayerDt.IsMianFeiTiYanPlayer = isMianFeiTiYanPlayer;
                        loginPlayerDt.m_PlayerWeiXinData = weiXinDt;
                        AddLoginGamePlayerData(loginPlayerDt);
                    }
                    return;
                }
            }
        }
#endif

        /// <summary>
        /// 玩家获得免费再玩一局游戏奖品之后,使玩家免费再玩一局游戏.
        /// </summary>
        internal void MakePlayerMianFeiZaiWanYiJu(PlayerEnum indexPlayer)
        {
            int indexVal = (int)indexPlayer - 1;
            if (indexVal < 0 || indexVal >= m_IndexPlayerActiveGameState.Length)
            {
                return;
            }
            SSDebug.Log("MakePlayerMianFeiZaiWanYiJu -> indexPlayer ============================ " + indexPlayer);
            //该玩家可以免费试玩游戏.
            //给玩家添加一个微信游戏币.
            AddWeiXinGameCoinToPlayer(indexPlayer, 1);

            GamePlayerData playerDt = FindGamePlayerData(indexPlayer);
            //if (m_HongDDGamePadWXPay != null && playerDt != null && playerDt.m_PlayerWeiXinData != null)
            if (playerDt != null && playerDt.m_PlayerWeiXinData != null)
            {
                //该玩家可以免费试玩游戏.
                //给玩家添加一个微信游戏币.
                //AddWeiXinGameCoinToPlayer(indexPlayer, 1);

                //免费体验游戏的玩家.
                playerDt.SetIsMianFeiTiYanPlayer(true);

                //记录玩家登陆游戏的信息.
                if (m_SSBoxPostNet != null)
                {
                    //免费再玩一局游戏道具激活玩家.
                    m_SSBoxPostNet.HttpSendPostUserLoginInfo(playerDt.m_PlayerWeiXinData.userId, playerDt.m_PlayerWeiXinData.userName, SSBoxPostNet.FuFeiState.MianFeiZaiWanYiJu);
                }

                //m_GmWXLoginDt[indexVal].IsActiveGame = true;
                //m_PlayerHeadUrl[indexVal] = playerDt.m_PlayerWeiXinData.headUrl;
                SetPlayerHeadUrl(indexVal, playerDt.m_PlayerWeiXinData.headUrl);
                //InputEventCtrl.GetInstance().OnClickGameStartBt(indexVal);
            }
            
            //免费体验游戏的玩家.
            //playerDt.IsMianFeiTiYanPlayer = true;

            //记录玩家登陆游戏的信息.
            //if (m_SSBoxPostNet != null)
            //{
            //    m_SSBoxPostNet.HttpSendPostUserLoginInfo(playerDt.m_PlayerWeiXinData.userId, playerDt.m_PlayerWeiXinData.userName, true);
            //}
            
            SetGmWXloginDtIsActiveGame(indexVal, true);
            //m_PlayerHeadUrl[indexVal] = playerDt.m_PlayerWeiXinData.headUrl;
            InputEventCtrl.GetInstance().OnClickGameStartBt(indexVal);
        }
        
        /// <summary>
        /// 发送玩家首次免费游戏登录信息给服务器.
        /// </summary>
        internal void SendPlayerShouCiMianFeiInfoToServer(PlayerEnum indexPlayer)
        {
            int indexVal = (int)indexPlayer - 1;
            if (indexVal < 0 || indexVal >= m_IndexPlayerActiveGameState.Length)
            {
                return;
            }
            //SSDebug.LogWarning("SendPlayerShouCiMianFeiInfoToServer -> indexPlayer ============================ " + indexPlayer);
            //该玩家可以免费试玩游戏.
            //给玩家添加一个微信游戏币.
            //AddWeiXinGameCoinToPlayer(indexPlayer, 1);

            GamePlayerData playerDt = FindGamePlayerData(indexPlayer);
            if (playerDt != null && playerDt.m_PlayerWeiXinData != null)
            {
                //该玩家可以继续免费试玩游戏.
                //免费体验游戏的玩家.
                playerDt.SetIsMianFeiTiYanPlayer(true);

                //记录玩家登陆游戏的信息.
                if (m_SSBoxPostNet != null)
                {
                    //继续免费试玩游戏.
                    m_SSBoxPostNet.HttpSendPostUserLoginInfo(playerDt.m_PlayerWeiXinData.userId, playerDt.m_PlayerWeiXinData.userName, SSBoxPostNet.FuFeiState.ShouCiMianFei);
                }
                SetPlayerHeadUrl(indexVal, playerDt.m_PlayerWeiXinData.headUrl);
            }
            SetGmWXloginDtIsActiveGame(indexVal, true);
            //InputEventCtrl.GetInstance().OnClickGameStartBt(indexVal);
        }

        /// <summary>
        /// 发送玩家付费激活游戏登录信息给服务器.
        /// </summary>
        internal void SendPlayerFuFeiActiveGameInfoToServer(PlayerEnum indexPlayer)
        {
            int indexVal = (int)indexPlayer - 1;
            if (indexVal < 0 || indexVal >= m_IndexPlayerActiveGameState.Length)
            {
                return;
            }
            //SSDebug.LogWarning("SendPlayerFuFeiActiveGameInfoToServer -> indexPlayer ============================ " + indexPlayer);

            GamePlayerData playerDt = FindGamePlayerData(indexPlayer);
            if (playerDt != null && playerDt.m_PlayerWeiXinData != null)
            {
                //付费激活游戏的玩家.
                playerDt.SetIsMianFeiTiYanPlayer(false);

                //记录玩家登陆游戏的信息.
                if (m_SSBoxPostNet != null)
                {
                    //付费激活游戏的玩家.
                    m_SSBoxPostNet.HttpSendPostUserLoginInfo(playerDt.m_PlayerWeiXinData.userId, playerDt.m_PlayerWeiXinData.userName, SSBoxPostNet.FuFeiState.FuFei);
                }
                SetPlayerHeadUrl(indexVal, playerDt.m_PlayerWeiXinData.headUrl);
            }
            SetGmWXloginDtIsActiveGame(indexVal, true);
        }

        /// <summary>
        /// 添加微信玩家游戏币.
        /// </summary>
        internal void AddWeiXinGameCoinToPlayer(int index, int coin)
        {
            //查找到游戏玩家信息.
            if (index > -1 && index < m_IndexPlayerActiveGameState.Length)
            {
                PlayerEnum indexPlayer = (PlayerEnum)(index + 1);
                CoinPlayerCtrl playerCoinCom = CoinPlayerCtrl.GetInstance(indexPlayer);
                if (playerCoinCom != null)
                {
                    playerCoinCom.SetActiveMianFeiTiYanUI(false);
                }
                AddWeiXinGameCoinToPlayer(indexPlayer, coin);
            }
            //test start
            //if (m_GamePlayerData.Count > 0
            //    && m_GamePlayerData[0] != null
            //    && m_GamePlayerData[0].m_PlayerWeiXinData != null)
            //{
            //    userId = m_GamePlayerData[0].m_PlayerWeiXinData.userId; //test
            //}
            //test end

            //GamePlayerData playerData = FindGamePlayerData(userId);
            //int length = m_GamePlayerData.Count;
            //for (int i = 0; i < length; i++)
            //{
            //    if (m_GamePlayerData[i] != null
            //        && m_GamePlayerData[i].m_PlayerWeiXinData != null
            //        && m_GamePlayerData[i].m_PlayerWeiXinData.userId == userId)
            //    {
            //        playerData = m_GamePlayerData[i];
            //        break;
            //    }
            //}

            //if (playerData != null)
            //{
            //    //查找到游戏玩家信息.
            //    int index = playerData.Index;
            //    if (index > -1 && index < m_IndexPlayerActiveGameState.Length)
            //    {
            //        PlayerEnum indexPlayer = (PlayerEnum)(index + 1);
            //        CoinPlayerCtrl playerCoinCom = CoinPlayerCtrl.GetInstance(indexPlayer);
            //        if (playerCoinCom != null)
            //        {
            //            playerCoinCom.SetActiveMianFeiTiYanUI(false);
            //        }
            //        AddWeiXinGameCoinToPlayer(indexPlayer, coin);
            //    }
            //}
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

            if (coinTotal >= XKGlobalData.GameNeedCoin && XkGameCtrl.GetIsActivePlayer(playerIndex) == false)
            {
                //玩家币值足够,则开启游戏.
                InputEventCtrl.GetInstance().OnClickGameStartBt((int)playerIndex - 1);
            }
        }
        #endregion

        #region 微信游戏手柄玩家红点点账户信息管理
        
        /// <summary>
        /// 游戏币和红点点金币的转换率.
        /// 1币 == 2元人民币 == 200分.
        /// 游戏币转换为红点点账户金币（单位为分）.
        /// </summary>
        internal int m_GameCoinToMoney
        {
            get
            {
                return XKGlobalData.GetInstance().GameCoinToMoney;
            }
            set
            {
                int val = value;
                if (val < 1 || val > 10000)
                {
                    //单位是分.
                    val = 200;
                }
                XKGlobalData.GetInstance().SetGameCoinToMoneyVal(val);
            }
        }

        /// <summary>
        /// 更新游戏一币等于多少人民币的信息.
        /// 数据单位是分.
        /// </summary>
        internal void UpdateGameCoinToMoney(int args)
        {
            m_GameCoinToMoney = args;
            XKGlobalData.GetInstance().SetCoinToCardVal(args / 100f);
            //SSDebug.Log("UpdateGameCoinToMoney -> m_GameCoinToMoney ====================== " + m_GameCoinToMoney);
            //SSDebug.Log("UpdateGameCoinToMoney -> m_CoinToCard ====================== " + XKGlobalData.GetInstance().m_CoinToCard.ToString());
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
                SSDebug.Log("RemoveLoopGetWXHddPayData -> remove user data, userId ================== " + userId);
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
                float dTime = Time.time - time;
                //SSDebug.LogWarning("LoopGetWXPlayerHddPayData -> time == " + time.ToString("f2")
                    //+ ", RealTime == " + Time.time.ToString("f2")
                    //+ ", dTime == " + dTime.ToString("f2")
                    //+ ", userId == " + userId);
                if (dTime >= 60f)
                {
                    //轮询检测超时,认为玩家已经不再继续游戏了.
                    //删除轮询检测玩家账户的数据.
                    RemoveLoopGetWXHddPayData(userId);

                    //此处添加通知玩家支付超时,请稍后重新扫码的消息给服务器.
                    SendWXPadPlayerCloseConnect(userId);
                    //清除玩家的扣费数据.
                    RemovePlayerPayData(userId);
                    //删除玩家微信信息.
                    RemoveGamePlayerData(userId);
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
                    //或者是游戏倒计时结束后清除玩家的微信数据了.
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
            PlayerPayData payData = FindPlayerPayData(userId);
            if (payData != null && payData.IsDeductionSuccess == false)
            {
                SSDebug.Log("OnReceivedWXPlayerHddPayData -> player have not been deduction success! userId ===== " + userId);
                return;
            }

            //Debug.Log("OnReceivedWXPlayerHddPayData -> userId == " + userId + ", money == " + money + ", time == " + Time.time);
            //SSDebug.LogWarning("OnReceivedWXPlayerHddPayData -> userId == " + userId + ", money == " + money + ", time == " + Time.time);
            if (money < m_GameCoinToMoney)
            {
                //玩家红点点游戏平台的金额不足,应该去进行充值.
                GamePlayerData data = FindGamePlayerData(userId);
                if (data == null)
                {
                    //在微信玩家列表信息中没有找到玩家信息.
                    //说明玩家在游戏倒计时结束之后仍然没有成功交费.
                    //删除轮询检测玩家账户的数据.
                    //RemoveLoopGetWXHddPayData(userId);

                    //此处添加通知玩家支付超时,请稍后重新扫码的消息给服务器.
                    //SendWXPadPlayerPayTimeOut(userId);
                }
                else
                {
                    //if (data != null && data.IsGetWXPlayerHddPayData == true)
                    //{
                        //玩家登陆微信游戏手柄后,游戏币值不足,需要进行充值.
                        //data.IsGetWXPlayerHddPayData = false;
                        //玩家币值不够,需要拉起微信游戏手柄的充值界面.
                    //    SendWXPadShowTopUpPanel(userId);
                    //}
                }
            }
            else
            {
                GamePlayerData playerDt = FindGamePlayerData(userId);
                if (playerDt != null)
                {
                    //当前玩家数据列表里有玩家的信息.
                    bool isActivePlayer = true; //是否激活玩家.
                    if (playerDt.Index >= 0 && playerDt.Index < m_GmWXLoginDt.Length)
                    {
                        if (m_GmWXLoginDt[playerDt.Index].IsActiveGame == true)
                        {
                            //多人同时付费时,有人先激活了该机位,所以后面的玩家需要重新查找是否有空余机位.
                            bool isHaveEmptyJiWei = GetIsHaveEmptyJiWei();
                            if (isHaveEmptyJiWei == true)
                            {
                                //有空余机位.
                                int indexPlayer = GetActivePlayerIndexAfterPay();
                                if (indexPlayer > -1 && indexPlayer < m_IndexPlayerActiveGameState.Length)
                                {
                                    playerDt.SetIndex(indexPlayer);
                                }
                                else
                                {
                                    //没有找到合适机位,无法激活玩家.
                                    isActivePlayer = false;
                                    playerDt.SetIndex(-1); //禁用玩家的手柄.
                                }
                            }
                            else
                            {
                                //没有空余机位时,需要提示玩家"当前游戏人数已满".
                                SendWXPadGamePlayerFull(userId);
                                isActivePlayer = false;
                                playerDt.SetIndex(-1); //禁用玩家的手柄.
                            }
                        }
                        else
                        {
                            //当前机位还没有被玩家激活.
                        }
                    }
                    else
                    {
                        //玩家索引信息不正确.
                        //重新查找是否有剩余机位.
                        bool isHaveEmptyJiWei = GetIsHaveEmptyJiWei();
                        if (isHaveEmptyJiWei == true)
                        {
                            //有空余机位.
                            int indexPlayer = GetActivePlayerIndexAfterPay();
                            if (indexPlayer > -1 && indexPlayer < m_IndexPlayerActiveGameState.Length)
                            {
                                playerDt.SetIndex(indexPlayer);
                            }
                            else
                            {
                                //没有找到合适机位,无法激活玩家.
                                isActivePlayer = false;
                                playerDt.SetIndex(-1);//禁用玩家的手柄.
                            }
                        }
                        else
                        {
                            //没有空余机位时,需要提示玩家"当前游戏人数已满".
                            SendWXPadGamePlayerFull(userId);
                            isActivePlayer = false;
                            playerDt.SetIndex(-1);//禁用玩家的手柄.
                        }
                    }

                    if (isActivePlayer == true)
                    {
                        SSDebug.Log("Active player, userId =============== " + userId + ", Index === " + playerDt.Index);
                        SetGmWXloginDtIsActiveGame(playerDt.Index, true);
                        SetPlayerHeadUrl(playerDt.Index, playerDt.m_PlayerWeiXinData.headUrl);
                        playerDt.SetIsMianFeiTiYanPlayer(false);
                        //在微信玩家列表信息中找到了玩家信息.
                        //说明玩家是在游戏倒计时结束前成功续费的.
                        //当前玩家的红点点游戏金额兑换为游戏币.
                        int coin = money / m_GameCoinToMoney;
                        //if (coin > 10)
                        //{
                        //    //最多给玩家显示9次复活信息.
                        //    coin = 10;
                        //}
                        AddWeiXinGameCoinToPlayer(playerDt.Index, coin);

                        //记录玩家登陆游戏的信息.
                        if (m_SSBoxPostNet != null)
                        {
                            //付费玩家.
                            m_SSBoxPostNet.HttpSendPostUserLoginInfo(userId, playerDt.m_PlayerWeiXinData.userName, SSBoxPostNet.FuFeiState.FuFei);
                        }
                    }
                }
                else
                {
                    //在微信玩家列表信息中没有找到玩家信息.
                    //说明玩家是在游戏倒计时结束之后成功交费的.
                    bool isHaveEmptyJiWei = GetIsHaveEmptyJiWei();
                    if (isHaveEmptyJiWei == true)
                    {
                        //空余机位索引.
                        int indexPlayer = GetActivePlayerIndexAfterPay();
                        if (indexPlayer > -1 && indexPlayer < m_IndexPlayerActiveGameState.Length)
                        {
                            //找到空余机位.
                            SSDebug.Log("Active player, indexPlayer == " + indexPlayer + ", userId =============== " + userId);
                            LoopGetWXHddPayData playerWxHddPayDt = FindLoopGetWXHddPayData(userId);
                            if (playerWxHddPayDt != null && playerWxHddPayDt.m_GamePlayerData != null)
                            {
                                //没有超出轮询检测付费消息时间.
                                playerDt = playerWxHddPayDt.m_GamePlayerData;
                                //playerDt.m_PlayerWeiXinData = val;
                                playerDt.SetIndex(indexPlayer);
                                //m_GamePlayerData.Add(playerDt);
                                AddGamePlayerData(playerDt);
                                //isActivePlayer = true;
                                SetGmWXloginDtGamePadType(indexPlayer, GamePadType.WeiXin_ShouBing);
                                SetGmWXloginDtIsActiveGame(indexPlayer, true);
                                SetGmWXloginDtIsLoginWX(indexPlayer, true);
                                SetPlayerHeadUrl(indexPlayer, playerDt.m_PlayerWeiXinData.headUrl);
                                playerDt.SetIsMianFeiTiYanPlayer(false);

                                //当前有空余机位可以进行游戏.
                                //当前玩家的红点点游戏金额兑换为游戏币.
                                int coin = money / m_GameCoinToMoney;
                                //if (coin > 10)
                                //{
                                //    //最多给玩家显示9次复活信息.
                                //    coin = 10;
                                //}
                                AddWeiXinGameCoinToPlayer(playerDt.Index, coin);

                                //记录玩家登陆游戏的信息.
                                if (m_SSBoxPostNet != null)
                                {
                                    //付费玩家.
                                    m_SSBoxPostNet.HttpSendPostUserLoginInfo(userId, playerDt.m_PlayerWeiXinData.userName, SSBoxPostNet.FuFeiState.FuFei);
                                }
                            }
                            else
                            {
                                //轮询检测数据列表中无法找到玩家数据.
                                //SSDebug.Log("out time loop check player pay data! userId ============== " + userId);
                                //在玩家登陆列表信息中进行查找.
                                GamePlayerData loginPlayerDt = FindLoginGamePlayerData(userId);
                                if (loginPlayerDt != null)
                                {
                                    //在玩家登陆列表信息中找到玩家数据.
                                    //没有超出轮询检测付费消息时间.
                                    playerDt = loginPlayerDt;
                                    //playerDt.m_PlayerWeiXinData = val;
                                    playerDt.SetIndex(indexPlayer);
                                    //m_GamePlayerData.Add(playerDt);
                                    AddGamePlayerData(playerDt);
                                    //isActivePlayer = true;
                                    SetGmWXloginDtGamePadType(indexPlayer, GamePadType.WeiXin_ShouBing);
                                    SetGmWXloginDtIsActiveGame(indexPlayer, true);
                                    SetGmWXloginDtIsLoginWX(indexPlayer, true);
                                    SetPlayerHeadUrl(indexPlayer, playerDt.m_PlayerWeiXinData.headUrl);
                                    playerDt.SetIsMianFeiTiYanPlayer(false);

                                    //当前有空余机位可以进行游戏.
                                    //当前玩家的红点点游戏金额兑换为游戏币.
                                    int coin = money / m_GameCoinToMoney;
                                    //if (coin > 10)
                                    //{
                                    //    //最多给玩家显示9次复活信息.
                                    //    coin = 10;
                                    //}
                                    AddWeiXinGameCoinToPlayer(playerDt.Index, coin);

                                    //记录玩家登陆游戏的信息.
                                    if (m_SSBoxPostNet != null)
                                    {
                                        //付费玩家.
                                        m_SSBoxPostNet.HttpSendPostUserLoginInfo(userId, playerDt.m_PlayerWeiXinData.userName, SSBoxPostNet.FuFeiState.FuFei);
                                    }

                                    //激活玩家之后需要删除玩家登陆数据信息.
                                    RemoveLoginGamePlayerData(userId);
                                }
                            }
                        }
                        else
                        {
                            //没有找到合适的空余机位.
                            //没有空余机位时,需要提示玩家"当前游戏人数已满".
                            SendWXPadGamePlayerFull(userId);
                            playerDt.SetIndex(-1);//禁用玩家的手柄.
                        }
                    }
                    else
                    {
                        //没有空余机位时,需要提示玩家"当前游戏人数已满".
                        SendWXPadGamePlayerFull(userId);
                    }
                }

                //删除轮询检测的玩家账户数据.
                RemoveLoopGetWXHddPayData(userId);
            }
        }

        /// <summary>
        /// 玩家账户扣费数据.
        /// </summary>
        public class PlayerPayData
        {
            /// <summary>
            /// 玩家Id.
            /// </summary>
            internal int userId = 0;
            /// <summary>
            /// 是否扣费成功.
            /// </summary>
            internal bool IsDeductionSuccess = false;
            /// <summary>
            /// 扣费次数.
            /// </summary>
            int DeductionCount = 1;
            internal int GetDeductionCount()
            {
                return DeductionCount;
            }
            internal void AddDeductionCount()
            {
                DeductionCount++;
            }
            public PlayerPayData(int userId)
            {
                this.userId = userId;
            }
        }
        /// <summary>
        ///  玩家账户扣费数据列表.
        /// </summary>
        List<PlayerPayData> m_PlayerPayDataList = new List<PlayerPayData>();
        /// <summary>
        /// 查找玩家账户扣费数据.
        /// </summary>
        PlayerPayData FindPlayerPayData(int userId)
        {
            if (m_PlayerPayDataList == null)
            {
                return null;
            }

            PlayerPayData payData = m_PlayerPayDataList.Find((dt) => {
                return dt.userId.Equals(userId);
            });
            return payData;
        }

        /// <summary>
        /// 添加玩家账户扣费数据.
        /// </summary>
        void AddPlayerPayData(int userId)
        {
            PlayerPayData payData = FindPlayerPayData(userId);
            if (payData == null && m_PlayerPayDataList != null)
            {
                m_PlayerPayDataList.Add(new PlayerPayData(userId));
            }
        }

        /// <summary>
        /// 删除玩家账户扣费数据.
        /// </summary>
        void RemovePlayerPayData(int userId)
        {
            PlayerPayData payData = FindPlayerPayData(userId);
            if (payData != null && m_PlayerPayDataList != null)
            {
                m_PlayerPayDataList.Remove(payData);
            }
        }
        
        /// <summary>
        ///  玩家账户扣费数据列表.
        /// </summary>
        List<PlayerPayData> m_PlayerPayDataUpdateList = new List<PlayerPayData>();
        /// <summary>
        /// 查找玩家账户扣费数据.
        /// </summary>
        PlayerPayData FindUpdatePlayerPayData(int userId)
        {
            if (m_PlayerPayDataUpdateList == null)
            {
                return null;
            }

            PlayerPayData payData = m_PlayerPayDataUpdateList.Find((dt) => {
                return dt.userId.Equals(userId);
            });
            return payData;
        }

        /// <summary>
        /// 查找玩家账户扣费数据.
        /// </summary>
        PlayerPayData FindUpdatePlayerPayData()
        {
            if (m_PlayerPayDataUpdateList == null)
            {
                return null;
            }

            if (m_PlayerPayDataUpdateList.Count > 0)
            {
                return m_PlayerPayDataUpdateList[0];
            }
            return null;
        }

        /// <summary>
        /// 添加玩家账户扣费数据.
        /// </summary>
        void AddUpdatePlayerPayData(int userId)
        {
            PlayerPayData payData = FindUpdatePlayerPayData(userId);
            if (payData == null && m_PlayerPayDataUpdateList != null)
            {
                m_PlayerPayDataUpdateList.Add(new PlayerPayData(userId));
            }
        }

        /// <summary>
        /// 删除玩家账户扣费数据.
        /// </summary>
        void RemoveUpdatePlayerPayData(int userId)
        {
            PlayerPayData payData = FindUpdatePlayerPayData(userId);
            if (payData != null && m_PlayerPayDataUpdateList != null)
            {
                m_PlayerPayDataUpdateList.Remove(payData);
            }
        }

        /// <summary>
        /// 收到扣费回传消息.
        /// </summary>
        void OnReceivedSendPostHddSubPlayerMoneyEvent(int userId, SSBoxPostNet.BoxLoginRt type)
        {
            SSDebug.Log("OnReceivedSendPostHddSubPlayerMoneyEvent -> userId ======= " + userId + ", type ===== " + type);
            switch (type)
            {
                case SSBoxPostNet.BoxLoginRt.Success:
                    {
                        //玩家扣费成功.
                        PlayerPayData payData = FindPlayerPayData(userId);
                        if (payData != null)
                        {
                            payData.IsDeductionSuccess = true;
                            RemovePlayerPayData(userId);
                        }
                    }
                    break;
                default:
                    {
                        //玩家扣费失败.
                        //重新扣费.
                        PlayerPayData payData = FindPlayerPayData(userId);
                        if (payData != null)
                        {
                            if (payData.GetDeductionCount() < 5)
                            {
                                payData.AddDeductionCount();
                                //StartCoroutine(DelayRestartSubWXPlayerHddPayData(userId));
                                AddUpdatePlayerPayData(userId);
                            }
                            else
                            {
                                //扣费失败次数超过一定数量,不再进行扣费.
                                SSDebug.LogWarning("OnReceivedSendPostHddSubPlayerMoneyEvent -> Deduction count over! userId == " + userId);
                            }
                        }
                    }
                    break;
            }
        }

        private void Update()
        {
            UpdateSubPlayerMoneyFailedUser();
        }

        /// <summary>
        /// 更新查询减去玩家账户失败的数据.
        /// </summary>
        void UpdateSubPlayerMoneyFailedUser()
        {
            PlayerPayData payDt = FindUpdatePlayerPayData();
            if (payDt != null)
            {
                //延迟重新扣除账户信息.
                StartCoroutine(DelayRestartSubWXPlayerHddPayData(payDt.userId));
                RemoveUpdatePlayerPayData(payDt.userId);
            }
        }

        /// <summary>
        /// 延迟重新扣除账户信息.
        /// </summary>
        IEnumerator DelayRestartSubWXPlayerHddPayData(int userId)
        {
            yield return new WaitForSeconds(1f);
            SubWXPlayerHddPayData(userId);
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
                int subCoin = 1; //减去1个游戏币(2元1个游戏币).
                int money = subCoin * m_GameCoinToMoney; //扣除的红点点账户金币.
                AddPlayerPayData(userId);
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
        /// 微信支付界面数据.
        /// </summary>
        public class WeiXinPayPanelData
        {
            internal int userId = 0;
            internal float timeLast = 0;
            internal WeiXinPayPanelData(int userIdVal)
            {
                userId = userIdVal;
                timeLast = Time.time;
            }
        }
        List<WeiXinPayPanelData> m_WeiXinPayPanelDtList = new List<WeiXinPayPanelData>();

        /// <summary>
        /// 检测是否可以发送充值界面消息给服务端.
        /// </summary>
        bool CheckSendPlayerShowPayPanel(int userId)
        {
            bool isDisplayPayPanel = false;
            WeiXinPayPanelData userDt = m_WeiXinPayPanelDtList.Find((dt) => {
                return dt.userId.Equals(userId);
            });

            if (userDt == null)
            {
                m_WeiXinPayPanelDtList.Add(new WeiXinPayPanelData(userId));
                isDisplayPayPanel = true;
                if (m_WeiXinPayPanelDtList.Count > 9)
                {
                    m_WeiXinPayPanelDtList.RemoveAt(0);
                }
            }
            else
            {
                if (Time.time - userDt.timeLast > 3f)
                {
                    userDt.timeLast = Time.time;
                    isDisplayPayPanel = true;
                }
            }
            return isDisplayPayPanel;
        }

        /// <summary>
        /// 发送红点点微信游戏手柄充值界面的倒计时信息.
        /// </summary>
        void SendWXPadTopUpPayTime(int userId)
        {
            if (m_SSBoxPostNet != null && m_SSBoxPostNet.m_GamePayPlatform == SSBoxPostNet.GamePayPlatform.HongDianDian)
            {
                m_SSBoxPostNet.SendGamePayTimeInfoToHddServer(userId, 30);
            }
        }
        
        /// <summary>
        /// 发送红点点微信游戏手柄展示防沉迷的消息.
        /// </summary>
        internal void SendWXPadShowFangChenMiPanel(PlayerEnum playerIndex)
        {
            if (m_SSBoxPostNet != null && m_SSBoxPostNet.m_GamePayPlatform == SSBoxPostNet.GamePayPlatform.HongDianDian)
            {
                GamePlayerData dt = FindGamePlayerData(playerIndex);
                if (dt != null && dt.m_PlayerWeiXinData != null)
                {
                    //此处添加防沉迷网络消息代码.
                    if (m_SSBoxPostNet != null && m_SSBoxPostNet.m_WebSocketSimpet != null)
                    {
                        m_SSBoxPostNet.m_WebSocketSimpet.NetSendWXPadPlayerFangChenMiMsg(dt.m_PlayerWeiXinData.userId);
                    }
                    //35秒游戏发送踢出该玩家的消息给手柄(同时需要清除玩家的微信数据)
                    StartCoroutine(DelaySendCloseConnectFromShowFangChenMi(dt.m_PlayerWeiXinData.userId));
                }
            }
        }

        /// <summary>
        /// 当发送红点点微信游戏手柄展示防沉迷的消息之后
        /// 35秒游戏发送踢出该玩家的消息给手柄(同时需要清除玩家的微信数据)
        /// </summary>
        IEnumerator DelaySendCloseConnectFromShowFangChenMi(int userId)
        {
            yield return new WaitForSeconds(35f);
            //此处添加踢人消息.
            //游戏续命次数超出发送踢人消息.
            if (m_SSBoxPostNet != null && m_SSBoxPostNet.m_WebSocketSimpet != null)
            {
                m_SSBoxPostNet.m_WebSocketSimpet.NetSendWXPadPlayerCloseConnect(userId);
            }

            //删除玩家数据.
            RemoveGamePlayerData(userId);
        }

        /// <summary>
        /// 发送打开微信游戏手柄充值界面.
        /// </summary>
        void SendWXPadShowTopUpPanel(int userId)
        {
            if (m_SSBoxPostNet != null && m_SSBoxPostNet.m_WebSocketSimpet != null
                && m_SSBoxPostNet.m_GamePayPlatform == SSBoxPostNet.GamePayPlatform.HongDianDian)
            {
                if (CheckSendPlayerShowPayPanel(userId) == true)
                {
                    SendWXPadTopUpPayTime(userId);
                    m_SSBoxPostNet.m_WebSocketSimpet.NetSendWeiXinPadShowTopUpPanel(userId);
                }
            }
            StartCoroutine(LoopGetWXPlayerHddPayData(userId));
        }
        
        /// <summary>
        /// 发送显示微信游戏手柄开始.
        /// </summary>
        void SendWXPadShowStartBt(int userId)
        {
            if (m_SSBoxPostNet != null && m_SSBoxPostNet.m_WebSocketSimpet != null
                && m_SSBoxPostNet.m_GamePayPlatform == SSBoxPostNet.GamePayPlatform.HongDianDian)
            {
                m_SSBoxPostNet.m_WebSocketSimpet.NetSendWeiXinPadShowStartBt(userId);
            }
        }

        /// <summary>
        /// 发送隐藏微信游戏手柄开始.
        /// </summary>
        void SendWXPadHiddenStartBt(int userId)
        {
            if (m_SSBoxPostNet != null && m_SSBoxPostNet.m_WebSocketSimpet != null
                && m_SSBoxPostNet.m_GamePayPlatform == SSBoxPostNet.GamePayPlatform.HongDianDian)
            {
                m_SSBoxPostNet.m_WebSocketSimpet.NetSendWeiXinPadHiddenStartBt(userId);
            }
        }

        /// <summary>
        /// 发送显示微信游戏手柄抽奖ui.
        /// </summary>
        internal void SendWXPadShowChouJiangUI(PlayerEnum playerIndex)
        {
            if (m_SSBoxPostNet != null && m_SSBoxPostNet.m_WebSocketSimpet != null
                && m_SSBoxPostNet.m_GamePayPlatform == SSBoxPostNet.GamePayPlatform.HongDianDian)
            {
                GamePlayerData playerDt = FindGamePlayerData(playerIndex);
                if (playerDt != null && playerDt.m_PlayerWeiXinData != null)
                {
                    int userId = playerDt.m_PlayerWeiXinData.userId;
                    m_SSBoxPostNet.m_WebSocketSimpet.NetSendWeiXinPadShowChouJiangUI(userId);
                }
            }
        }

        /// <summary>
        /// 发送隐藏微信游戏手柄抽奖ui.
        /// </summary>
        internal void SendWXPadHiddenChouJiangUI(PlayerEnum playerIndex)
        {
            if (m_SSBoxPostNet != null && m_SSBoxPostNet.m_WebSocketSimpet != null
                && m_SSBoxPostNet.m_GamePayPlatform == SSBoxPostNet.GamePayPlatform.HongDianDian)
            {
                GamePlayerData playerDt = FindGamePlayerData(playerIndex);
                if (playerDt != null && playerDt.m_PlayerWeiXinData != null)
                {
                    int userId = playerDt.m_PlayerWeiXinData.userId;
                    m_SSBoxPostNet.m_WebSocketSimpet.NetSendWeiXinPadHiddenChouJiangUI(userId);
                }
            }
        }

        /// <summary>
        /// 发送玩家获取商家代金券的信息给服务器.
        /// indexPlayer玩家索引.
        /// money代金券金额(元).
        /// </summary>
        internal void SendPostHddPlayerCouponInfo(PlayerEnum indexPlayer, int money, SSCaiPiaoDataManage.GameCaiPiaoData.DaiJinQuanState daiJinQuanType)
        {
            if (XkGameCtrl.GetIsActivePlayer(indexPlayer) == false)
            {
                return;
            }

            GamePlayerData data = FindGamePlayerData(indexPlayer);
            if (data != null && data.m_PlayerWeiXinData != null)
            {
                int userId = data.m_PlayerWeiXinData.userId;
                Debug.Log("SendPostHddPlayerCouponInfo -> userId ==== " + userId + ", money ==== " + money);
                SendPostHddPlayerCouponInfo(userId, money, daiJinQuanType);
            }
        }

        /// <summary>
        /// 发送玩家获取商家代金券的信息给服务器.
        /// userId玩家id.
        /// account代金券金额(元).
        /// </summary>
        void SendPostHddPlayerCouponInfo(int userId, int account, SSCaiPiaoDataManage.GameCaiPiaoData.DaiJinQuanState daiJinQuanType)
        {
            if (m_SSBoxPostNet != null && m_SSBoxPostNet.m_BoxLoginData != null)
            {
                //boxId 游戏盒子Id.最终应该为商家id(有的商家可能是连锁店).
                string boxId = m_SSBoxPostNet.m_BoxLoginData.boxNumber;
                m_SSBoxPostNet.HttpSendPostHddPlayerCouponInfo(userId, account, boxId, daiJinQuanType);
            }
        }


        /// <summary>
        /// 通过抽奖获取的代金券.
        /// 发送玩家获取商家代金券的信息给服务器.
        /// indexPlayer玩家索引.
        /// money代金券金额(元).
        /// </summary>
        internal void SendPostHddPlayerCouponInfoByChouJiang(PlayerEnum indexPlayer, int money, SSCaiPiaoDataManage.GameCaiPiaoData.DaiJinQuanState daiJinQuanType)
        {
            if (XkGameCtrl.GetIsActivePlayer(indexPlayer) == false)
            {
                return;
            }

            GamePlayerData data = FindGamePlayerData(indexPlayer);
            if (data != null && data.m_PlayerWeiXinData != null)
            {
                int userId = data.m_PlayerWeiXinData.userId;
                Debug.Log("SendPostHddPlayerCouponInfoByChouJiang -> userId ==== " + userId + ", money ==== " + money);
                SendPostHddPlayerCouponInfoByChouJiang(userId, money, daiJinQuanType);
            }
        }

        /// <summary>
        /// 通过抽奖获取的代金券.
        /// 发送玩家获取商家代金券的信息给服务器.
        /// userId玩家id.
        /// account代金券金额(元).
        /// </summary>
        void SendPostHddPlayerCouponInfoByChouJiang(int userId, int account, SSCaiPiaoDataManage.GameCaiPiaoData.DaiJinQuanState daiJinQuanType)
        {
            if (m_SSBoxPostNet != null && m_SSBoxPostNet.m_BoxLoginData != null)
            {
                //boxId 游戏盒子Id.最终应该为商家id(有的商家可能是连锁店).
                string boxId = m_SSBoxPostNet.m_BoxLoginData.boxNumber;
                m_SSBoxPostNet.HttpSendPostHddPlayerCouponInfoByChouJiang(userId, account, boxId, daiJinQuanType);
            }
        }

        [Serializable]
        public class GamePlayerFullData
        {
            public int userId = 0;
            public float timeLast = 0f;
            public GamePlayerFullData(int userId)
            {
                this.userId = userId;
                timeLast = Time.time;
            }
        }
        public List<GamePlayerFullData> m_GamePlayerFullDtList = new List<GamePlayerFullData>();

        /// <summary>
        /// 添加玩家数据.
        /// </summary>
        void AddGamePlayerFullDataToList(GamePlayerFullData playerDt)
        {
            if (playerDt != null && m_GamePlayerFullDtList != null)
            {
                if (m_GamePlayerFullDtList.Contains(playerDt) == false)
                {
                    m_GamePlayerFullDtList.Add(playerDt);
                }
            }
        }

        /// <summary>
        /// 删除玩家数据.
        /// </summary>
        void RemoveGamePlayerFullDataFromList(GamePlayerFullData playerDt)
        {
            if (playerDt != null && m_GamePlayerFullDtList != null)
            {
                if (m_GamePlayerFullDtList.Contains(playerDt) == true)
                {
                    m_GamePlayerFullDtList.Remove(playerDt);
                }
            }
        }

        /// <summary>
        /// 检测是否发送游戏人数已满的信息.
        /// </summary>
        bool CheckIsSendWXPadGamePlayerFull(int userId)
        {
            GamePlayerFullData playerDt = m_GamePlayerFullDtList.Find((dt) => {
                return dt.userId.Equals(userId);
            });

            bool isSendMsg = false;
            if (playerDt == null)
            {
                //没有找到玩家数据,可以发送消息.
                isSendMsg = true;
                GamePlayerFullData playerDtTmp = new GamePlayerFullData(userId);
                AddGamePlayerFullDataToList(playerDtTmp);
                //循环检测.
                StartCoroutine(LoopCheckSendWXPadGamePlayerFull(playerDtTmp));
            }
            else
            {
                if (Time.time - playerDt.timeLast > 1.5f)
                {
                    //找到玩家数据,间隔时间大于一定数值,可以发送消息.
                    isSendMsg = true;
                    playerDt.timeLast = Time.time;
                }
            }
            return isSendMsg;
        }

        /// <summary>
        /// 循环检测是否发送没有多余机位消息的数据.
        /// </summary>
        IEnumerator LoopCheckSendWXPadGamePlayerFull(GamePlayerFullData playerDt)
        {
            if (playerDt == null)
            {
                yield break;
            }

            yield return new WaitForSeconds(1f);
            bool isLoopContinue = true;
            do
            {
                if (playerDt == null)
                {
                    isLoopContinue = false;
                    yield break;
                }
                else
                {
                    if (Time.time - playerDt.timeLast > 30f)
                    {
                        //删除玩家数据.
                        RemoveGamePlayerFullDataFromList(playerDt);
                        playerDt = null;
                        isLoopContinue = false;
                        SSDebug.Log("LoopCheckSendWXPadGamePlayerFull -> RemoveGamePlayerFullDataFromList...");
                        yield break;
                    }
                }
                yield return new WaitForSeconds(1f);
            }
            while (isLoopContinue == true);
        }

        /// <summary>
        /// 发送游戏当前处于满员状态消息给微信游戏手柄.
        /// 当前游戏人数已满,请稍后重新扫码.
        /// </summary>
        void SendWXPadGamePlayerFull(int userId)
        {
            if (m_SSBoxPostNet != null && m_SSBoxPostNet.m_WebSocketSimpet != null
                && m_SSBoxPostNet.m_GamePayPlatform == SSBoxPostNet.GamePayPlatform.HongDianDian)
            {
                if (CheckIsSendWXPadGamePlayerFull(userId) == true)
                {
                    m_SSBoxPostNet.m_WebSocketSimpet.NetSendWeiXinPadGamePlayerFull(userId);
                }
            }

            //发送踢人消息.
            StartCoroutine(DelaySendWXPadPlayerCloseConnectMsg(userId));
        }
        
        /// <summary>
        /// 玩家付费超时或主动退出付费界面的数据.
        /// </summary>
        public class PayTimeOutData
        {
            public int userId = 0;
            public PayTimeOutData(int userId)
            {
                this.userId = userId;
            }
        }
        List<PayTimeOutData> m_PayTimeOutDtList = new List<PayTimeOutData>();
        PayTimeOutData FindPayTimeOutData(int userId)
        {
            PayTimeOutData payTimeOutDt = null;
            if (m_PayTimeOutDtList != null)
            {
                payTimeOutDt = m_PayTimeOutDtList.Find((dt) => {
                    return dt.userId.Equals(userId);
                });
            }
            return payTimeOutDt;
        }

        void AddPayTimeOutData(int userId)
        {
            if (FindPayTimeOutData(userId) == null)
            {
                if (m_PayTimeOutDtList != null)
                {
                    m_PayTimeOutDtList.Add(new PayTimeOutData(userId));
                }
            }
        }

        void RemovePayTimeOutData(int userId)
        {
            PayTimeOutData dt = FindPayTimeOutData(userId);
            if (dt != null)
            {
                m_PayTimeOutDtList.Remove(dt);
            }
        }

        IEnumerator DelayRemovePayTimeOutData(int userId)
        {
            yield return new WaitForSeconds(2f);
            RemovePayTimeOutData(userId);
        }

        /// <summary>
        /// 发送玩家支付超时或主动放弃支付后请稍后重新扫码的消息给服务器.
        /// </summary>
        void SendWXPadPlayerCloseConnect(int userId)
        {
            if (m_SSBoxPostNet != null && m_SSBoxPostNet.m_WebSocketSimpet != null
                && m_SSBoxPostNet.m_GamePayPlatform == SSBoxPostNet.GamePayPlatform.HongDianDian)
            {
                if (FindPayTimeOutData(userId) == null)
                {
                    //在付费超时或主动退出付费界面的玩家数据中找不到该玩家信息时,允许发送付费超时信息.
                    //并且将玩家信息踢出.
                    AddPayTimeOutData(userId);
                    StartCoroutine(DelayRemovePayTimeOutData(userId));
                    //付费超时发送踢人消息.
                    m_SSBoxPostNet.m_WebSocketSimpet.NetSendWXPadPlayerCloseConnect(userId);
                }
            }
        }

        /// <summary>
        /// 延迟发送踢人消息.
        /// </summary>
        IEnumerator DelaySendWXPadPlayerCloseConnectMsg(int userId)
        {
            yield return new WaitForSeconds(5f);
            if (m_SSBoxPostNet != null
                && m_SSBoxPostNet.m_WebSocketSimpet != null
                && m_SSBoxPostNet.m_GamePayPlatform == SSBoxPostNet.GamePayPlatform.HongDianDian)
            {
                //在游戏没有空余机位时延迟一定时间发送玩家被踢出的消息给服务器.
                //并且将玩家踢出.
                //没有空余机位发送踢人消息.
                m_SSBoxPostNet.m_WebSocketSimpet.NetSendWXPadPlayerCloseConnect(userId);
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
        
        /// <summary>
        /// 关闭WebSocket
        /// </summary>
        internal void CloseWebSocket()
        {
            if (m_SSBoxPostNet != null)
            {
                m_SSBoxPostNet.CloseWebSocket();
            }
        }
        
        /// <summary>
        /// 获取游戏在红点点平台的屏幕码信息.
        /// </summary>
        internal void GetGameHddScreenNum()
        {
            if (m_SSBoxPostNet != null
                && m_SSBoxPostNet.m_GamePayPlatform == SSBoxPostNet.GamePayPlatform.HongDianDian)
            {
                m_SSBoxPostNet.HttpSendGetGameScreenId();
            }
        }

        /// <summary>
        /// 当心跳消息检测超时来自网络故障UI提示.
        /// </summary>
        internal void OnXiTiaoMsgTimeOutFromWangLuoGuZhang()
        {
            if (m_SSBoxPostNet != null
                && m_SSBoxPostNet.m_WebSocketSimpet != null
                && m_SSBoxPostNet.m_GamePayPlatform == SSBoxPostNet.GamePayPlatform.HongDianDian)
            {
                m_SSBoxPostNet.m_WebSocketSimpet.OnXiTiaoMsgTimeOutFromWangLuoGuZhang();
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
