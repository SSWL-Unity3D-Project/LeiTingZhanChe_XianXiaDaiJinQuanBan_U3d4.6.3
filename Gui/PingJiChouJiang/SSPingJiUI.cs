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
    /// 分数滚动音效.
    /// </summary>
    public AudioSource m_FenShuAinAudio;
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

        m_PlayerPingJiLevel = SSPingJiData.PingJiLevel.D;
        if (XkGameCtrl.GetInstance().m_PingJiData != null)
        {
            m_PlayerPingJiLevel = XkGameCtrl.GetInstance().m_PingJiData.GetPlayerPingJiLevel(fenShu);
        }
        else
        {
            SSDebug.LogWarning("XkGameCtrl.GetInstance().m_PingJiData was null");
        }

        if (fenShu.ToString().Length > m_FenShuNumUI.m_UISpriteArray.Length)
        {
            fenShu = (int)Mathf.Pow(10, fenShu.ToString().Length) - 1;
        }
        m_PlayerFenShu = fenShu;
        m_FenShuAnimationData = new FenShuAnimationData(m_TimeFenShuAni, fenShu);

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
            Destroy(gameObject);
        }
    }

    private void FixedUpdate()
    {
        if (IsEndFenShuAni == false)
        {
            UpdatePlayerFenAni();
        }
    }

    void Update()
    {
        if (Time.time - m_TimeStart >= m_TimeHidden && IsRemoveSelf == false)
        {
            if (m_PlayerPingJiLevel < SSPingJiData.PingJiLevel.A)
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
