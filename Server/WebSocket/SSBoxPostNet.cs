using LitJson;
using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using UnityEngine;

public class SSBoxPostNet : MonoBehaviour
{
    public enum GamePadState
    {
        Default = 0,                //默认手柄.
        LeiTingZhanChe = 1,         //雷霆战车手柄.
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
        strLen = strLen > 6 ? 6 : strLen;
        indexStart = ip.Length - strLen;
        ip = ip.Substring(indexStart, strLen);
        
        string key = ip + (char)UnityEngine.Random.Range(97, 122)
            + (DateTime.Now.Ticks % 999999).ToString();
        string boxNum = UnityEngine.Random.Range(10, 95) + m_GamePadState.ToString() + key;
        boxNum = boxNum.Length > 28 ? boxNum.Substring(0, 28) : boxNum;
        m_BoxLoginData.boxNumber = boxNum;
        Debug.Log("boxNumber == " + m_BoxLoginData.boxNumber);

        if (m_WebSocketSimpet != null)
        {
            m_WebSocketSimpet.Init(this);
        }
        HttpSendPostLoginBox();
        HttpSendGetWeiXinXiaoChengXuUrl();
        HttpSendGetServerTimeInfo();

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
    /// post命令消息.
    /// </summary>
    enum PostCmd
    {
        /// <summary>
        /// 盒子登录.
        /// </summary>
        BoxLogin = 0,
        /// <summary>
        /// 微信小程序Url获取post.
        /// </summary>
        WX_XCX_URL_POST = 1,
        /// <summary>
        /// 获取服务器的时间数据.
        /// 用来与当前安卓盒子或PC机器的系统时间进行比较.
        /// </summary>
        ServerTimeGet = 2,
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
        /// <summary>
        /// 游戏盒子登陆url.
        /// </summary>
        public string url = "http://h5.hdiandian.com/gameBox/logon";
        /// <summary>
        /// 域名地址.
        /// </summary>
        string _address = "";
        /// <summary>
        /// 域名地址.
        /// </summary>
        public string m_Address { get { return _address; } }

        /// <summary>
        /// 微信小程序游戏代码雷霆战车.
        /// </summary>
        public const string WXCodeGameLeiTingZheChe = "1";
        /// <summary>
        /// 微信小程序游戏代码.
        /// </summary>
        public string WXCodeGame = "0";
        public string _WX_XiaoChengXu_Url_Post = "https://game.hdiandian.com/wxbackstage/wechat/qrcode";
        /// <summary>
        /// 微信小程序url获取的地址.
        /// https://game.hdiandian.com/wxbackstage/wechat/qrcode/{boxNumber}/{code}
        /// </summary>
        string WX_XiaoChengXu_Url_Post = "https://game.hdiandian.com/wxbackstage/wechat/qrcode/{1}/{0}";

        /// <summary>
        /// 微信小程序二维码图片保存路径.
        /// </summary>
        public string WX_XiaoChengXu_ErWeiMa_Path
        {
            get
            {
                string path = "";
                string fileName = "WXChengXu.png";
#if UNITY_ANDROID
                path = Application.persistentDataPath + "/" + fileName;
#endif
#if UNITY_STANDALONE_WIN
                path = Application.dataPath + "/" + fileName;
#endif
                return path;
            }
        }

        /// <summary>
        /// 获取小程序代码
        /// </summary>
        public string GetWXCodeGame(GamePadState pad)
        {
            switch (pad)
            {
                case GamePadState.LeiTingZhanChe:
                    {
                        WXCodeGame = WXCodeGameLeiTingZheChe;
                        break;
                    }
            }
            return WXCodeGame;
        }
        /// <summary>
        /// 获取微信小程序Url的Post地址.
        /// </summary>
        public string GetWeiXinXiaoChengXuUrlPostInfo(GamePadState pad)
        {
            GetWXCodeGame(pad);
            //设置微信小程序url获取的地址.
            WX_XiaoChengXu_Url_Post = _WX_XiaoChengXu_Url_Post + "/" + _boxNumber + "/" + WXCodeGame;
            //WX_XiaoChengXu_Url_Post = _WX_XiaoChengXu_Url_Post;
            //Debug.Log("Unity: WX_XiaoChengXu_Url_Post ==== " + WX_XiaoChengXu_Url_Post);
            return WX_XiaoChengXu_Url_Post;
        }
        
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
        //public string gameId = "16";              //游戏id.
        public string gameId = "17";                //游戏id.

        //测试域名.
        //string _hDianDianGamePadUrl = "http://game.hdiandian.com/gamepad/index.html?boxNumber=";
        //正式域名.
       string _hDianDianGamePadUrl = "http://h5.hdiandian.com/gamepad/index.html?boxNumber=";
        /// <summary>
        /// 红点点游戏手柄的url.
        /// </summary>
        //public string hDianDianGamePadUrl = "http://game.hdiandian.com/gamepad/index.html?boxNumber=1";
        public string hDianDianGamePadUrl = "http://h5.hdiandian.com/gamepad/index.html?boxNumber=1";
        public BoxLoginData(string address, string idGame)
        {
            gameId = idGame;
            _address = address;
            url = address + "/gameBox/logon";
            _hDianDianGamePadUrl = address + "/gamepad/index.html?boxNumber=";
            hDianDianGamePadUrl = address + "/gamepad/index.html?boxNumber=1";
        }
    }
    public BoxLoginData m_BoxLoginData = new BoxLoginData("http://game.hdiandian.com", "16"); //测试号.
    //public BoxLoginData m_BoxLoginData = new BoxLoginData("http://h5.hdiandian.com", "17"); //雷霆战车游戏正式号.

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
        WWW postData = null;
        if (_wForm == null)
        {
            postData = new WWW(_url);
        }
        else
        {
            postData = new WWW(_url, _wForm);
        }
        yield return postData;

        if (postData.error != null)
        {
            Debug.Log("Unity:"+"PostError: " + postData.error);
            //网络故障,请检查网络并重启游戏.
            if (SSUIRoot.GetInstance().m_GameUIManage != null)
            {
                SSUIRoot.GetInstance().m_GameUIManage.CreatWangLuoGuZhangUI();
            }
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
    /// Get网络数据.
    /// </summary>
    IEnumerator SendGet(string _url, PostCmd cmd)
    {
        WWW getData = new WWW(_url);
        yield return getData;
        if (getData.error != null)
        {
            Debug.Log("Unity:" + "GetError: " + getData.error);
            //网络故障,请检查网络并重启游戏.
            if (SSUIRoot.GetInstance().m_GameUIManage != null)
            {
                SSUIRoot.GetInstance().m_GameUIManage.CreatWangLuoGuZhangUI();
            }
        }
        else
        {
            Debug.Log("Unity:" + cmd + " -> GetData: " + getData.text);
            switch (cmd)
            {
                case PostCmd.ServerTimeGet:
                    {
                        //GetData: {"code":0,"message":"成功","data":"2018-09-28 12:58:56"}
                        string timeSystem = DateTime.Now.ToString("yyyy-MM-dd");
                        JsonData jd = JsonMapper.ToObject(getData.text);
                        if (Convert.ToInt32(jd["code"].ToString()) == (int)BoxLoginRt.Success)
                        {
                            string serverTime = jd["data"].ToString().Substring(0, 10);
                            //Debug.Log("Unity: serverTime == " + serverTime + ", systenTime == " + timeSystem);
                            if (serverTime != timeSystem)
                            {
                                //系统与服务器日期信息不一致,请修改机器系统日期信息!
                                if (SSUIRoot.GetInstance().m_GameUIManage != null)
                                {
                                    SSUIRoot.GetInstance().m_GameUIManage.CreatFixSystemTimeUI();
                                }
                            }
                        }

                        break;
                    }
                case PostCmd.WX_XCX_URL_POST:
                    {
                        /**
                         code : 响应码
                         message：响应状态说明
                         data：数据信息
                                   qrcodeUrl：获取微信小程序码的请求地址
                                   scene：传入的boxNumber
                                   page：小程序码对应的小程序入口
                         */
                        JsonData jd = JsonMapper.ToObject(getData.text);
                        //m_BoxLoginRt = (BoxLoginRt)Convert.ToInt32(jd["code"].ToString());
                        if (Convert.ToInt32(jd["code"].ToString()) == (int)BoxLoginRt.Success)
                        {
                            if (m_BoxLoginData != null)
                            {
                                string scene = jd["data"]["scene"].ToString();
                                string sceneTmp = m_BoxLoginData.boxNumber + "," + m_BoxLoginData.GetWXCodeGame(m_GamePadState);
                                Debug.Log("Unity: scene == " + scene + ", sceneTmp ==== " + sceneTmp);
                                if (sceneTmp == scene)
                                {
                                    //盒子编号和游戏代码信息一致.
                                    //重新刷新微信虚拟手柄二维码.
                                    string qrcodeUrl = jd["data"]["qrcodeUrl"].ToString();
                                    string page = jd["data"]["page"].ToString();
                                    Debug.Log("Unity: qrcodeUrl == " + qrcodeUrl + ", page == " + page);

                                    WeiXinXiaoChengXuData data = new WeiXinXiaoChengXuData();
                                    data.qrcodeUrl = qrcodeUrl;
                                    data.scene = scene;
                                    data.page = page;
                                    HttpRequestWeiXinXiaoChengXuErWeiMa(data);
                                }
                                else
                                {
                                    //盒子编号信息错误.
                                    Debug.LogWarning("Unity: scene was wrong! scene ==== " + scene + ", sceneTmp == " + sceneTmp);
                                }
                            }
                        }
                        else
                        {
                            Debug.Log("Unity:" + "Get  WX_XCX_URL failed! code == " + jd["code"]);
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
        Debug.Log("boxNumber == " + m_BoxLoginData.boxNumber
            + ", storeId == " + m_BoxLoginData.storeId
            + ", channel == " + m_BoxLoginData.channel
            + ", gameId == " + m_BoxLoginData.gameId);
        Debug.Log("url ==== " + m_BoxLoginData.url);
        StartCoroutine(SendPost(m_BoxLoginData.url, form, PostCmd.BoxLogin));
    }
    
    /// <summary>
    /// 发送微信小程序Url获取的消息.
    /// </summary>
    public void HttpSendGetWeiXinXiaoChengXuUrl()
    {
        Debug.Log("Unity:" + "HttpSendGetWeiXinXiaoChengXuUrl...");
        //GET方法.
        string url = m_BoxLoginData.GetWeiXinXiaoChengXuUrlPostInfo(m_GamePadState);
        Debug.Log("url ==== " + url);
        StartCoroutine(SendGet(url, PostCmd.WX_XCX_URL_POST));
    }


    /// <summary>
    /// 发送获取服务器系统时间的消息.
    /// 2.2.1 盒子获取服务器时间
    /// 地址：https://域名/wxbackstage/data/now
    /// 注：用来判断盒子时间是否正确
    /// </summary>
    public void HttpSendGetServerTimeInfo()
    {
        Debug.Log("Unity:" + "HttpSendGetServerTimeInfo...");
        //GET方法.
        string url = m_BoxLoginData.m_Address + "/wxbackstage/data/now";
        Debug.Log("url ==== " + url);
        StartCoroutine(SendGet(url, PostCmd.ServerTimeGet));
    }

    /// <summary>
    /// 微信小程序数据信息.
    /// </summary>
    public class WeiXinXiaoChengXuData
    {
        /// <summary>
        /// 获取微信小程序码的请求地址.
        /// </summary>
        public string qrcodeUrl = "";
        /// <summary>
        /// 盒子ID，游戏编号.
        /// </summary>
        public string scene = "";
        /// <summary>
        /// 小程序码对应的小程序入口.
        /// </summary>
        public string page = "";
    }

    public class postData
    {
        /// <summary>
        /// 盒子ID，游戏编号.
        /// </summary>
        public string scene = "";
        /// <summary>
        /// 小程序码对应的小程序入口.
        /// </summary>
        public string page = "";
    }

    /// <summary>
    /// 向微信请求游戏虚拟手柄小程序的二维码图片信息.
    /// </summary>
    void HttpRequestWeiXinXiaoChengXuErWeiMa(WeiXinXiaoChengXuData data)
    {
        string url = data.qrcodeUrl;
        Encoding encoding = Encoding.GetEncoding("utf-8");
        postData postdata = new postData();
        postdata.scene = data.scene;
        postdata.page = data.page;
        Debug.Log("HttpRequestWeiXinXiaoChengXuErWeiMa -> url   ==== " + url);
        Debug.Log("HttpRequestWeiXinXiaoChengXuErWeiMa -> scene ==== " + postdata.scene);
        Debug.Log("HttpRequestWeiXinXiaoChengXuErWeiMa -> page  ==== " + postdata.page);

        string str = JsonMapper.ToJson(postdata);
        byte[] postData = Encoding.UTF8.GetBytes(str);
        HttpWebResponse response = null;
        try
        {
            PostHttpResponse postHttpResponse = new PostHttpResponse();
            response = postHttpResponse.CreatePostHttpResponse(url, postData, encoding);
        }
        catch (Exception ex)
        {
            Debug.LogWarning("Unity: ex == " + ex);
            return;
        }

        //打印返回值.
        Stream stream = null; //获取响应的流.
        try
        {
            Debug.Log("Unity: response.ContentLength ==================== " + response.ContentLength);
            //以字符流的方式读取HTTP响应.
            stream = response.GetResponseStream();
            //System.Drawing.Image.FromStream(stream).Save(path);
            MemoryStream ms = null;
            byte[] buffer = new byte[response.ContentLength];
            int offset = 0, actuallyRead = 0;
            do
            {
                actuallyRead = stream.Read(buffer, offset, buffer.Length - offset);
                offset += actuallyRead;
            }
            while (actuallyRead > 0);

            ms = new MemoryStream(buffer);
            byte[] buffurPic = ms.ToArray();
            Debug.Log("Unity: buffurPic.length ==================== " + buffurPic.Length);

            string path = m_BoxLoginData.WX_XiaoChengXu_ErWeiMa_Path;
            Debug.Log("Unity: path ==== " + path);
            File.WriteAllBytes(path, buffurPic);
        }
        finally
        {
            //释放资源.
            if (stream != null)
            {
                stream.Close();
            }

            if (response != null)
            {
                response.Close();
            }
        }

        //更新微信小程序二维码.
        if (ErWeiMaUI.GetInstance() != null)
        {
            ErWeiMaUI.GetInstance().ReloadGameWXPadXiaoChengXuErWeiMa();
        }
    }

    public enum WeiXinShouBingEnum
    {
        /// <summary>
        /// H5手柄.
        /// </summary>
        H5 = 0,
        /// <summary>
        /// 微信小程序手柄.
        /// </summary>
        XiaoChengXu = 1,
    }
    /// <summary>
    /// 是否下载微信小程序二维码图片.
    /// </summary>
    bool IsReloadWeiXinXiaoChengXuErWeiMa = false;
    /// <summary>
    /// 推迟下载微信小程序二维码图片.
    /// </summary>
    public void DelayReloadWeiXinXiaoChengXuErWeiMa(UITexture erWeiMaUI)
    {
        if (IsReloadWeiXinXiaoChengXuErWeiMa == true)
        {
            return;
        }
        IsReloadWeiXinXiaoChengXuErWeiMa = true;
        StartCoroutine(ReloadWeiXinXiaoChengXuErWeiMa(erWeiMaUI));
    }

    /// <summary>
    /// 字节流转图片.
    /// 下载微信小程序二维码图片.
    /// </summary>
    IEnumerator ReloadWeiXinXiaoChengXuErWeiMa(UITexture erWeiMaUI)
    {
        yield return new WaitForSeconds(2f);
        int width = 430;
        int height = 430;
        string path = m_BoxLoginData.WX_XiaoChengXu_ErWeiMa_Path;
        if (File.Exists(path) == true && erWeiMaUI != null)
        {
            byte[] bytes = File.ReadAllBytes(path);//资源
            Texture2D texture = new Texture2D(width, height);
            texture.LoadImage(bytes);
            yield return new WaitForSeconds(0.01f);
            //Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            //img.sprite = sprite;
            erWeiMaUI.mainTexture = texture;
            //保存图片.
            pcvr.GetInstance().m_HongDDGamePadInterface.GetBarcodeCam().m_ErWeuMaImg = texture;
            erWeiMaUI.gameObject.SetActive(true);

            //删除网络故障,请检查网络并重启游戏UI.
            if (SSUIRoot.GetInstance().m_GameUIManage != null)
            {
                SSUIRoot.GetInstance().m_GameUIManage.RemoveWangLuoGuZhangUI();
            }
            yield return new WaitForSeconds(0.01f);
            Resources.UnloadUnusedAssets(); //一定要清理游离资源.
        }
        IsReloadWeiXinXiaoChengXuErWeiMa = false;
    }

    /// <summary>
    /// 微信小程序数据请求组件.
    /// </summary>
    public class PostHttpResponse
    {
        string DefaultUserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Widows NT 5.2; SV1; .NET CLR 1.1.4322; .NET CLR 2.0.50727)";
        bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            return true; //总是接受.
        }

        public HttpWebResponse CreatePostHttpResponse(string url, byte[] jsonDataPost, Encoding charset)
        {
            HttpWebRequest request = null;
            //HTTPSQ请求.
            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
            request = WebRequest.Create(url) as HttpWebRequest;
            request.ProtocolVersion = HttpVersion.Version10;
            request.Method = "POST";
            request.ContentType = "application/json";
            request.UserAgent = DefaultUserAgent;

            using (Stream stream = request.GetRequestStream())
            {
                stream.Write(jsonDataPost, 0, jsonDataPost.Length);
            }
            return request.GetResponse() as HttpWebResponse;
        }
    }
}