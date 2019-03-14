using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 游戏场景中摆放的抽奖分数信息管理.
/// x万分 可抽奖.
/// </summary>
public class SSChouJiangFenShu3DManage : MonoBehaviour
{
    /// <summary>
    /// 分数数字列表.
    /// </summary>
    List<SSChouJiangFenShu3D> m_ChouJiangFenShuList = new List<SSChouJiangFenShu3D>();
    /// <summary>
    /// 添加抽奖分数.
    /// </summary>
    internal void AddChouJiangFenShu(SSChouJiangFenShu3D chouJiangFenShu)
    {
        if (m_ChouJiangFenShuList != null && chouJiangFenShu != null)
        {
            if (m_ChouJiangFenShuList.Contains(chouJiangFenShu) == false)
            {
                m_ChouJiangFenShuList.Add(chouJiangFenShu);
            }
        }
    }

    /// <summary>
    /// 分数信息.
    /// </summary>
    int m_FenShuValue = -1;
    /// <summary>
    /// 显示数字.
    /// </summary>
    internal void ShowNum(int num)
    {
        if (m_ChouJiangFenShuList == null)
        {
            return;
        }

        if (m_ChouJiangFenShuList.Count == 0)
        {
            return;
        }

        if (m_FenShuValue == num)
        {
            return;
        }
        m_FenShuValue = num;

        for (int i = 0; i < m_ChouJiangFenShuList.Count; i++)
        {
            if (m_ChouJiangFenShuList[i] != null)
            {
                m_ChouJiangFenShuList[i].ShowNum(num);
            }
        }
    }
}
