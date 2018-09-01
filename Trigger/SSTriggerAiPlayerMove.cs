using UnityEngine;

public class SSTriggerAiPlayerMove : SSGameMono
{
    public enum TiggerState
    {
        Qian = 0,
        Hou = 1,
        Zuo = 2,
        You = 3,
    }
    /// <summary>
    /// 触发器方位信息.
    /// </summary>
    public TiggerState m_TriggerState = TiggerState.Qian;
    void OnTriggerEnter(Collider other)
    {
        if (XkGameCtrl.GetInstance().m_GamePlayerAiData.IsActiveAiPlayer == false)
        {
            //关闭玩家Ai坦克运动逻辑.
            return;
        }

        XKPlayerMoveCtrl playerMoveCom = other.GetComponent<XKPlayerMoveCtrl>();
        if (playerMoveCom == null || playerMoveCom.m_PlayerAiMove == null)
        {
            return;
        }
        playerMoveCom.m_PlayerAiMove.OnHitAiMoveTrigger(m_TriggerState);
    }
}