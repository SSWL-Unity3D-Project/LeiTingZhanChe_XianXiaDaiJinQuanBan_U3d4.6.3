using UnityEngine;

public class SSJPBossWeapon : SSGameMono
{
    /// <summary>
    /// 炮台类型攻击武器.
    /// </summary>
    public XKCannonCtrl m_PaoTaiArray;
    /// <summary>
    /// 对玩家造成伤害的触发器.
    /// </summary>
    public SSTriggerPlayerDamage m_DamageArray;
    /// <summary>
    /// 爆炸粒子预制.
    /// </summary>
    public GameObject m_ExplorePrefab;
    /// <summary>
    /// 爆炸粒子产生点.
    /// </summary>
    public Transform[] m_ExplorePoint;
    /// <summary>
    /// 爆炸时需要隐藏的对象数组.
    /// </summary>
    public GameObject[] m_HiddenObjArray;
    /// <summary>
    /// 是否爆炸.
    /// </summary>
    bool IsExplore = false;

    /// <summary>
    /// 初始化.
    /// </summary>
    internal void Init()
    {
        SetIsOpenFire(false);
        SetIsExplore(false);
        //展示攻击武器.
        SetHiddenObjArray(false);
    }

    /// <summary>
    /// 设置是否开火.
    /// </summary>
    internal void SetIsOpenFire(bool isOpen)
    {
        if (IsExplore == true)
        {
            if (isOpen == true)
            {
                return;
            }
        }

        if (m_PaoTaiArray != null)
        {
            m_PaoTaiArray.SetIsJPBossTeShuWeapon();
            m_PaoTaiArray.SetIsActiveJPBossTeShuWeapon(isOpen);
            m_PaoTaiArray.FireDis = isOpen == false ? 0f : 1000f;
        }

        if (m_DamageArray != null)
        {
            m_DamageArray.SetIsOpenTrigger(isOpen);
        }
    }

    /// <summary>
    /// 设置是否爆炸.
    /// </summary>
    internal void SetIsExplore(bool isExplore)
    {
        if (IsExplore == isExplore)
        {
            return;
        }
        IsExplore = isExplore;

        if (IsExplore == true)
        {
            //隐藏攻击武器.
            SetHiddenObjArray(true);
            if (m_ExplorePrefab != null)
            {
                for (int i = 0; i < m_ExplorePoint.Length; i++)
                {
                    if (m_ExplorePoint[i] != null)
                    {
                        GameObject obj = (GameObject)Instantiate(m_ExplorePrefab, XkGameCtrl.MissionCleanup, m_ExplorePoint[i]);
                        XkGameCtrl.CheckObjDestroyThisTimed(obj);
                    }
                }
            }
        }
    }

    /// <summary>
    /// 获取是否爆炸.
    /// </summary>
    internal bool GetIsExplore()
    {
        return IsExplore;
    }

    /// <summary>
    /// 设置隐藏组件.
    /// </summary>
    void SetHiddenObjArray(bool isHidden)
    {
        for (int i = 0; i < m_HiddenObjArray.Length; i++)
        {
            if (m_HiddenObjArray[i] != null)
            {
                m_HiddenObjArray[i].SetActive(!isHidden);
            }
        }
    }
}
