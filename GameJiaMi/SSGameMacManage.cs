using UnityEngine;
using System.Net.NetworkInformation;
using System;
using System.Security.Cryptography;
using System.IO;
using System.Text;
using System.Xml;

public class SSGameMacManage : MonoBehaviour
{
	// Use this for initialization
	internal bool Init(int[] iv, int[] key, GameDate gmDt)
    {
        m_GameDate = gmDt;
        if (gmDt != null)
        {
            GameKeyFile = gmDt.keyFile;
            GameValueFile = gmDt.valueFile;
        }

        MD5_iv = new byte[iv.Length];
        for (int i = 0; i < iv.Length; i++)
        {
            MD5_iv[i] = (byte)iv[i];
        }

        MD5_key = new byte[key.Length];
        for (int i = 0; i < key.Length; i++)
        {
            MD5_key[i] = (byte)key[i];
        }

        string defaultPcMac = "000000000000";
        string boxNum = defaultPcMac;
#if UNITY_STANDALONE_WIN
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
                //SSDebug.LogWarning("not find local area connection!");
            }
        }
        catch (Exception ex)
        {
            SSDebug.LogWarning("error::ex == " + ex);
        }
#endif

        //是否校验失败.
        bool isJiaoYanFailed = true;
        //是否刷新秘钥文件.
        bool isRefreshKeyValue = true;
        if (boxNum != defaultPcMac && GameKeyFile != "" && GameValueFile != "")
        {
            boxNum = boxNum.ToLower();
            //开始进行电脑Mac地址信息校验.
            string keyValue = ReadFromFileXml(GameKeyFile, "value"); //GameKey.db文件中的数据.
            string jiaoYanValue = ReadFromFileXml(GameValueFile, "value"); //GameValue.db文件中的数据.
            //if (keyValue == "" || jiaoYanValue == "")
            if (keyValue == "")
            {
                //获取的加密校验数据有问题.
            }
            else
            {
                //最终进行数据校验判断时，将GameValue.db的value数据用md5解密之后
                //和GameKey.db文件中的value数据进行比较，如果相等则校验通过，否则提示
                //“游戏校验失败，请将“GameKey.db”。
                string jieMiKeyValue = Decrypt(keyValue);
                string[] keyValArray = jieMiKeyValue.Split('#');
                if (keyValArray.Length >= 4)
                {
                    if (keyValArray[0] == boxNum)
                    {
                        //解密后的mac信息和本电脑的mac信息一致.
                        //可以继续进行加密数据校验.
                        string jieMiValue = "";
                        if (jiaoYanValue == "")
                        {
                            //校验数据错误.
                        }
                        else
                        {
                            //解析校验数据.
                            jieMiValue = Decrypt(jiaoYanValue);
                        }

                        if (jieMiValue == keyValue)
                        {
                            //加密校验数据符合,通过校验.
                            isJiaoYanFailed = false;
                        }
                        else
                        {
                            //秘钥信息中的Mac地址和该电脑的Mac信息一致,不用进行秘钥信息刷新.
                            isRefreshKeyValue = false;
                        }
                    }
                    else
                    {
                        //解密后的mac信息和本电脑的mac信息不一致.
                        //不用继续进行数据校验了.
                    }
                }
            }

            if (isJiaoYanFailed == true && isRefreshKeyValue == true)
            {
                //刷新GameKey.db文件中的数据.
                string gameName = "LeiTingZhanChe_" + SSGameLogoData.m_GameVersionState + "_" + XKGlobalData.m_GameVersionHddServer;
                string gameVersion = XKGameVersionCtrl.GameVersion;
                string randStr = "";
                for (int i = 0; i < 4096; i++)
                {
                    randStr += (UnityEngine.Random.Range(0, 100) % 10).ToString();
                }
                keyValue = boxNum + "#" + DateTime.Now.ToString() + "#" + gameName + "#" + gameVersion + "#" + randStr;
                keyValue = Encrypt(keyValue); //对keyValue进行MD5数据加密.
                WriteToFileXml(GameKeyFile, "value", keyValue);
            }
        }
        else
        {
            IsNotFindLocalMac = true;
        }

        //IsNotFindLocalMac = true; //test
        if (IsNotFindLocalMac == true)
        {
            SSBoxPostDebug boxPostDebug = gameObject.AddComponent<SSBoxPostDebug>();
            if (boxPostDebug != null)
            {
                //没有获取到电脑的Mac地址.
                boxPostDebug.SetIsNotFindLocalMac();
            }
        }
        IsMd5JiaoYanFailed = isJiaoYanFailed;

        if (isJiaoYanFailed == true)
        {
            m_GuiStyle = new GUIStyle();
            m_GuiStyle.fontSize = 50;
        }
        return isJiaoYanFailed;
    }

    /// <summary>
    /// 没有找到本地连接.
    /// 需要将电脑主板网卡名称修改为"本地连接".
    /// </summary>
    bool IsNotFindLocalMac = false;

    /// <summary>
    /// 是否Md5数据校验失败.
    /// </summary>
    bool IsMd5JiaoYanFailed = false;
    GUIStyle m_GuiStyle = null;
    private void OnGUI()
    {
        if (IsNotFindLocalMac == true)
        {
            return;
        }

        if (IsMd5JiaoYanFailed == false)
        {
            return;
        }

        if (m_GuiStyle == null)
        {
            return;
        }

        if (m_GameDate == null)
        {
            return;
        }

        GUI.Box(new Rect(0f, 0f, Screen.width, Screen.height), "");
        GUI.Label(new Rect(30f, 50f, Screen.width, m_GuiStyle.fontSize), m_GameDate.msg1, m_GuiStyle);
        GUI.Label(new Rect(30f, 50f + m_GuiStyle.fontSize, Screen.width, m_GuiStyle.fontSize), m_GameDate.msg2, m_GuiStyle);
        //GUI.Label(new Rect(30f, 50f, Screen.width, m_GuiStyle.fontSize),
        //    "游戏校验失败!", m_GuiStyle);
        //GUI.Label(new Rect(30f, 50f + m_GuiStyle.fontSize, Screen.width, m_GuiStyle.fontSize),
        //    "请将游戏路径中的\"GameKey.db\"文件发送给游戏提供商.", m_GuiStyle);
    }

    [Serializable]
    public class GameDate
    {
        /// <summary>
        /// 游戏校验文件.
        /// </summary>
        public string keyFile = "";
        /// <summary>
        /// 游戏秘钥文件.
        /// </summary>
        public string valueFile = "";
        /// <summary>
        /// 游戏校验失败提示信息1.
        /// </summary>
        public string msg1 = "";
        /// <summary>
        /// 游戏校验失败提示信息2.
        /// </summary>
        public string msg2 = "";
    }
    /// <summary>
    /// 游戏校验数据信息.
    /// </summary>
    GameDate m_GameDate;

    #region 读写数据功能
    /// <summary>
    /// 游戏校验文件.
    /// </summary>
    //string GameKeyFile = "../GameKey.db";
    string GameKeyFile = "";
    /// <summary>
    /// 游戏秘钥文件.
    /// </summary>
    //string GameValueFile = "../GameValue.db";
    string GameValueFile = "";
    public string ReadFromFileXml(string fileName, string attribute)
    {
        string filepath = Application.dataPath + "/" + fileName;
#if UNITY_ANDROID
		//filepath = Application.persistentDataPath + "//" + fileName;
#endif
        string valueStr = "";

        if (File.Exists(filepath))
        {
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(filepath);
                XmlNodeList nodeList = xmlDoc.SelectSingleNode("gameConfig").ChildNodes;
                foreach (XmlElement xe in nodeList)
                {
                    valueStr = xe.GetAttribute(attribute);
                }
                File.SetAttributes(filepath, FileAttributes.Normal);
                xmlDoc.Save(filepath);
            }
            catch (Exception exception)
            {
                File.SetAttributes(filepath, FileAttributes.Normal);
                File.Delete(filepath);
                SSDebug.LogError("error: xml was wrong! " + exception);
            }
        }
        return valueStr;
    }

    public void WriteToFileXml(string fileName, string attribute, string valueStr)
    {
        string filepath = Application.dataPath + "/" + fileName;
#if UNITY_ANDROID
		filepath = Application.persistentDataPath + "//" + fileName;
#endif

        //create file
        if (!File.Exists(filepath))
        {
            XmlDocument xmlDoc = new XmlDocument();
            XmlElement root = xmlDoc.CreateElement("gameConfig");
            XmlElement elmNew = xmlDoc.CreateElement("config");

            root.AppendChild(elmNew);
            xmlDoc.AppendChild(root);
            xmlDoc.Save(filepath);
            File.SetAttributes(filepath, FileAttributes.Normal);
        }

        //update value
        if (File.Exists(filepath))
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(filepath);
            XmlNodeList nodeList = xmlDoc.SelectSingleNode("gameConfig").ChildNodes;

            foreach (XmlElement xe in nodeList)
            {
                xe.SetAttribute(attribute, valueStr);
            }
            File.SetAttributes(filepath, FileAttributes.Normal);
            xmlDoc.Save(filepath);
        }
    }
    #endregion
    
    #region MD5秘钥
    //建立加密对象的密钥和偏移量
    //byte[] iv = { 102, 66, 93, 156, 78, 56, 253, 36 };//定义偏移量
    //byte[] key = { 55, 36, 226, 128, 36, 99, 89, 39 };//定义密钥
    byte[] MD5_iv;//定义偏移量
    byte[] MD5_key;//定义密钥
    #endregion

    #region MD5加密
    /// <summary>   
    /// MD5加密   
    /// </summary>   
    /// <param name="strSource">需要加密的字符串</param>   
    /// <returns>MD5加密后的字符串</returns>   
    string Encrypt(string strSource)
    {
        //把字符串放到byte数组中   
        byte[] bytIn = System.Text.Encoding.Default.GetBytes(strSource);
        //实例DES加密类   
        DESCryptoServiceProvider mobjCryptoService = new DESCryptoServiceProvider();
        mobjCryptoService.Key = MD5_iv;
        mobjCryptoService.IV = MD5_key;
        ICryptoTransform encrypto = mobjCryptoService.CreateEncryptor();
        //实例MemoryStream流加密密文件   
        System.IO.MemoryStream ms = new System.IO.MemoryStream();
        CryptoStream cs = new CryptoStream(ms, encrypto, CryptoStreamMode.Write);
        cs.Write(bytIn, 0, bytIn.Length);
        cs.FlushFinalBlock();
        return System.Convert.ToBase64String(ms.ToArray());
    }
    #endregion

    #region MD5解密
    /// <summary>   
    /// MD5解密   
    /// </summary>   
    /// <param name="Source">需要解密的字符串</param>   
    /// <returns>MD5解密后的字符串</returns>   
    string Decrypt(string Source)
    {
        string val = "0";
        try
        {
            //将解密字符串转换成字节数组   
            byte[] bytIn = System.Convert.FromBase64String(Source);
            //给出解密的密钥和偏移量，密钥和偏移量必须与加密时的密钥和偏移量相同   
            DESCryptoServiceProvider mobjCryptoService = new DESCryptoServiceProvider();
            mobjCryptoService.Key = MD5_iv;
            mobjCryptoService.IV = MD5_key;
            //实例流进行解密   
            System.IO.MemoryStream ms = new System.IO.MemoryStream(bytIn, 0, bytIn.Length);
            ICryptoTransform encrypto = mobjCryptoService.CreateDecryptor();
            CryptoStream cs = new CryptoStream(ms, encrypto, CryptoStreamMode.Read);
            StreamReader strd = new StreamReader(cs, Encoding.Default);
            val = strd.ReadToEnd();
        }
        catch (Exception ex)
        {
            Debug.Log("Decrypt -> ex == " + ex);
        }
        return val;
    }
    #endregion
}
