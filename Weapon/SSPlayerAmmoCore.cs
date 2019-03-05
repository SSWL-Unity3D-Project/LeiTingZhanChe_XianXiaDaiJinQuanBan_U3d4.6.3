using UnityEngine;
using System.Collections;

public class SSPlayerAmmoCore : MonoBehaviour
{
    /// <summary>
    /// 玩家子弹数据.
    /// </summary>
    [System.Serializable]
    public class PlayerAmmoData
    {
        /// <summary>
        /// 玩家子弹数据列表.
        /// </summary>
        public GameObject[] ammoCoreArray;
        internal void Init(PlayerEnum indexPlayer)
        {
            if (ammoCoreArray.Length <= 0)
            {
                return;
            }

            int indexVal = (int)indexPlayer - 1;
            for (int i = 0; i < ammoCoreArray.Length; i++)
            {
                if (ammoCoreArray[i] != null)
                {
                    ammoCoreArray[i].SetActive(i == indexVal ? true : false);
                }
            }
        }
    }
    /// <summary>
    /// 玩家子弹数据.
    /// </summary>
    public PlayerAmmoData m_PlayerAmmoDt;

    /// <summary>
    /// 初始化玩家子弹.
    /// </summary>
    internal void InitPlayerAmmo(PlayerEnum indexPlayer)
    {
        if (m_PlayerAmmoDt != null)
        {
            m_PlayerAmmoDt.Init(indexPlayer);
        }
    }
}
