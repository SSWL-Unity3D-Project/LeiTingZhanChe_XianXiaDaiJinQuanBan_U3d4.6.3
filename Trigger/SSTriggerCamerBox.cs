using UnityEngine;

public class SSTriggerCamerBox : MonoBehaviour
{
    public enum TriggerState
    {
        /// <summary>
        /// JPBoss触发器.
        /// </summary>
        JPBoss = 0,
        /// <summary>
        /// 战车Boss触发器.
        /// </summary>
        ZhanCheBoss = 1,
    }
    public TriggerState m_TriggerState = TriggerState.JPBoss;

    void OnTriggerExit(Collider other)
    {
        XKNpcMoveCtrl npcMoveCom = other.gameObject.GetComponent<XKNpcMoveCtrl>();
        if (npcMoveCom != null && npcMoveCom.IsCaiPiaoZhanChe)
        {
            //Debug.Log("Unity: OnTriggerExit******************name === " + npcMoveCom.name);
            bool isExit = false;
            switch (m_TriggerState)
            {
                case TriggerState.JPBoss:
                    {
                        if (npcMoveCom.IsEnterCameraBox == true && npcMoveCom.GetIsBossNpc() == true)
                        {
                            isExit = true;
                        }

                        if (npcMoveCom.IsCaiPiaoZhanChe == false)
                        {
                            //普通npc走出JPBoss的镜头范围盒子.
                            if (XkPlayerCtrl.GetInstanceFeiJi() != null)
                            {
                                if (XkPlayerCtrl.GetInstanceFeiJi().m_SpawnNpcManage != null)
                                {
                                    XkPlayerCtrl.GetInstanceFeiJi().m_SpawnNpcManage.RemovePuTongNpcToBuJiDt(npcMoveCom.gameObject);
                                }
                            }
                        }
                        break;
                    }
                case TriggerState.ZhanCheBoss:
                    {
                        if (npcMoveCom.IsEnterCameraBox == true && npcMoveCom.GetIsBossNpc() == false)
                        {
                            isExit = true;
                        }
                        break;
                    }
            }

            if (isExit == false)
            {
                return;
            }

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
                //SSDebug.LogWarning("SSTriggerCamerBox::OnTriggerExit -> time ==================== " + Time.time);
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        XKNpcMoveCtrl npcMoveCom = other.gameObject.GetComponent<XKNpcMoveCtrl>();
        if (npcMoveCom != null && npcMoveCom.IsCaiPiaoZhanChe)
        {
            //Debug.Log("Unity: OnTriggerEnter******************name === " + npcMoveCom.name);
            //彩票战车和boss进入摄像机盒子.
            bool isEnter = false;
            switch (m_TriggerState)
            {
                case TriggerState.JPBoss:
                    {
                        if (npcMoveCom.GetIsBossNpc() == true)
                        {
                            isEnter = true;
                        }

                        if (npcMoveCom.IsCaiPiaoZhanChe == false)
                        {
                            //普通npc进入JPBoss的镜头范围盒子.
                            if (XkPlayerCtrl.GetInstanceFeiJi() != null)
                            {
                                if (XkPlayerCtrl.GetInstanceFeiJi().m_SpawnNpcManage != null)
                                {
                                    XkPlayerCtrl.GetInstanceFeiJi().m_SpawnNpcManage.AddPuTongNpcToBuJiDt(npcMoveCom.gameObject);
                                }
                            }
                        }
                        break;
                    }
                case TriggerState.ZhanCheBoss:
                    {
                        if (npcMoveCom.GetIsBossNpc() == false)
                        {
                            isEnter = true;
                        }
                        break;
                    }
            }

            if (isEnter == true)
            {
                npcMoveCom.SetIsEnterCameraBox();
            }
        }
    }
}