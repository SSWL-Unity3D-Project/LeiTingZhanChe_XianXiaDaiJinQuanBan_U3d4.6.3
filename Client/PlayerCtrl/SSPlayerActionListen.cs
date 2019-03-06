using UnityEngine;

public class SSPlayerActionListen : MonoBehaviour
{
    /// <summary>
    /// 玩家无任何操作的最小休眠时间.
    /// </summary>
    float m_MinSleepTime = 15f;
    float m_TimeLast = 0f;
    /// <summary>
    /// 玩家是否处于休眠无操作状态.
    /// </summary>
    bool IsPlayerSleep = false;
    internal void Init()
    {
        if (XkGameCtrl.GetInstance() != null)
        {
            m_MinSleepTime = XkGameCtrl.GetInstance().m_PlayerNoActionMinTime;
            //SSDebug.LogWarning("m_MinSleepTime =================== " + m_MinSleepTime);
        }
    }

    internal void SetIsPlayerSleep(bool isPlayerSleep)
    {
        IsPlayerSleep = isPlayerSleep;
    }

    internal bool GetIsPlayerSleep()
    {
        return IsPlayerSleep;
    }

    /// <summary>
    /// 当玩家有操作时.
    /// </summary>
    internal void OnPlayerAction()
    {
        IsPlayerSleep = false;
        m_TimeLast = Time.time;
    }

    /// <summary>
    /// 更新玩家游戏操作状态.
    /// </summary>
    internal void UpdatePlayerAction()
    {
        if (IsPlayerSleep == false)
        {
            if (Time.time - m_TimeLast >= m_MinSleepTime)
            {
                //玩家已经很久没有操作游戏了.
                IsPlayerSleep = true;
            }
        }
    }
}
