using UnityEngine;

public class SSCaiPiaoBossMove : MonoBehaviour
{
    internal XKNpcMoveCtrl m_NpcMoveCom;
    /// <summary>
    /// 是否移动.
    /// </summary>
    internal bool IsMoveNpc = false;
    /// <summary>
    /// 移动的方向.
    /// </summary>
    float m_MoveDir = 1f;
    /// <summary>
    /// 碰了几次范围触发器.
    /// </summary>
    int m_CountHitFanWeiTrigger = 0;
    
    void FixedUpdate()
    {
        if (IsMoveNpc == true && m_NpcMoveCom != null)
        {
            UpdateMoveNpc();
        }
    }

    /// <summary>
    /// 是否有碰上中心触发器.
    /// </summary>
    bool IsHitCenterTrigger = false;
    public void ResetInfo()
    {
        IsMoveNpc = false;
        IsHitCenterTrigger = false;
    }

    /// <summary>
    /// 初始化运动.
    /// </summary>
    void InitMove()
    {
        if (IsMoveNpc == true)
        {
            return;
        }

        if (m_NpcMoveCom != null)
        {
            m_NpcMoveCom.CloseBossMoveing();
        }

        m_MoveDir = 1f;
        IsMoveNpc = true;
        IsHitCenterTrigger = true;
        m_CountHitFanWeiTrigger = 0;
        //停止镜头移动.
        XkGameCtrl.GetInstance().SetGameCameraIsMoveing(false, NpcJiFenEnum.Boss);
    }

    /// <summary>
    /// 结束移动.
    /// </summary>
    void CloseMove()
    {
        if (IsMoveNpc == false)
        {
            return;
        }
        IsMoveNpc = false;
        //镜头继续移动.
        XkGameCtrl.GetInstance().SetGameCameraIsMoveing(true, NpcJiFenEnum.Boss);

        if (m_NpcMoveCom != null)
        {
            m_NpcMoveCom.RestartMoveingBoss();
        }
    }

    public void OnHitBossMoveTrigger(SSTriggerCaiPiaoBossMove trigger)
    {
        if (trigger != null)
        {
            switch (trigger.m_TriggerDir)
            {
                case SSTriggerCaiPiaoBossMove.TriggerDir.Center:
                    {
                        if (m_NpcMoveCom != null)
                        {
                            switch (m_NpcMoveCom.m_TriggerDir)
                            {
                                case SSTriggerCaiPiaoBossMove.TriggerDir.Qian:
                                case SSTriggerCaiPiaoBossMove.TriggerDir.Hou:
                                    {
                                        if (trigger.m_TriggerCenter == SSTriggerCaiPiaoBossMove.TriggerCenter.ZYCenter)
                                        {
                                            //前后产生的彩票boss必须碰前后中心触发器.
                                            return;
                                        }
                                        break;
                                    }
                                case SSTriggerCaiPiaoBossMove.TriggerDir.Zuo:
                                case SSTriggerCaiPiaoBossMove.TriggerDir.You:
                                    {
                                        if (trigger.m_TriggerCenter == SSTriggerCaiPiaoBossMove.TriggerCenter.QHCenter)
                                        {
                                            //左右产生的彩票boss必须碰左右中心触发器.
                                            return;
                                        }
                                        break;
                                    }
                            }

                            if (IsHitCenterTrigger == false)
                            {
                                //首次碰上中心触发器.
                                InitMove();
                            }
                        }
                        break;
                    }
                default:
                    {
                        //碰上方位触发器.
                        //变更运动方向.
                        ChangeMoveDirection();
                        break;
                    }
            }
        }
    }

    /// <summary>
    /// 改变运动方向.
    /// </summary>
    void ChangeMoveDirection()
    {
        if (IsMoveNpc == false)
        {
            return;
        }

        m_CountHitFanWeiTrigger++;
        int max = 2;
        if (m_NpcMoveCom != null)
        {
            switch (m_NpcMoveCom.m_TriggerDir)
            {
                case SSTriggerCaiPiaoBossMove.TriggerDir.Qian:
                case SSTriggerCaiPiaoBossMove.TriggerDir.Hou:
                    {
                        max = XkGameCtrl.GetInstance().m_MaxHitQHBossMoveTrigger;
                        break;
                    }

                case SSTriggerCaiPiaoBossMove.TriggerDir.Zuo:
                case SSTriggerCaiPiaoBossMove.TriggerDir.You:
                    {
                        max = XkGameCtrl.GetInstance().m_MaxHitBossMoveTrigger;
                        break;
                    }
            }
        }

        if (m_CountHitFanWeiTrigger >= max)
        {
            //彩票boss的特殊移动逻辑结束.
            CloseMove();
            return;
        }
        m_MoveDir *= -1f;
    }

    void UpdateMoveNpc()
    {
        //if (IsMoveNpc == false)
        //{
        //    return;
        //}

        //if (m_NpcMoveCom == null)
        //{
        //    return;
        //}

        Vector3 pos = Vector3.zero;
        pos = transform.position + (m_MoveDir * transform.forward * m_NpcMoveCom.m_BossTeShuMoveSpeed * Time.fixedDeltaTime);
        transform.position = pos;
    }
}