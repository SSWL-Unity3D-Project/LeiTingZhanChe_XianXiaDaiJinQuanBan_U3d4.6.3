using System;
using UnityEngine;
using BestHTTP;
using BestHTTP.WebSocket;

public class WabData
{
    /// <summary>  
    /// The WebSocket address to connect  
    /// </summary>  
    private string address = "ws://192.168.1.16:1818";

    /// <summary>  
    /// Default text to send  
    /// </summary>  
    private string _msgToSend = "Hello World!";

    /// <summary>  
    /// Debug text to draw on the gui  
    /// </summary>  
    private string _text = string.Empty;

    /// <summary>  
    /// Saved WebSocket instance  
    /// </summary>  
    private WebSocket _webSocket;

    /// <summary>
    /// WebSocket通讯控制组件.
    /// </summary>
    public WebSocketSimpet m_WebSocketSimpet;
    public WebSocket WebSocket { get { return _webSocket; } }
    public string Address
    {
        set { address = value; }
        get { return address; }
    }
    public string Text { get { return _text; } }

    public string MsgToSend
    {
        get { return _msgToSend; }
        set
        {
            _msgToSend = value;
            SendMsg(value);
        }
    }

    public void OpenWebSocket()
    {
        if (_webSocket == null)
        {
            SSDebug.LogWarning("OpenWebSocket ......... address == " + address);
            SSDebug.LogWarning("OpenWebSocket ......... time == " + Time.time.ToString("f2"));
            // Create the WebSocket instance  
            _webSocket = new WebSocket(new Uri(address));

            if (HTTPManager.Proxy != null)
                _webSocket.InternalRequest.Proxy = new HTTPProxy(HTTPManager.Proxy.Address, HTTPManager.Proxy.Credentials, false);

            // Subscribe to the WS events  
            _webSocket.OnOpen += OnOpen;
            _webSocket.OnMessage += OnMessageReceived;
            _webSocket.OnClosed += OnClosed;
            _webSocket.OnError += OnError;

            // Start connecting to the server  
            _webSocket.Open();
        }
    }

    public void RestartOpenWebSocket()
    {
        if (_webSocket != null)
        {
            if (_webSocket.IsOpen == true)
            {
                _webSocket.Close();
            }
        }

        // Create the WebSocket instance  
        _webSocket = new WebSocket(new Uri(address));

        if (HTTPManager.Proxy != null)
            _webSocket.InternalRequest.Proxy = new HTTPProxy(HTTPManager.Proxy.Address, HTTPManager.Proxy.Credentials, false);

        // Subscribe to the WS events  
        _webSocket.OnOpen += OnOpen;
        _webSocket.OnMessage += OnMessageReceived;
        _webSocket.OnClosed += OnClosed;
        _webSocket.OnError += OnError;

        if (_webSocket != null && _webSocket.IsOpen == false)
        {
            SSDebug.LogWarning("RestartOpenWebSocket ......... address == " + address);
            SSDebug.LogWarning("RestartOpenWebSocket ......... time == " + Time.time.ToString("f2"));
            // Start connecting to the server  
            _webSocket.Open();
        }
    }

    public void SendMsg(string msg)
    {
        // Send message to the server  
        _webSocket.Send(msg);
    }

    public void CloseSocket()
    {
        if (_webSocket != null)
        {
            // Close the connection  
            _webSocket.Close(1000, "Bye!");
        }
    }

    /// <summary>  
    /// Called when the web socket is open, and we are ready to send and receive data  
    /// </summary>  
    void OnOpen(WebSocket ws)
    {
        SSDebug.LogWarning("WebSocket Open!");
        //if (m_WebSocketSimpet != null)
        //{
        //    m_WebSocketSimpet.NetInitGameWeiXinShouBingData();
        //}
    }

    /// <summary>
    /// Called when we received a text message from the server.
    /// </summary>
    void OnMessageReceived(WebSocket ws, string message)
    {
        //Debug.Log("Unity:"+"OnMessageReceived -> message == " + message);
        if (m_WebSocketSimpet != null)
        {
            m_WebSocketSimpet.OnMessageReceived(message);
        }
    }

    /// <summary>  
    /// Called when the web socket closed  
    /// </summary>  
    void OnClosed(WebSocket ws, UInt16 code, string message)
    {
        SSDebug.LogWarning("WebData::OnClosed -> " + string.Format("-WebSocket closed! Code: {0} Message: {1}", code, message));
        _webSocket = null;
        //if (m_WebSocketSimpet != null && Application.isPlaying)
        //{
        //    Debug.Log("Unity:" + "OnClosed::Restart Web Socket -> url == " + Address);
        //    m_WebSocketSimpet.OpenWebSocket(Address);
        //}
    }

    float m_LastTimeError = -100f;
    /// <summary>
    /// 收到错误消息的最大次数.
    /// </summary>
    int m_MaxOnErrorNum = 3;
    /// <summary>
    /// 错误消息的记录次数.
    /// </summary>
    int m_OnErrorCount = 0;
    /// <summary>
    /// 检测是否退出错误消息.
    /// </summary>
    bool CheckIsReturnOnError()
    {
        bool isReturn = false;
        m_OnErrorCount++;
        if (m_OnErrorCount <= m_MaxOnErrorNum)
        {
            isReturn = true;
        }
        else
        {
            m_OnErrorCount = 0;
        }
        return isReturn;
    }

    /// <summary>  
    /// Called when an error occured on client side  
    /// </summary>  
    void OnError(WebSocket ws, Exception ex)
    {
        string errorMsg = string.Empty;
        if (ws.InternalRequest.Response != null)
            errorMsg = string.Format("Status Code from Server: {0} and Message: {1}", ws.InternalRequest.Response.StatusCode, ws.InternalRequest.Response.Message);

        SSDebug.LogWarning("WebData::OnError -> " + string.Format("-An error occured: {0}", ex != null ? ex.Message : "Unknown Error " + errorMsg)
            + ", time == " + Time.time.ToString("f2"));

        //Unity: WebData::OnError-> - An error occured: Unknown Error Status Code from Server: 101 and Message: , time == 567.88
        if (ex != null)
        {
            switch (ex.Message)
            {
                case "Read failure":
                    {
                        //从服务器读取数据失败.
                        return;
                    }
                case "TCP Stream closed unexpectedly by the remote server":
                    {
                        //服务器关闭了websocket.
                        if (_webSocket != null)
                        {
                            if (_webSocket.IsOpen == true)
                            {
                                //关闭webSocket.
                                _webSocket.Close();
                            }
                            _webSocket = null;
                            return;
                        }
                        break;
                    }
            }
        }

        _webSocket = null;
        if (Time.time - m_LastTimeError < 5f)
        {
            return;
        }
        m_LastTimeError = Time.time;

        if (CheckIsReturnOnError() == true)
        {
            return;
        }

        //string errorMsg = string.Empty;
        //if (ws.InternalRequest.Response != null)
        //    errorMsg = string.Format("Status Code from Server: {0} and Message: {1}", ws.InternalRequest.Response.StatusCode, ws.InternalRequest.Response.Message);

        //Debug.LogWarning("Unity:"+string.Format("-An error occured: {0}\n", ex != null ? ex.Message : "Unknown Error " + errorMsg)
        //    + ", time == " + Time.time.ToString("f2"));
        if (ex != null)
        {
            switch (ex.Message)
            {
                case "TCP Stream closed unexpectedly by the remote server":
                    {
                        //服务器故障.
                        break;
                    }
                default:
                    {
                        if (Application.isPlaying == true)
                        {
                            //网络故障,请检查网络并重启游戏.
                            if (SSUIRoot.GetInstance().m_GameUIManage != null
                                && SSUIRoot.GetInstance().m_GameUIManage.m_WangLuoGuZhangUI == null)
                            {
                                SSUIRoot.GetInstance().m_GameUIManage.CreatWangLuoGuZhangUI();
                            }
                        }
                        break;
                    }
            }
        }
        else
        {
            if (Application.isPlaying == true)
            {
                //网络故障,请检查网络并重启游戏.
                if (SSUIRoot.GetInstance().m_GameUIManage != null
                    && SSUIRoot.GetInstance().m_GameUIManage.m_WangLuoGuZhangUI == null)
                {
                    SSUIRoot.GetInstance().m_GameUIManage.CreatWangLuoGuZhangUI();
                }
            }
        }
    }
}