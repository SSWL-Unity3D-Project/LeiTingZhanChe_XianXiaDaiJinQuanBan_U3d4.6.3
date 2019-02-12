using UnityEngine;

/// <summary>
/// 玩家受伤触发器.
/// 玩家碰上该触发器后减少一定数量的血值.
/// </summary>
public class SSTriggerPlayerDamage : MonoBehaviour
{
    /// <summary>
    /// 对玩家造成多少伤害.
    /// </summary>
    [Range(0f, 10000f)]
    public float PlayerDamage = 1f;
    void OnTriggerEnter(Collider other)
    {
        if (IsOpenTrigger == false)
        {
            return;
        }

        XKPlayerMoveCtrl playerMoveCom = other.GetComponent<XKPlayerMoveCtrl>();
        if (playerMoveCom == null)
        {
            return;
        }

        if (!playerMoveCom.GetIsWuDiState() && !playerMoveCom.GetIsShanShuoState())
        {
            if (!playerMoveCom.GetIsDeathPlayer())
            {
                XkGameCtrl.GetInstance().SubGamePlayerHealth(playerMoveCom.PlayerIndex, PlayerDamage);
            }
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (IsOpenTrigger == false)
        {
            return;
        }

        XKPlayerMoveCtrl playerMoveCom = other.GetComponent<XKPlayerMoveCtrl>();
        if (playerMoveCom == null)
        {
            return;
        }
        
        if (!playerMoveCom.GetIsWuDiState() && !playerMoveCom.GetIsShanShuoState())
        {
            if (!playerMoveCom.GetIsDeathPlayer())
            {
                XkGameCtrl.GetInstance().SubGamePlayerHealth(playerMoveCom.PlayerIndex, PlayerDamage);
            }
        }
    }

    /// <summary>
    /// 武器特效数组.
    /// </summary>
    public GameObject[] m_WeaponTXArray;
    bool IsOpenTrigger = true;
    /// <summary>
    /// 设置是否打开触发器.
    /// </summary>
    internal void SetIsOpenTrigger(bool isOpen)
    {
        IsOpenTrigger = isOpen;
        BoxCollider boxCol = GetComponent<BoxCollider>();
        if (boxCol != null)
        {
            boxCol.enabled = isOpen;
        }

        for (int i = 0; i < m_WeaponTXArray.Length; i++)
        {
            if (m_WeaponTXArray[i] != null)
            {
                m_WeaponTXArray[i].SetActive(isOpen);
            }
        }
    }
}