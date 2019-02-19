//#define USE_TEST_HDD_SERVER //使用红点点测试服务器.
//#define USE_OLD_GET_HDD_GAME_CONFIG //使用旧版本后台数据管理接口.
//#define TEST_DAI_JIN_QUAN //测试代金券.
using Assets.XKGame.Script.Comm;
using Assets.XKGame.Script.Server.WebSocket;
using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using UnityEngine;

public class SSBoxPostNet : MonoBehaviour
{
    public enum GamePadState
    {
        Default = 0,                //默认手柄.
        LeiTingZhanChe = 1,         //雷霆战车手柄.
    }
    /// <summary>
    /// 游戏手柄枚举,红点点游戏码.
    /// </summary>
    [HideInInspector]
    public GamePadState m_GamePadState = GamePadState.LeiTingZhanChe;

    public void Init()
    {
#if !USE_TEST_HDD_SERVER
        //设置红点点正式服务器版本信息.
        XKGameVersionCtrl.SetReleaseGameVersion();
#else
        //设置游戏为红点点测试服务器版本信息.
        XKGameVersionCtrl.SetTestGameVersion();
#endif
        string boxNum = "000000000000";
#if UNITY_STANDALONE_WIN

        //try
        //{
        //    NetworkInterface[] nis = NetworkInterface.GetAllNetworkInterfaces();
        //    foreach (NetworkInterface ni in nis)
        //    {
        //        string macTest = ni.GetPhysicalAddress().ToString();
        //        SSDebug.Log("macTest == " + macTest);
        //        string description = ni.Description;
        //        SSDebug.Log("description == " + description);
        //        string name = ni.Name;
        //        SSDebug.Log("name == " + name);
        //        string id = ni.Id;
        //        SSDebug.Log("id == " + id);
        //    }
        //}
        //catch (Exception ex)
        //{
        //    SSDebug.LogWarning("Mac get error! ex == " + ex);
        //}

        try
        {
            bool isFindLocalAreaConnection = false;
            NetworkInterface[] nis = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface ni in nis)
            {
                if (ni.Name == "本地连接" || ni.Name == "Local Area Connection")
                {
                    isFindLocalAreaConnection = true;
                    boxNum = ni.GetPhysicalAddress().ToString();
                    break;
                }
            }

            if (isFindLocalAreaConnection == false)
            {
                SSDebug.LogWarning("not find local area connection!");
            }
        }
        catch (Exception ex)
        {
            SSDebug.LogWarning("Mac get error! ex == " + ex);
        }
#endif

#if UNITY_ANDROID
        string ip = Network.player.ipAddress;
        ip = ip.Replace('.', (char)UnityEngine.Random.Range(97, 122));
        int indexStart = UnityEngine.Random.Range(0, 5);
        int strLen = ip.Length - indexStart;
        strLen = strLen > 6 ? 6 : strLen;
        indexStart = ip.Length - strLen;
        ip = ip.Substring(indexStart, strLen);
        
        string key = ip + (char)UnityEngine.Random.Range(97, 122)
            + (DateTime.Now.Ticks % 999999).ToString();
        boxNum = UnityEngine.Random.Range(10, 95) + m_GamePadState.ToString() + key;
#endif
        //boxNum = "111111111111"; //test
        boxNum = boxNum.Length > 12 ? boxNum.Substring(0, 12) : boxNum;
        m_BoxLoginData.boxNumber = boxNum;
        SSDebug.Log("boxNumber == " + m_BoxLoginData.boxNumber);

        if (m_WebSocketSimpet != null)
        {
            m_WebSocketSimpet.Init(this);
        }
        HttpSendPostLoginBox();
        //获取微信红点点游戏手柄小程序二维码.
        //HttpSendGetWeiXinXiaoChengXuUrl();
        //获取服务器的时间信息
        //HttpSendGetServerTimeInfo();

        //Debug.Log("Unity:"+"md5: " + Md5Sum("23456sswl"));
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
        /// <summary>
        /// 获取红点点平台玩家账户数据.
        /// </summary>
        GET_HDD_PLAYER_PAY_DATA = 3,
        /// <summary>
        /// 获取红点点游戏屏幕码Id.
        /// </summary>
        GET_HDD_GAME_SCREEN_ID = 4,
        /// <summary>
        /// 从红点点服务器获取游戏的配置信息.
        /// </summary>
        GET_GAME_CONFIG_FROM_HDD_SERVER = 5,
        /// <summary>
        /// 用户登录信息记录.
        /// </summary>
        POST_USER_LOGIN_INFO = 6,
        /// <summary>
        /// 向红点点服务器发送支付界面倒计时信息.
        /// </summary>
        GET_GAME_PAY_TIME = 6,
    }

    /// <summary>
    /// 盒子登陆返回枚举值.
    /// </summary>
    public enum BoxLoginRt
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
        public string _HAED_WX_XiaoChengXu_Url = "https://game.hdiandian.com";
        public string _WX_XiaoChengXu_Url_Post = "/wxbackstage/wechat/qrcode";
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
            WX_XiaoChengXu_Url_Post = _HAED_WX_XiaoChengXu_Url + _WX_XiaoChengXu_Url_Post + "/" + _boxNumber + "/" + WXCodeGame;
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
                //_boxNumber = value;
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

        /// <summary>
        /// 红点点后台游戏屏幕码.
        /// </summary>
        public string screenId = "0";

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
            _HAED_WX_XiaoChengXu_Url = address; //微信小程序地址修改.
        }
    }
#if USE_TEST_HDD_SERVER
    public BoxLoginData m_BoxLoginData = new BoxLoginData("http://game.hdiandian.com", "16"); //测试号.
#else
    public BoxLoginData m_BoxLoginData = new BoxLoginData("http://h5.hdiandian.com", "17"); //雷霆战车游戏正式号.
#endif

    public enum GamePayPlatform
    {
        Null = 0,
        /// <summary>
        /// 红点点支付平台.
        /// </summary>
        HongDianDian = 1,
    }
    /// <summary>
    /// 游戏支付平台.
    /// </summary>
    internal GamePayPlatform m_GamePayPlatform = GamePayPlatform.HongDianDian;

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
        //if (_url.Contains("https://game.hdiandian.com") == true)
        //{
        //    SSDebug.LogWarning("SendPost -> _url ======= " + _url); //test
        //}

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
            SSDebug.Log("PostError: " + postData.error + ", url == " + _url + ", cmd == " + cmd);
            //网络故障,请检查网络并重启游戏.
            if (SSUIRoot.GetInstance().m_GameUIManage != null)
            {
                SSUIRoot.GetInstance().m_GameUIManage.CreatWangLuoGuZhangUI( SSGameUICtrl.WangLuoGuZhang.Post );
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

                            //删除网络故障,请检查网络并重启游戏.
                            //if (SSUIRoot.GetInstance().m_GameUIManage != null)
                            //{
                            //    SSUIRoot.GetInstance().m_GameUIManage.RemoveWangLuoGuZhangUI();
                            //}

                            //获取游戏红点点后台屏幕码信息.
                            HttpSendGetGameScreenId();
                            //获取微信红点点游戏手柄小程序二维码.
                            HttpSendGetWeiXinXiaoChengXuUrl();
                            //获取服务器的时间信息
                            HttpSendGetServerTimeInfo();
                        }
                        else
                        {
                            Debug.Log("Unity:"+"Login box failed! code == " + jd["code"]);
                            //网络故障,请检查网络并重启游戏.
                            if (SSUIRoot.GetInstance().m_GameUIManage != null)
                            {
                                SSUIRoot.GetInstance().m_GameUIManage.CreatWangLuoGuZhangUI();
                            }
                        }
                        break;
                    }
            }
        }
    }

    /// <summary>
    /// 收到微信玩家在红点点平台的账户信息.
    /// </summary>
    public delegate void EventReceivedWXPlayerHddPayData(int userId, int money);
    public event EventReceivedWXPlayerHddPayData OnReceivedWXPlayerHddPayData;
    /// <summary>
    /// 收到微信玩家的红点点游戏账户数据.
    /// </summary>
    void ReceivedWXPlayerHddPayData(int userId, int money)
    {
        if (OnReceivedWXPlayerHddPayData != null)
        {
            OnReceivedWXPlayerHddPayData(userId, money);
        }
    }

    /// <summary>
    /// Get网络数据.
    /// </summary>
    IEnumerator SendGet(string _url, PostCmd cmd, int userIdVal = 0)
    {
        //if (_url.Contains("https://game.hdiandian.com") == true)
        //{
        //    SSDebug.LogWarning("SendGet -> _url ======= " + _url); //test
        //}
        WWW getData = new WWW(_url);
        yield return getData;
        if (getData.error != null)
        {
            SSDebug.LogWarning("Unity:" + "GetError: " + getData.error + ", cmd ============== " + cmd);
            //网络故障,请检查网络并重启游戏.
            //if (SSUIRoot.GetInstance().m_GameUIManage != null)
            //{
            //    SSUIRoot.GetInstance().m_GameUIManage.CreatWangLuoGuZhangUI();
            //}
        }
        else
        {
            Debug.Log("Unity:" + cmd + " -> GetData: " + getData.text);
            switch (cmd)
            {
                case PostCmd.GET_GAME_PAY_TIME:
                    {
                        JsonData jd = JsonMapper.ToObject(getData.text);
                        if (Convert.ToInt32(jd["code"].ToString()) == (int)BoxLoginRt.Success)
                        {
                            //发送支付倒计时信息成功.
                            SSDebug.Log("Send GAME_PAY_TIME to HddServer Success.");
                        }
                        else
                        {
                            //发送支付倒计时信息失败.
                            SSDebug.LogWarning("Send GAME_PAY_TIME to HddServer Failed!");
                        }
                        break;
                    }
                case PostCmd.GET_GAME_CONFIG_FROM_HDD_SERVER:
                    {
#if USE_OLD_GET_HDD_GAME_CONFIG
#else
                        //从红点点服务器获取游戏的配置信息.
                        //{"code":0,"message":"成功",
                        //"data":{"setting":
                        //{"id":4,"name":"西安盛世10155","screenCode":10155,"gameCode":1,"totalReturnRate":"30","superPrizeBusinessMoney":"3","mod":0,"barrage":"盛世欢迎您！","revenue":"15.0370","startTime":0,"endTime":7},
                        //"prizeList":[
                        //{"id":13,"settingId":4,"prizeName":"奖品1(超级JP大奖)","prizeMoney":"4","returnRate":"100","burstRate":"100","prizeMoneyPool":"73.016000","isLimitless":0,"increment":"0","decrement":"0","level":1},
                        //{"id":14,"settingId":4,"prizeName":"奖品2(标准大奖)","prizeMoney":"3","returnRate":"0","burstRate":"100","prizeMoneyPool":"8.006000","isLimitless":1,"increment":"0","decrement":"0","level":2},
                        //{"id":15,"settingId":4,"prizeName":"奖品3(基础小奖)","prizeMoney":"2","returnRate":"0","burstRate":"100","prizeMoneyPool":"8.004000","isLimitless":1,"increment":"0","decrement":"0","level":3},
                        //{"id":16,"settingId":4,"prizeName":"奖品4(附赠道具奖)","prizeMoney":"1","returnRate":"0","burstRate":"100","prizeMoneyPool":"0.002000","isLimitless":0,"increment":"0","decrement":"0","level":4}],
                        //"payItem":{"id":44,"money":1,"name":"充值激活游戏","description":"10155测试","gameCode":1,"screenCode":10155,"createTime":"2018-12-26 13:29:28"}}}

                        //code | int | 状态码
                        //message | string | 状态信息
                        //id | int | 绑定列表id
                        //totalReturnRate | int | 总返奖率，单位：%
                        //superRewardMoney | int | 超级JP大奖支付金额
                        //mod | string | 运营模式
                        //prizeName | string | 奖品名称
                        //money | string | 代金券金额
                        //returnRate | int | 返奖率，单位：%
                        //burstRate | int | 爆奖率，单位：%
                        //isLimit | string | 是否无限
                        //level | 奖品等级 | 1:一等奖，2:二等奖，依次类推
                        //prizePool | string | 奖池信息（如果没有绑定盒子，或者绑定的盒子没有收入，则没有奖池信息）
                        //totalIncome | string | 同一盒子，该款游戏的总收入，如果没有绑定盒子，则没有该信息
                        //barrage 弹幕信息.

                        JsonData jd = JsonMapper.ToObject(getData.text);
                        if (Convert.ToInt32(jd["code"].ToString()) == (int)BoxLoginRt.Success)
                        {
                            string jsonDataInfo = jd["data"].ToJson();
                            JsonData jd_Data = JsonMapper.ToObject(jsonDataInfo);
                            //SSDebug.Log("data ============ " + jsonDataInfo);
                            string totalReturnRate = jd_Data["setting"]["totalReturnRate"].ToString(); //总返奖率，单位：%
                            string superRewardMoney = jd_Data["setting"]["superPrizeBusinessMoney"].ToString(); //超级JP大奖商户支付金额
                            string mod = jd_Data["setting"]["mod"].ToString(); //运营模式
                            string barrage = jd_Data["setting"]["barrage"].ToString(); //弹幕信息
                            //SSDebug.Log("totalReturnRate ============ " + totalReturnRate);
                            //SSDebug.Log("superRewardMoney ============ " + superRewardMoney);
                            //SSDebug.Log("mod ============ " + mod);

                            string payMoney = jd_Data["payItem"]["money"].ToString();
                            //SSDebug.Log("payItems.money ============ " + payMoney);
                            
                            int startTime = 0;
                            int endTime = 7;
                            if (XKGlobalData.GetInstance() != null)
                            {
                                if (jd_Data["setting"].Keys.Contains("startTime") == true)
                                {
                                    startTime = Convert.ToInt32(jd_Data["setting"]["startTime"].ToString());
                                }
                                if (jd_Data["setting"].Keys.Contains("endTime") == true)
                                {
                                    endTime = Convert.ToInt32(jd_Data["setting"]["endTime"].ToString());
                                }
                                //更新代金券有效期限数据.
                                XKGlobalData.GetInstance().SetHddDaiJinQuanYouXiaoQiDt(startTime, endTime);

                                int freeTimeInterval = 20;
                                if (jd_Data["setting"].Keys.Contains("freeTimeInterval") == true)
                                {
                                    freeTimeInterval = Convert.ToInt32(jd_Data["setting"]["freeTimeInterval"].ToString());
                                }
                                //跟新游戏下次免费间隔时间数据.
                                XKGlobalData.GetInstance().SetTimeMianFeiNum(freeTimeInterval);
                            }

                            //"money":20,"returnRate":30,"burstRate":0,"isLimit":0,"prizePool":"0"
                            string jsonData_prizeDetailVoListInfo = jd["data"]["prizeList"].ToJson();
                            JsonData jd_Data_prizeDetailVoList = JsonMapper.ToObject(jsonData_prizeDetailVoListInfo);
                            SSServerConfigData gameConfigDt = new SSServerConfigData();
                            int prizeCount = jd_Data_prizeDetailVoList.Count;
                            //SSDebug.Log("prizeCount ============================== " + prizeCount);
                            string jpBossMoney = "";
                            string jpBossReturnRate = "";
                            string jpBossBurstRate = "";
                            string jpBossIsLimit = "";
                            string jpBossPrizePool = "";

                            string zhanCheMoney_01 = "";
                            string zhanCheReturnRate_01 = "";
                            string zhanCheBurstRate_01 = "";
                            string zhanCheIsLimit_01 = "";
                            string zhanChePrizePool_01 = "";

                            string zhanCheMoney_02 = "";
                            string zhanCheReturnRate_02 = "";
                            string zhanCheBurstRate_02 = "";
                            string zhanCheIsLimit_02 = "";
                            string zhanChePrizePool_02 = "";

                            string daoJuMoney = "";
                            string daoJuReturnRate = "";
                            string daoJuBurstRate = "";
                            string daoJuIsLimit = "";
                            string daoJuPrizePool = "";

                            string prizeLevel = ""; //奖品等级.
                            bool isFindJPBossPrizeInfo = false;
                            bool isFindZhanChe01PrizeInfo = false;
                            bool isFindZhanChe02PrizeInfo = false;
                            bool isFindSuiJiDaoJuPrizeInfo = false;
                            for (int i = 0; i < prizeCount; i++)
                            {
                                prizeLevel = jd_Data_prizeDetailVoList[i]["level"].ToString(); //奖品等级 1:一等奖，2:二等奖，依次类推
                                switch (prizeLevel)
                                {
                                    case "1":
                                        {
                                            if (XKGlobalData.GetInstance() != null)
                                            {
                                                //设置红点点奖品ID信息.
                                                XKGlobalData.GetInstance().SetHddJiangPinId(0, jd_Data_prizeDetailVoList[i]["id"].ToString());
                                                XKGlobalData.GetInstance().SetHddJiangChiIsLimit(0, jd_Data_prizeDetailVoList[i]["isLimitless"].ToString());
                                            }
                                            isFindJPBossPrizeInfo = true;
                                            jpBossMoney = jd_Data_prizeDetailVoList[i]["prizeMoney"].ToString(); //代金券金额
                                            jpBossReturnRate = jd_Data_prizeDetailVoList[i]["returnRate"].ToString(); //返奖率，单位：%
                                            jpBossBurstRate = jd_Data_prizeDetailVoList[i]["burstRate"].ToString(); //爆奖率，单位：%
                                            jpBossIsLimit = jd_Data_prizeDetailVoList[i]["isLimitless"].ToString(); //是否无限
                                            jpBossPrizePool = jd_Data_prizeDetailVoList[i]["prizeMoneyPool"].ToString(); //代金券奖池
                                            //SSDebug.Log("jpBossMoney ============ " + jpBossMoney);
                                            //SSDebug.Log("jpBossReturnRate ============ " + jpBossReturnRate);
                                            //SSDebug.Log("jpBossBurstRate ============ " + jpBossBurstRate);
                                            //SSDebug.Log("jpBossIsLimit ============ " + jpBossIsLimit);
                                            //SSDebug.Log("jpBossPrizePool ============ " + jpBossPrizePool);
                                            break;
                                        }
                                    case "2":
                                        {
                                            if (XKGlobalData.GetInstance() != null)
                                            {
                                                //设置红点点奖品ID信息.
                                                XKGlobalData.GetInstance().SetHddJiangPinId(1, jd_Data_prizeDetailVoList[i]["id"].ToString());
                                                XKGlobalData.GetInstance().SetHddJiangChiIsLimit(1, jd_Data_prizeDetailVoList[i]["isLimitless"].ToString());
                                            }
                                            isFindZhanChe01PrizeInfo = true;
                                            zhanCheMoney_01 = jd_Data_prizeDetailVoList[i]["prizeMoney"].ToString(); //代金券金额
                                            zhanCheReturnRate_01 = jd_Data_prizeDetailVoList[i]["returnRate"].ToString(); //返奖率，单位：%
                                            zhanCheBurstRate_01 = jd_Data_prizeDetailVoList[i]["burstRate"].ToString(); //爆奖率，单位：%
                                            zhanCheIsLimit_01 = jd_Data_prizeDetailVoList[i]["isLimitless"].ToString(); //是否无限
                                            zhanChePrizePool_01 = jd_Data_prizeDetailVoList[i]["prizeMoneyPool"].ToString(); //代金券奖池
                                            //SSDebug.Log("zhanCheMoney_01 ============ " + zhanCheMoney_01);
                                            //SSDebug.Log("zhanCheReturnRate_01 ============ " + zhanCheReturnRate_01);
                                            //SSDebug.Log("zhanCheBurstRate_01 ============ " + zhanCheBurstRate_01);
                                            //SSDebug.Log("zhanCheIsLimit_01 ============ " + zhanCheIsLimit_01);
                                            //SSDebug.Log("zhanChePrizePool_01 ============ " + zhanChePrizePool_01);
                                            break;
                                        }
                                    case "3":
                                        {
                                            if (XKGlobalData.GetInstance() != null)
                                            {
                                                //设置红点点奖品ID信息.
                                                XKGlobalData.GetInstance().SetHddJiangPinId(2, jd_Data_prizeDetailVoList[i]["id"].ToString());
                                                XKGlobalData.GetInstance().SetHddJiangChiIsLimit(2, jd_Data_prizeDetailVoList[i]["isLimitless"].ToString());
                                            }
                                            isFindZhanChe02PrizeInfo = true;
                                            zhanCheMoney_02 = jd_Data_prizeDetailVoList[i]["prizeMoney"].ToString(); //代金券金额
                                            zhanCheReturnRate_02 = jd_Data_prizeDetailVoList[i]["returnRate"].ToString(); //返奖率，单位：%
                                            zhanCheBurstRate_02 = jd_Data_prizeDetailVoList[i]["burstRate"].ToString(); //爆奖率，单位：%
                                            zhanCheIsLimit_02 = jd_Data_prizeDetailVoList[i]["isLimitless"].ToString(); //是否无限
                                            zhanChePrizePool_02 = jd_Data_prizeDetailVoList[i]["prizeMoneyPool"].ToString(); //代金券奖池
                                            //SSDebug.Log("zhanCheMoney_02 ============ " + zhanCheMoney_02);
                                            //SSDebug.Log("zhanCheReturnRate_02 ============ " + zhanCheReturnRate_02);
                                            //SSDebug.Log("zhanCheBurstRate_02 ============ " + zhanCheBurstRate_02);
                                            //SSDebug.Log("zhanCheIsLimit_02 ============ " + zhanCheIsLimit_02);
                                            //SSDebug.Log("zhanChePrizePool_02 ============ " + zhanChePrizePool_02);
                                            break;
                                        }
                                    case "4":
                                        {
                                            if (XKGlobalData.GetInstance() != null)
                                            {
                                                //设置红点点奖品ID信息.
                                                XKGlobalData.GetInstance().SetHddJiangPinId(3, jd_Data_prizeDetailVoList[i]["id"].ToString());
                                                XKGlobalData.GetInstance().SetHddJiangChiIsLimit(3, jd_Data_prizeDetailVoList[i]["isLimitless"].ToString());
                                            }
                                            isFindSuiJiDaoJuPrizeInfo = true;
                                            daoJuMoney = jd_Data_prizeDetailVoList[i]["prizeMoney"].ToString(); //代金券金额
                                            daoJuReturnRate = jd_Data_prizeDetailVoList[i]["returnRate"].ToString(); //返奖率，单位：%

                                            if (SSGameLogoData.m_GameDaiJinQuanMode == SSGameLogoData.GameDaiJinQuanMode.HDL_CaiPinQuan)
                                            {
                                                //海底捞菜品券版本可以掉落随机道具.
                                                daoJuBurstRate = jd_Data_prizeDetailVoList[i]["burstRate"].ToString(); //爆奖率，单位：%
                                            }
                                            else
                                            {
                                                //其它版本游戏暂时不允许掉落随机道具.
                                                daoJuBurstRate = "0";
                                            }
                                            daoJuIsLimit = jd_Data_prizeDetailVoList[i]["isLimitless"].ToString(); //是否无限
                                            daoJuPrizePool = jd_Data_prizeDetailVoList[i]["prizeMoneyPool"].ToString(); //代金券奖池
                                            //SSDebug.Log("daoJuMoney ============ " + daoJuMoney);
                                            //SSDebug.Log("daoJuReturnRate ============ " + daoJuReturnRate);
                                            //SSDebug.Log("daoJuBurstRate ============ " + daoJuBurstRate);
                                            //SSDebug.Log("daoJuIsLimit ============ " + daoJuIsLimit);
                                            //SSDebug.Log("daoJuPrizePool ============ " + daoJuPrizePool);
                                            break;
                                        }
                                }
                            }
                            
                            if (isFindJPBossPrizeInfo == true)
                            {
                                gameConfigDt.JPBossDaiJinQuan = Convert.ToInt32(jpBossMoney); //jpBoss代金券面额
                                gameConfigDt.JPBossChuPiaoLv = Convert.ToInt32(jpBossReturnRate); //jpBoss返奖率
                                gameConfigDt.JPBossBaoJiangLv = Convert.ToInt32(jpBossBurstRate); //jpBoss爆奖率
                                gameConfigDt.IsWuQiongDaJiangChiJPBossDaiJinQuan = jpBossIsLimit == "0" ? false : true; //jpBoss奖池是否无限
                                gameConfigDt.JPBossDeCai = MathConverter.StringToFloat(jpBossPrizePool); //jpBoss奖池
                            }

                            if (isFindZhanChe01PrizeInfo == true)
                            {
                                gameConfigDt.ZhanCheDaiJinQuan_01 = Convert.ToInt32(zhanCheMoney_01); //战车01代金券面额
                                gameConfigDt.ZhanCheChuPiaoLv_01 = Convert.ToInt32(zhanCheReturnRate_01); //战车01返奖率
                                gameConfigDt.ZhanCheBaoJiangLv_01 = Convert.ToInt32(zhanCheBurstRate_01); //战车01爆奖率
                                gameConfigDt.IsWuQiongDaJiangChiZhanCheDaiJinQuan_01 = zhanCheIsLimit_01 == "0" ? false : true; //战车01奖池是否无限
                                gameConfigDt.ZhanCheDeCai_01 = MathConverter.StringToFloat(zhanChePrizePool_01); //战车01奖池
                            }

                            if (isFindZhanChe02PrizeInfo == true)
                            {
                                gameConfigDt.ZhanCheDaiJinQuan_02 = Convert.ToInt32(zhanCheMoney_02); //战车02代金券面额
                                gameConfigDt.ZhanCheChuPiaoLv_02 = Convert.ToInt32(zhanCheReturnRate_02); //战车02返奖率
                                gameConfigDt.ZhanCheBaoJiangLv_02 = Convert.ToInt32(zhanCheBurstRate_02); //战车02爆奖率
                                gameConfigDt.IsWuQiongDaJiangChiZhanCheDaiJinQuan_02 = zhanCheIsLimit_02 == "0" ? false : true; //战车02奖池是否无限
                                gameConfigDt.ZhanCheDeCai_02 = MathConverter.StringToFloat(zhanChePrizePool_02); //战车02奖池
                            }

                            if (isFindSuiJiDaoJuPrizeInfo == true)
                            {
                                gameConfigDt.SuiJiDaoJuDaiJinQuan = Convert.ToInt32(daoJuMoney); //道具代金券面额
                                gameConfigDt.SuiJiDaoJuChuPiaoLv = Convert.ToInt32(daoJuReturnRate); //道具返奖率
                                gameConfigDt.SuiJiDaoJuBaoJiangLv = Convert.ToInt32(daoJuBurstRate); //道具爆奖率
                                gameConfigDt.IsWuQiongDaJiangChiSuiJiDaoJuDaiJinQuan = daoJuIsLimit == "0" ? false : true; //道具奖池是否无限
                                gameConfigDt.SuiJiDaoJuDeCai = MathConverter.StringToFloat(daoJuPrizePool); //道具奖池
                            }

                            gameConfigDt.GameCoinToMoney = Convert.ToInt32(payMoney); //付费金额信息
                            gameConfigDt.CaiChiFanJiangLv = Convert.ToInt32(totalReturnRate) / 100f; //总返奖率，单位：%

                            string danMuInfo = barrage;
                            string[] danMuInfoArray = danMuInfo.Split('#');
                            if (danMuInfoArray.Length > 0)
                            {
                                //SSDebug.Log("barrage ============ " + danMuInfoArray[0]);
                                gameConfigDt.GameDanMuInfo = danMuInfoArray[0]; //弹幕信息
                            }
                            gameConfigDt.MianFeiShiWanCount = mod == "0" ? 1 : 0; //运营模式(0 可以免费试玩一次， 其它为不允许免费试玩)
                            gameConfigDt.JPBossDaiJinQuanShangHuZhiFu = Convert.ToInt32(superRewardMoney);
                            gameConfigDt.UpdataAllServerConfigData();

                            //商户信息，在字符串中已#分割.
                            //1 -> 自定义弹幕
                            //2 -> 商户代金券弹幕1
                            //3 -> 商户代金券弹幕2
                            //4 -> 商户代金券弹幕3
                            //5 -> 商户代金券弹幕4
                            //6 -> 战车代金券商户名称1
                            //7 -> 战车代金券商户名称2
                            //8 -> 战车代金券商户名称3
                            //9 -> 战车代金券商户名称4
                            //10 -> JPBoss代金券商户名称1
                            //11 -> JPBoss代金券商户名称2
                            //12 -> JPBoss代金券商户名称3
                            //13 -> JPBoss代金券商户名称4
                            //14 -> 战车代金券使用详情1
                            //15 -> 战车代金券使用详情2
                            //16 -> 战车代金券使用详情3
                            //17 -> 战车代金券使用详情4
                            //18 -> JPBoss代金券使用详情1
                            //19 -> JPBoss代金券使用详情2
                            //20 -> JPBoss代金券使用详情3
                            //21 -> JPBoss代金券使用详情4
                            //22 -> 战车代金券名称1
                            //23 -> 战车代金券名称2
                            //24 -> 战车代金券名称3
                            //25 -> 战车代金券名称4
                            //26 -> JPBoss代金券名称1
                            //27 -> JPBoss代金券名称2
                            //28 -> JPBoss代金券名称3
                            //29 -> JPBoss代金券名称4
                            //30 -> 随机道具奖品4商户名称
                            //31 -> 随机道具奖品4代金券名称
                            //32 -> 随机道具奖品4代金券使用详情
                            if (XkGameCtrl.GetInstance().m_SSShangHuInfo != null)
                            {
                                string shangHuInfo = barrage;
                                string[] shangHuInfoArray = shangHuInfo.Split('#');
                                SSShangHuInfo shangHuInfoDt = XkGameCtrl.GetInstance().m_SSShangHuInfo;
                                if (shangHuInfoArray.Length >= 5)
                                {
                                    string[] infoArray = new string[4];
                                    for (int i = 1; i < 5; i++)
                                    {
                                        infoArray[i - 1] = shangHuInfoArray[i];
                                    }
                                    //更新游戏商户弹幕数据信息.
                                    shangHuInfoDt.UpdateShangHuDanMuInfo(infoArray);
                                }

                                if (shangHuInfoArray.Length >= 9)
                                {
                                    string[] infoArray = new string[4];
                                    for (int i = 5; i < 9; i++)
                                    {
                                        infoArray[i - 5] = shangHuInfoArray[i];
                                    }
                                    //更新游戏商户数据信息.
                                    shangHuInfoDt.UpdateShangHuInfo(infoArray);
                                }

                                if (shangHuInfoArray.Length >= 13)
                                {
                                    string[] infoArray = new string[4];
                                    for (int i = 9; i < 13; i++)
                                    {
                                        infoArray[i - 9] = shangHuInfoArray[i];
                                    }
                                    //更新游戏大奖Boss商户数据信息.
                                    shangHuInfoDt.UpdateDaJiangBossShangHuInfo(infoArray);
                                }

                                if (shangHuInfoArray.Length >= 17)
                                {
                                    string[] infoArray = new string[4];
                                    for (int i = 13; i < 17; i++)
                                    {
                                        infoArray[i - 13] = shangHuInfoArray[i];
                                    }
                                    //更新游戏商户战车代金券使用详情数据信息.
                                    shangHuInfoDt.UpdateShangHuDaiJinQuanXiangQing(infoArray);
                                }

                                if (shangHuInfoArray.Length >= 21)
                                {
                                    string[] infoArray = new string[4];
                                    for (int i = 17; i < 21; i++)
                                    {
                                        infoArray[i - 17] = shangHuInfoArray[i];
                                    }
                                    //更新游戏大奖Boss代金券使用详情数据信息.
                                    shangHuInfoDt.UpdateDaJiangBossDaiJinQuanXiangQing(infoArray);
                                }

                                if (shangHuInfoArray.Length >= 25)
                                {
                                    string[] infoArray = new string[4];
                                    for (int i = 21; i < 25; i++)
                                    {
                                        infoArray[i - 21] = shangHuInfoArray[i];
                                    }
                                    //更新游戏战车代金券名称数据信息.
                                    shangHuInfoDt.UpdateShangHuDaiJinQuanName(infoArray);
                                }

                                if (shangHuInfoArray.Length >= 29)
                                {
                                    string[] infoArray = new string[4];
                                    for (int i = 25; i < 29; i++)
                                    {
                                        infoArray[i - 25] = shangHuInfoArray[i];
                                    }
                                    //更新游戏JPBoss代金券名称数据信息.
                                    shangHuInfoDt.UpdateDaJiangBossDaiJinQuanName(infoArray);
                                }

                                if (shangHuInfoArray.Length >= 32)
                                {
                                    string[] infoArray = new string[3];
                                    for (int i = 29; i < 32; i++)
                                    {
                                        infoArray[i - 29] = shangHuInfoArray[i];
                                    }
                                    //更新游戏随机道具奖品4代金券数据信息.
                                    shangHuInfoDt.UpdateSuiJiDaoJuShangHuInfo(infoArray);
                                }
                            }
                        }
                        else
                        {
                            SSDebug.LogWarning("GET_GAME_CONFIG_FROM_HDD_SERVER -> get gameConfig info was failed!");
                        }
#endif
                        break;
                    }
                case PostCmd.GET_HDD_GAME_SCREEN_ID:
                    {
                        //红点点线下游戏屏幕码Id.
                        //{"code":0,"message":"成功","data":{"id":10004,"boxId":"89leitingzhanche68q1q6o30765"}}
                        JsonData jd = JsonMapper.ToObject(getData.text);
                        if (Convert.ToInt32(jd["code"].ToString()) == (int)BoxLoginRt.Success)
                        {
                            if (m_BoxLoginData != null)
                            {
                                m_BoxLoginData.screenId = jd["data"]["id"].ToString();
                                if (SSUIRoot.GetInstance().m_GameUIManage != null
                                    && SSUIRoot.GetInstance().m_GameUIManage.IsCreatGameScreenIdUI == false)
                                {
                                    //创建游戏红点点屏幕码UI.
                                    int screenId = Convert.ToInt32(m_BoxLoginData.screenId);
                                    SSUIRoot.GetInstance().m_GameUIManage.CreatGameScreenIdUI(screenId);
                                }

                                //获取游戏在红点点服务器的配置信息.
                                GetGameConfigInfoFromHddServer();
                            }
                        }
                        else
                        {
                            Debug.LogWarning("GET_HDD_GAME_SCREEN_ID -> get screenId was failed!");
                        }
                        break;
                    }
                case PostCmd.GET_HDD_PLAYER_PAY_DATA:
                    {
                        //玩家在红点点平台的账户信息.
                        //{"code":-1,"message":"NO ACCOUNT FOR THIS MEMBER"} //没有该账户.
                        //{"code":0,"message":"成功","data":{"account":1,"memberId":93124}} //成功获取账户信息.
                        JsonData jd = JsonMapper.ToObject(getData.text);
                        if (Convert.ToInt32(jd["code"].ToString()) == (int)BoxLoginRt.Success)
                        {
                            //如果有账户信息数据,需要将账户信息数据转换为游戏币.
                            int userId = Convert.ToInt32(jd["data"]["memberId"].ToString());
                            int money = Convert.ToInt32(jd["data"]["account"].ToString());
                            //money = 200; //test
                            ReceivedWXPlayerHddPayData(userId, money);
                        }
                        else
                        {
                            //没有账户信息.
                            //发送充值消息给微信手柄.
                            //if (m_WebSocketSimpet != null)
                            //{
                            //    m_WebSocketSimpet.NetSendWeiXinPadShowTopUpPanel(userIdVal);
                            //}
                            ReceivedWXPlayerHddPayData(userIdVal, 0);
                        }
                        break;
                    }
                case PostCmd.ServerTimeGet:
                    {
                        //GetData: {"code":0,"message":"成功","data":"2018-09-28 12:58:56"}
                        JsonData jd = JsonMapper.ToObject(getData.text);
                        if (Convert.ToInt32(jd["code"].ToString()) == (int)BoxLoginRt.Success)
                        {
                            //string timeSystem = DateTime.Now.ToString("yyyy-MM-dd");
                            //string serverTime = jd["data"].ToString().Substring(0, 10);
                            DateTime systemTimeDt = DateTime.Now;
                            DateTime serverTimeDt = Convert.ToDateTime(jd["data"].ToString());
                            //test
                            //systemTimeDt = Convert.ToDateTime("2018-10-01 15:58:56");
                            //serverTimeDt = Convert.ToDateTime("2018-09-30 12:58:56");
                            //test
                            TimeSpan sp = systemTimeDt.Subtract(serverTimeDt);
                            int daysOffset = sp.Days;
                            //if (pcvr.GetInstance() != null)
                            //{
                            //    pcvr.GetInstance().AddDebugMsg("Unity: daysOffset == " + Mathf.Abs(daysOffset));
                            //    pcvr.GetInstance().AddDebugMsg("Unity: serverTime == " + serverTime + ", systemTime == " + timeSystem);
                            //}

                            if (Mathf.Abs(daysOffset) > 1)
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
                                //获取游戏红点点后台屏幕码信息.
                                //HttpSendGetGameScreenId();

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
    /// 关闭WebSocket
    /// </summary>
    public void CloseWebSocket()
    {
        if (m_WebSocketSimpet != null)
        {
            m_WebSocketSimpet.CloseWebSocket();
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

        //获取微信红点点游戏手柄小程序二维码.
        //HttpSendGetWeiXinXiaoChengXuUrl();
    }

    /// <summary>
    /// 扣除微信玩家在红点点平台的消费数据.
    /// </summary>
    public class PostDataHddSubPlayerMoney
    {
        /// <summary>
        /// 用户ID.
        /// </summary>
        public int memberId = 0;
        /// <summary>
        /// 消费金额，单位分，1块钱=100分.
        /// </summary>
        public int account = 0;
        public PostDataHddSubPlayerMoney(int memberIdVal, int accountVal)
        {
            memberId = memberIdVal;
            account = accountVal;
        }
    }

    /// <summary>
    /// 扣费响应事件.
    /// </summary>
    public delegate void EventHandel(int userId, BoxLoginRt type);
    public static event EventHandel OnReceivedSendPostHddSubPlayerMoneyEvent;
    /// <summary>
    /// 收到扣费回传消息.
    /// </summary>
    public static  void OnReceivedSendPostHddSubPlayerMoney(int userId, BoxLoginRt type)
    {
        if (OnReceivedSendPostHddSubPlayerMoneyEvent != null)
        {
            OnReceivedSendPostHddSubPlayerMoneyEvent(userId, type);
        }
    }

    /// <summary>
    /// 消费用户余额 | 域名/wxbackstage/memberAccount/spend | POST
    /// memberId | Integer | 用户ID，等同于 UserID
    /// account | Integer | 消费金额，单位分，1块钱=100分
    /// </summary>
    public void HttpSendPostHddSubPlayerMoney(int userId, int account)
    {
        Debug.Log("Unity:" + "HttpSendPostHddSubPlayerMoney...");
        Debug.Log("Unity: memberId == " + userId + ", account == " + account);
        //消费用户余额的url.
        string url = m_BoxLoginData.m_Address + "/wxbackstage/memberAccount/spend";
        Debug.Log("Unity: url == " + url);
        
        PostDataHddSubPlayerMoney postDt = new PostDataHddSubPlayerMoney(userId, account);
        //"{\"memberId\":93124,\"account\":100}" //发送的消息.
        string jsonData = JsonMapper.ToJson(postDt);
        byte[] postData = Encoding.UTF8.GetBytes(jsonData);

        ThreadHttpSendPostHddSubPlayerMoney threadPostSubMoneyInfo = new ThreadHttpSendPostHddSubPlayerMoney(userId, this, url, postData);
        if (threadPostSubMoneyInfo != null)
        {
            Thread threadPost = new Thread(new ThreadStart(threadPostSubMoneyInfo.Run));
            threadPost.Start();
        }
    }
    
    /// <summary>
    /// 通过线程发送玩家的游戏扣费信息到服务器.
    /// </summary>
    public class ThreadHttpSendPostHddSubPlayerMoney
    {
        string m_Url = "";
        byte[] m_PostData = null;
        int userId = 0;
        public ThreadHttpSendPostHddSubPlayerMoney(int userId, SSBoxPostNet boxPostNet, string url, byte[] postData)
        {
            this.userId = userId;
            m_Url = url;
            m_PostData = postData;
        }

        ~ThreadHttpSendPostHddSubPlayerMoney()
        {
            //SSDebug.Log("~ThreadHttpSendPostHddSubPlayerMoney -> destory this thread*********************************");
        }

        internal void Run()
        {
            Encoding encoding = Encoding.GetEncoding("utf-8");
            HttpWebResponse response = PostHttpResponse.CreatePostHttpResponse(m_Url, m_PostData, encoding);
            //打印返回值.
            Stream stream = null; //获取响应的流.

            try
            {
                //以字符流的方式读取HTTP响应.
                stream = response.GetResponseStream();
                StreamReader sr = new StreamReader(stream); //创建一个stream读取流
                string msg = sr.ReadToEnd();   //从头读到尾，放到字符串html
                //{"code":-1,"message":"该用户没有充值信息，不能扣款！"}
                //{"code":0,"message":"成功","data":{"id":4,"openId":"oefFM5cqhWSws1BVxgzuLTLWKAnk","account":1,"memberId":93124}}
                SSDebug.Log("ThreadHttpSendPostHddSubPlayerMoney -> msg == " + msg);

                JsonData jd = JsonMapper.ToObject(msg);
                if (Convert.ToInt32(jd["code"].ToString()) == (int)BoxLoginRt.Success)
                {
                    //红点点支付平台扣费成功.
                    OnReceivedSendPostHddSubPlayerMoney(userId, BoxLoginRt.Success);
                }
                else
                {
                    //红点点支付平台扣费失败.
                    SSDebug.Log("ThreadHttpSendPostHddSubPlayerMoney -> HttpSendPostHddSubPlayerMoney failed! code == " + jd["code"]);
                    OnReceivedSendPostHddSubPlayerMoney(userId, BoxLoginRt.Failed);
                }
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
        }
    }

    /// <summary>
    /// 生成代金券的数据信息.
    /// </summary>
    public class PostDataPlayerCouponInfo
    {
        /// <summary>
        /// 现金券金额，单位：分.
        /// </summary>
        public int worth = 0;
        /// <summary>
        /// 该现金券绑定的盒子ID.
        /// </summary>
        public string boxId = "";
        /// <summary>
        /// 用户ID.
        /// </summary>
        public int userId = 0;
        /// <summary>
        /// 游戏码
        /// 四国坦克(tank) | 0
        /// 雷霆战车/装甲突击(gamepad) | 1
        /// 坦克争霸战（tank_battle） | 2
        /// 摇色子(deadly_dice) | 3
        /// 凯奇大冒险(KQ_adventure) | 4
        /// 勇者大冒险 | 5
        /// </summary>
        public int gameCode = 0;
        /// <summary>
        /// 屏幕码.
        /// </summary>
        public int screenCode = 0;
        /// <summary>
        /// 代金券名称.
        /// </summary>
        public string name = "恭喜获得抵扣代金券";
        /// <summary>
        /// 代金券使用详情.
        /// </summary>
        public string description = "此代金券只能在游戏合作商家内使用。";
        /// <summary>
        /// 是否是超级JP大奖，0：否；1：是；不发送此字段，默认是0 | false
        /// </summary>
        public int superPrize = 0;
        /// <summary>
        /// prizeId | Integer | 奖品ID(后台设置的) | true
        /// </summary>
        public int prizeId = 0;
        /// <summary>
        /// isLimit | Integer | 是否无限(后台设置)  | true
        /// </summary>
        public int isLimit = 0;
        /// <summary>
        /// 代金券开始时间.
        /// </summary>
        public int startTime = 0;
        /// <summary>
        /// 代金券结束期限时间.
        /// </summary>
        public int endTime = 7;
        public PostDataPlayerCouponInfo(int worthVal, string boxIdVal, int userIdVal)
        {
            worth = worthVal;
            boxId = boxIdVal;
            userId = userIdVal;
        }
        public PostDataPlayerCouponInfo(int worthVal, string boxIdVal, int userIdVal, int gameCodeVal, int screenCodeVal, string nameVal, int superPrizeVal)
        {
            worth = worthVal;
            boxId = boxIdVal;
            userId = userIdVal;
            gameCode = gameCodeVal;
            screenCode = screenCodeVal;
            name = nameVal;
            superPrize = superPrizeVal;
        }
        public PostDataPlayerCouponInfo(int worthVal, string boxIdVal, int userIdVal, int gameCodeVal, int screenCodeVal, string nameVal, int superPrizeVal, int prizeIdVal, int isLimitVal)
        {
            worth = worthVal;
            boxId = boxIdVal;
            userId = userIdVal;
            gameCode = gameCodeVal;
            screenCode = screenCodeVal;
            name = nameVal;
            superPrize = superPrizeVal;
            prizeId = prizeIdVal;
            isLimit = isLimitVal;
            SSDebug.Log("PostDataPlayerCouponInfo -> " + ToString());
        }
        public PostDataPlayerCouponInfo(int worthVal, string boxIdVal, int userIdVal, int gameCodeVal, int screenCodeVal,
            string nameVal, string descriptionVal, int superPrizeVal, int prizeIdVal, int isLimitVal,
            int startTimeVal, int endTimeVal)
        {
            worth = worthVal;
            boxId = boxIdVal;
            userId = userIdVal;
            gameCode = gameCodeVal;
            screenCode = screenCodeVal;
            name = nameVal;
            description = descriptionVal;
            superPrize = superPrizeVal;
            prizeId = prizeIdVal;
            isLimit = isLimitVal;
            startTime = startTimeVal;
            endTime = endTimeVal;
            SSDebug.Log("PostDataPlayerCouponInfo -> " + ToString());
        }
        public override string ToString()
        {
            return "worth == " + worth + ", boxId == " + boxId + ", userId == " + userId + ", gameCode == " + gameCode
                + ", screenCode == " + screenCode
                + ", name == " + name
                + ", description == " + description
                + ", superPrize == " + superPrize
                + ", prizeId == " + prizeId
                + ", isLimit == " + isLimit
                + ", startTime == " + startTime
                + ", endTime == " + endTime;
        }
    }

    /// <summary>
    /// 生成现金券 | 域名/wxbackstage/client/coupon/generate | POST
    /// worth | Integer | 现金券金额，单位：分 | true
    /// boxId | String | 该现金券绑定的盒子ID | true
    /// userId | Integer | 用户ID | true
    /// </summary>
    public void HttpSendPostHddPlayerCouponInfo(int userId, int account, string boxId, SSCaiPiaoDataManage.GameCaiPiaoData.DaiJinQuanState daiJinQuanType)
    {
        //account单位是人民币元.
        //worth单位是人民币分.
        int worth = account * 100; //单位从元转换为分.

        //int suiJiDaoJuDaiJinQuan = 10;
        //int zhanCheDaiJinQuan_01 = 5;
        //int zhanCheDaiJinQuan_02 = 20;
        //int jpBossDaiJinQuan = 200;
        //if (XkPlayerCtrl.GetInstanceFeiJi() != null
        //    && XkPlayerCtrl.GetInstanceFeiJi().m_SpawnNpcManage != null
        //    && XkPlayerCtrl.GetInstanceFeiJi().m_SpawnNpcManage.m_CaiPiaoDataManage != null
        //    && XkPlayerCtrl.GetInstanceFeiJi().m_SpawnNpcManage.m_CaiPiaoDataManage.m_GameCaiPiaoData != null)
        //{
            //suiJiDaoJuDaiJinQuan = (int)XkPlayerCtrl.GetInstanceFeiJi().m_SpawnNpcManage.m_CaiPiaoDataManage.m_GameCaiPiaoData.SuiJiDaoJuDaiJinQuan;
            //zhanCheDaiJinQuan_01 = (int)XkPlayerCtrl.GetInstanceFeiJi().m_SpawnNpcManage.m_CaiPiaoDataManage.m_GameCaiPiaoData.ZhanCheDaiJinQuan_01;
            //zhanCheDaiJinQuan_02 = (int)XkPlayerCtrl.GetInstanceFeiJi().m_SpawnNpcManage.m_CaiPiaoDataManage.m_GameCaiPiaoData.ZhanCheDaiJinQuan_02;
            //jpBossDaiJinQuan = (int)XkPlayerCtrl.GetInstanceFeiJi().m_SpawnNpcManage.m_CaiPiaoDataManage.m_GameCaiPiaoData.JPBossDaiJinQuan;
        //}
#if TEST_DAI_JIN_QUAN
        //测试代金券.
        if (account == suiJiDaoJuDaiJinQuan)
        {
            worth = 1; //1分钱.
        }
        else if (account == zhanCheDaiJinQuan_01)
        {
            worth = 2; //2分钱.
        }
        else if (account == zhanCheDaiJinQuan_02)
        {
            worth = 3; //3分钱.
        }
        else if (account == jpBossDaiJinQuan)
        {
            worth = 4; //4分钱.
        }
#endif
        Debug.Log("Unity:" + "HttpSendPostHddSubPlayerMoney...");
        Debug.Log("Unity: memberId == " + userId + ", worth == " + worth + "分, boxId == " + boxId);
        //生成现金券的url.
        string url = m_BoxLoginData.m_Address + "/wxbackstage/client/coupon/generate";
        Debug.Log("Unity: url == " + url);
        
        int gameCode = (int)m_GamePadState; //游戏码.
        int screenCode = 0;
        if (m_BoxLoginData != null)
        {
            screenCode = Convert.ToInt32(m_BoxLoginData.screenId); //屏幕码.
        }
        
        int superPrize = 0; //是否为JPBoss代金券.
        int indexJiangPinId = -1;
        SSGameLogoData.GameDaiJinQuanMode daiJinQuanMode = SSGameLogoData.m_GameDaiJinQuanMode;
        if (daiJinQuanMode == SSGameLogoData.GameDaiJinQuanMode.HDL_CaiPinQuan)
        {
            //海底捞代金券版本游戏.
            //代金券面值写为0.
            bool isWuXianJiangChi = false;
            if (XKGlobalData.GetInstance() != null)
            {
                isWuXianJiangChi = XKGlobalData.GetInstance().GetIsWuXianJiangChi(daiJinQuanType);
            }

            if (isWuXianJiangChi == true)
            {
                //海底捞版本奖池为无限大时，发送奖券面值信息为0.
                worth = 0;
            }
        }

        if (daiJinQuanType == SSCaiPiaoDataManage.GameCaiPiaoData.DaiJinQuanState.JPBossDaiJinQuan)
        {
            superPrize = 1; //JPBoss代金券.
            indexJiangPinId = 0;
        }
        else if (daiJinQuanType == SSCaiPiaoDataManage.GameCaiPiaoData.DaiJinQuanState.ZhanCheDaiJinQuan_01)
        {
            indexJiangPinId = 1;
        }
        else if (daiJinQuanType == SSCaiPiaoDataManage.GameCaiPiaoData.DaiJinQuanState.ZhanCheDaiJinQuan_02)
        {
            indexJiangPinId = 2;
        }
        else if (daiJinQuanType == SSCaiPiaoDataManage.GameCaiPiaoData.DaiJinQuanState.SuiJiDaoJuDaiJinQuan)
        {
            indexJiangPinId = 3;
        }

        string hddJiangPinId = "0";
        string isLimitStr = "1";
        if (XKGlobalData.GetInstance() != null)
        {
            //获取红点点奖品ID信息.
            hddJiangPinId = XKGlobalData.GetInstance().GetHddJiangPinId(indexJiangPinId);
            isLimitStr = XKGlobalData.GetInstance().GetHddJiangChiIsLimit(indexJiangPinId);
        }

        int prizeId = Convert.ToInt32(hddJiangPinId);
        int isLimit = Convert.ToInt32(isLimitStr);
        string daiJinQuanName = ""; //代金券名称.
        string xiangQingInfo = ""; //代金券使用提示.
        if (XkGameCtrl.GetInstance().m_SSShangHuInfo != null && XkGameCtrl.GetInstance().m_SSShangHuInfo.m_DaiJinQuanDt != null)
        {
            //这里获取游戏当前代金券的商户信息.
            if (daiJinQuanType == SSCaiPiaoDataManage.GameCaiPiaoData.DaiJinQuanState.SuiJiDaoJuDaiJinQuan)
            {
                //随机道具代金券.
                SSShangHuInfo.ShangHuData shangHuData = XkGameCtrl.GetInstance().m_SSShangHuInfo.GetSuiJiDaoJuShangHuInfo();
                daiJinQuanName = shangHuData.DaiJinQuanName;
                xiangQingInfo = shangHuData.XiangQingInfo;
            }
            else
            {
                daiJinQuanName = XkGameCtrl.GetInstance().m_SSShangHuInfo.m_DaiJinQuanDt.DaiJinQuanName;
                xiangQingInfo = XkGameCtrl.GetInstance().m_SSShangHuInfo.m_DaiJinQuanDt.XiangQingInfo;
            }
        }
        SSDebug.Log("HttpSendPostHddPlayerCouponInfo -> shangHuInfo == " + daiJinQuanName + ", xiangQingInfo == " + xiangQingInfo);

        int startTime = 0;
        int endTime = 7;
        if (XKGlobalData.GetInstance() != null)
        {
            //更新代金券有效期限数据.
            startTime = XKGlobalData.GetInstance().GetHddDaiJinQuanStartDay();
            endTime = XKGlobalData.GetInstance().GetHddDaiJinQuanQiXian();
        }

        PostDataPlayerCouponInfo postDt = new PostDataPlayerCouponInfo(worth, boxId, userId, gameCode, screenCode,
            daiJinQuanName, xiangQingInfo, superPrize, prizeId, isLimit, startTime, endTime);
        //"{\"worth\":100,\"boxId\":\"123456\",\"userId\":93124}" //发送的消息.
        string jsonData = JsonMapper.ToJson(postDt);
        byte[] postData = Encoding.UTF8.GetBytes(jsonData);

        ThreadHttpSendPostHddPlayerCouponInfo threadPostCouponInfo = new ThreadHttpSendPostHddPlayerCouponInfo(url, postData);
        if (threadPostCouponInfo != null)
        {
            Thread threadPost = new Thread(new ThreadStart(threadPostCouponInfo.Run));
            threadPost.Start();
        }

        if (m_WebSocketSimpet != null)
        {
            //玩家获得优惠券时发送该消息给服务器.
            m_WebSocketSimpet.NetSendWeiXinPadPlayerGetCoupon(userId);
        }
    }
    
    /// <summary>
    /// 通过线程发送玩家的奖券信息到服务器.
    /// </summary>
    public class ThreadHttpSendPostHddPlayerCouponInfo
    {
        string m_Url = "";
        byte[] m_PostData = null;
        public ThreadHttpSendPostHddPlayerCouponInfo(string url, byte[] postData)
        {
            m_Url = url;
            m_PostData = postData;
        }

        ~ThreadHttpSendPostHddPlayerCouponInfo()
        {
            //SSDebug.Log("~ThreadHttpSendPostHddPlayerCouponInfo -> destory this thread*********************************");
        }

        internal void Run()
        {
            Encoding encoding = Encoding.GetEncoding("utf-8");
            //PostDataPlayerCouponInfo postDt = new PostDataPlayerCouponInfo(worth, boxId, userId, gameCode, screenCode, name, superPrize);
            //"{\"worth\":100,\"boxId\":\"123456\",\"userId\":93124}" //发送的消息.
            //string jsonData = JsonMapper.ToJson(postDt);
            //byte[] postData = m_PostData;
            HttpWebResponse response = PostHttpResponse.CreatePostHttpResponse(m_Url, m_PostData, encoding);
            //打印返回值.
            Stream stream = null; //获取响应的流.

            try
            {
                //以字符流的方式读取HTTP响应.
                stream = response.GetResponseStream();
                StreamReader sr = new StreamReader(stream); //创建一个stream读取流
                string msg = sr.ReadToEnd();   //从头读到尾，放到字符串html
                SSDebug.Log("msg == " + msg);
                //{"code":0,"message":"成功","data":{"id":4,"couponId":"36ecce4e-0b5c-4808-8284-18c1fad8bc27",
                //"worth":100,"boxId":"408d5cbc1371leitingzhanche","createTime":"2018-10-27T03:41:43.025+0000",
                //"userId":93124}}

                JsonData jd = JsonMapper.ToObject(msg);
                if (Convert.ToInt32(jd["code"].ToString()) == (int)BoxLoginRt.Success)
                {
                    //红点点支付平台玩家代金券添加成功.
                }
                else
                {
                    //红点点支付平台玩家代金券添加失败.
                    SSDebug.LogWarning("HttpSendPostHddSubPlayerMoney failed! code == " + jd["code"]);
                }
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
        }
    }
    
    /// <summary>
    /// 游戏付费状态.
    /// </summary>
    public enum FuFeiState
    {
        /// <summary>
        /// 首次免费
        /// </summary>
        ShouCiMianFei = 0,
        /// <summary>
        /// 付费
        /// </summary>
        FuFei = 1,
        /// <summary>
        /// 免费再玩一局
        /// </summary>
        MianFeiZaiWanYiJu = 2,
    }

    public class PostDataPlayerLogin
    {
        /// <summary>
        /// 游戏对照码.
        /// </summary>
        public int gameCode = 0;
        /// <summary>
        /// 游戏屏幕码.
        /// </summary>
        public int screenCode = 0;
        /// <summary>
        /// 玩家登陆Id信息.
        /// </summary>
        public int memberId = 0;
        /// <summary>
        /// 用户昵称.
        /// </summary>
        public string memberName = "";
        /// <summary>
        /// 登录时是否付费.
        /// </summary>
        public string isFree = "";
        public PostDataPlayerLogin(int gameCode, int screenCode, int memberId)
        {
            this.gameCode = gameCode;
            this.screenCode = screenCode;
            this.memberId = memberId;
        }
        public PostDataPlayerLogin(int gameCode, int screenCode, int memberId, string memberName, FuFeiState fuFeiStata)
        {
            this.gameCode = gameCode;
            this.screenCode = screenCode;
            this.memberId = memberId;
            this.memberName = memberName;
            string fuFeiStr = "首次免费";
            switch (fuFeiStata)
            {
                case FuFeiState.ShouCiMianFei:
                    {
                        fuFeiStr = "首次免费";
                        break;
                    }
                case FuFeiState.FuFei:
                    {
                        fuFeiStr = "付费";
                        break;
                    }
                case FuFeiState.MianFeiZaiWanYiJu:
                    {
                        fuFeiStr = "免费再玩一局";
                        break;
                    }
            }
            this.isFree = fuFeiStr;
        }
    }

    /// <summary>
    /// 用户登录记录| 域名/wxbackstage/client/memberLogin | POST |
    /// gameCode | Integer | 游戏码 | 是
    /// screenCode | Integer | 屏幕码 | 是
    /// memberId | Integer | 会员ID | 是
    /// memberName | String | 用户昵称 | 是
    /// isFree | String | 登录时是否付费（取值：付费、免费；2选1） | 是
    /// </summary>
    internal void HttpSendPostUserLoginInfo(int userId, string userName, FuFeiState fuFeiStata)
    {
        if (m_BoxLoginData == null)
        {
            SSDebug.LogWarning("HttpSendPostUserLoginInfo -> m_BoxLoginData was null");
            return;
        }
        //游戏对照码.
        int gameCode = (int)m_GamePadState;
        int screenCode = Convert.ToInt32(m_BoxLoginData.screenId);
        int memberId = userId;
        //POST方法.
        string url = m_BoxLoginData.m_Address + "/wxbackstage/client/memberLogin";
        //http://game.hdiandian.com/wxbackstage/client/memberLogin
        SSDebug.Log("HttpSendPostUserLoginInfo -> url == " + url);
        
        PostDataPlayerLogin postDt = new PostDataPlayerLogin(gameCode, screenCode, memberId, userName, fuFeiStata);
        //"{\"gameCode\":1,\"screenCode\":10155,\"memberId\":94180}" //发送的消息.
        string jsonData = JsonMapper.ToJson(postDt);
        byte[] postData = Encoding.UTF8.GetBytes(jsonData);
        ThreadHttpSendPostHddPlayerLoginGameInfo threadPostPlayerLoginGameInfo = new ThreadHttpSendPostHddPlayerLoginGameInfo(userId, url, postData);
        if (threadPostPlayerLoginGameInfo != null)
        {
            Thread threadPost = new Thread(new ThreadStart(threadPostPlayerLoginGameInfo.Run));
            threadPost.Start();
        }
    }
    
    /// <summary>
    /// 通过线程发送玩家的登录游戏状态信息到服务器.
    /// </summary>
    public class ThreadHttpSendPostHddPlayerLoginGameInfo
    {
        int userId = 0;
        string m_Url = "";
        byte[] m_PostData = null;
        public ThreadHttpSendPostHddPlayerLoginGameInfo(int userId, string url, byte[] postData)
        {
            this.userId = userId;
            m_Url = url;
            m_PostData = postData;
        }

        ~ThreadHttpSendPostHddPlayerLoginGameInfo()
        {
            //SSDebug.Log("~ThreadHttpSendPostHddPlayerLoginGameInfo -> destory this thread*********************************");
        }

        internal void Run()
        {
            Encoding encoding = Encoding.GetEncoding("utf-8");
            HttpWebResponse response = PostHttpResponse.CreatePostHttpResponse(m_Url, m_PostData, encoding);
            //打印返回值.
            Stream stream = null; //获取响应的流.

            try
            {

                //以字符流的方式读取HTTP响应.
                stream = response.GetResponseStream();
                StreamReader sr = new StreamReader(stream); //创建一个stream读取流
                string msg = sr.ReadToEnd();   //从头读到尾，放到字符串html
                SSDebug.Log("ThreadHttpSendPostHddPlayerLoginGameInfo::run -> msg == " + msg);
                //{"code":0,"message":"成功",
                //"data":{"id":2032,"memberId":94180,"screenCode":10155,"gameCode":1,"memberName":"Allen","isFree":"首次免费","loginTime":"2019-02-18 13:31:01"}}

                JsonData jd = JsonMapper.ToObject(msg);
                if (Convert.ToInt32(jd["code"].ToString()) == (int)BoxLoginRt.Success)
                {
                    //红点点支付平台玩家的登录游戏状态信息发送成功.
                    int id = Convert.ToInt32(jd["data"]["id"].ToString());
                    AddPostUserLoginReceiveData(userId, id);
                }
                else
                {
                    //红点点支付平台玩家的登录游戏状态信息发送失败.
                    SSDebug.LogWarning("ThreadHttpSendPostHddPlayerLoginGameInfo failed! code == " + jd["code"]);
                }
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
        }
    }

    /// <summary>
    /// 玩家游戏登录消息返回的数据信息.
    /// </summary>
    public class PostUserLoginReceiveData
    {
        /// <summary>
        /// 用户id.
        /// </summary>
        internal int userId = 0;
        /// <summary>
        /// 游戏用户登录消息返回的id信息.
        /// </summary>
        internal int id = 0;
        public PostUserLoginReceiveData(int userId, int id)
        {
            this.userId = userId;
            this.id = id;
        }
    }
    static List<PostUserLoginReceiveData> m_PostUserLoginReceiveData = new List<PostUserLoginReceiveData>();
    static PostUserLoginReceiveData FindPostUserLoginReceiveData(int userId)
    {
        PostUserLoginReceiveData data = m_PostUserLoginReceiveData.Find((dt) => {
            return dt.userId.Equals(userId);
        });
        return data;
    }
    /// <summary>
    /// 添加玩家游戏登录消息返回的数据信息.
    /// </summary>
    static void AddPostUserLoginReceiveData(int userId, int id)
    {
        PostUserLoginReceiveData data = FindPostUserLoginReceiveData(userId);
        if (data == null)
        {
            m_PostUserLoginReceiveData.Add(new PostUserLoginReceiveData(userId, id));
        }
    }
    /// <summary>
    /// 删除玩家游戏登录消息返回的数据信息.
    /// </summary>
    static void RemovePostUserLoginReceiveData(int userId)
    {
        PostUserLoginReceiveData data = FindPostUserLoginReceiveData(userId);
        if (data != null)
        {
            m_PostUserLoginReceiveData.Remove(data);
        }
    }
    /// <summary>
    /// 玩家游戏时长数据信息.
    /// </summary>
    public class PostDataPlayerPlayGameTime
    {
        /// <summary>
        /// 玩家登录信息返回的id信息.
        /// </summary>
        public int id = 0;
        /// <summary>
        /// 游戏时长.
        /// </summary>
        public int gameTime = 0;
        public PostDataPlayerPlayGameTime(int id, int gameTime)
        {
            this.id = id;
            this.gameTime = gameTime;
        }
        public override string ToString()
        {
            return "id ==== " + id + ", gameTime ==== " + gameTime;
        }
    }

    /// <summary>
    /// 用户游戏时长| 域名/wxbackstage/client/memberGameTime | POST | 表5.9 | 
    /// id | Integer | 用户登录统计返回的id | 是
    /// gameTime | Integer | 游戏时长，单位（秒） | 是
    /// </summary>
    internal void HttpSendPostUserPlayGameTimeInfo(int userId, int time)
    {
        if (m_BoxLoginData == null)
        {
            SSDebug.LogWarning("HttpSendPostUserPlayGameTimeInfo -> m_BoxLoginData was null");
            return;
        }

        //POST方法.
        string url = m_BoxLoginData.m_Address + "/wxbackstage/client/memberGameTime";
        //http://game.hdiandian.com/wxbackstage/client/memberGameTime
        //SSDebug.Log("HttpSendPostUserPlayGameTimeInfo -> url == " + url);

        //Encoding encoding = Encoding.GetEncoding("utf-8");
        PostUserLoginReceiveData userLoginDt = FindPostUserLoginReceiveData(userId);
        if (userLoginDt == null)
        {
            SSDebug.LogWarning("HttpSendPostUserPlayGameTimeInfo -> not find userLoginData!");
            return;
        }
        //删除玩家登录的返回信息.
        RemovePostUserLoginReceiveData(userId);

        PostDataPlayerPlayGameTime postDt = new PostDataPlayerPlayGameTime(userLoginDt.id, time);
        //SSDebug.Log("HttpSendPostUserPlayGameTimeInfo -> postDt == " + postDt.ToString());
        //"{\"id\":2560,\"gameTime\":60}" //发送的消息.
        string jsonData = JsonMapper.ToJson(postDt);
        byte[] postData = Encoding.UTF8.GetBytes(jsonData);
        
        ThreadHttpSendPostHddPlayerGameTimeInfo threadPostGameTimeInfo = new ThreadHttpSendPostHddPlayerGameTimeInfo(url, postData);
        if (threadPostGameTimeInfo != null)
        {
            Thread threadPost = new Thread(new ThreadStart(threadPostGameTimeInfo.Run));
            threadPost.Start();
        }
    }
    
    /// <summary>
    /// 通过线程发送玩家的游戏时间信息到服务器.
    /// </summary>
    public class ThreadHttpSendPostHddPlayerGameTimeInfo
    {
        string m_Url = "";
        byte[] m_PostData = null;
        public ThreadHttpSendPostHddPlayerGameTimeInfo(string url, byte[] postData)
        {
            m_Url = url;
            m_PostData = postData;
        }

        ~ThreadHttpSendPostHddPlayerGameTimeInfo()
        {
            //SSDebug.Log("~ThreadHttpSendPostHddPlayerGameTimeInfo -> destory this thread*********************************");
        }

        internal void Run()
        {
            Encoding encoding = Encoding.GetEncoding("utf-8");
            HttpWebResponse response = PostHttpResponse.CreatePostHttpResponse(m_Url, m_PostData, encoding);
            //打印返回值.
            Stream stream = null; //获取响应的流.

            try
            {
                //以字符流的方式读取HTTP响应.
                stream = response.GetResponseStream();
                StreamReader sr = new StreamReader(stream); //创建一个stream读取流
                string msg = sr.ReadToEnd();   //从头读到尾，放到字符串html
                //SSDebug.Log("HttpSendPostUserPlayGameTimeInfo -> msg == " + msg);
                JsonData jd = JsonMapper.ToObject(msg);
                if (Convert.ToInt32(jd["code"].ToString()) == (int)BoxLoginRt.Success)
                {
                    //红点点支付平台玩家游戏时长消息发送成功.
                }
                else
                {
                    //红点点支付平台玩家游戏时长消息发送失败.
                    SSDebug.LogWarning("ThreadHttpSendPostHddPlayerGameTimeInfo failed! code == " + jd["code"]);
                }
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
        }
    }

    bool IsDelayReadWeiXinXiaoChengXuErWeiMa = false;
    /// <summary>
    /// 读取微信小程序二维码.
    /// </summary>
    void DelayReadWeiXinXiaoChengXuErWeiMa()
    {
        if (IsDelayReadWeiXinXiaoChengXuErWeiMa == false)
        {
            IsDelayReadWeiXinXiaoChengXuErWeiMa = true;
            StartCoroutine(StartReadWeiXinXiaoChengXuErWeiMa());
        }
    }

    IEnumerator StartReadWeiXinXiaoChengXuErWeiMa()
    {
        yield return new WaitForSeconds(3f);
        //读取微信小程序二维码.
        if (ErWeiMaUI.GetInstance() != null)
        {
            Debug.Log("Unity: Reading WeiXinXiaoChengXu ErWeiMa......");
            ErWeiMaUI.GetInstance().ReloadGameWXPadXiaoChengXuErWeiMa();
        }
        else
        {
            Debug.LogWarning("Unity: ============================= ErWeiMaUI was null");
        }
    }

    /// <summary>
    /// 发送微信小程序Url获取的消息.
    /// </summary>
    void HttpSendGetWeiXinXiaoChengXuUrl()
    {
        if (pcvr.GetInstance().m_HongDDGamePadInterface != null
            && pcvr.GetInstance().m_HongDDGamePadInterface.GetBarcodeCam() != null
            && pcvr.GetInstance().m_HongDDGamePadInterface.GetBarcodeCam().m_ErWeuMaImg != null)
        {
            //已经获取过微信小程序二维码.
            return;
        }
        
        string key = "HddGameBoxNum";
        //在电脑注册表里读取的游戏盒子编号信息.
        string boxNumRead = PlayerPrefs.GetString(key);
        //在配置文件里读取的游戏盒子编号信息.
        string boxNumConfigRead = XKGlobalData.GetInstance().m_HddBoxNumInfo;
        //Debug.Log("Unity: =============================== boxNumRead: " + boxNumRead
            //+ ", boxNumber: " + m_BoxLoginData.boxNumber);
        if (boxNumRead == m_BoxLoginData.boxNumber && boxNumRead == boxNumConfigRead)
        {
#if !UNITY_EDITOR
            //发布版游戏强制重新获取小程序二维码.
            SSDebug.Log("Reload WX_XiaoChengXu_ErWeiMa....................................");
#else
            string path = m_BoxLoginData.WX_XiaoChengXu_ErWeiMa_Path;
            //Debug.Log("Unity: path ============================= " + path);
            if (File.Exists(path) == true)
            {
                DelayReadWeiXinXiaoChengXuErWeiMa();
                return;
            }
#endif
        }
        else
        {
            Debug.Log("Setting HddGameBoxNumInfo......");
            PlayerPrefs.SetString(key, m_BoxLoginData.boxNumber);
            XKGlobalData.GetInstance().SetHddBoxNumInfo(m_BoxLoginData.boxNumber);
        }
        
        Debug.Log("Unity:" + "HttpSendGetWeiXinXiaoChengXuUrl...");
        //GET方法.
        string url = m_BoxLoginData.GetWeiXinXiaoChengXuUrlPostInfo(m_GamePadState);
        Debug.Log("Unity: HttpSendGetWeiXinXiaoChengXuUrl -> url ==== " + url);
        StartCoroutine(SendGet(url, PostCmd.WX_XCX_URL_POST));
    }
    
    /// <summary>
    /// 发送获取服务器系统时间的消息.
    /// 2.2.1 盒子获取服务器时间
    /// 地址：https://域名/wxbackstage/data/now
    /// 注：用来判断盒子时间是否正确
    /// </summary>
    void HttpSendGetServerTimeInfo()
    {
        Debug.Log("Unity:" + "HttpSendGetServerTimeInfo...");
        //GET方法.
        string url = m_BoxLoginData.m_Address + "/wxbackstage/data/now";
        Debug.Log("HttpSendGetServerTimeInfo -> url ==== " + url);
        StartCoroutine(SendGet(url, PostCmd.ServerTimeGet));
    }

    /// <summary>
    /// 获取用户账户余额 | 域名/wxbackstage/memberAccount/info/{userId} | GET.
    /// </summary>
    public void HttpSendGetPlayerPayData(int userId)
    {
        if (m_BoxLoginData == null)
        {
            Debug.LogWarning("HttpSendGetPlayerPayData -> m_BoxLoginData was null");
            return;
        }

        //GET方法.
        string url = m_BoxLoginData.m_Address + "/wxbackstage/memberAccount/info/" + userId.ToString();
        Debug.Log("HttpSendGetPlayerPayData -> url ==== " + url);
        StartCoroutine(SendGet(url, PostCmd.GET_HDD_PLAYER_PAY_DATA, userId));
    }

    /// <summary>
    /// 获取屏幕码 | 域名/wxbackstage/client/sceneCode/{boxNumber} | GET | boxNumber
    /// </summary>
    internal void HttpSendGetGameScreenId()
    {
        if (m_BoxLoginData == null)
        {
            Debug.LogWarning("HttpSendGetGameScreenId -> m_BoxLoginData was null");
            return;
        }
        
        if (SSUIRoot.GetInstance().m_GameUIManage != null
            && SSUIRoot.GetInstance().m_GameUIManage.IsCreatGameScreenIdUI == false)
        {
            //GET方法.
            string url = m_BoxLoginData.m_Address + "/wxbackstage/client/sceneCode/" + m_BoxLoginData.boxNumber;
            Debug.Log("HttpSendGetGameScreenId -> url ==== " + url);
            StartCoroutine(SendGet(url, PostCmd.GET_HDD_GAME_SCREEN_ID));
        }
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
        /// <summary>
        /// 二维码宽度.
        /// </summary>
        public int width = 280;
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
        Debug.Log("HttpRequestWeiXinXiaoChengXuErWeiMa -> width  ==== " + postdata.width);

        string str = JsonMapper.ToJson(postdata);
        byte[] postData = Encoding.UTF8.GetBytes(str);
        HttpWebResponse response = null;
        try
        {
            response = PostHttpResponse.CreatePostHttpResponse(url, postData, encoding);
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
        int width = 280;
        int height = width;
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
            SSDebug.Log("ReloadWeiXinXiaoChengXuErWeiMa.....................................");
            if (ErWeiMaUI.GetInstance() != null)
            {
                ErWeiMaUI.GetInstance().SetActive(true);
            }

            //删除网络故障,请检查网络并重启游戏UI.
            //if (SSUIRoot.GetInstance().m_GameUIManage != null)
            //{
            //    SSUIRoot.GetInstance().m_GameUIManage.RemoveWangLuoGuZhangUI();
            //}
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
        static string DefaultUserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Widows NT 5.2; SV1; .NET CLR 1.1.4322; .NET CLR 2.0.50727)";
        static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            return true; //总是接受.
        }

        public static HttpWebResponse CreatePostHttpResponse(string url, byte[] jsonDataPost, Encoding charset)
        {
            HttpWebRequest request = null;
            //HTTPSQ请求.
            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
            request = WebRequest.Create(url) as HttpWebRequest;
            request.ProtocolVersion = HttpVersion.Version10;
            request.Method = "POST";
            request.ContentType = "application/json;charset=UTF-8";
            request.UserAgent = DefaultUserAgent;

            using (Stream stream = request.GetRequestStream())
            {
                stream.Write(jsonDataPost, 0, jsonDataPost.Length);
                stream.Close();
            }
            return request.GetResponse() as HttpWebResponse;
        }
    }

#region 获取游戏在红点点服务器端的配置信息
    public class GameConfigUrlData
    {
        string m_HeadUrl = "https://game.hdiandian.com";
        /// <summary>
        /// 获取游戏配置信息的地址.
        /// </summary>
#if USE_OLD_GET_HDD_GAME_CONFIG
        public string m_Url = "/xcx_backstage/thunderTank/getPrizeList?"; //旧版本.
#else
        public string m_Url = "/xcx_backstage/game_back/get/setting_info?"; //新版本.
#endif
        public int LeiTingZhanCheGameCode = 1;
        public GameConfigUrlData(string headUrl)
        {
            m_HeadUrl = headUrl;
        }

        public string GetUrl(GamePadState padState, int screenCode)
        {
            int gameCode = 0;
            switch(padState)
            {
                case GamePadState.LeiTingZhanChe:
                    {
                        gameCode = LeiTingZhanCheGameCode;
                        break;
                    }
            }
            string urlStr = m_HeadUrl + m_Url + "screenCode=" + screenCode + "&gameCode=" + gameCode;
            return urlStr;
        }
    }

    /// <summary>
    /// 获取游戏在红点点服务器端的配置信息.
    /// screenCode 屏幕码.
    /// gameCode 游戏码.
    /// </summary>
    void GetGameConfigInfoFromHddServer()
    {
//#if !USE_TEST_HDD_SERVER
//        return; //test.
//#endif
        if (m_BoxLoginData == null)
        {
            SSDebug.LogWarning("GetGameConfigInfoFromHddServer -> m_BoxLoginData was null");
            return;
        }

        if (m_BoxLoginData != null)
        {
            GameConfigUrlData configUrl = new GameConfigUrlData(m_BoxLoginData.m_Address);
            int screenId = Convert.ToInt32(m_BoxLoginData.screenId);
            string url = configUrl.GetUrl(m_GamePadState, screenId);
            SSDebug.Log("GetGameConfigInfoFromHddServer -> url ==== " + url);
            StartCoroutine(SendGet(url, PostCmd.GET_GAME_CONFIG_FROM_HDD_SERVER));
        }
    }

    /// <summary>
    /// 发送游戏支付倒计时信息给红点点服务器.
    /// gameCode | Integer | 游戏码 | 是
    /// userId | Integer | 用户ID | 是
    /// countDown | Integer | 倒计时时间（秒） | 是
    /// 支付倒计时 | 域名/backstage/client/client_pay_count_down | GET | 表5.8 | 
    /// </summary>
    internal void SendGamePayTimeInfoToHddServer(int userId, int countDown)
    {
        if (m_BoxLoginData == null)
        {
            SSDebug.LogWarning("SendGamePayTimeInfoToHddServer -> m_BoxLoginData was null");
            return;
        }

        if (m_BoxLoginData != null)
        {
            int gameCode = (int)m_GamePadState;
            string url = m_BoxLoginData.m_Address + "/wxbackstage/client/client_pay_count_down"
                + "?gameCode=" + gameCode + "&userId=" + userId + "&countDown=" + countDown;
            SSDebug.Log("SendGamePayTimeInfoToHddServer -> url ==== " + url);
            StartCoroutine(SendGet(url, PostCmd.GET_GAME_PAY_TIME));
        }
    }

    float m_TimeLastGetGameConfigData = 0f;
    /// <summary>
    /// 间隔一定时间请求一次配置数据.
    /// </summary>
    internal void LoopGetGameConfigInfoFromHddServer()
    {
        if (Time.time - m_TimeLastGetGameConfigData < 60f * 3f)
        {
            return;
        }
        m_TimeLastGetGameConfigData = Time.time;
        GetGameConfigInfoFromHddServer();
    }
#endregion
}