using UnityEngine;

public class SSTriggerCamerBox : MonoBehaviour
{
    void OnTriggerExit(Collider other)
    {
        XKNpcMoveCtrl npcMoveCom = other.gameObject.GetComponent<XKNpcMoveCtrl>();
        if (npcMoveCom != null && npcMoveCom.IsCaiPiaoZhanChe)
        {
            //Debug.Log("SSTriggerCamerBox******************name === " + npcMoveCom.name);
            if (npcMoveCom.GetIsBossNpc() == true)
            {
                //Boss走出镜头范围.
                if (XkGameCtrl.GetInstance() != null && XkGameCtrl.GetInstance().m_AiPathGroup != null
                    && XkGameCtrl.GetInstance().m_AiPathGroup.m_CameraMoveType != AiPathGroupCtrl.MoveState.YuLe)
                {
                    XkGameCtrl.GetInstance().m_AiPathGroup.SetCameraMoveType(AiPathGroupCtrl.MoveState.Default);
                }
                npcMoveCom.TriggerRemovePointNpc(0);
            }
            else
            {
                npcMoveCom.TriggerRemovePointNpc(0);
            }
        }
    }
}