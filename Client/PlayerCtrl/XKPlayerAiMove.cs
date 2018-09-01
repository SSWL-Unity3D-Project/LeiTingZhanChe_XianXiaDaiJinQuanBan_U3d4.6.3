using UnityEngine;

public class XKPlayerAiMove : SSGameMono
{
    XKPlayerMoveCtrl m_PlayerMoveCom;
    /// <summary>
    /// 运动方向枚举.
    /// </summary>
    enum DirectionMove
    {
        Stop = 0,
        Qian = 1,
        Hou = 2,
        Zuo = 3,
        You = 4,
        ZuoQian = 5,
        YouQian = 6,
        ZuoHou = 7,
        YouHou = 8,
    }

    /// <summary>
    /// 时间记录信息.
    /// </summary>
    float m_LastMoveTime = 0f;
    /// <summary>
    /// 运动随机时间.
    /// </summary>
    float m_RandMoveTime = 0f;

    /// <summary>
    /// 是否打开玩家坦克Ai运动.
    /// </summary>
    internal bool IsOpenPlayerAiMove = false;

    public void Init(XKPlayerMoveCtrl playerMove)
    {
        m_PlayerMoveCom = playerMove;
    }

    void Update()
    {
        UpdatePlayerAiMove();
    }

    void UpdatePlayerAiMove()
    {
        if (IsOpenPlayerAiMove == false)
        {
            return;
        }

        if (Time.time - m_LastMoveTime >= m_RandMoveTime)
        {
            ChangePlayerMoveDirBtInfo();
        }
    }

    /// <summary>
    /// 当玩家碰上范围触发器时获取随机运动方向.
    /// </summary>
    DirectionMove GetPlayerMoveDirOnHitTrigger(SSTriggerAiPlayerMove.TiggerState type)
    {
        int randVal = Random.Range(0, 100) % 5;
        DirectionMove dirType = DirectionMove.Stop;
        switch (type)
        {
            case SSTriggerAiPlayerMove.TiggerState.Qian:
                {
                    if (randVal == 0)
                    {
                        dirType = DirectionMove.Zuo;
                    }
                    else if (randVal == 1)
                    {
                        dirType = DirectionMove.ZuoHou;
                    }
                    else if (randVal == 2)
                    {
                        dirType = DirectionMove.Hou;
                    }
                    else if (randVal == 3)
                    {
                        dirType = DirectionMove.YouHou;
                    }
                    else if (randVal == 4)
                    {
                        dirType = DirectionMove.You;
                    }
                    break;
                }
            case SSTriggerAiPlayerMove.TiggerState.Hou:
                {
                    if (randVal == 0)
                    {
                        dirType = DirectionMove.Zuo;
                    }
                    else if (randVal == 1)
                    {
                        dirType = DirectionMove.ZuoQian;
                    }
                    else if (randVal == 2)
                    {
                        dirType = DirectionMove.Qian;
                    }
                    else if (randVal == 3)
                    {
                        dirType = DirectionMove.YouQian;
                    }
                    else if (randVal == 4)
                    {
                        dirType = DirectionMove.You;
                    }
                    break;
                }
            case SSTriggerAiPlayerMove.TiggerState.Zuo:
                {
                    if (randVal == 0)
                    {
                        dirType = DirectionMove.Hou;
                    }
                    else if (randVal == 1)
                    {
                        dirType = DirectionMove.YouHou;
                    }
                    else if (randVal == 2)
                    {
                        dirType = DirectionMove.You;
                    }
                    else if (randVal == 3)
                    {
                        dirType = DirectionMove.YouQian;
                    }
                    else if (randVal == 4)
                    {
                        dirType = DirectionMove.Qian;
                    }
                    break;
                }
            case SSTriggerAiPlayerMove.TiggerState.You:
                {
                    if (randVal == 0)
                    {
                        dirType = DirectionMove.Hou;
                    }
                    else if (randVal == 1)
                    {
                        dirType = DirectionMove.ZuoHou;
                    }
                    else if (randVal == 2)
                    {
                        dirType = DirectionMove.Zuo;
                    }
                    else if (randVal == 3)
                    {
                        dirType = DirectionMove.ZuoQian;
                    }
                    else if (randVal == 4)
                    {
                        dirType = DirectionMove.Qian;
                    }
                    break;
                }
        }
        return dirType;
    }

    /// <summary>
    /// 玩家Ai坦克碰上范围触发器后执行.
    /// </summary>
    public void OnHitAiMoveTrigger(SSTriggerAiPlayerMove.TiggerState type)
    {
        if (IsOpenPlayerAiMove == false)
        {
            return;
        }

        if (m_PlayerMoveCom == null)
        {
            UnityLogWarning("OnHitAiMoveTrigger -> m_PlayerMoveCom was null..........");
            return;
        }

        DirectionMove dirMove = GetPlayerMoveDirOnHitTrigger(type);
        m_RandMoveTime = Random.Range(1f, 3f);
        m_LastMoveTime = Time.time;
        OnChangePlayerMoveDirBtInfo(dirMove);
    }

    /// <summary>
    /// 改变玩家运动方向.
    /// </summary>
    void OnChangePlayerMoveDirBtInfo(DirectionMove dirMove)
    {
        if (m_PlayerMoveCom == null)
        {
            UnityLogWarning("OnHitAiMoveTrigger -> m_PlayerMoveCom was null..........");
            return;
        }

        int indexVal = (int)m_PlayerMoveCom.PlayerIndex - 1;
        if (indexVal < 0 || indexVal > 3)
        {
            UnityLogWarning("OnChangePlayerMoveDirBtInfo -> indexVal was wrong! indexVal ========= " + indexVal);
            return;
        }

        switch (dirMove)
        {
            case DirectionMove.Stop:
                {
                    InputEventCtrl.GetInstance().OnClickFangXiangUBt(indexVal, pcvr.ButtonState.UP);
                    InputEventCtrl.GetInstance().OnClickFangXiangDBt(indexVal, pcvr.ButtonState.UP);
                    InputEventCtrl.GetInstance().OnClickFangXiangLBt(indexVal, pcvr.ButtonState.UP);
                    InputEventCtrl.GetInstance().OnClickFangXiangRBt(indexVal, pcvr.ButtonState.UP);
                    break;
                }
            case DirectionMove.Qian:
                {
                    InputEventCtrl.GetInstance().OnClickFangXiangUBt(indexVal, pcvr.ButtonState.DOWN);
                    InputEventCtrl.GetInstance().OnClickFangXiangDBt(indexVal, pcvr.ButtonState.UP);
                    InputEventCtrl.GetInstance().OnClickFangXiangLBt(indexVal, pcvr.ButtonState.UP);
                    InputEventCtrl.GetInstance().OnClickFangXiangRBt(indexVal, pcvr.ButtonState.UP);
                    break;
                }
            case DirectionMove.Hou:
                {
                    InputEventCtrl.GetInstance().OnClickFangXiangUBt(indexVal, pcvr.ButtonState.UP);
                    InputEventCtrl.GetInstance().OnClickFangXiangDBt(indexVal, pcvr.ButtonState.DOWN);
                    InputEventCtrl.GetInstance().OnClickFangXiangLBt(indexVal, pcvr.ButtonState.UP);
                    InputEventCtrl.GetInstance().OnClickFangXiangRBt(indexVal, pcvr.ButtonState.UP);
                    break;
                }
            case DirectionMove.Zuo:
                {
                    InputEventCtrl.GetInstance().OnClickFangXiangUBt(indexVal, pcvr.ButtonState.UP);
                    InputEventCtrl.GetInstance().OnClickFangXiangDBt(indexVal, pcvr.ButtonState.UP);
                    InputEventCtrl.GetInstance().OnClickFangXiangLBt(indexVal, pcvr.ButtonState.DOWN);
                    InputEventCtrl.GetInstance().OnClickFangXiangRBt(indexVal, pcvr.ButtonState.UP);
                    break;
                }
            case DirectionMove.You:
                {
                    InputEventCtrl.GetInstance().OnClickFangXiangUBt(indexVal, pcvr.ButtonState.UP);
                    InputEventCtrl.GetInstance().OnClickFangXiangDBt(indexVal, pcvr.ButtonState.UP);
                    InputEventCtrl.GetInstance().OnClickFangXiangLBt(indexVal, pcvr.ButtonState.UP);
                    InputEventCtrl.GetInstance().OnClickFangXiangRBt(indexVal, pcvr.ButtonState.DOWN);
                    break;
                }
            case DirectionMove.ZuoQian:
                {
                    InputEventCtrl.GetInstance().OnClickFangXiangUBt(indexVal, pcvr.ButtonState.DOWN);
                    InputEventCtrl.GetInstance().OnClickFangXiangDBt(indexVal, pcvr.ButtonState.UP);
                    InputEventCtrl.GetInstance().OnClickFangXiangLBt(indexVal, pcvr.ButtonState.DOWN);
                    InputEventCtrl.GetInstance().OnClickFangXiangRBt(indexVal, pcvr.ButtonState.UP);
                    break;
                }
            case DirectionMove.YouQian:
                {
                    InputEventCtrl.GetInstance().OnClickFangXiangUBt(indexVal, pcvr.ButtonState.DOWN);
                    InputEventCtrl.GetInstance().OnClickFangXiangDBt(indexVal, pcvr.ButtonState.UP);
                    InputEventCtrl.GetInstance().OnClickFangXiangLBt(indexVal, pcvr.ButtonState.UP);
                    InputEventCtrl.GetInstance().OnClickFangXiangRBt(indexVal, pcvr.ButtonState.DOWN);
                    break;
                }
            case DirectionMove.ZuoHou:
                {
                    InputEventCtrl.GetInstance().OnClickFangXiangUBt(indexVal, pcvr.ButtonState.UP);
                    InputEventCtrl.GetInstance().OnClickFangXiangDBt(indexVal, pcvr.ButtonState.DOWN);
                    InputEventCtrl.GetInstance().OnClickFangXiangLBt(indexVal, pcvr.ButtonState.DOWN);
                    InputEventCtrl.GetInstance().OnClickFangXiangRBt(indexVal, pcvr.ButtonState.UP);
                    break;
                }
            case DirectionMove.YouHou:
                {
                    InputEventCtrl.GetInstance().OnClickFangXiangUBt(indexVal, pcvr.ButtonState.UP);
                    InputEventCtrl.GetInstance().OnClickFangXiangDBt(indexVal, pcvr.ButtonState.DOWN);
                    InputEventCtrl.GetInstance().OnClickFangXiangLBt(indexVal, pcvr.ButtonState.UP);
                    InputEventCtrl.GetInstance().OnClickFangXiangRBt(indexVal, pcvr.ButtonState.DOWN);
                    break;
                }
        }
    }

    /// <summary>
    /// 获取随机运动方向.
    /// </summary>
    DirectionMove GetRandomPlayerMoveDir()
    {
        return (DirectionMove)(Random.Range(0, 100) % 9);
    }

    /// <summary>
    /// 打开玩家Ai坦克运动.
    /// </summary>
    public void OpenPlayerAiMove()
    {
        if (IsOpenPlayerAiMove == true)
        {
            return;
        }
        IsOpenPlayerAiMove = true;
        enabled = true;
        ChangePlayerMoveDirBtInfo();
    }
    
    /// <summary>
    /// 改变玩家Ai坦克运动方向.
    /// </summary>
    void ChangePlayerMoveDirBtInfo()
    {
        if (IsOpenPlayerAiMove == false)
        {
            return;
        }

        m_LastMoveTime = Time.time;
        DirectionMove dirMove = GetRandomPlayerMoveDir();
        if (dirMove == DirectionMove.Stop)
        {
            m_RandMoveTime = Random.Range(0.5f, 1.5f);
        }
        else
        {
            m_RandMoveTime = Random.Range(1.5f, 2.5f);
        }
        OnChangePlayerMoveDirBtInfo(dirMove);
    }
    
    /// <summary>
    /// 关闭玩家Ai坦克运动.
    /// </summary>
    public void ClosePlayerAiMove()
    {
        if (IsOpenPlayerAiMove == false)
        {
            return;
        }
        IsOpenPlayerAiMove = false;
        enabled = false;
    }
}