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
}