using UnityEngine;

public class SSJPBossWeaponManage : MonoBehaviour
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
        /// 初始化.
        /// </summary>
        internal void Init()
        {
            IsExploreWeapon = false;
            for (int i = 0; i < weaponArray.Length; i++)
            {
                if (weaponArray[i] != null)
                {
                    weaponArray[i].Init();
                }
            }
        }
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
            if (IsExploreWeapon == true)
            {
                return;
            }
            IsExploreWeapon = true;

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
    bool IsInit = false;
    /// <summary>
    /// 初始化.
    /// </summary>
    internal void Init()
    {
        if (IsInit == true)
        {
            return;
        }
        IsInit = true;

        for (int i = 0; i < m_WeaponDtArray.Length; i++)
        {
            if (m_WeaponDtArray[i] != null)
            {
                m_WeaponDtArray[i].Init();
                if (i == 0)
                {
                    //第一套武器准备攻击玩家.
                    m_WeaponDtArray[i].SetIsOpenFire(true);
                }
            }
        }
        m_TimeLastWeapon = Time.time;
        m_IndexWeapon = 0;
        m_HealthPerVal = 1f;
        IsCloseWeapon = false;
    }

    private void Update()
    {
        if (IsCloseWeapon == false)
        {
            CheckIsOpenNextWeapon();
        }
    }

    /// <summary>
    /// 时间信息记录.
    /// </summary>
    float m_TimeLastWeapon = 0f;
    int m_IndexWeapon = 0;
    bool IsCloseWeapon = false;
    /// <summary>
    /// 检测是否打开下一阶段的攻击武器.
    /// </summary>
    void CheckIsOpenNextWeapon()
    {
        int indexVal = m_IndexWeapon % m_WeaponDtArray.Length;
        if (m_WeaponDtArray[indexVal] != null && Time.time - m_TimeLastWeapon >= m_WeaponDtArray[indexVal].timeFire)
        {
            ChangeNextWeapon();
        }
    }

    /// <summary>
    /// 炸掉当前武器.
    /// </summary>
    void ExploreWeaponCurrent()
    {
        int indexVal = m_IndexWeapon % m_WeaponDtArray.Length;
        //SSDebug.LogWarning("ExploreWeaponCurrent -> indexVal ====================== " + indexVal);
        ExploreWeapon(indexVal);
    }

    /// <summary>
    /// 炸掉武器.
    /// </summary>
    void ExploreWeapon(int indexVal)
    {
        if (indexVal < 0 || indexVal >= m_WeaponDtArray.Length)
        {
            return;
        }

        if (m_WeaponDtArray[indexVal] != null)
        {
            //关闭当前阶段武器.
            m_WeaponDtArray[indexVal].SetIsOpenFire(false);
            //创建爆炸粒子.
            m_WeaponDtArray[indexVal].CreatWeaponExpore();
        }
    }

    /// <summary>
    /// 炸掉所有武器.
    /// </summary>
    void ExploreAllWeapon()
    {
        for (int i = 0; i < m_WeaponDtArray.Length; i++)
        {
            ExploreWeapon(i);
        }
    }

    /// <summary>
    /// 启用下一套攻击武器.
    /// </summary>
    void ChangeNextWeapon()
    {
        int indexVal = m_IndexWeapon % m_WeaponDtArray.Length;
        //SSDebug.LogWarning("ChangeNextWeapon -> indexVal ====================== " + indexVal);
        if (m_WeaponDtArray[indexVal] != null)
        {
            m_TimeLastWeapon = Time.time;
            //关闭当前阶段武器.
            m_WeaponDtArray[indexVal].SetIsOpenFire(false);
            //开启下一阶段武器.
            JPBossWeaponData weaponDt = GetNextWeaponData(indexVal);
            if (weaponDt != null)
            {
                weaponDt.SetIsOpenFire(true);
            }
        }
    }

    JPBossWeaponData GetNextWeaponData(int indexVal)
    {
        JPBossWeaponData dt = null;
        int indexNext = (indexVal + 1) % m_WeaponDtArray.Length;
        int count = 0;
        do
        {
            if (count >= m_WeaponDtArray.Length)
            {
                //找不到可以用来攻击的武器.
                break;
            }

            count++;
            if (m_WeaponDtArray[indexNext] != null && m_WeaponDtArray[indexNext].IsExploreWeapon == false)
            {
                //SSDebug.LogWarning("GetNextWeaponData -> indexNext ==================== " + indexNext);
                dt = m_WeaponDtArray[indexNext];
                m_IndexWeapon = indexNext;
                break;
            }
            indexNext = (indexNext + 1) % m_WeaponDtArray.Length;
        } while (true);
        return dt;
    }

    /// <summary>
    /// 关闭武器.
    /// </summary>
    internal void CloseWeapon()
    {
        if (IsCloseWeapon == true)
        {
            return;
        }

        IsInit = false;
        IsCloseWeapon = true;
        for (int i = 0; i < m_WeaponDtArray.Length; i++)
        {
            if (m_WeaponDtArray[i] != null)
            {
                m_WeaponDtArray[i].SetIsOpenFire(false);
            }
        }
    }

    /// <summary>
    /// JPBoss当前血量的百分比.
    /// </summary>
    float m_HealthPerVal = 1f;
    internal void OnDamage(float healthPer)
    {
        if (IsCloseWeapon == true)
        {
            return;
        }

        float healthPerUnit = 0.25f;
        if (m_HealthPerVal <= healthPerUnit)
        {
            //没有武器可以再被摧毁了.
            return;
        }

        if (m_HealthPerVal - healthPer >= healthPerUnit)
        {
            //炸掉当前武器.
            ExploreWeaponCurrent();
            if (m_HealthPerVal > healthPerUnit)
            {
                //启用下一套攻击武器.
                ChangeNextWeapon();
            }
            
            if (healthPer <= healthPerUnit)
            {
                //JPBoss的特殊武器已经全部被破坏.
                CloseWeapon();
                //炸掉所有武器.
                ExploreAllWeapon();
            }

            //玩家破坏了JPBoss的一套武器.
            m_HealthPerVal = healthPer;
        }
    }
}
