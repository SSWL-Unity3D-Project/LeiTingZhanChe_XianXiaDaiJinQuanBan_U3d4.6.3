using UnityEngine;

public class SSTriggerCaiPiaoBossMove : MonoBehaviour
{
    public enum TriggerCenter
    {
        /// <summary>
        /// 前后中心触发器.
        /// </summary>
        QHCenter = 0,
        /// <summary>
        /// 左右中心触发器.
        /// </summary>
        ZYCenter = 1,
    }
    public TriggerCenter m_TriggerCenter = TriggerCenter.QHCenter;

    public enum TriggerDir
    {
        Center = 0,
        Qian = 1,
        Hou = 2,
        Zuo = 3,
        You = 4,
    }
    public TriggerDir m_TriggerDir = TriggerDir.Center;

    public void OnTriggerEnter(Collider other)
    {
        XKNpcMoveCtrl npcMoveCom = other.gameObject.GetComponent<XKNpcMoveCtrl>();
        if (npcMoveCom != null
            && npcMoveCom.IsCaiPiaoZhanChe
            && npcMoveCom.GetIsBossNpc())
        {
            //彩票Boss.
            if (npcMoveCom.m_CaiPiaoBossMoveCom != null)
            {
                npcMoveCom.m_CaiPiaoBossMoveCom.OnHitBossMoveTrigger(this);
            }
        }
    }
}