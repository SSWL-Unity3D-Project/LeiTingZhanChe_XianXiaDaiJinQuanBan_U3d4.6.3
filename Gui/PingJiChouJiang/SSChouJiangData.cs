using UnityEngine;
using System.Collections;

public class SSChouJiangData : MonoBehaviour
{
    /// <summary>
    /// 再玩一局游戏的人次概率控制.
    /// 最大人数没有达到时随机放奖,如果最大人数已经够了但是奖品仍没有放出时就100%给最后的这个人发奖(同时清除当前记录的人数信息)
    /// </summary>
    public class ZaiWanYiJuJiangPinData
    {
        /// <summary>
        /// 最大人数.
        /// </summary>
        internal int maxPlayer = 10;
        /// <summary>
        /// 当前人数.
        /// </summary>
        internal int curPlayer = 10;
        /// <summary>
        /// 随机概率.
        /// </summary>
        int suiJiGaiLv = 50;
        /// <summary>
        /// 没有人可以爆奖.
        /// </summary>
        int ZeroPlayer = 999999999;
        internal void SetMaxPlayer(int baoJiangLv)
        {
            if (baoJiangLv < 0 || baoJiangLv > 100)
            {
                baoJiangLv = 50;
            }

            if (baoJiangLv == 0)
            {
                maxPlayer = ZeroPlayer;
            }
            else
            {
                int addVal = 0;
                if (100 % baoJiangLv != 0)
                {
                    addVal = 1;
                }
                maxPlayer = (100 / baoJiangLv) + addVal;
            }
        }

        /// <summary>
        /// 是否已经爆奖.
        /// </summary>
        bool IsHaveBaoJiang = false;
        internal void SetIsHaveJiBaoNpc(bool isJiBaoNpc)
        {
            IsHaveBaoJiang = isJiBaoNpc;
        }
        internal bool GetIsHaveJiBaoNpc()
        {
            return IsHaveBaoJiang;
        }
    }
    ZaiWanYiJuJiangPinData m_ZaiWanYiJuJiangPinDt = new ZaiWanYiJuJiangPinData();

    /// <summary>
    /// 初始化.
    /// </summary>
    internal void Init(int gaiLv)
    {
        SetZaiWanYiJuJiangPinDtMaxPlayer(gaiLv);
    }

    /// <summary>
    /// 设置再玩一局游戏奖品的最大人数.
    /// </summary>
    void SetZaiWanYiJuJiangPinDtMaxPlayer(int gaiLv)
    {
        if (m_ZaiWanYiJuJiangPinDt != null)
        {

        }
    }
}
