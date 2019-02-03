using UnityEngine;
using System.Collections;

public class SSPingJiUI : MonoBehaviour
{
    /// <summary>
    /// 多长时间之后隐藏评级UI界面.
    /// </summary>
    [Range(1f, 30f)]
    public float m_TimeHidden = 6f;
    float m_TimeStart = 0f;
    /// <summary>
    /// 玩家游戏得分UI展示.
    /// </summary>
    public SSGameNumUI m_FenShuNumUI;
    /// <summary>
    /// 玩家评级UI组件.
    /// </summary>
    public UITexture m_PingJiUI;
    /// <summary>
    /// 评分等级对应的图片资源.
    /// </summary>
    public Texture[] m_PingJiImgArray = new Texture[7];
    PlayerEnum m_IndexPlayer = PlayerEnum.Null;
    SSPingJiData.PingJiLevel m_PlayerPingJiLevel = SSPingJiData.PingJiLevel.A;
    internal void Init(PlayerEnum indexPlayer, int fenShu)
    {
        SetActive(false);
        m_IndexPlayer = indexPlayer;
        m_TimeStart = Time.time;
        if (m_PingJiUI == null)
        {
            SSDebug.LogWarning("SSPingJiUI::Init -> m_PingJiUI was null");
            return;
        }

        m_PlayerPingJiLevel = SSPingJiData.PingJiLevel.A;
        if (XkGameCtrl.GetInstance().m_PingJiData != null)
        {
            m_PlayerPingJiLevel = XkGameCtrl.GetInstance().m_PingJiData.GetPlayerPingJiLevel(fenShu);
        }
        else
        {
            SSDebug.LogWarning("XkGameCtrl.GetInstance().m_PingJiData was null");
        }

        int indexVal = (int)m_PlayerPingJiLevel;
        if (indexVal < m_PingJiImgArray.Length && m_PingJiImgArray[indexVal] != null)
        {
            m_PingJiUI.mainTexture = m_PingJiImgArray[indexVal];
        }
        else
        {
            SSDebug.LogWarning("indexVal or m_PingJiImgArray was wrong");
        }
    }

    internal void SetActive(bool isActive)
    {
        gameObject.SetActive(isActive);
        if (isActive == true)
        {
            m_TimeStart = Time.time;
        }
    }

    bool IsRemoveSelf = false;
    internal void RemoveSelf()
    {
        if (IsRemoveSelf == false)
        {
            IsRemoveSelf = true;
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if (Time.time - m_TimeStart >= m_TimeHidden && IsRemoveSelf == false)
        {
            if (m_PlayerPingJiLevel < SSPingJiData.PingJiLevel.S)
            {
                //设置玩家状态信息.
                XkGameCtrl.SetActivePlayer(m_IndexPlayer, false);
                //玩家评级过低,显示倒计时界面.
                DaoJiShiCtrl daoJiShiCom = DaoJiShiCtrl.GetInstance(m_IndexPlayer);
                if (daoJiShiCom != null)
                {
                    daoJiShiCom.StartPlayDaoJiShi();
                }
            }
            else
            {
                //玩家评级达到抽奖水平,显示抽奖界面.
                if (SSUIRoot.GetInstance().m_GameUIManage != null)
                {
                    SSUIRoot.GetInstance().m_GameUIManage.CreatPlayerChouJiangUI(m_IndexPlayer);
                }
            }

            if (SSUIRoot.GetInstance().m_GameUIManage != null)
            {
                //删除玩家评级界面.
                SSUIRoot.GetInstance().m_GameUIManage.RemovePlayerPingJiUI(m_IndexPlayer);
            }
        }
    }
}
