using UnityEngine;

public class SSJPBossFireManage : MonoBehaviour
{
    /// <summary>
    /// JPBoss攻击数据.
    /// </summary>
    [System.Serializable]
    public class JPBossWeaponData
    {
        /// <summary>
        /// 攻击时长.
        /// </summary>
        [Range(1f, 100f)]
        public float timeFire = 5f;
        /// <summary>
        /// 攻击武器数组.
        /// </summary>
        public SSJPBossWeapon[] weaponArray;
        /// <summary>
        /// 是否已经被击爆的武器.
        /// </summary>
        internal bool IsExploreWeapon = false;
        /// <summary>
        /// 设置是否开火.
        /// </summary>
        internal void SetIsOpenFire(bool isOpen)
        {
            for (int i = 0; i < weaponArray.Length; i++)
            {
                if (weaponArray[i] != null)
                {
                    weaponArray[i].SetIsOpenFire(isOpen);
                }
            }
        }

        /// <summary>
        /// 当JPBoss减少一部分血量之后,该攻击武器被玩家摧毁,创建爆炸粒子.
        /// </summary>
        internal void CreatWeaponExpore()
        {
            for (int i = 0; i < weaponArray.Length; i++)
            {
                if (weaponArray[i] != null)
                {
                    weaponArray[i].SetIsExplore(true);
                }
            }
        }
    }
    /// <summary>
    /// JPBoss的3套不同的特殊攻击武器.
    /// </summary>
    public JPBossWeaponData[] m_WeaponDtArray = new JPBossWeaponData[3];

}
