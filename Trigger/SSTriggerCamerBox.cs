using UnityEngine;

public class SSTriggerCamerBox : MonoBehaviour
{
    void OnTriggerExit(Collider other)
    {
        XKNpcMoveCtrl npcMoveCom = other.gameObject.GetComponent<XKNpcMoveCtrl>();
        if (npcMoveCom != null && npcMoveCom.IsCaiPiaoZhanChe)
        {
            Debug.Log("Unity: SSTriggerCamerBox******************name === " + npcMoveCom.name);
            if (npcMoveCom.GetIsBossNpc() == true)
            {
                //Boss走出镜头范围.
                if (XkGameCtrl.GetInstance() != null && XkGameCtrl.GetInstance().m_AiPathGroup != null
                    && XkGameCtrl.GetInstance().m_AiPathGroup.m_CameraMoveType != AiPathGroupCtrl.MoveState.YuLe)
                {
                    XkGameCtrl.GetInstance().m_AiPathGroup.SetCameraMoveType(AiPathGroupCtrl.MoveState.Default);
                }
                npcMoveCom.TriggerRemovePointNpc(0);
                //boss删除后切换背景音效.
                AudioBeiJingCtrl.StopGameBeiJingAudio();
                //镜头继续移动.
                XkGameCtrl.GetInstance().SetGameCameraIsMoveing(true, NpcJiFenEnum.Boss);
            }
            else
            {
                //彩票战车npc走出镜头范围.
                npcMoveCom.TriggerRemovePointNpc(0);
                //镜头继续移动.
                XkGameCtrl.GetInstance().SetGameCameraIsMoveing(true, NpcJiFenEnum.CheLiang);
            }
        }
    }
}