using LitJson;
using System;
using System.Collections;
using System.Net.NetworkInformation;
using System.Text;
using UnityEngine;

public class SSBoxPostNet : MonoBehaviour
{
    public enum GamePadState
    {
        Default,                //默认手柄.
        LeiTingZhanChe,         //雷霆战车手柄.
    }
    /// <summary>
    /// 游戏手柄枚举.
    /// </summary>
    [HideInInspector]
    public GamePadState m_GamePadState = GamePadState.LeiTingZhanChe;

    public void Init()
    {
        //NetworkInterface[] nis = NetworkInterface.GetAllNetworkInterfaces();
        //foreach (NetworkInterface ni in nis)
        //{
        //    Debug.Log("Name = " + ni.Name);
        //    Debug.Log("Des = " + ni.Description);
        //    Debug.Log("Type = " + ni.NetworkInterfaceType.ToString());
        //    Debug.Log("Mac地址 = " + ni.GetPhysicalAddress().ToString());
        //    Debug.Log("------------------------------------------------");
        //    m_BoxLoginData.boxNumber = UnityEngine.Random.Range(10, 95) + m_GamePadState.ToString() + ni.GetPhysicalAddress().ToString();
        //    break;
        //}

        string ip = Network.player.ipAddress;
        ip = ip.Replace('.', (char)UnityEngine.Random.Range(97, 122));
        int indexStart = UnityEngine.Random.Range(0, 5);
        int strLen = ip.Length - indexStart;
        strLen = strLen > 8 ? 8 : strLen;
        ip = ip.Substring(indexStart, strLen);

        string key = ip + (char)UnityEngine.Random.Range(97, 122)
            + (DateTime.Now.Ticks % 999999).ToString();
        string boxNum = UnityEngine.Random.Range(10, 95) + m_GamePadState.ToString() + key;
        boxNum = boxNum.Length > 40 ? boxNum.Substring(0, 39) : boxNum;
        m_BoxLoginData.boxNumber = boxNum;
        Debug.Log("boxNumber == " + m_BoxLoginData.boxNumber);

        if (m_WebSocketSimpet != null)
        {
            m_WebSocketSimpet.Init(this);
        }
        HttpSendPostLoginBox();
        //Debug.Log("Unity:"+"md5: " + Md5Sum("23456sswl"));
    }
    
    /// <summary>
    /// MD5加密数据算法.
    /// </summary>
    public string Md5Sum(string strToEncrypt)
    {
        byte[] bs = UTF8Encoding.UTF8.GetBytes(strToEncrypt);
        System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5CryptoServiceProvider.Create();

        byte[] hashBytes = md5.ComputeHash(bs);

        string hashString = "";
        for (int i = 0; i < hashBytes.Length; i++)
        {
            hashString += System.Convert.ToString(hashBytes[i], 16).PadLeft(2, '0');
        }
        return hashString.PadLeft(32, '0');
    }

    /// <summary>
    /// Get网络数据.
    /// </summary>
    IEnumerator SendGet(string _url)
    {
        WWW getData = new WWW(_url);
        yield return getData;
        if (getData.error != null)
        {
            Debug.Log("Unity:"+"GetError: " + getData.error);
        }
        else
        {
            Debug.Log("Unity:"+"GetData: " + getData.text);
        }
    }

    /// <summary>
    /// post命令消息.
    /// </summary>
    enum PostCmd
    {
        BoxLogin, //盒子登录.
    }

    /// <summary>
    /// 盒子登陆返回枚举值.
    /// </summary>
    enum BoxLoginRt
    {
        Null = -1,
        Success = 0,          //登录成功.
        Failed = 1,           //异常.
        ZhuCeBoxFailed = 2,   //注册盒子失败.
    }
    BoxLoginRt m_BoxLoginRt = BoxLoginRt.Null;

    /// <summary>
    /// 盒子登陆数据.
    /// </summary>
    public class BoxLoginData
    {
        //public string url = "http://game.hdiandian.com/gameBox/logon";
        public string url = "http://h5.hdiandian.com/gameBox/logon";
        string _boxNumber = "1";
        /// <summary>
        /// 盒子编号(必须全是小写字母加数字).
        /// </summary>
        public string boxNumber
        {
            set
            {
                _boxNumber = value.ToLower();
                //設置紅點點遊戲手柄的url.
                string url = _hDianDianGamePadUrl + _boxNumber + "&gameId=1";
                hDianDianGamePadUrl = url;
            }
            get
            {
                //Debug.LogWarning("_boxNumber == " + _boxNumber);
                return _boxNumber;
            }
        }
        public string storeId = "150";              //商户id.
        public string channel = "CyberCloud";       //渠道.
        //public string gameId = "16";                //游戏id.
        public string gameId = "17";                //游戏id.

        //string _hDianDianGamePadUrl = "http://game.hdiandian.com/gamepad/index.html?boxNumber=";
        string _hDianDianGamePadUrl = "http://h5.hdiandian.com/gamepad/index.html?boxNumber=";
        /// <summary>
        /// 紅點點遊戲手柄的url.
        /// </summary>
        //public string hDianDianGamePadUrl = "http://game.hdiandian.com/gamepad/index.html?boxNumber=1";
        public string hDianDianGamePadUrl = "http://h5.hdiandian.com/gamepad/index.html?boxNumber=1";
        public BoxLoginData(string address, string idGame)
        {
            gameId = idGame;
            url = address + "/gameBox/logon";
            _hDianDianGamePadUrl = address + "/gamepad/index.html?boxNumber=";
            hDianDianGamePadUrl = address + "/gamepad/index.html?boxNumber=1";
        }
    }
    //public BoxLoginData m_BoxLoginData = new BoxLoginData("http://game.hdiandian.com", "16"); //测试号.
    public BoxLoginData m_BoxLoginData = new BoxLoginData("http://h5.hdiandian.com", "17"); //雷霆战车游戏正式号.

    /// <summary>
    /// 盒子登陆成功后返回的数据信息.
    /// </summary>
    class BoxLoginRtData
    {
        public string serverIp = "";           //连接websocket的ip.
        public string token = "";              //连接websocket的令牌.
        public string versionNumber = "";      //当前游戏最新的版本号,可能为空.
    }
    BoxLoginRtData m_BoxLoginDt = new BoxLoginRtData();

    /// <summary>
    /// Post网络数据.
    /// </summary>
    IEnumerator SendPost(string _url, WWWForm _wForm, PostCmd cmd)
    {
        WWW postData = new WWW(_url, _wForm);
        yield return postData;
        if (postData.error != null)
        {
            Debug.Log("Unity:"+"PostError: " + postData.error);
        }
        else
        {
            Debug.Log("Unity:"+cmd + " -> PostData: " + postData.text);
            switch(cmd)
            {
                case PostCmd.BoxLogin:
                    {
                        JsonData jd = JsonMapper.ToObject(postData.text);
                        m_BoxLoginRt = (BoxLoginRt)Convert.ToInt32(jd["code"].ToString());
                        if (Convert.ToInt32(jd["code"].ToString()) == (int)BoxLoginRt.Success)
                        {
                            m_BoxLoginDt.serverIp = jd["data"]["serverIp"].ToString();
                            m_BoxLoginDt.token = jd["data"]["token"].ToString();
                            Debug.Log("Unity:"+"serverIp " + m_BoxLoginDt.serverIp + ", token " + m_BoxLoginDt.token);
                            ConnectWebSocketServer();
                        }
                        else
                        {
                            Debug.Log("Unity:"+"Login box failed! code == " + jd["code"]);
                        }
                        break;
                    }
            }
        }
    }

    /// <summary>
    /// WebSocket通讯控制组件.
    /// </summary>
    public WebSocketSimpet m_WebSocketSimpet;
    /// <summary>
    /// 链接游戏服务器.
    /// </summary>
    void ConnectWebSocketServer()
    {
        if (m_BoxLoginRt != BoxLoginRt.Success)
        {
            Debug.Log("Unity:"+"ConnectWebSocket -> m_BoxLoginRt == " + m_BoxLoginRt);
            return;
        }

        string url = "ws://" + m_BoxLoginDt.serverIp + "/websocket.do?token=" + m_BoxLoginDt.token;
        Debug.Log("Unity:"+"ConnectWebSocket -> url " + url);
        if (m_WebSocketSimpet != null)
        {
            m_WebSocketSimpet.OpenWebSocket(url);
        }
    }

    /// <summary>
    /// 发送登陆盒子的消息.
    /// </summary>
    public void HttpSendPostLoginBox()
    {
        Debug.Log("Unity:"+"HttpSendPostLoginBox...");
        //POST方法.
        WWWForm form = new WWWForm();
        form.AddField("boxNumber", m_BoxLoginData.boxNumber);
        form.AddField("storeId", m_BoxLoginData.storeId);
        form.AddField("channel", m_BoxLoginData.channel);
        form.AddField("gameId", m_BoxLoginData.gameId);
        StartCoroutine(SendPost(m_BoxLoginData.url, form, PostCmd.BoxLogin));
    }
}