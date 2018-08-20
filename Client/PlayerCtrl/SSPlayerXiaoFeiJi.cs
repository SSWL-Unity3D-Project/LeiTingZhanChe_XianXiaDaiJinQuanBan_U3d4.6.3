using System.Collections.Generic;
using UnityEngine;

public class SSPlayerXiaoFeiJi : MonoBehaviour
{
    /// <summary>
    /// 跟踪点.
    /// </summary>
    Transform m_FollowPoint;
    /// <summary>
    /// 跟踪速度.
    /// </summary>
    public float m_FollowSpeed = 1f;
    /// <summary>
    /// 真实飞机转向tr.
    /// </summary>
    public Transform m_RealFeiJiRot;
    // Use this for initialization
    public void Init(XKPlayerAutoFire playerWeapon, Transform followTr)
    {
        Debug.Log("Unity: SSPlayerXiaoFeiJi.Init...............");
        m_PlayerIndex = playerWeapon.PlayerIndex;
        m_FireLayer = playerWeapon.FireLayer;
        m_FollowPoint = followTr;
        transform.SetParent(XkGameCtrl.MissionCleanup);
        gameObject.SetActive(false);
	}

    public void ShowSelf()
    {
        transform.position = Vector3.Lerp(transform.position, m_FollowPoint.position, m_FollowSpeed * Time.deltaTime);
        gameObject.SetActive(true);
    }

    public void HiddenSelf()
    {
        gameObject.SetActive(false);
    }
	
	// Update is called once per frame
	void Update()
    {
        UpdateFeiJiToFollowPoint();
        UpdateFeiJiAimCaiPiaoNpc();
    }

    /// <summary>
    /// 跟踪玩家.
    /// </summary>
    void UpdateFeiJiToFollowPoint()
    {
        if (m_FollowPoint == null)
        {
            return;
        }
        transform.position = Vector3.Lerp(transform.position, m_FollowPoint.position, m_FollowSpeed * Time.deltaTime);
        transform.forward = Vector3.MoveTowards(transform.forward, m_FollowPoint.forward, 0.1f);
    }

    /// <summary>
    /// 瞄准彩票npc.
    /// </summary>
    void UpdateFeiJiAimCaiPiaoNpc()
    {
        if (m_RealFeiJiRot == null)
        {
            return;
        }

        Transform aimTr = null;
        GameObject obj = XkPlayerCtrl.GetInstanceFeiJi().m_SpawnNpcManage.GetCaiPiaoNpc();
        if (obj != null)
        {
            aimTr = obj.transform;
        }

        if (aimTr == null)
        {
            m_RealFeiJiRot.localEulerAngles = Vector3.Lerp(m_RealFeiJiRot.localEulerAngles, Vector3.zero, Time.deltaTime);
        }
        else
        {
            Vector3 forwardVal = Vector3.zero;
            Vector3 posA = aimTr.position;
            Vector3 posB = m_RealFeiJiRot.position;
            posA.y = posB.y = 0f;
            forwardVal = Vector3.Normalize(posA - posB);
            m_RealFeiJiRot.forward = Vector3.Lerp(m_RealFeiJiRot.forward, forwardVal, 6f * Time.deltaTime);
        }
    }

    [System.Serializable]
    public class FeiJiWeaponData
    {
        /// <summary>
        /// 子弹预制.
        /// </summary>
        public GameObject AmmoPrefab;
        /// <summary>
        /// 子弹产生点.
        /// </summary>
        public Transform[] AmmoCreatPoints;
    }
    public FeiJiWeaponData m_WeaponData;

    public void CreatFeiJiAmmo()
    {
        if (m_WeaponData != null)
        {
            if (m_WeaponData.AmmoPrefab != null)
            {
                int length = m_WeaponData.AmmoCreatPoints.Length;
                for (int i = 0; i < length; i++)
                {
                    SpawnJiQiangAmmo(i);
                }
            }
            else
            {
                Debug.LogWarning("Unity: CreatFeiJiAmmo -> AmmoPrefab was null...");
            }
        }
    }

    /// <summary>
    /// 玩家索引.
    /// </summary>
    PlayerEnum m_PlayerIndex;
    /// <summary>
    /// 子弹可以攻击的碰撞层.
    /// </summary>
    LayerMask m_FireLayer;
    /// <summary>
    /// 子弹列表.
    /// </summary>
    List<PlayerAmmoCtrl> m_AmmoList = new List<PlayerAmmoCtrl>();

    void AddAmmoToList(PlayerAmmoCtrl scriptAmmo)
    {
        if (m_AmmoList.Contains(scriptAmmo))
        {
            return;
        }
        m_AmmoList.Add(scriptAmmo);
    }
    
    GameObject SpawnAmmo(GameObject ammoPrefab)
    {
        return (GameObject)Instantiate(ammoPrefab);
    }

    GameObject GetFeiJiAmmo(Vector3 ammoPos, Quaternion ammoRot)
    {
        int max = 0;
        GameObject objAmmo = null;
        max = m_AmmoList.Count;
        for (int i = 0; i < max; i++)
        {
            if (!m_AmmoList[i].gameObject.activeSelf)
            {
                objAmmo = m_AmmoList[i].gameObject;
                break;
            }
        }

        if (objAmmo == null)
        {
            objAmmo = SpawnAmmo(m_WeaponData.AmmoPrefab);
            AddAmmoToList(objAmmo.GetComponent<PlayerAmmoCtrl>());
        }

        if (objAmmo != null)
        {
            objAmmo.transform.position = ammoPos;
            objAmmo.transform.rotation = ammoRot;
        }
        return objAmmo;
    }

    void SpawnJiQiangAmmo(int indexVal)
    {
        if (m_WeaponData.AmmoCreatPoints == null)
        {
            return;
        }

        Vector3 ammoSpawnForward = m_WeaponData.AmmoCreatPoints[indexVal].forward;
        Vector3 ammoSpawnPos = m_WeaponData.AmmoCreatPoints[indexVal].position;
        Quaternion ammoSpawnRot = m_WeaponData.AmmoCreatPoints[indexVal].rotation;
        GameObject obj = GetFeiJiAmmo(ammoSpawnPos, ammoSpawnRot);
        if (obj == null)
        {
            return;
        }
        PlayerAmmoCtrl ammoScript = obj.GetComponent<PlayerAmmoCtrl>();

        float OffsetForward = 30f;
        float firePosValTmp = 100f;
        float fireRayDirLen = 100f;
        Vector3 mousePosInput = Input.mousePosition;
        RaycastHit hit;
        Vector3 firePos = Vector3.zero;
        Vector3 mousePos = mousePosInput + Vector3.forward * OffsetForward;
        Vector3 posTmp = Camera.main.ScreenToWorldPoint(mousePos);
        Vector3 ammoForward = Vector3.Normalize(posTmp - ammoSpawnPos);
        firePos = firePosValTmp * ammoSpawnForward + ammoSpawnPos;
        fireRayDirLen = ammoScript.MvSpeed * ammoScript.LiveTime;
        if (Physics.Raycast(ammoSpawnPos, ammoSpawnForward, out hit, fireRayDirLen, m_FireLayer.value))
        {
            if (ammoScript.AmmoType != PlayerAmmoType.ChuanTouAmmo)
            {
                firePos = hit.point;
            }
        }
        ammoScript.StartMoveAmmo(firePos, m_PlayerIndex);
    }
}