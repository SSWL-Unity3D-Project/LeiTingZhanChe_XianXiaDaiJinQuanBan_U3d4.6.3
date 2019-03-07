using UnityEngine;

public class SSChouJiangTiShi : MonoBehaviour
{
    /// <summary>
    /// 抽奖提示数据.
    /// </summary>
    [System.Serializable]
    public class ChouJiangTiShiData
    {
        /// <summary>
        /// 复活X图片.
        /// </summary>
        public GameObject fuHuoImg;
        /// <summary>
        /// 复活数字控制.
        /// </summary>
        public SSGameNumUI fuHuoNumUi;
        /// <summary>
        /// 任务卡图片.
        /// </summary>
        public GameObject renWuKaImg;
        internal void ShowFuHuoInfo(PlayerEnum indexPlayer)
        {
            int fuHuoCiShu = XKGlobalData.GetPlayerFuHuoCiShuInfo(indexPlayer);
            bool isActiveFuHuoImg = fuHuoCiShu > 0 ? true : false;
            if (fuHuoImg != null)
            {
                fuHuoImg.SetActive(isActiveFuHuoImg);
            }

            if (renWuKaImg != null)
            {
                renWuKaImg.SetActive(!isActiveFuHuoImg);
            }

            if (isActiveFuHuoImg == true)
            {
                if (fuHuoNumUi != null)
                {
                    fuHuoNumUi.ShowNumUI(fuHuoCiShu);
                }
            }
        }
    }
    /// <summary>
    /// 抽奖提示数据.
    /// </summary>
    public ChouJiangTiShiData m_ChouJiangTiShiDt;
    /// <summary>
    /// 位移时间.
    /// </summary>
    [Range(0.1f, 10f)]
    public float PiaoZiTime = 0.5f;
    /// <summary>
    /// 位移的高度.
    /// </summary>
    [Range(10f, 500f)]
    public float PiaoZiPY = 100f;
    /// <summary>
    /// 设定大小.
    /// </summary>
    public Vector2 LocalScale = new Vector2(1f, 1f);
    /// <summary>
    /// 显示抽奖提示.
    /// </summary>
    public void ShowChouJiangTiShi(PlayerEnum indexPlayer, Vector3 startPos)
    {
        if (m_ChouJiangTiShiDt != null)
        {
            m_ChouJiangTiShiDt.ShowFuHuoInfo(indexPlayer);
        }

        startPos.y += XKDaoJuGlobalDt.GetInstance().DaoJuMaoZiPY;
        transform.localPosition = startPos;
        transform.localEulerAngles = Vector3.zero;
        transform.localScale = new Vector3(LocalScale.x, LocalScale.y, 1f);
        gameObject.SetActive(true);
        TweenPosition twPos = gameObject.AddComponent<TweenPosition>();
        twPos.from = startPos;
        twPos.to = startPos + new Vector3(0f, PiaoZiPY, 0f);
        twPos.duration = PiaoZiTime;
        twPos.PlayForward();

        EventDelegate.Add(twPos.onFinished, delegate {
            StartCoroutine(StartPlayTweenAlpha());
        });
        m_TimeLast = Time.time;
    }

    /// <summary>
    /// 开始播放淡化效果.
    /// </summary>
    System.Collections.IEnumerator StartPlayTweenAlpha()
    {
        yield return new WaitForSeconds(m_TingLiuTime);
        TweenAlpha twAlp = gameObject.AddComponent<TweenAlpha>();
        twAlp.from = 1f;
        twAlp.to = 0f;
        twAlp.duration = 1f;
        twAlp.PlayForward();
    }

    void Update()
    {
        UpdateRemoveSelf();
    }

    bool IsRemoveSelf = false;
    float m_TimeLast = 0f;
    /// <summary>
    /// 停留展示时间.
    /// </summary>
    public float m_TingLiuTime = 3f;
    void UpdateRemoveSelf()
    {
        if (IsRemoveSelf == false)
        {
            if (Time.time - m_TimeLast >= 1f + m_TingLiuTime + PiaoZiTime)
            {
                m_TimeLast = Time.time;
                RemoveSelf();
            }
        }
    }

    void RemoveSelf()
    {
        if (IsRemoveSelf == false)
        {
            IsRemoveSelf = true;
            Destroy(gameObject);
        }
    }
}
