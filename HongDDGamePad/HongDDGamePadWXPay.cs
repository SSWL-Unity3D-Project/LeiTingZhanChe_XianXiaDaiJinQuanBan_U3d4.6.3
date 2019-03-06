using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;

namespace Assets.XKGame.Script.HongDDGamePad
{
    /// <summary>
    /// 红点点微信虚拟游戏手柄微信支付控制单元.
    /// </summary>
    public class HongDDGamePadWXPay
    {
        #region 试玩游戏的玩家数据管理.
        internal void Init()
        {
            ReadGamePlayerData();
        }

        string m_FileName = "GamePlayerInfo.db";
        /// <summary>
        /// 创建配置文件.
        /// </summary>
        void CreatGamePlayerData(string filepath)
        {
            XmlDocument xmlDoc = new XmlDocument();
            XmlElement root = xmlDoc.CreateElement("ConfigData");
            for (int i = 0; i < m_MaxPlayerNum; i++)
            {
                //玩家信息.
                XmlElement elmNew = xmlDoc.CreateElement("PlayerData");
                root.AppendChild(elmNew);
            }
            xmlDoc.AppendChild(root);
            xmlDoc.Save(filepath);
            File.SetAttributes(filepath, FileAttributes.Normal);
        }

        /// <summary>
        /// 从配置文件读取玩家的数据信息.
        /// </summary>
        void ReadGamePlayerData()
        {
            string filepath = Application.dataPath + "/" + m_FileName;
#if UNITY_ANDROID
		    filepath = Application.persistentDataPath + "//" + m_FileName;
#endif
            //create file
            if (!File.Exists(filepath))
            {
                CreatGamePlayerData(filepath);
            }

            if (File.Exists(filepath))
            {
                try
                {
                    string elementName = "PlayerData";
                    string attribute1 = "UserId";
                    string attribute2 = "TimeVal";
                    string valueStr1 = "";
                    string valueStr2 = "";
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.Load(filepath);
                    XmlNodeList nodeList = xmlDoc.SelectSingleNode("ConfigData").ChildNodes;
                    foreach (XmlElement xe in nodeList)
                    {
                        if (xe.Name == elementName)
                        {
                            valueStr1 = xe.GetAttribute(attribute1);
                            valueStr2 = xe.GetAttribute(attribute2);
                            if (valueStr1 != null && valueStr1 != "" && valueStr2 != null && valueStr2 != "")
                            {
                                SSDebug.Log("ReadGamePlayerData -> userId == " + valueStr1 + ", timeVal == " + valueStr2);
                                AddFreePlayGamePlayerInfo(System.Convert.ToInt32(valueStr1), System.Convert.ToDateTime(valueStr2));
                            }
                        }
                    }
                    File.SetAttributes(filepath, FileAttributes.Normal);
                    xmlDoc.Save(filepath);
                }
                catch (System.Exception exception)
                {
                    File.SetAttributes(filepath, FileAttributes.Normal);
                    File.Delete(filepath);
                    SSDebug.LogError("error: xml was wrong! " + exception);
                }
            }
        }

        /// <summary>
        /// 向配置文件写入玩家的数据信息.
        /// </summary>
        void WriteGamePlayerData()
        {
            string filepath = Application.dataPath + "/" + m_FileName;
#if UNITY_ANDROID
		    filepath = Application.persistentDataPath + "//" + m_FileName;
#endif

            //create file
            if (!File.Exists(filepath))
            {
                CreatGamePlayerData(filepath);
            }

            //update value
            if (File.Exists(filepath))
            {
                try
                {
                    string elementName = "PlayerData";
                    string attribute1 = "UserId";
                    string attribute2 = "TimeVal";
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.Load(filepath);
                    XmlNodeList nodeList = xmlDoc.SelectSingleNode("ConfigData").ChildNodes;
                    int countNum = m_FreePlayGamePlayerDataList.Count;
                    if (countNum > 0)
                    {
                        int indexVal = 0;
                        foreach (XmlElement xe in nodeList)
                        {
                            if (xe.Name == elementName && countNum > indexVal)
                            {
                                //SSDebug.Log("WriteGamePlayerData -> userId == " + m_FreePlayGamePlayerDataList[indexVal].UserId
                                //    + ", timeVal == " + m_FreePlayGamePlayerDataList[indexVal].TimeVal.ToString("G"));
                                xe.SetAttribute(attribute1, m_FreePlayGamePlayerDataList[indexVal].UserId.ToString());
                                xe.SetAttribute(attribute2, m_FreePlayGamePlayerDataList[indexVal].TimeVal.ToString("G"));
                                indexVal++;
                            }
                        }
                    }
                    File.SetAttributes(filepath, FileAttributes.Normal);
                    xmlDoc.Save(filepath);
                }
                catch(System.Exception exception)
                {
                    File.SetAttributes(filepath, FileAttributes.Normal);
                    File.Delete(filepath);
                    SSDebug.LogError("error: xml was wrong! " + exception);
                }
            }
        }

        /// <summary>
        /// 免费玩过游戏的玩家数据.
        /// </summary>
        public class FreePlayGamePlayerData
        {
            /// <summary>
            /// 玩家Id.
            /// </summary>
            public int UserId = 0;
            /// <summary>
            /// 时间信息记录.
            /// </summary>
            public System.DateTime TimeVal = System.DateTime.Now;
            public FreePlayGamePlayerData()
            {
            }
            public FreePlayGamePlayerData(int userId, System.DateTime timeVal)
            {
                UserId = userId;
                TimeVal = timeVal;
            }
        }
        /// <summary>
        /// 免费玩过游戏的玩家数据列表信息.
        /// </summary>
        List<FreePlayGamePlayerData> m_FreePlayGamePlayerDataList = new List<FreePlayGamePlayerData>();

        /// <summary>
        /// 检测玩家是否可以免费试玩游戏.
        /// </summary>
        public bool CheckPlayerIsCanFreePlayGame(int userId)
        {
            if (m_GameConfigData.MianFeiShiWanCount == 0)
            {
                //后台配置信息里免费试玩次数为0.
                return false;
            }

            bool isCanPlayGame = GetPlayerIsCanFreePlayGame(userId);
            return isCanPlayGame;
        }
        
        /// <summary>
        /// 检测登录游戏的玩家是否可以免费试玩游戏.
        /// </summary>
        public bool CheckLoginPlayerIsCanFreePlayGame(int userId)
        {
            if (m_GameConfigData.MianFeiShiWanCount == 0)
            {
                //后台配置信息里免费试玩次数为0.
                return false;
            }

            bool isCanPlayGame = GetLoginPlayerIsCanFreePlayGame(userId);
            return isCanPlayGame;
        }

        /// <summary>
        /// 添加数据.
        /// </summary>
        void AddFreePlayGamePlayerInfo(int userId, System.DateTime timeVal)
        {
            FreePlayGamePlayerData playerDt = m_FreePlayGamePlayerDataList.Find((dt) => { return dt.UserId.Equals(userId); });
            if (playerDt == null)
            {
                m_FreePlayGamePlayerDataList.Add(new FreePlayGamePlayerData(userId, timeVal));
            }
        }

        /// <summary>
        /// 当玩家血值耗尽后更新时间数据.
        /// </summary>
        internal void SetFreePlayGamePlayerInfoTime(int userId)
        {
            FreePlayGamePlayerData playerDt = m_FreePlayGamePlayerDataList.Find((dt) => { return dt.UserId.Equals(userId); });
            if (playerDt != null)
            {
                playerDt.TimeVal = System.DateTime.Now;
                //将玩家信息存入配置信息文件中.
                WriteGamePlayerData();
            }
        }

        /// <summary>
        /// 玩家数量最大值信息.
        /// </summary>
        int m_MaxPlayerNum = 90;
        /// <summary>
        /// 获取玩家是否可以免费试玩游戏.
        /// </summary>
        bool GetPlayerIsCanFreePlayGame(int userId)
        {
            bool isCanFreePlayGame = false;
            FreePlayGamePlayerData playerDt = m_FreePlayGamePlayerDataList.Find((dt) => { return dt.UserId.Equals(userId); });
            if (playerDt == null)
            {
                playerDt = new FreePlayGamePlayerData();
                playerDt.UserId = userId;
                playerDt.TimeVal = System.DateTime.Now;
                m_FreePlayGamePlayerDataList.Add(playerDt);

                //免费试玩游戏玩家信息记录m_MaxPlayerNum个,超过m_MaxPlayerNum个后之前被挤出的玩家可以再次免费试玩游戏!
                if (m_FreePlayGamePlayerDataList.Count > m_MaxPlayerNum)
                {
                    //删除试玩游戏玩家列表信息的第一个元素.
                    m_FreePlayGamePlayerDataList.RemoveAt(0);
                }
                //将玩家信息存入配置信息文件中.
                WriteGamePlayerData();
                //可以免费试玩游戏.
                isCanFreePlayGame = true;
            }
            else
            {
                //数据列表里有玩家的记录信息.
                System.DateTime timeNow = System.DateTime.Now;
                System.DateTime timeRecord = playerDt.TimeVal;

                System.TimeSpan ts1 = new System.TimeSpan(timeNow.Ticks);
                System.TimeSpan ts2 = new System.TimeSpan(timeRecord.Ticks);
                System.TimeSpan ts = ts2.Subtract(ts1).Duration();

                int dTime = ts.Hours * 3600 + ts.Minutes * 60 + ts.Seconds;
                int minTime = XKGlobalData.GetInstance().m_TimeMianFeiNum * 60; //秒.
                if (dTime > minTime)
                {
                    playerDt.TimeVal = System.DateTime.Now;
                    //时间差值大于免费间隔时间后,可以被激活.
                    //可以免费试玩游戏.
                    isCanFreePlayGame = true;

                    //将玩家信息存入配置信息文件中.
                    WriteGamePlayerData();
                }
                //SSDebug.Log("GetPlayerIsCanFreePlayGame -> dTime =============== " + dTime + "s");
            }

            //yyyy - MM - dd hh: mm: ss
            //System.DateTime t1 = System.DateTime.Now;
            //System.DateTime t2 = System.Convert.ToDateTime("2019-01-07 13:45:10");
            //System.TimeSpan ts11 = new System.TimeSpan(t1.Ticks);
            //System.TimeSpan ts21 = new System.TimeSpan(t2.Ticks);
            //System.TimeSpan ts00 = ts21.Subtract(ts11).Duration();
            //int dTime11 = ts00.Hours * 3600 + ts00.Minutes * 60 + ts00.Seconds;
            //SSDebug.Log("AddFreePlayGamePlayerInfo -> dTime =============== " + dTime11 + "s");
            return isCanFreePlayGame;
        }

        /// <summary>
        /// 获取登录游戏的玩家是否可以免费试玩游戏.
        /// </summary>
        bool GetLoginPlayerIsCanFreePlayGame(int userId)
        {
            bool isCanFreePlayGame = false;
            FreePlayGamePlayerData playerDt = m_FreePlayGamePlayerDataList.Find((dt) => { return dt.UserId.Equals(userId); });
            if (playerDt == null)
            {
                //可以免费试玩游戏.
                isCanFreePlayGame = true;
            }
            else
            {
                //数据列表里有玩家的记录信息.
                System.DateTime timeNow = System.DateTime.Now;
                System.DateTime timeRecord = playerDt.TimeVal;

                System.TimeSpan ts1 = new System.TimeSpan(timeNow.Ticks);
                System.TimeSpan ts2 = new System.TimeSpan(timeRecord.Ticks);
                System.TimeSpan ts = ts2.Subtract(ts1).Duration();

                int dTime = ts.Hours * 3600 + ts.Minutes * 60 + ts.Seconds;
                int minTime = XKGlobalData.GetInstance().m_TimeMianFeiNum * 60; //秒.
                if (dTime > minTime)
                {
                    //playerDt.TimeVal = System.DateTime.Now;
                    //时间差值大于免费间隔时间后,可以被激活.
                    //可以免费试玩游戏.
                    isCanFreePlayGame = true;
                }
                //SSDebug.LogWarning("GetLoginPlayerIsCanFreePlayGame -> dTime =============== " + dTime + "s");
            }

            //yyyy - MM - dd hh: mm: ss
            //System.DateTime t1 = System.DateTime.Now;
            //System.DateTime t2 = System.Convert.ToDateTime("2019-01-07 13:45:10");
            //System.TimeSpan ts11 = new System.TimeSpan(t1.Ticks);
            //System.TimeSpan ts21 = new System.TimeSpan(t2.Ticks);
            //System.TimeSpan ts00 = ts21.Subtract(ts11).Duration();
            //int dTime11 = ts00.Hours * 3600 + ts00.Minutes * 60 + ts00.Seconds;
            //SSDebug.Log("GetLoginPlayerIsCanFreePlayGame -> dTime =============== " + dTime11 + "s");
            return isCanFreePlayGame;
        }
        #endregion

        #region 游戏后台服务器配置信息管理.
        /// <summary>
        /// 游戏配置信息.
        /// </summary>
        public class GameConfigData
        {
            /// <summary>
            /// 免费试玩生命次数.
            /// </summary>
            public int MianFeiShiWanCount = 0;
        }
        /// <summary>
        /// 游戏在后台服务器的配置信息.
        /// </summary>
        public GameConfigData m_GameConfigData = new GameConfigData();
        #endregion

        #region 游戏客户端发送给服务器或手柄的消息.
        /// <summary>
        /// 游戏向服务端请求当前设备的游戏配置信息(是否可以试玩)
        /// </summary>
        public void CToS_RequestGameConfigInfo(string args)
        {
            UnityEngine.Debug.Log("Unity: CToS_RequestGameConfigInfo -> args == " + args);
        }

        /// <summary>
        /// 游戏通知服务器或手柄当前游戏试玩次数信息,方便手机端拉起支付或手柄界面.
        /// </summary>
        public void CToS_GameIsCanFreePlay(string args)
        {
            UnityEngine.Debug.Log("Unity: CToS_GameIsCanFreePlay -> args == " + args
                + ", FreePlayCount == " + m_GameConfigData.MianFeiShiWanCount);
        }

        /// <summary>
        /// 游戏通知服务器或手柄当前游戏人数已满,此时手机端应该提示"当前人数已满,请稍后重新扫码"
        /// </summary>
        public void CToS_GamePlayerIsFull(string args)
        {
            UnityEngine.Debug.Log("Unity: CToS_GamePlayerIsFull -> args == " + args);
        }

        /// <summary>
        /// 游戏通知服务器或手柄当前玩家以阵亡,需要手机端拉起复活界面.
        /// </summary>
        public void CToS_OnPlayerDeath(string args)
        {
            UnityEngine.Debug.Log("Unity: CToS_OnPlayerDeath -> args == " + args);
        }

        /// <summary>
        /// 当玩家收到代金券时,游戏将信息发送给服务器.
        /// </summary>
        public void CToS_OnPlayerReceiveDaiJinQuan(string args)
        {
            UnityEngine.Debug.Log("Unity: CToS_OnPlayerReceiveDaiJinQuan -> args == " + args);
        }
        #endregion

        #region 服务器或手柄发送给游戏客户端的消息.
        /// <summary>
        /// 服务器返回当前游戏设备的配置信息(是否可以试玩)
        /// </summary>
        public void SToC_ReceiveGameConfigInfo(int args)
        {
            SSDebug.LogWarning("SToC_ReceiveGameConfigInfo -> args ================ " + args);
            //m_GameConfigData.MianFeiShiWanCount = Mathf.Clamp(args, 0, 1);
            m_GameConfigData.MianFeiShiWanCount = Mathf.Clamp(args, 0, 3); //最多3次免费机会.
        }

        /// <summary>
        /// 服务器发送玩家充值成功或失败(失败信息包括充值异常和充值超时等内容)的消息给游戏客户端,
        /// 方便游戏完成激活对应玩家游戏或踢除占位而没有进行付费的玩家信息.
        /// 如果玩家充值成功,需要服务器返回给游戏客户端的消息中包含玩家选择的复活次数信息,方便游戏给玩家
        /// 分配对应的复活次数信息.
        /// </summary>
        //public void SToC_PlayerPayStateInfo(string args)
        //{
        //    UnityEngine.Debug.Log("Unity: SToC_PlayerPayStateInfo -> args == " + args);
        //    bool isPaySuccess = false;
        //    isPaySuccess = true; //test
        //    if (isPaySuccess == true)
        //    {
        //        //支付成功.
        //        int userId = 0;
        //        int fuHuoCiShu = 2;
        //        int gameCoin = fuHuoCiShu + 1;
        //        if (pcvr.GetInstance() != null)
        //        {
        //            pcvr.GetInstance().m_HongDDGamePadInterface.AddWeiXinGameCoinToPlayer(userId, gameCoin);
        //        }
        //    }
        //    else
        //    {
        //        //支付失败.
        //        bool isPayTimeOut = false; //是否支付超时.
        //        if (isPayTimeOut == true)
        //        {
        //            //支付超时.
        //            //剔除占位而没有付款的玩家.
        //            int userId = 0;
        //            if (pcvr.GetInstance() != null)
        //            {
        //                pcvr.GetInstance().m_HongDDGamePadInterface.RemoveWeiXinPadPlayerData(userId);
        //            }
        //        }
        //    }
        //}
        #endregion
    }
}
