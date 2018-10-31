using System.Collections.Generic;
/// <summary>
/// 红点点游戏手柄端玩家的账户数据管理.
/// </summary>
public class SSHDDPayManage
{
    /// <summary>
    /// 玩家账户信息.
    /// </summary>
    public class PlayerPayData
    {
        /// <summary>
        /// 用户Id.
        /// </summary>
        public int userId = 0;
        /// <summary>
        /// 账户余额.
        /// </summary>
        public int money = 0;
        public PlayerPayData()
        {
        }

        public PlayerPayData(int userIdVal, int moneyVal)
        {
            userId = userIdVal;
            money = moneyVal;
        }
    }

    /// <summary>
    /// 游戏中玩家账户数据列表.
    /// </summary>
    List<PlayerPayData> m_PlayerPayDtList = new List<PlayerPayData>();
    /// <summary>
    /// 添加玩家账户数据.
    /// </summary>
    void AddPlayerPayData(PlayerPayData payDt)
    {
        if (m_PlayerPayDtList.Contains(payDt) == false && payDt != null)
        {
            m_PlayerPayDtList.Add(payDt);
        }
    }

    /// <summary>
    /// 删除玩家账户数据.
    /// </summary>
    void RemovePlayerPayData(PlayerPayData payDt)
    {
        if (m_PlayerPayDtList.Contains(payDt) == true)
        {
            m_PlayerPayDtList.Remove(payDt);
        }
    }

    /// <summary>
    /// 通过玩家Id查找用户账户信息.
    /// </summary>
    PlayerPayData FindPlayerPayData(int userId)
    {
        return m_PlayerPayDtList.Find((dt) => { return dt.userId.Equals(userId); });
    }

    /// <summary>
    /// 保存玩家账户信息.
    /// </summary>
    internal void SavePlayerPayData(int userId, int money)
    {
        PlayerPayData data = FindPlayerPayData(userId);
        if (data == null)
        {
            AddPlayerPayData(new PlayerPayData(userId, money));
        }
        else
        {
            data.money = money;
        }
    }
}
