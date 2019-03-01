using UnityEngine;

/// <summary>
/// 玩家分数UI管理.
/// 还差多少分可以抽奖.
/// 恭喜获得抽奖资格.
/// </summary>
public class SSPlayerScoreManage : MonoBehaviour
{
    /// <summary>
    /// 玩家索引信息.
    /// </summary>
    public PlayerEnum m_PlayerIndex = PlayerEnum.Null;
    /// <summary>
    /// 还差多少分UI对象.
    /// </summary>
    public GameObject m_HaiChaFenShuObj;
    /// <summary>
    /// 还差多少分数的动画控制组件.
    /// </summary>
    public Animator m_HaiChaFenShuAni;
    /// <summary>
    /// 恭喜获得抽奖资格UI对象.
    /// </summary>
    public GameObject m_GongXiHuoDeObj;

    /// <summary>
    /// 各个玩家的分数管理组件.
    /// </summary>
    static SSPlayerScoreManage[] _InstanceArray = new SSPlayerScoreManage[3];
    public static SSPlayerScoreManage GetInstance(PlayerEnum playerIndex)
    {
        int indexVal = (int)playerIndex - 1;
        if (indexVal < 0 || indexVal >= _InstanceArray.Length)
        {
            return null;
        }
        return _InstanceArray[indexVal];
    }

    private void Awake()
    {
        Init();
    }

    // Use this for initialization
    void Init()
    {
        InitInstance();
        //隐藏恭喜获得抽奖资格.
        SetActiveGongXiHuoDe(false);
        //隐藏距抽奖还差多少分.
        SetActiveHaiChaFenShuObj(false);
        //重置还差多少分数UI的动画.
        ResetHaiChaFenShuAni();
    }

    /// <summary>
    /// 初始化玩家分数管理脚本.
    /// </summary>
    void InitInstance()
    {
        int indexVal = (int)m_PlayerIndex - 1;
        if (indexVal < 0 || indexVal >= _InstanceArray.Length)
        {
            return;
        }
        _InstanceArray[indexVal] = this;
    }

    /// <summary>
    /// 设置是否显示恭喜获得抽奖资格.
    /// </summary>
    void SetActiveGongXiHuoDe(bool isActive)
    {
        if (m_GongXiHuoDeObj != null)
        {
            m_GongXiHuoDeObj.SetActive(isActive);
        }
    }

    /// <summary>
    /// 设置是否显示距抽奖还差多少分.
    /// </summary>
    void SetActiveHaiChaFenShuObj(bool isActive)
    {
        if (m_HaiChaFenShuObj != null)
        {
            m_HaiChaFenShuObj.SetActive(isActive);
        }
    }

    /// <summary>
    /// 当展示玩家分数UI界面时.
    /// </summary>
    internal void OnDisplayPlayerScore()
    {
        //重置还差多少分数UI的动画.
        ResetHaiChaFenShuAni();
        //显示距抽奖还差多少分.
        SetActiveHaiChaFenShuObj(true);
        //隐藏恭喜获得抽奖资格.
        SetActiveGongXiHuoDe(false);
    }

    /// <summary>
    /// 当创建玩家分数评级界面时.
    /// </summary>
    internal void OnCreatePingJiPanel()
    {
        if (m_HaiChaFenShuObj != null && m_HaiChaFenShuObj.activeInHierarchy == true)
        {
            //播放距抽奖还差多少分的UI动画.
            PlayHaiChaFenShuAni();
        }
        //隐藏恭喜获得抽奖资格.
        SetActiveGongXiHuoDe(false);
    }

    /// <summary>
    /// 玩家分数是否足够.
    /// </summary>
    internal bool IsOnPlayerFenShuZuGou = false;
    /// <summary>
    /// 当玩家获得分数足够抽奖时.
    /// </summary>
    internal void OnPlayerFenShuZuGouChouJiang()
    {
        if (IsOnPlayerFenShuZuGou == true)
        {
            return;
        }
        IsOnPlayerFenShuZuGou = true;
        //隐藏距抽奖还差多少分.
        SetActiveHaiChaFenShuObj(false);
        //显示"恭喜玩家获得抽奖机会"
        SetActiveGongXiHuoDe(true);
        
        if (SSUIRoot.GetInstance().m_GameUIManage != null)
        {
            //创建玩家分数足够游戏抽奖的界面.
            SSUIRoot.GetInstance().m_GameUIManage.CreatePlayerChouJiangFenShuZuGouPanel(m_PlayerIndex);
        }
    }

    /// <summary>
    /// 当删除玩家抽奖界面时.
    /// </summary>
    internal void OnRemovePlayerChouJiangPanel()
    {
        //隐藏恭喜获得抽奖资格.
        SetActiveGongXiHuoDe(false);
        //隐藏距抽奖还差多少分.
        SetActiveHaiChaFenShuObj(false);
        //重置还差多少分数UI的动画.
        ResetHaiChaFenShuAni();
        IsOnPlayerFenShuZuGou = false;
    }

    /// <summary>
    /// 播放还差多少分数的位移动画.
    /// </summary>
    void PlayHaiChaFenShuAni()
    {
        if (m_HaiChaFenShuAni != null)
        {
            m_HaiChaFenShuAni.enabled = true;
            m_HaiChaFenShuAni.SetBool("isPlay", true);
        }
    }

    /// <summary>
    /// 重置还差多少分数UI的动画.
    /// </summary>
    void ResetHaiChaFenShuAni()
    {
        if (m_HaiChaFenShuAni != null)
        {
            m_HaiChaFenShuAni.SetBool("isPlay", false);
            m_HaiChaFenShuAni.enabled = false;
        }
    }
}
