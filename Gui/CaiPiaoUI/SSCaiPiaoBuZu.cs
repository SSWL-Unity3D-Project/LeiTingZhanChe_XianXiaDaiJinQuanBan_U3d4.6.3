using System;
using UnityEngine;

public class SSCaiPiaoBuZu : SSGameMono
{
    //PlayerEnum m_IndexPlayer;
    // Use this for initialization
	public void Init(PlayerEnum indexPlayer)
    {
        //m_IndexPlayer = indexPlayer;
        //switch (indexPlayer)
        //{
        //    case PlayerEnum.PlayerOne:
        //        {
        //            InputEventCtrl.GetInstance().ClickStartBtOneEvent += ClickStartBtOneEvent;
        //            break;
        //        }
        //    case PlayerEnum.PlayerTwo:
        //        {
        //            InputEventCtrl.GetInstance().ClickStartBtTwoEvent += ClickStartBtTwoEvent;
        //            break;
        //        }
        //    case PlayerEnum.PlayerThree:
        //        {
        //            InputEventCtrl.GetInstance().ClickStartBtThreeEvent += ClickStartBtThreeEvent;
        //            break;
        //        }
        //}
        InputEventCtrl.GetInstance().ClickSetMoveBtEvent += ClickSetMoveBtEvent;
    }

    private void ClickStartBtOneEvent(pcvr.ButtonState val)
    {
        if (val == pcvr.ButtonState.UP)
        {
            PcvrRestartPrintCaiPiao(PlayerEnum.PlayerOne);
        }
    }

    private void ClickStartBtTwoEvent(pcvr.ButtonState val)
    {
        if (val == pcvr.ButtonState.UP)
        {
            PcvrRestartPrintCaiPiao(PlayerEnum.PlayerTwo);
        }
    }

    private void ClickStartBtThreeEvent(pcvr.ButtonState val)
    {
        if (val == pcvr.ButtonState.UP)
        {
            PcvrRestartPrintCaiPiao(PlayerEnum.PlayerThree);
        }
    }

    /// <summary>
    /// 重新开始出票.
    /// </summary>
    void PcvrRestartPrintCaiPiao(PlayerEnum indexPlayer)
    {
        //这里添加pcvr重新出票的代码.
        pcvr.GetInstance().RestartPrintCaiPiao(indexPlayer);
    }

    private void ClickSetMoveBtEvent(pcvr.ButtonState val)
    {
        IsBtDownSetMove = val == pcvr.ButtonState.DOWN ? true : false;
        if (IsBtDownSetMove)
        {
            m_TimeValBtDown = 0f;
        }
    }

    /// <summary>
    /// 是否按下移动按键.
    /// </summary>
    bool IsBtDownSetMove = false;
    /// <summary>
    /// 时间记录信息.
    /// </summary>
    float m_TimeValBtDown = 0f;
    void FixedUpdate()
    {
        if (IsRemoveSelf)
        {
            return;
        }

        if (IsBtDownSetMove)
        {
            m_TimeValBtDown += Time.fixedDeltaTime;
            if (m_TimeValBtDown >= 2f)
            {
                //工作人员长按移动按键时.
                //清空当前缺票机位的彩票信息.
                if (SSUIRoot.GetInstance().m_GameUIManage != null)
                {
                    SSUIRoot.GetInstance().m_GameUIManage.RemoveAllCaiPiaoBuZuPanel();
                }
            }
        }
    }

    bool IsRemoveSelf = false;
    public void RemoveSelf()
    {
        if (!IsRemoveSelf)
        {
            IsRemoveSelf = true;
            //switch (m_IndexPlayer)
            //{
            //    case PlayerEnum.PlayerOne:
            //        {
            //            InputEventCtrl.GetInstance().ClickStartBtOneEvent -= ClickStartBtOneEvent;
            //            break;
            //        }
            //    case PlayerEnum.PlayerTwo:
            //        {
            //            InputEventCtrl.GetInstance().ClickStartBtTwoEvent -= ClickStartBtTwoEvent;
            //            break;
            //        }
            //    case PlayerEnum.PlayerThree:
            //        {
            //            InputEventCtrl.GetInstance().ClickStartBtThreeEvent -= ClickStartBtThreeEvent;
            //            break;
            //        }
            //}
            InputEventCtrl.GetInstance().ClickSetMoveBtEvent -= ClickSetMoveBtEvent;
            Destroy(gameObject);
        }
    }
}