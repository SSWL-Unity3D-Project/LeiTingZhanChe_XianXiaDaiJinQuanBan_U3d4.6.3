﻿using UnityEngine;

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

    void RemoveSelf()
    {
        if (IsRemoveSelf == false)
        {
            IsRemoveSelf = true;
            Destroy(gameObject);
        }
    }
}
