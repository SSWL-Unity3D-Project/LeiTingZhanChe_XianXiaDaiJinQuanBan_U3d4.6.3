using UnityEngine;

namespace Assets.XKGame.Script.HongDDGamePad
{
    /// <summary>
    /// 红点点微信虚拟游戏手柄控制接口.
    /// </summary>
    public class HongDDGamePadInterface : MonoBehaviour
    {
        #region 红点点微信虚拟游戏手柄控制接口
        /// <summary>
        /// 红点点微信手柄消息处理组件.
        /// </summary>
        HongDDGamePad m_HongDDGamePadCom;

        /// <summary>
        /// 创建红点点微信虚拟游戏手柄组件.
        /// </summary>
        internal void CreatHongDDGanePad()
        {
            if (pcvr.IsHongDDShouBing == true)
            {
                m_HongDDGamePadCom = gameObject.AddComponent<HongDDGamePad>();
                if (m_HongDDGamePadCom != null)
                {
                    m_HongDDGamePadCom.Init();
                }
            }
        }

        /// <summary>
        /// 获取微信小程序虚拟手柄类型.
        /// </summary>
        internal SSBoxPostNet.WeiXinShouBingEnum GetWXShouBingType()
        {
            if (m_HongDDGamePadCom == null)
            {
                return SSBoxPostNet.WeiXinShouBingEnum.XiaoChengXu;
            }
            return m_HongDDGamePadCom.m_WXShouBingType;
        }

        /// <summary>
        /// 获取二维码生成脚本组件.
        /// </summary>
        internal BarcodeCam GetBarcodeCam()
        {
            if (m_HongDDGamePadCom == null)
            {
                return null;
            }
            return m_HongDDGamePadCom.m_BarcodeCam;
        }

        /// <summary>
        /// 获取BoxPostNet控制组件.
        /// </summary>
        internal SSBoxPostNet GetBoxPostNet()
        {
            if (m_HongDDGamePadCom == null)
            {
                return null;
            }
            return m_HongDDGamePadCom.m_SSBoxPostNet;
        }

        /// <summary>
        /// 获取红点点微信虚拟游戏手柄微信支付控制组件.
        /// </summary>
        internal HongDDGamePadWXPay GetHongDDGamePadWXPay()
        {
            if (m_HongDDGamePadCom == null)
            {
                return null;
            }
            return m_HongDDGamePadCom.m_HongDDGamePadWXPay;
        }

        /// <summary>
        /// 添加遥控器按键信息响应事件.
        /// </summary>
        internal void AddTVYaoKongBtEvent()
        {
            if (m_HongDDGamePadCom != null)
            {
                m_HongDDGamePadCom.AddTVYaoKongBtEvent();
            }
        }

        /// <summary>
        /// 设置玩家激活状态信息.
        /// </summary>
        internal void SetIndexPlayerActiveGameState(int index, byte activeState)
        {
            if (m_HongDDGamePadCom != null)
            {
                m_HongDDGamePadCom.SetIndexPlayerActiveGameState(index, activeState);
            }
        }

        /// <summary>
        /// 删除微信手柄玩家的数据信息.
        /// </summary>
        internal void RemoveWeiXinPadPlayerData(int userId)
        {
            if (m_HongDDGamePadCom != null)
            {
                m_HongDDGamePadCom.RemoveWeiXinPadPlayerData(userId);
            }
        }

        /// <summary>
        /// 清理玩家微信数据信息.
        /// </summary>
        internal void ClearGameWeiXinData()
        {
            if (m_HongDDGamePadCom != null)
            {
                m_HongDDGamePadCom.ClearGameWeiXinData();
            }
        }

        /// <summary>
        /// 添加微信玩家游戏币.
        /// </summary>
        internal void AddWeiXinGameCoinToPlayer(int userId, int coin)
        {
            if (m_HongDDGamePadCom != null)
            {
                m_HongDDGamePadCom.AddWeiXinGameCoinToPlayer(userId, coin);
            }
        }

        /// <summary>
        /// 获取玩家微信头像url.
        /// </summary>
        internal string GetPlayerHeadUrl(int index)
        {
            string url = "";
            if (m_HongDDGamePadCom != null)
            {
                url = m_HongDDGamePadCom.m_PlayerHeadUrl[index];
            }
            return url;
        }
        #endregion
    }
}
