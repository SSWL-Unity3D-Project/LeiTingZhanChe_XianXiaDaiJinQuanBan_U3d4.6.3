using System;
using System.IO;
using System.Xml;
using UnityEngine;

/// <summary>
/// 游戏商户配置信息.
/// </summary>
public class SSShangHuInfo : MonoBehaviour
{
    /// <summary>
    /// 大奖Boss商户信息.
    /// </summary>
    [Serializable]
    public class DaJiangBossShangHuData
    {
        /// <summary>
        /// 商户名信息.
        /// 最多5个字.
        /// </summary>
        public string ShangHuMing = "盛世网络";
    }
    /// <summary>
    /// 大奖Boss商户信息.
    /// </summary>
    public DaJiangBossShangHuData m_DaJiangBossShangHuDt;

    /// <summary>
    /// 商户信息.
    /// </summary>
    [Serializable]
    public class ShangHuData
    {
        /// <summary>
        /// 商户名列表信息索引.
        /// </summary>
        internal int IndexShangHu = 0;
        /// <summary>
        /// 商户名信息.
        /// 最多5个字.
        /// </summary>
        public string ShangHuMing = "盛世网络";
        /// <summary>
        /// 商户弹幕文本信息.
        /// 最多9个字.
        /// </summary>
        public string ShangHuDanMuInfo = "盛世网络50元";
        public override string ToString()
        {
            return "IndexShangHu == " + IndexShangHu + ", ShangHuMing == " + ShangHuMing + ", ShangHuDanMuInfo == " + ShangHuDanMuInfo;
        }
    }
    /// <summary>
    /// 商户配置信息.
    /// 最多4个商户数据信息.
    /// </summary>
    public ShangHuData[] m_ShangHuDt = new ShangHuData[4];
    /// <summary>
    /// 商户名列表信息索引.
    /// </summary>
    int m_IndexShangHu = 0;

    internal void Init()
    {
        for (int i = 0; i < m_ShangHuDt.Length; i++)
        {
            m_ShangHuDt[i].IndexShangHu = i;
            SSDebug.Log("Init -> ShangHuMing[" + i + "] ===== " + m_ShangHuDt[i].ShangHuMing);
        }
        InitReadConfig();
    }

    /// <summary>
    /// 更新游戏大奖Boss商户数据信息.
    /// </summary>
    internal void UpdateDaJiangBossShangHuInfo(string shangHuInfo)
    {
        if (m_DaJiangBossShangHuDt != null)
        {
            m_DaJiangBossShangHuDt.ShangHuMing = shangHuInfo;
            SSDebug.Log("UpdateDaJiangBossShangHuInfo -> ShangHuMing ===== " + shangHuInfo);
        }
    }

    /// <summary>
    /// 更新游戏商户数据信息.
    /// </summary>
    internal void UpdateShangHuInfo(string[] shangHuInfoArray)
    {
        for (int i = 0; i < m_ShangHuDt.Length; i++)
        {
            m_ShangHuDt[i].ShangHuMing = shangHuInfoArray[i];
            SSDebug.Log("UpdateShangHuInfo -> ShangHuMing[" + i + "] ===== " + shangHuInfoArray[i]);
        }
    }

    /// <summary>
    /// 更新游戏商户弹幕数据信息.
    /// </summary>
    internal void UpdateShangHuDanMuInfo(string[] shangHuDanMuInfoArray)
    {
        for (int i = 0; i < m_ShangHuDt.Length; i++)
        {
            m_ShangHuDt[i].ShangHuDanMuInfo = shangHuDanMuInfoArray[i];
            SSDebug.Log("UpdateShangHuDanMuInfo -> ShangHuDanMuInfo[" + i + "] ===== " + shangHuDanMuInfoArray[i]);
        }

        if (SSUIRoot.GetInstance().m_GameUIManage != null
            && SSUIRoot.GetInstance().m_GameUIManage.m_DanMuTextUI != null)
        {
            //更新游戏弹幕的商户名信息.
            SSUIRoot.GetInstance().m_GameUIManage.m_DanMuTextUI.UpdateShangJiaDanMuInfo();
        }
    }

    /// <summary>
    /// 获取代金券npc的商户名信息.
    /// </summary>
    internal ShangHuData GetShangHuMingInfo()
    {
        int indexVal = m_IndexShangHu;
        m_IndexShangHu++;
        if (m_IndexShangHu >= m_ShangHuDt.Length)
        {
            m_IndexShangHu = 0;
        }
        //SSDebug.Log("GetShangHuMingInfo -> " + m_ShangHuDt[indexVal].ToString());
        return m_ShangHuDt[indexVal];
    }

    #region 从配置文件读取商户信息
    /// <summary>
    /// 普通商户配置信息.
    /// </summary>
    string ShangHuFileName = "../config/ShangHuConfig.xml";
    /// <summary>
    /// 大奖商户配置信息.
    /// </summary>
    string DaJiangFileName = "../config/DaJiangShangHuConfig.xml";
    /**
     * <!-- DaJiangBoss -> ShangHuMing 该属性用于jpBoss出场时的商户信息            最多5个字 -->
     * <!-- ShangHuData -> ShangHuMing 该属性用于战车出场时的商户信息              最多5个字 -->
     * <!-- ShangHuData -> ShangHuDanMuInfo 该属性用于循环动画时的商户代金券信息   最多9个字 -->
     */
    void InitReadConfig()
    {
        //读取jpBoss商户名信息.
        string daJiangBossShangHuMing = ReadFromFileXml(DaJiangFileName, "ShangHuMing");
        if (daJiangBossShangHuMing == null || daJiangBossShangHuMing == "")
        {
            daJiangBossShangHuMing = "盛世网络";
            WriteToFileXml(DaJiangFileName, "ShangHuMing", daJiangBossShangHuMing);
        }

        bool isFix = false;
        //商户名信息.
        string[] shangHuMingArray = ReadArrayFromFileXml(ShangHuFileName, "ShangHuMing");
        if (shangHuMingArray == null || shangHuMingArray.Length < 4)
        {
            isFix = true;
        }
        else if (shangHuMingArray.Length >= 4)
        {
            for (int i = 0; i < shangHuMingArray.Length; i++)
            {
                if (shangHuMingArray[i] == "")
                {
                    isFix = true;
                }
            }
        }

        if (isFix == true)
        {
            //初始化信息.
            shangHuMingArray = new string[4] { "盛世网络", "陕西纷腾", "西安纷腾", "三角犀" };
            WriteToFileXml(ShangHuFileName, "ShangHuMing", shangHuMingArray);
        }

        //商户代金券信息,在游戏循环动画弹幕中展示.
        isFix = false;
        string[] shangHuDaiJinQuanArray = ReadArrayFromFileXml(ShangHuFileName, "ShangHuDanMuInfo");
        if (shangHuDaiJinQuanArray == null || shangHuDaiJinQuanArray.Length < 4)
        {
            isFix = true;
        }
        else if (shangHuDaiJinQuanArray.Length >= 4)
        {
            for (int i = 0; i < shangHuDaiJinQuanArray.Length; i++)
            {
                if (shangHuDaiJinQuanArray[i] == "")
                {
                    isFix = true;
                }
            }
        }

        if (isFix == true)
        {
            //初始化信息.
            shangHuDaiJinQuanArray = new string[4] { "盛世网络50元", "陕西纷腾50元", "西安纷腾50元", "三角犀50元" };
            WriteToFileXml(ShangHuFileName, "ShangHuDanMuInfo", shangHuDaiJinQuanArray);
        }
        
        UpdateDaJiangBossShangHuInfo(daJiangBossShangHuMing);
        UpdateShangHuInfo(shangHuMingArray);
        UpdateShangHuDanMuInfo(shangHuDaiJinQuanArray);
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
            XmlElement root = xmlDoc.CreateElement("ConfigDaJiangBoss");
            XmlElement elmNew = xmlDoc.CreateElement("DaJiangBoss");

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
            XmlNodeList nodeList = xmlDoc.SelectSingleNode("ConfigDaJiangBoss").ChildNodes;

            foreach (XmlElement xe in nodeList)
            {
                xe.SetAttribute(attribute, valueStr);
            }
            File.SetAttributes(filepath, FileAttributes.Normal);
            xmlDoc.Save(filepath);
        }
    }

    public void WriteToFileXml(string fileName, string attribute, string[] valueStr)
    {
        string filepath = Application.dataPath + "/" + fileName;
#if UNITY_ANDROID
		filepath = Application.persistentDataPath + "//" + fileName;
#endif

        //create file
        if (!File.Exists(filepath))
        {
            XmlDocument xmlDoc = new XmlDocument();
            XmlElement root = xmlDoc.CreateElement("ConfigShangHuData");
            //XmlElement elmNew = xmlDoc.CreateElement("ShangHuData");
            for (int i = 0; i < valueStr.Length; i++)
            {
                XmlElement elmNew = xmlDoc.CreateElement("ShangHuData");
                root.AppendChild(elmNew);
            }
            xmlDoc.AppendChild(root);
            xmlDoc.Save(filepath);
            File.SetAttributes(filepath, FileAttributes.Normal);
        }

        //update value
        if (File.Exists(filepath))
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(filepath);
            XmlNodeList nodeList = xmlDoc.SelectSingleNode("ConfigShangHuData").ChildNodes;
            int countNum = 0;
            foreach (XmlElement xe in nodeList)
            {
                xe.SetAttribute(attribute, valueStr[countNum]);
                countNum++;
            }
            File.SetAttributes(filepath, FileAttributes.Normal);
            xmlDoc.Save(filepath);
        }
    }

    public string ReadFromFileXml(string fileName, string attribute)
    {
        string filepath = Application.dataPath + "/" + fileName;
#if UNITY_ANDROID
		//filepath = Application.persistentDataPath + "//" + fileName;
#endif
        string valueStr = null;
        if (File.Exists(filepath))
        {
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(filepath);
                XmlNodeList nodeList = xmlDoc.SelectSingleNode("ConfigDaJiangBoss").ChildNodes;
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
    
    public string[] ReadArrayFromFileXml(string fileName, string attribute)
    {
        string filepath = Application.dataPath + "/" + fileName;
#if UNITY_ANDROID
		//filepath = Application.persistentDataPath + "//" + fileName;
#endif
        string[] valueStr = null;
        if (File.Exists(filepath))
        {
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(filepath);
                XmlNodeList nodeList = xmlDoc.SelectSingleNode("ConfigShangHuData").ChildNodes;
                valueStr = new string[nodeList.Count];
                int countNum = 0;
                foreach (XmlElement xe in nodeList)
                {
                    valueStr[countNum] = xe.GetAttribute(attribute);
                    countNum++;
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
    #endregion
}
