using UnityEngine;

/// <summary>
/// 抽奖分数足够后UI提示.
/// </summary>
public class SSChouJiangFenShuZuGou : MonoBehaviour
{
    /// <summary>
    /// 玩家头像.
    /// </summary>
    public UITexture m_HeadUITexture;
    /// <summary>
    /// 头像框UI组件.
    /// </summary>
    public UITexture headKuangUI;
    /// <summary>
    /// 头像框UI图片资源.
    /// </summary>
    public Texture[] headKuangImgArray = new Texture[3];
    /// <summary>
    /// 初始化.
    /// </summary>
    internal void Init(PlayerEnum playerIndex)
    {
        //设置玩家微信头像.
        SetPlayerHeadImg(playerIndex);
    }

    /// <summary>
    /// 设置玩家微信头像.
    /// </summary>
    void SetPlayerHeadImg(PlayerEnum playerIndex)
    {
        m_TimeLast = Time.time;
        if (m_HeadUITexture != null)
        {
            Texture headImg = null;
            if (XueKuangCtrl.GetInstance(playerIndex) != null && XueKuangCtrl.GetInstance(playerIndex).m_WeiXinHead != null)
            {
                headImg = XueKuangCtrl.GetInstance(playerIndex).m_WeiXinHead.mainTexture;
            }

            if (headImg != null)
            {
                m_HeadUITexture.mainTexture = headImg;
            }
        }

        if (headKuangUI != null)
        {
            int indexVal = (int)playerIndex - 1;
            if (indexVal >= 0 && indexVal < headKuangImgArray.Length && headKuangImgArray[indexVal] != null)
            {
                //玩家头像框.
                headKuangUI.mainTexture = headKuangImgArray[indexVal];
            }
        }
    }

    void Update()
    {
        UpdateRemoveSelf();
    }

    bool IsRemoveSelf = false;
    float m_TimeLast = 0f;
    /// <summary>
    /// 多少时间之后删除.
    /// </summary>
    [Range(1f, 30f)]
    public float m_TimeRemove = 3f;
    void UpdateRemoveSelf()
    {
        if (IsRemoveSelf == false)
        {
            if (Time.time - m_TimeLast >= m_TimeRemove)
            {
                m_TimeLast = Time.time;
                RemoveSelf();
            }
        }
    }

    internal void RemoveSelf()
    {
        if (IsRemoveSelf == false)
        {
            IsRemoveSelf = true;
            if (SSUIRoot.GetInstance().m_GameUIManage != null)
            {
                SSUIRoot.GetInstance().m_GameUIManage.RemoveChouJiangFenShuZuGou(this);
            }
            Destroy(gameObject);
        }
    }
}
