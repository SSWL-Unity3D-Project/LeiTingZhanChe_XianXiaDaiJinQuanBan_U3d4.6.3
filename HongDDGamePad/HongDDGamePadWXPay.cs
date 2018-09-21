using System.Collections.Generic;

namespace Assets.XKGame.Script.HongDDGamePad
{
    /// <summary>
    /// 红点点微信虚拟游戏手柄微信支付控制单元.
    /// </summary>
    public class HongDDGamePadWXPay
    {
        #region 试玩游戏的玩家数据管理.
        /// <summary>
        /// 免费玩过游戏的玩家数据.
        /// </summary>
        public class FreePlayGamePlayerData
        {
            /// <summary>
            /// 玩家Id.
            /// </summary>
            public int UserId = 0;
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

            bool isCanPlayGame = AddFreePlayGamePlayerInfo(userId);
            return isCanPlayGame;
        }

        /// <summary>
        /// 添加免费试玩游戏的玩家信息.
        /// </summary>
        bool AddFreePlayGamePlayerInfo(int userId)
        {
            bool isCanFreePlayGame = false;
            FreePlayGamePlayerData playerDt = m_FreePlayGamePlayerDataList.Find((dt) => { return dt.UserId.Equals(userId); });
            if (playerDt == null)
            {
                playerDt = new FreePlayGamePlayerData();
                playerDt.UserId = userId;
                m_FreePlayGamePlayerDataList.Add(playerDt);

                //免费试玩游戏玩家信息记录12个,超过12个后之前被挤出的玩家可以再次免费试玩游戏!
                if (m_FreePlayGamePlayerDataList.Count > 12)
                {
                    //删除试玩游戏玩家列表信息的第一个元素.
                    m_FreePlayGamePlayerDataList.RemoveAt(0);
                }
                //可以免费试玩游戏.
                isCanFreePlayGame = true;
            }
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
        #endregion

        #region 服务器或手柄发送给游戏客户端的消息.
        /// <summary>
        /// 服务器返回当前游戏设备的配置信息(是否可以试玩)
        /// </summary>
        public void SToC_ReceiveGameConfigInfo(string args)
        {
            UnityEngine.Debug.Log("Unity: SToC_ReceiveGameConfigInfo -> args == " + args);
            int mianFeiCount = 1;
            m_GameConfigData.MianFeiShiWanCount = mianFeiCount;
        }

        /// <summary>
        /// 服务器发送玩家充值成功或失败(失败信息包括充值异常和充值超时等内容)的消息给游戏客户端,
        /// 方便游戏完成激活对应玩家游戏或踢除占位而没有进行付费的玩家信息.
        /// 如果玩家充值成功,需要服务器返回给游戏客户端的消息中包含玩家选择的复活次数信息,方便游戏给玩家
        /// 分配对应的复活次数信息.
        /// </summary>
        public void SToC_PlayerPayStateInfo(string args)
        {
            UnityEngine.Debug.Log("Unity: SToC_PlayerPayStateInfo -> args == " + args);
            bool isPaySuccess = false;
            isPaySuccess = true; //test
            if (isPaySuccess == true)
            {
                //支付成功.
                int userId = 0;
                int fuHuoCiShu = 2;
                int gameCoin = fuHuoCiShu + 1;
                if (pcvr.GetInstance() != null)
                {
                    pcvr.GetInstance().m_HongDDGamePadInterface.AddWeiXinGameCoinToPlayer(userId, gameCoin);
                }
            }
            else
            {
                //支付失败.
                bool isPayTimeOut = false; //是否支付超时.
                if (isPayTimeOut == true)
                {
                    //支付超时.
                    //剔除占位而没有付款的玩家.
                    int userId = 0;
                    if (pcvr.GetInstance() != null)
                    {
                        pcvr.GetInstance().m_HongDDGamePadInterface.RemoveWeiXinPadPlayerData(userId);
                    }
                }
            }
        }
        #endregion
    }
}
