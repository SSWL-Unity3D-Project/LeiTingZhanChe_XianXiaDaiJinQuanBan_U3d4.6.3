using UnityEngine;

public class SSPlayerAmmoTiShiData : MonoBehaviour
{
    /// <summary>
    /// 玩家子弹提示数据.
    /// </summary>
    [System.Serializable]
    public class PlayerAmmoTiShiData
    {
        /// <summary>
        /// 玩家子弹数据列表.
        /// </summary>
        public GameObject[] ammoTiShiArray;
        internal void Init(PlayerEnum indexPlayer)
        {
            if (ammoTiShiArray.Length <= 0)
            {
                return;
            }

            int indexVal = (int)indexPlayer - 1;
            for (int i = 0; i < ammoTiShiArray.Length; i++)
            {
                if (ammoTiShiArray[i] != null)
                {
                    ammoTiShiArray[i].SetActive(i == indexVal ? true : false);
                }
            }
        }
    }
    /// <summary>
    /// 玩家子弹提示数据.
    /// </summary>
    public PlayerAmmoTiShiData m_PlayerAmmoTiShiDt;

    /// <summary>
    /// 初始化玩家子弹提示.
    /// </summary>
    internal void InitPlayerAmmo(PlayerEnum indexPlayer)
    {
        if (m_PlayerAmmoTiShiDt != null)
        {
            m_PlayerAmmoTiShiDt.Init(indexPlayer);
        }
    }
}
