using LitJson;
using System;
using UnityEngine;

public class WebSocketSimpet : MonoBehaviour
{
    #region Private Fields
    private WabData _wabData;
    #endregion

    #region Unity Events

    SSBoxPostNet m_SSBoxPostNet = null;
    bool IsInit = false;
    public void Init(SSBoxPostNet postNet)
    {
        if (IsInit)
        {
            return;
        }

        _wabData = new WabData();
        _wabData.m_WebSocketSimpet = this;

        m_SSBoxPostNet = postNet;
        IsInit = true;
    }

    void Update()
    {
        if (Application.isLoadingLevel)
        {
            m_TimeLastXinTiao = Time.time;
            return;
        }

        if (IsClosedWebSocket == true)
        {
            //游戏已经关闭WebScoket
            //SSDebug.Log("The game have been closed webSocket...");
            return;
        }

        if (Time.frameCount % 30 == 0)
        {
            if (m_SSBoxPostNet != null)
            {
                m_SSBoxPostNet.LoopGetGameConfigInfoFromHddServer();
            }
        }

        if (Time.time - m_TimeLastXinTiao >= 10f)
        {
            m_TimeLastXinTiao = Time.time;
            NetSendWebSocketXinTiaoMsg();
        }

        if (IsCheckXinTiaoMsg)
        {
            if (Time.time - m_TimeSendXinTiaoMsg > 30f)
            {
                IsCheckXinTiaoMsg = false;
                Debug.Log("Unity:"+"XinTiao Check TimeOut!");
                if (m_SSBoxPostNet != null)
                {
                    m_SSBoxPostNet.HttpSendPostLoginBox();
                }

                //网络故障,请检查网络并重启游戏.
                if (SSUIRoot.GetInstance().m_GameUIManage != null)
                {
                    SSUIRoot.GetInstance().m_GameUIManage.CreatWangLuoGuZhangUI();
                }
            }
        }
    }

    void OnDestroy()
    {
        Debug.Log("Unity:"+"OnDestroy...");
        if (_wabData != null && _wabData.WebSocket != null)
        {
            _wabData.WebSocket.Close();
        }
    }

    /// <summary>
    /// 打开WebSocket.
    /// </summary>
    public void OpenWebSocket(string url)
    {
        //if (url.Contains("https://game.hdiandian.com") == true)
        //{
        //    SSDebug.LogWarning("SendPost -> url ======= " + url); //test
        //}

        if (IsClosedWebSocket == true)
        {
            //游戏已经关闭WebScoket
            SSDebug.Log("The game have been closed webSocket...");
            return;
        }

        if (_wabData != null)
        {
            //设置服务器地址信息.
            _wabData.Address = url;

            if (_wabData.WebSocket == null)
            {
                _wabData.OpenWebSocket();
                Debug.Log("Unity:" + "Opening Web Socket -> url == " + url);
            }
            else
            {
                //Debug.Log("Unity:" + "Close Web Socket...");
                //_wabData.WebSocket.Close();
                Debug.Log("Unity:" + "Restart Open WebSocket...");
                _wabData.RestartOpenWebSocket();
            }
        }
    }

    bool IsClosedWebSocket = false;
    /// <summary>
    /// 关闭WebSocket
    /// </summary>
    public void CloseWebSocket()
    {
        if (_wabData != null)
        {
            if (IsClosedWebSocket == false)
            {
                SSDebug.Log("CloseWebSocket...");
                IsClosedWebSocket = true;
                _wabData.CloseSocket();
            }
        }
    }

    bool IsCheckXinTiaoMsg = false;
    float m_TimeLastXinTiao = 0f;
    float m_TimeSendXinTiaoMsg = 0f;
    /// <summary>
    /// 心跳消息检测服务器返回的数据.
    /// </summary>
    string m_XinTiaoReturnMsg = "{\"data\":\"{\\\"_msg_name\\\":\\\"GameCenter_Logon\\\"}\",\"type\":\"message\"}";
    /// <summary>
    /// 发送心跳消息(建议30秒一次),用来检测服务器是否掉线.
    /// </summary>
    void NetSendWebSocketXinTiaoMsg()
    {
        if (m_SSBoxPostNet == null)
        {
            return;
        }

        if (_wabData.WebSocket != null && _wabData.WebSocket.IsOpen)
        {
            if (IsCheckXinTiaoMsg)
            {
                return;
            }
            IsCheckXinTiaoMsg = true;
            m_TimeSendXinTiaoMsg = Time.time;
            string boxNumber = m_SSBoxPostNet.m_BoxLoginData.boxNumber;
            string msgToSend = boxNumber + "," + boxNumber + ",0,{\"_msg_name\":\"GameCenter_Logon\"}";
            // Send message to the server.
            _wabData.WebSocket.Send(msgToSend);
#if UNITY_EDITOR
            Debug.Log("Unity:"+"NetSendWebSocketXinTiaoMsg -> msgToSend == " + msgToSend);
#endif
        }
        else
        {
            Debug.Log("Unity:"+"NetSendWebSocketXinTiaoMsg -> Restart game box!");
            m_SSBoxPostNet.HttpSendPostLoginBox();
        }
    }

    bool IsInitGameWeiXinShouBingData = false;
    /// <summary>
    /// 初始化游戏微信手柄资源数据.
    /// </summary>
    public void NetInitGameWeiXinShouBingData()
    {
        if (m_SSBoxPostNet == null)
        {
            return;
        }

        if (_wabData.WebSocket != null && _wabData.WebSocket.IsOpen)
        {

            if (IsInitGameWeiXinShouBingData)
            {
                return;
            }
            IsInitGameWeiXinShouBingData = true;

            string boxNumber = m_SSBoxPostNet.m_BoxLoginData.boxNumber;
            string msgToSend = boxNumber + "," + boxNumber + ",0,";
            //m_GamePadState = GamePadState.Default; //test.
            switch (m_SSBoxPostNet.m_GamePadState)
            {
                case SSBoxPostNet.GamePadState.LeiTingZhanChe:
                    {
                        msgToSend += "{\"_msg_name\":\"initGamepad\","
                            + "\"_msg_object_str\":{\"backgroundImage\":\"bg_ltzj.png\","
                            + "\"loadGroup\":\"ltzj\","
                            + "\"buttonStyle\":["
                            + "{\"name\":\"direction\",\"image\":\"ball_ltzj.png\"},"
                            + "{\"name\":\"directionBg\",\"image\":\"circle_ltzj.png\"},"
                            + "{\"name\":\"fireA\",\"image\":\"a_ltzj.png\"},"
                            + "{\"name\":\"fireB\",\"image\":\"b_ltzj.png\"}"
                            + "]}}";
                        break;
                    }
                default:
                    {
                        msgToSend += "{\"_msg_name\":\"initGamepad\",\"_msg_object_str\":{\"loadGroup\":\"default\"}}";
                        break;
                    }
            }

            Debug.Log("InitGameWeiXinShouBingData:: m_GamePadState == " + m_SSBoxPostNet.m_GamePadState);
            Debug.Log("InitGameWeiXinShouBingData:: msg == " + msgToSend);
            _wabData.WebSocket.Send(msgToSend);
        }
    }

    /// <summary>
    /// 当用户余额消费完毕后，需要进行充值，客户端需要发送.
    /// 发送网络消息,请求微信游戏手柄显示充值界面.
    /// </summary>
    public void NetSendWeiXinPadShowTopUpPanel(int userId)
    {
        if (m_SSBoxPostNet == null)
        {
            return;
        }

        if (_wabData.WebSocket != null && _wabData.WebSocket.IsOpen)
        {
            string boxNumber = m_SSBoxPostNet.m_BoxLoginData.boxNumber;
            //boxNumber,boxNumber,用户ID,{ "_msg_object_str":{ "data":"","type":"topUp"},"_msg_name":"gamepad"}
            string msgToSend = boxNumber + "," + boxNumber + "," + userId
                + ",{\"_msg_object_str\":{\"data\":\"\",\"type\":\"topUp\"},\"_msg_name\":\"gamepad\"}";

            Debug.Log("NetSendWeiXinPadShowTopUpPanel:: m_GamePadState == " + m_SSBoxPostNet.m_GamePadState);
            Debug.Log("NetSendWeiXinPadShowTopUpPanel:: msg == " + msgToSend);
            _wabData.WebSocket.Send(msgToSend);
        }
    }
    
    /// <summary>
    /// 当游戏所有角色处于激活状态时，客户端需要发送游戏当前处于满员的消息给微信游戏手柄.
    /// </summary>
    public void NetSendWeiXinPadGamePlayerFull(int userId)
    {
        if (m_SSBoxPostNet == null)
        {
            return;
        }

        if (_wabData.WebSocket != null && _wabData.WebSocket.IsOpen)
        {
            string boxNumber = m_SSBoxPostNet.m_BoxLoginData.boxNumber;
            //boxNumber,boxNumber,用户ID,{ "_msg_object_str":{ "data":"","type":"full"},"_msg_name":"gamepad"}
            string msgToSend = boxNumber + "," + boxNumber + "," + userId
                + ",{\"_msg_object_str\":{\"data\":\"\",\"type\":\"full\"},\"_msg_name\":\"gamepad\"}";

            Debug.Log("NetSendWeiXinPadGamePlayerFull:: msg == " + msgToSend);
            _wabData.WebSocket.Send(msgToSend);
        }
        NetSendWXPadPlayerCloseConnect(userId);
    }

    /// <summary>
    /// 当游戏中玩家续费倒计时结束之后,玩家仍然没有成功续费时,客户端需要发送"充值超时,请稍后重新扫码"的消息给服务器.
    /// </summary>
    public void NetSendWXPadPlayerPayTimeOut(int userId)
    {
        NetSendWXPadPlayerCloseConnect(userId);
    }

    /// <summary>
    /// 发送关闭游戏手柄的消息.
    /// </summary>
    void NetSendWXPadPlayerCloseConnect(int userId)
    {
        if (m_SSBoxPostNet == null)
        {
            return;
        }

        if (_wabData.WebSocket != null && _wabData.WebSocket.IsOpen)
        {
            string boxNumber = m_SSBoxPostNet.m_BoxLoginData.boxNumber;
            //boxNumber,boxNumber,用户ID,{"_msg_object_str":{"data":"","type":"CloseConnect","userId":"94164"},"_msg_name":"gamepad"}
            string msgToSend = boxNumber + "," + boxNumber + "," + userId
                + ",{\"_msg_object_str\":{\"data\":\"\",\"type\":\"CloseConnect\",\"userId\":\"" + userId + "\"},\"_msg_name\":\"gamepad\"}";
            SSDebug.Log("NetSendWXPadPlayerCloseConnect:: msg == " + msgToSend);
            _wabData.WebSocket.Send(msgToSend);
        }
    }

    /// <summary>
    /// 收到WebSocket网络消息.
    /// </summary>
    public void OnMessageReceived(string message)
    {
        //Debug.LogWarning("OnMessageReceived -> message == " + message);
        if (IsCheckXinTiaoMsg)
        {
            if (message == m_XinTiaoReturnMsg)
            {
#if UNITY_EDITOR
                Debug.Log("Unity:"+"XinTiao Check Success!");
#endif
                IsCheckXinTiaoMsg = false;
                //删除网络故障,请检查网络并重启游戏.
                if (SSUIRoot.GetInstance().m_GameUIManage != null)
                {
                    SSUIRoot.GetInstance().m_GameUIManage.RemoveWangLuoGuZhangUI();
                }
            }
            return;
        }
        
        JsonData jd = JsonMapper.ToObject(message);
        string msgType = jd["type"].ToString();
        switch(msgType)
        {
            case "userInfo": //玩家登陆盒子信息.
                {
                    //{"data":{"sex":1,"headUrl":"http://game.hdiandian.com/h5/public/image/head/93124.jpg","userName":"666","userId":"93124"},"type":"userInfo"}
                    PlayerWeiXinData playerDt = new PlayerWeiXinData();
                    playerDt.sex = jd["data"]["sex"].ToString();
                    playerDt.headUrl = jd["data"]["headUrl"].ToString();
                    playerDt.userName = jd["data"]["userName"].ToString();
                    playerDt.userId = Convert.ToInt32(jd["data"]["userId"].ToString());
                    OnNetReceivePlayerLoginBoxMsg(playerDt);
                    break;
                }
            case "ReleasePlayer": //玩家退出盒子或其他消息.
                {
                    //{"type":"ReleasePlayer","userId":"93124"}
                    int userId = Convert.ToInt32(jd["userId"].ToString());
                    OnNetReceivePlayerExitBoxMsg(userId);
                    break;
                }
            case "directionAngle": //手柄方向消息.
                {
                    //{"data":53,"type":"directionAngle","userId":"93124"}
                    string dirVal = jd["data"].ToString();
                    int userId = Convert.ToInt32(jd["userId"].ToString());
                    OnNetReceiveDirectionAngleMsg(dirVal, userId);
                    break;
                }
            case "actionOperation": //手柄按键消息.
                {
                    //{"data":"fireA","type":"actionOperation","userId":"93124"}
                    string btVal = jd["data"].ToString();
                    int userId = Convert.ToInt32(jd["userId"].ToString());
                    OnNetReceiveActionOperationMsg(btVal, userId);
                    break;
                }
        }
    }
    
    public class PlayerWeiXinData
    {
        /// <summary>
        /// 性别.
        /// </summary>
        public enum SexPlayer
        {
            Man = 1,
            Women = 2,
        }
        public SexPlayer m_SexPlayer = SexPlayer.Man;

        string _sex = ""; //性别: 1 男, 2 女.
        public string sex
        {
            set
            {
                _sex = value;
                switch (_sex)
                {
                    case "1":
                        {
                            m_SexPlayer = SexPlayer.Man;
                            break;
                        }
                    case "2":
                        {
                            m_SexPlayer = SexPlayer.Man;
                            break;
                        }
                }
            }
        }

        public string headUrl = ""; //头像地址.
        public string userName = ""; //用户名称.
        public int userId = 0; //用户id.
    }

    /// <summary>
    /// 玩家登陆盒子响应事件.
    /// </summary>
    public delegate void EventPlayerLoginBox(PlayerWeiXinData val);
    public event EventPlayerLoginBox OnEventPlayerLoginBox;
    public void OnNetReceivePlayerLoginBoxMsg(PlayerWeiXinData val)
    {
        Debug.Log("Unity:"+"OnNetReceivePlayerLoginBoxMsg -> userName == " + val.userName + ", userId == " + val.userId);
        if (OnEventPlayerLoginBox != null)
        {
            OnEventPlayerLoginBox(val);
        }
    }

    /// <summary>
    /// 玩家退出盒子响应事件.
    /// </summary>
    public delegate void EventPlayerExitBox(int userId);
    public event EventPlayerExitBox OnEventPlayerExitBox;
    public void OnNetReceivePlayerExitBoxMsg(int userId)
    {
        Debug.Log("Unity:"+"OnNetReceivePlayerExitBoxMsg -> userId == " + userId);
        if (OnEventPlayerExitBox != null)
        {
            OnEventPlayerExitBox(userId);
        }
    }

    /// <summary>
    /// 手柄方向响应事件.
    /// </summary>
    public delegate void EventDirectionAngle(string val, int userId);
    public event EventDirectionAngle OnEventDirectionAngle;
    public void OnNetReceiveDirectionAngleMsg(string val, int userId)
    {
        //Debug.Log("Unity:"+"OnNetReceiveDirectionAngleMsg -> val == " + val + ", userId == " + userId);
        if (OnEventDirectionAngle != null)
        {
            OnEventDirectionAngle(val, userId);
        }
    }

    /// <summary>
    /// 手柄按键响应事件.
    /// </summary>
    public delegate void EventActionOperation(string val, int userId);
    public event EventActionOperation OnEventActionOperation;
    public void OnNetReceiveActionOperationMsg(string val, int userId)
    {
        //Debug.Log("Unity:"+"OnNetReceiveActionOperationMsg -> val == " + val + ", userId == " + userId);
        if (OnEventActionOperation != null)
        {
            OnEventActionOperation(val, userId);
        }
    }

    //void OnGUI()
    //{
    //    GUI.Box(new Rect(10f, 0f, Screen.width - 20f, 25f), _address);
    //}
    #endregion
}