#define TEST_DAI_JIN_QUAN
using Assets.XKGame.Script.Server.WebSocket;
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
        string boxNum = m_GamePadState.ToString();
#if UNITY_STANDALONE_WIN
        try
        {
            /*NetworkInterface[] nis = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface ni in nis)
            {
                //Debug.Log("Name = " + ni.Name);
                //Debug.Log("Des = " + ni.Description);
                //Debug.Log("Type = " + ni.NetworkInterfaceType.ToString());
                Debug.Log("Unity: Mac = " + ni.GetPhysicalAddress().ToString());
                //Debug.Log("------------------------------------------------");
                //boxNum = UnityEngine.Random.Range(0, 9) + ni.GetPhysicalAddress().ToString() + m_GamePadState.ToString();
                boxNum = ni.GetPhysicalAddress().ToString() + m_GamePadState.ToString();
                break;
            }*/
            
            boxNum = SystemInfo.deviceUniqueIdentifier + m_GamePadState.ToString();
            if (boxNum.Length > 28)
            {
                boxNum = boxNum.Substring((boxNum.Length - 28), 28);
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
        boxNum = boxNum.Length > 28 ? boxNum.Substring(0, 28) : boxNum;
        m_BoxLoginData.boxNumber = boxNum;
        Debug.Log("boxNumber == " + m_BoxLoginData.boxNumber);

        if (m_WebSocketSimpet != null)
        {
            m_WebSocketSimpet.Init(this);
        }
        HttpSendPostLoginBox();
        //HttpSendGetWeiXinXiaoChengXuUrl();
        HttpSendGetServerTimeInfo();

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
        }
    }
    public BoxLoginData m_BoxLoginData = new BoxLoginData("http://game.hdiandian.com", "16"); //测试号.
    //public BoxLoginData m_BoxLoginData = new BoxLoginData("http://h5.hdiandian.com", "17"); //雷霆战车游戏正式号.
    
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
        WWW getData = new WWW(_url);
        yield return getData;
        if (getData.error != null)
        {
            Debug.Log("Unity:" + "GetError: " + getData.error);
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
                case PostCmd.GET_GAME_CONFIG_FROM_HDD_SERVER:
                    {
                        //从红点点服务器获取游戏的配置信息.
                        //{ "code":0,"message":"成功",
                        //"data":{ "commonDetailVo":{ "totalReturnRate":50,"superRewardMoney":150,"mod":0,"barrage":"代金券送不停"},
                        //"prizeDetailVoList":[
                        //{"id":7,"prizeName":"奖品1(超级JP大奖)","money":200,"returnRate":20,"burstRate":30,"isLimit":0,"prizePool":"0","totalIncome":"0"},
                        //{"id":8,"prizeName":"奖品2(标准大奖)","money":20,"returnRate":30,"burstRate":0,"isLimit":0,"prizePool":"0","totalIncome":"0"},
                        //{"id":9,"prizeName":"奖品3(基础小奖)","money":5,"returnRate":50,"burstRate":0,"isLimit":0,"prizePool":"0","totalIncome":"0"},
                        //{"id":10,"prizeName":"奖品4(赠送道具奖)","money":10,"returnRate":0,"burstRate":0,"isLimit":0,"prizePool":"0","totalIncome":"0"}],
                        //"payItems":[{"id":1,"money":1,"name":"雷霆战车测试1","description":"","gameCode":1,"createTime":"2018-12-13 17:37:32"}]}}

                        //缺少内容：
                        //1.自定义弹幕
                        //2.付费金额

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
                        //prizePool | string | 奖池信息（如果没有绑定盒子，或者绑定的盒子没有收入，则没有奖池信息）
                        //totalIncome | string | 同一盒子，该款游戏的总收入，如果没有绑定盒子，则没有该信息
                        //barrage 弹幕信息.

                        JsonData jd = JsonMapper.ToObject(getData.text);
                        if (Convert.ToInt32(jd["code"].ToString()) == (int)BoxLoginRt.Success)
                        {
                            string jsonDataInfo = jd["data"].ToJson();
                            JsonData jd_Data = JsonMapper.ToObject(jsonDataInfo);
                            SSDebug.Log("data ============ " + jsonDataInfo);
                            string totalReturnRate = jd_Data["commonDetailVo"]["totalReturnRate"].ToString(); //总返奖率，单位：%
                            string superRewardMoney = jd_Data["commonDetailVo"]["superRewardMoney"].ToString(); //超级JP大奖支付金额
                            string mod = jd_Data["commonDetailVo"]["mod"].ToString(); //运营模式
                            string barrage = jd_Data["commonDetailVo"]["barrage"].ToString(); //弹幕信息
                            //string money = jd_Data["payItems"]["money"].ToString(); //付费金额信息
                            SSDebug.Log("totalReturnRate ============ " + totalReturnRate);
                            SSDebug.Log("superRewardMoney ============ " + superRewardMoney);
                            SSDebug.Log("mod ============ " + mod);
                            SSDebug.Log("barrage ============ " + barrage);
                            //SSDebug.Log("money ============ " + money);
                            
                            string jsonData_payItemsInfo = jd["data"]["payItems"].ToJson();
                            JsonData jd_Data_payItems = JsonMapper.ToObject(jsonData_payItemsInfo);
                            string payMoney = jd_Data_payItems[0]["money"].ToString(); //付费金额信息
                            SSDebug.Log("payItems.money ============ " + payMoney);


                            //"money":20,"returnRate":30,"burstRate":0,"isLimit":0,"prizePool":"0"
                            string jsonData_prizeDetailVoListInfo = jd["data"]["prizeDetailVoList"].ToJson();
                            JsonData jd_Data_prizeDetailVoList = JsonMapper.ToObject(jsonData_prizeDetailVoListInfo);
                            string jpBossMoney = jd_Data_prizeDetailVoList[0]["money"].ToString(); //代金券金额
                            string jpBossReturnRate = jd_Data_prizeDetailVoList[0]["returnRate"].ToString(); //返奖率，单位：%
                             string jpBossBurstRate = jd_Data_prizeDetailVoList[0]["burstRate"].ToString(); //爆奖率，单位：%
                            string jpBossIsLimit = jd_Data_prizeDetailVoList[0]["isLimit"].ToString(); //是否无限
                            string jpBossPrizePool = jd_Data_prizeDetailVoList[0]["prizePool"].ToString(); //代金券奖池
                            SSDebug.Log("jpBossMoney ============ " + jpBossMoney);
                            SSDebug.Log("jpBossReturnRate ============ " + jpBossReturnRate);
                            SSDebug.Log("jpBossBurstRate ============ " + jpBossBurstRate);
                            SSDebug.Log("jpBossIsLimit ============ " + jpBossIsLimit);
                            SSDebug.Log("jpBossPrizePool ============ " + jpBossPrizePool);
                            
                            string zhanCheMoney_01 = jd_Data_prizeDetailVoList[1]["money"].ToString(); //代金券金额
                            string zhanCheReturnRate_01 = jd_Data_prizeDetailVoList[1]["returnRate"].ToString(); //返奖率，单位：%
                            string zhanCheBurstRate_01 = jd_Data_prizeDetailVoList[1]["burstRate"].ToString(); //爆奖率，单位：%
                            string zhanCheIsLimit_01 = jd_Data_prizeDetailVoList[1]["isLimit"].ToString(); //是否无限
                            string zhanChePrizePool_01 = jd_Data_prizeDetailVoList[1]["prizePool"].ToString(); //代金券奖池
                            SSDebug.Log("zhanCheMoney_01 ============ " + zhanCheMoney_01);
                            SSDebug.Log("zhanCheReturnRate_01 ============ " + zhanCheReturnRate_01);
                            SSDebug.Log("zhanCheBurstRate_01 ============ " + zhanCheBurstRate_01);
                            SSDebug.Log("zhanCheIsLimit_01 ============ " + zhanCheIsLimit_01);
                            SSDebug.Log("zhanChePrizePool_01 ============ " + zhanChePrizePool_01);
                            
                            string zhanCheMoney_02 = jd_Data_prizeDetailVoList[2]["money"].ToString(); //代金券金额
                            string zhanCheReturnRate_02 = jd_Data_prizeDetailVoList[2]["returnRate"].ToString(); //返奖率，单位：%
                            string zhanCheBurstRate_02 = jd_Data_prizeDetailVoList[2]["burstRate"].ToString(); //爆奖率，单位：%
                            string zhanCheIsLimit_02 = jd_Data_prizeDetailVoList[2]["isLimit"].ToString(); //是否无限
                            string zhanChePrizePool_02 = jd_Data_prizeDetailVoList[2]["prizePool"].ToString(); //代金券奖池
                            SSDebug.Log("zhanCheMoney_02 ============ " + zhanCheMoney_02);
                            SSDebug.Log("zhanCheReturnRate_02 ============ " + zhanCheReturnRate_02);
                            SSDebug.Log("zhanCheBurstRate_02 ============ " + zhanCheBurstRate_02);
                            SSDebug.Log("zhanCheIsLimit_02 ============ " + zhanCheIsLimit_02);
                            SSDebug.Log("zhanChePrizePool_02 ============ " + zhanChePrizePool_02);

                            string daoJuMoney = jd_Data_prizeDetailVoList[3]["money"].ToString(); //代金券金额
                            string daoJuReturnRate = jd_Data_prizeDetailVoList[3]["returnRate"].ToString(); //返奖率，单位：%
                            string daoJuBurstRate = jd_Data_prizeDetailVoList[3]["burstRate"].ToString(); //爆奖率，单位：%
                            string daoJuIsLimit = jd_Data_prizeDetailVoList[3]["isLimit"].ToString(); //是否无限
                            string daoJuPrizePool = jd_Data_prizeDetailVoList[3]["prizePool"].ToString(); //代金券奖池
                            SSDebug.Log("daoJuMoney ============ " + daoJuMoney);
                            SSDebug.Log("daoJuReturnRate ============ " + daoJuReturnRate);
                            SSDebug.Log("daoJuBurstRate ============ " + daoJuBurstRate);
                            SSDebug.Log("daoJuIsLimit ============ " + daoJuIsLimit);
                            SSDebug.Log("daoJuPrizePool ============ " + daoJuPrizePool);

                            SSServerConfigData gameConfigDt = new SSServerConfigData();
                            gameConfigDt.GameCoinToMoney = Convert.ToInt32(payMoney); //付费金额信息
                            gameConfigDt.CaiChiFanJiangLv = Convert.ToInt32(totalReturnRate) / 100f; //总返奖率，单位：%
                            gameConfigDt.GameDanMuInfo = barrage; //弹幕信息
                            gameConfigDt.MianFeiShiWanCount = mod == "0" ? 0 : 1; //运营模式(0 可以免费试玩一次， 其它为不允许免费试玩)
                            gameConfigDt.JPBossDaiJinQuanShangHuZhiFu = Convert.ToInt32(superRewardMoney);
                            
                            gameConfigDt.JPBossDaiJinQuan = Convert.ToInt32(jpBossMoney); //jpBoss代金券面额
                            gameConfigDt.ZhanCheDaiJinQuan_01 = Convert.ToInt32(zhanCheMoney_01); //战车01代金券面额
                            gameConfigDt.ZhanCheDaiJinQuan_02 = Convert.ToInt32(zhanCheMoney_02); //战车02代金券面额
                            gameConfigDt.SuiJiDaoJuDaiJinQuan = Convert.ToInt32(daoJuMoney); //道具代金券面额

                            gameConfigDt.JPBossChuPiaoLv = Convert.ToInt32(jpBossReturnRate); //jpBoss返奖率
                            gameConfigDt.ZhanCheChuPiaoLv_01 = Convert.ToInt32(zhanCheReturnRate_01); //战车01返奖率
                            gameConfigDt.ZhanCheChuPiaoLv_02 = Convert.ToInt32(zhanCheReturnRate_02); //战车02返奖率
                            gameConfigDt.SuiJiDaoJuChuPiaoLv = Convert.ToInt32(daoJuReturnRate); //道具返奖率

                            gameConfigDt.JPBossBaoJiangLv = Convert.ToInt32(jpBossBurstRate); //jpBoss爆奖率
                            gameConfigDt.ZhanCheBaoJiangLv_01 = Convert.ToInt32(zhanCheBurstRate_01); //战车01爆奖率
                            gameConfigDt.ZhanCheBaoJiangLv_02 = Convert.ToInt32(zhanCheBurstRate_02); //战车02爆奖率
                            gameConfigDt.SuiJiDaoJuBaoJiangLv = Convert.ToInt32(daoJuBurstRate); //道具爆奖率

                            gameConfigDt.IsWuQiongDaJiangChiJPBossDaiJinQuan = jpBossIsLimit == "0" ? false : true; //jpBoss奖池是否无限
                            gameConfigDt.IsWuQiongDaJiangChiZhanCheDaiJinQuan_01 = zhanCheIsLimit_01 == "0" ? false : true; //战车01奖池是否无限
                            gameConfigDt.IsWuQiongDaJiangChiZhanCheDaiJinQuan_02 = zhanCheIsLimit_02 == "0" ? false : true; //战车02奖池是否无限
                            gameConfigDt.IsWuQiongDaJiangChiSuiJiDaoJuDaiJinQuan = daoJuIsLimit == "0" ? false : true; //道具奖池是否无限

                            gameConfigDt.JPBossDeCai = Convert.ToInt32(jpBossPrizePool); //jpBoss奖池
                            gameConfigDt.ZhanCheDeCai_01 = Convert.ToInt32(zhanChePrizePool_01); //战车01奖池
                            gameConfigDt.ZhanCheDeCai_02 = Convert.ToInt32(zhanChePrizePool_02); //战车02奖池
                            gameConfigDt.SuiJiDaoJuDeCai = Convert.ToInt32(daoJuPrizePool); //道具奖池
                            gameConfigDt.UpdataAllServerConfigData();
                        }
                        else
                        {
                            SSDebug.LogWarning("GET_GAME_CONFIG_FROM_HDD_SERVER -> get gameConfig info was failed!");
                        }
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
                            string timeSystem = DateTime.Now.ToString("yyyy-MM-dd");
                            string serverTime = jd["data"].ToString().Substring(0, 10);
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

        //获取微信红点点游戏手柄小程序二位码.
        HttpSendGetWeiXinXiaoChengXuUrl();
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

        Encoding encoding = Encoding.GetEncoding("utf-8");
        PostDataHddSubPlayerMoney postDt = new PostDataHddSubPlayerMoney(userId, account);
        //"{\"memberId\":93124,\"account\":100}" //发送的消息.
        string jsonData = JsonMapper.ToJson(postDt);
        byte[] postData = Encoding.UTF8.GetBytes(jsonData);
        HttpWebResponse response = PostHttpResponse.CreatePostHttpResponse(url, postData, encoding);
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
            Debug.Log("unity: msg == " + msg);
            
            JsonData jd = JsonMapper.ToObject(msg);
            if (Convert.ToInt32(jd["code"].ToString()) == (int)BoxLoginRt.Success)
            {
                //红点点支付平台扣费成功.
            }
            else
            {
                //红点点支付平台扣费失败.
                Debug.Log("Unity:" + "HttpSendPostHddSubPlayerMoney failed! code == " + jd["code"]);
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

    /// <summary>
    /// 
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
        public PostDataPlayerCouponInfo(int worthVal, string boxIdVal, int userIdVal)
        {
            worth = worthVal;
            boxId = boxIdVal;
            userId = userIdVal;
        }
    }

    /// <summary>
    /// 生成现金券 | 域名/wxbackstage/client/coupon/generate | POST
    /// worth | Integer | 现金券金额，单位：分 | true
    /// boxId | String | 该现金券绑定的盒子ID | true
    /// userId | Integer | 用户ID | true
    /// </summary>
    public void HttpSendPostHddPlayerCouponInfo(int userId, int account, string boxId)
    {
        //account单位是人民币元.
        //worth单位是人民币分.
        int worth = account * 100; //单位从元转换为分.
#if TEST_DAI_JIN_QUAN
        //测试代金券.
        int suiJiDaoJuDaiJinQuan = 10;
        int zhanCheDaiJinQuan_01 = 5;
        int zhanCheDaiJinQuan_02 = 20;
        int jpBossDaiJinQuan = 200;
        if (XkPlayerCtrl.GetInstanceFeiJi() != null
            && XkPlayerCtrl.GetInstanceFeiJi().m_SpawnNpcManage != null
            && XkPlayerCtrl.GetInstanceFeiJi().m_SpawnNpcManage.m_CaiPiaoDataManage != null
            && XkPlayerCtrl.GetInstanceFeiJi().m_SpawnNpcManage.m_CaiPiaoDataManage.m_GameCaiPiaoData != null)
        {
            suiJiDaoJuDaiJinQuan = (int)XkPlayerCtrl.GetInstanceFeiJi().m_SpawnNpcManage.m_CaiPiaoDataManage.m_GameCaiPiaoData.SuiJiDaoJuDaiJinQuan;
            zhanCheDaiJinQuan_01 = (int)XkPlayerCtrl.GetInstanceFeiJi().m_SpawnNpcManage.m_CaiPiaoDataManage.m_GameCaiPiaoData.ZhanCheDaiJinQuan_01;
            zhanCheDaiJinQuan_02 = (int)XkPlayerCtrl.GetInstanceFeiJi().m_SpawnNpcManage.m_CaiPiaoDataManage.m_GameCaiPiaoData.ZhanCheDaiJinQuan_02;
            jpBossDaiJinQuan = (int)XkPlayerCtrl.GetInstanceFeiJi().m_SpawnNpcManage.m_CaiPiaoDataManage.m_GameCaiPiaoData.JPBossDaiJinQuan;
        }

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

        //switch (account)
        //{
        //    case 1:
        //        {
        //            worth = 1; //1分钱.
        //            break;
        //        }
        //    case 20:
        //        {
        //            worth = 2; //2分钱.
        //            break;
        //        }
        //    case 50:
        //        {
        //            worth = 3; //3分钱.
        //            break;
        //        }
        //    case 100:
        //        {
        //            worth = 4; //4分钱.
        //            break;
        //        }
        //}
        //worth = 100;
#endif
        Debug.Log("Unity:" + "HttpSendPostHddSubPlayerMoney...");
        Debug.Log("Unity: memberId == " + userId + ", worth == " + worth + "分, boxId == " + boxId);
        //生成现金券的url.
        string url = m_BoxLoginData.m_Address + "/wxbackstage/client/coupon/generate";
        Debug.Log("Unity: url == " + url);
        
        Encoding encoding = Encoding.GetEncoding("utf-8");
        PostDataPlayerCouponInfo postDt = new PostDataPlayerCouponInfo(worth, boxId, userId);
        //"{\"worth\":100,\"boxId\":\"123456\",\"userId\":93124}" //发送的消息.
        string jsonData = JsonMapper.ToJson(postDt);
        byte[] postData = Encoding.UTF8.GetBytes(jsonData);
        HttpWebResponse response = PostHttpResponse.CreatePostHttpResponse(url, postData, encoding);
        //打印返回值.
        Stream stream = null; //获取响应的流.

        try
        {
            //以字符流的方式读取HTTP响应.
            stream = response.GetResponseStream();
            StreamReader sr = new StreamReader(stream); //创建一个stream读取流
            string msg = sr.ReadToEnd();   //从头读到尾，放到字符串html
            Debug.Log("unity: msg == " + msg);
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
                Debug.Log("Unity:" + "HttpSendPostHddSubPlayerMoney failed! code == " + jd["code"]);
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
            string path = m_BoxLoginData.WX_XiaoChengXu_ErWeiMa_Path;
            //Debug.Log("Unity: path ============================= " + path);
            if (File.Exists(path) == true)
            {
                DelayReadWeiXinXiaoChengXuErWeiMa();
                return;
            }
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
        Debug.Log("Unity: url ==== " + url);
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
    void HttpSendGetGameScreenId()
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
            erWeiMaUI.gameObject.SetActive(true);

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
        /// <summary>
        /// 获取游戏配置信息的地址.
        /// </summary>
        public string m_Url = "https://game.hdiandian.com/xcx_backstage/thunderTank/getPrizeList?";
        public int LeiTingZhanCheGameCode = 1;
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
            string urlStr = m_Url + "screenCode=" + screenCode + "&gameCode=" + gameCode;
            return urlStr;
        }
    }
    /// <summary>
    /// 获取游戏在红点点服务器端的配置信息.
    /// https://game.hdiandian.com/xcx_backstage/thunderTank/getPrizeList?screenCode=xx&gameCode=xx
    /// screenCode 屏幕码.
    /// gameCode 游戏码.
    /// </summary>
    void GetGameConfigInfoFromHddServer()
    {
        if (m_BoxLoginData == null)
        {
            SSDebug.LogWarning("GetGameConfigInfoFromHddServer -> m_BoxLoginData was null");
            return;
        }

        GameConfigUrlData configUrl = new GameConfigUrlData();
        int screenId = 0;
        if (m_BoxLoginData != null)
        {
            screenId = Convert.ToInt32(m_BoxLoginData.screenId);
        }
        string url = configUrl.GetUrl(m_GamePadState, screenId);
        SSDebug.Log("GetGameConfigInfoFromHddServer -> url ==== " + url);
        StartCoroutine(SendGet(url, PostCmd.GET_GAME_CONFIG_FROM_HDD_SERVER));
    }
    #endregion
}