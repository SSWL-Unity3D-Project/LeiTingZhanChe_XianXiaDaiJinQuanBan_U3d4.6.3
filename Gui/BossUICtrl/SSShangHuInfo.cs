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
        /// <summary>
        /// 代金券名称.
        /// </summary>
        public string DaiJinQuanName = "恭喜获得抵扣代金券";
        /// <summary>
        /// 代金券详情信息.
        /// </summary>
        public string XiangQingInfo = "此代金券只能在游戏合作商家内使用。";
        public override string ToString()
        {
            return "ShangHuMing == " + ShangHuMing + ", DaiJinQuanName == " + DaiJinQuanName + ", XiangQingInfo == " + XiangQingInfo;
        }
    }
    /// <summary>
    /// 大奖Boss商户信息.
    /// </summary>
    public DaJiangBossShangHuData[] m_DaJiangBossShangHuDt = new DaJiangBossShangHuData[4];

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
        /// 商户名信息/游戏中展示的奖品名称.
        /// 最多5个字.
        /// </summary>
        public string ShangHuJiangPinName = "盛世网络";
        /// <summary>
        /// 商户弹幕文本信息.
        /// 最多9个字.
        /// </summary>
        public string ShangHuDanMuInfo = "盛世网络50元";
        public override string ToString()
        {
            return "IndexShangHu == " + IndexShangHu + ", ShangHuMing == " + ShangHuJiangPinName + ", ShangHuDanMuInfo == " + ShangHuDanMuInfo;
        }
        /// <summary>
        /// 代金券名称.
        /// </summary>
        public string DaiJinQuanName = "恭喜获得抵扣代金券";
        /// <summary>
        /// 代金券详情信息.
        /// </summary>
        public string XiangQingInfo = "此代金券只能在游戏合作商家内使用。";
    }
    /// <summary>
    /// 战车01奖品配置信息.
    /// 最多2个商户数据信息.
    /// </summary>
    public ShangHuData[] m_ShangHuDtZhanChe01 = new ShangHuData[2];
    /// <summary>
    /// 战车02奖品配置信息.
    /// 最多2个商户数据信息.
    /// </summary>
    public ShangHuData[] m_ShangHuDtZhanChe02 = new ShangHuData[2];
    /// <summary>
    /// 奖品4-游戏中随机道具奖品.
    /// </summary>
    public ShangHuData m_ShangHuDtSuiJiDaoJu = new ShangHuData();
    /// <summary>
    /// 游戏弹幕中商户或奖品名称信息.
    /// 最多4个商户数据信息.
    /// </summary>
    public ShangHuData[] m_ShangHuDanMuDt = new ShangHuData[4];

    /// <summary>
    /// 手机端代金券展示信息.
    /// </summary>
    public class DaiJinQuanData
    {
        /// <summary>
        /// 商户名信息.
        /// 最多5个字.
        /// </summary>
        public string ShangHuMing = "盛世网络";
        /// <summary>
        /// 代金券名称.
        /// </summary>
        public string DaiJinQuanName = "恭喜获得抵扣代金券";
        /// <summary>
        /// 代金券详情信息.
        /// </summary>
        public string XiangQingInfo = "此代金券只能在游戏合作商家内使用。";
        public void Reset()
        {
            ShangHuMing = "";
            DaiJinQuanName = "";
            XiangQingInfo = "";
        }
    }
    internal DaiJinQuanData m_DaiJinQuanDt;
    
    /// <summary>
    /// 战车01奖品商户名列表信息索引.
    /// </summary>
    int m_IndexShangHuZhanChe01 = 0;
    /// <summary>
    /// 战车02奖品商户名列表信息索引.
    /// </summary>
    int m_IndexShangHuZhanChe02 = 0;

    internal void Init()
    {
        m_DaiJinQuanDt = new DaiJinQuanData();
        m_IndexShangHuZhanChe01 = m_ShangHuDtZhanChe01.Length;
        m_IndexShangHuZhanChe02 = m_ShangHuDtZhanChe02.Length;
        m_IndexJPShangHu = m_DaJiangBossShangHuDt.Length;
        for (int i = 0; i < m_ShangHuDtZhanChe01.Length; i++)
        {
            m_ShangHuDtZhanChe01[i].IndexShangHu = i;
            SSDebug.Log("Init -> ShangHuMing[" + i + "] ===== " + m_ShangHuDtZhanChe01[i].ShangHuJiangPinName);
        }
        InitReadConfig();
    }
    
    /// <summary>
    /// 更新游戏大奖Boss商户数据信息.
    /// </summary>
    internal void UpdateDaJiangBossShangHuInfo(string[] shangHuInfoArray)
    {
        if (m_DaJiangBossShangHuDt != null)
        {
            for (int i = 0; i < m_DaJiangBossShangHuDt.Length; i++)
            {
                m_DaJiangBossShangHuDt[i].ShangHuMing = shangHuInfoArray[i];
                //SSDebug.Log("UpdateDaJiangBossShangHuInfo -> JPShangHuMing[" + i + "] ===== " + shangHuInfoArray[i]);
            }
        }
    }

    /// <summary>
    /// 更新游戏大奖Boss代金券使用详情数据信息.
    /// </summary>
    internal void UpdateDaJiangBossDaiJinQuanXiangQing(string[] xiangQingInfoArray)
    {
        if (m_DaJiangBossShangHuDt != null)
        {
            for (int i = 0; i < m_DaJiangBossShangHuDt.Length; i++)
            {
                m_DaJiangBossShangHuDt[i].XiangQingInfo = xiangQingInfoArray[i];
                //SSDebug.Log("UpdateDaJiangBossDaiJinQuanXiangQing -> JPDescription[" + i + "] ===== " + xiangQingInfoArray[i]);
            }
        }
    }

    /// <summary>
    /// 更新游戏大奖Boss代金券名称数据信息.
    /// </summary>
    internal void UpdateDaJiangBossDaiJinQuanName(string[] infoArray)
    {
        if (m_DaJiangBossShangHuDt != null)
        {
            for (int i = 0; i < m_DaJiangBossShangHuDt.Length; i++)
            {
                m_DaJiangBossShangHuDt[i].DaiJinQuanName = infoArray[i];
                //SSDebug.Log("UpdateDaJiangBossDaiJinQuanName -> JPName[" + i + "] ===== " + infoArray[i]);
            }
        }
    }

    /// <summary>
    /// 更新游戏商户数据信息.
    /// </summary>
    internal void UpdateShangHuInfo(string[] shangHuInfoArray)
    {
        for (int i = 0; i < m_ShangHuDtZhanChe01.Length; i++)
        {
            m_ShangHuDtZhanChe01[i].ShangHuJiangPinName = shangHuInfoArray[i];
        }

        for (int i = 0; i < m_ShangHuDtZhanChe02.Length; i++)
        {
            m_ShangHuDtZhanChe02[i].ShangHuJiangPinName = shangHuInfoArray[i + 2];
        }
    }

    /// <summary>
    /// 更新游戏商户战车代金券使用详情数据信息.
    /// </summary>
    internal void UpdateShangHuDaiJinQuanXiangQing(string[] xiangQingInfoArray)
    {
        for (int i = 0; i < m_ShangHuDtZhanChe01.Length; i++)
        {
            m_ShangHuDtZhanChe01[i].XiangQingInfo = xiangQingInfoArray[i];
        }

        for (int i = 0; i < m_ShangHuDtZhanChe02.Length; i++)
        {
            m_ShangHuDtZhanChe02[i].XiangQingInfo = xiangQingInfoArray[i + 2];
        }
    }
    
    /// <summary>
    /// 更新游戏商户战车代金券名称数据信息.
    /// </summary>
    internal void UpdateShangHuDaiJinQuanName(string[] infoArray)
    {
        for (int i = 0; i < m_ShangHuDtZhanChe01.Length; i++)
        {
            m_ShangHuDtZhanChe01[i].DaiJinQuanName = infoArray[i];
        }

        for (int i = 0; i < m_ShangHuDtZhanChe02.Length; i++)
        {
            m_ShangHuDtZhanChe02[i].DaiJinQuanName = infoArray[i + 2];
        }
    }

    /// <summary>
    /// 更新游戏商户弹幕数据信息.
    /// </summary>
    internal void UpdateShangHuDanMuInfo(string[] shangHuDanMuInfoArray)
    {
        for (int i = 0; i < m_ShangHuDanMuDt.Length; i++)
        {
            m_ShangHuDanMuDt[i].ShangHuDanMuInfo = shangHuDanMuInfoArray[i];
            //SSDebug.Log("UpdateShangHuDanMuInfo -> ShangHuDanMuInfo[" + i + "] ===== " + shangHuDanMuInfoArray[i]);
        }

        if (SSUIRoot.GetInstance().m_GameUIManage != null
            && SSUIRoot.GetInstance().m_GameUIManage.m_DanMuTextUI != null)
        {
            //更新游戏弹幕的商户名信息.
            SSUIRoot.GetInstance().m_GameUIManage.m_DanMuTextUI.UpdateShangJiaDanMuInfo();
        }
    }

    int m_IndexJPShangHu = 0;
    /// <summary>
    /// 获取JPBoss代金券的商户名信息.
    /// </summary>
    internal string GetJPBossShangHuMingInfo()
    {
        m_IndexJPShangHu++;
        if (m_IndexJPShangHu >= m_DaJiangBossShangHuDt.Length)
        {
            m_IndexJPShangHu = 0;
        }
        int indexVal = m_IndexJPShangHu;
        //SSDebug.LogWarning("GetJPBossShangHuMingInfo -> " + m_DaJiangBossShangHuDt[indexVal].ToString());
        m_DaiJinQuanDt.ShangHuMing = m_DaJiangBossShangHuDt[indexVal].ShangHuMing;
        m_DaiJinQuanDt.DaiJinQuanName = m_DaJiangBossShangHuDt[indexVal].DaiJinQuanName;
        m_DaiJinQuanDt.XiangQingInfo = m_DaJiangBossShangHuDt[indexVal].XiangQingInfo;
        return m_DaJiangBossShangHuDt[indexVal].ShangHuMing;
    }

    /// <summary>
    /// 获取JPBoss代金券的商户名信息.
    /// </summary>
    internal string GetJPBossShangHuMingDt()
    {
        int indexVal = m_IndexJPShangHu;
        if (indexVal >= m_DaJiangBossShangHuDt.Length)
        {
            indexVal = 0;
        }
        //SSDebug.LogWarning("GetJPBossShangHuMingDt -> " + m_DaJiangBossShangHuDt[indexVal].ToString());
        return m_DaJiangBossShangHuDt[indexVal].ShangHuMing;
    }

    /// <summary>
    /// 战车
    /// 获取代金券npc的商户名信息.
    /// </summary>
    internal ShangHuData AddZhanCheShangHuMingInfo(SSCaiPiaoDataManage.GameCaiPiaoData.DaiJinQuanState type)
    {
        int indexVal = 0;
        ShangHuData[] dataArray = null;
        switch (type)
        {
            case SSCaiPiaoDataManage.GameCaiPiaoData.DaiJinQuanState.ZhanCheDaiJinQuan_01:
                {
                    dataArray = m_ShangHuDtZhanChe01;
                    m_IndexShangHuZhanChe01++;
                    if (m_IndexShangHuZhanChe01 >= dataArray.Length)
                    {
                        m_IndexShangHuZhanChe01 = 0;
                    }
                    indexVal = m_IndexShangHuZhanChe01;
                }
                break;
            case SSCaiPiaoDataManage.GameCaiPiaoData.DaiJinQuanState.ZhanCheDaiJinQuan_02:
                {
                    dataArray = m_ShangHuDtZhanChe02;
                    m_IndexShangHuZhanChe02++;
                    if (m_IndexShangHuZhanChe02 >= dataArray.Length)
                    {
                        m_IndexShangHuZhanChe02 = 0;
                    }
                    indexVal = m_IndexShangHuZhanChe02;
                }
                break;
        }

        if (dataArray == null)
        {
            SSDebug.LogWarning("AddShangHuMingInfo -> dataArray was null");
            return null;
        }
        
        //SSDebug.LogWarning("GetShangHuMingInfo -> ====================== " + dataArray[indexVal].ToString());
        m_DaiJinQuanDt.ShangHuMing = dataArray[indexVal].ShangHuJiangPinName;
        m_DaiJinQuanDt.DaiJinQuanName = dataArray[indexVal].DaiJinQuanName;
        m_DaiJinQuanDt.XiangQingInfo = dataArray[indexVal].XiangQingInfo;
        return dataArray[indexVal];
    }

    /// <summary>
    /// 战车
    /// 获取代金券npc的商户名信息.
    /// </summary>
    internal ShangHuData GetShangHuMingDt(SSCaiPiaoDataManage.GameCaiPiaoData.DaiJinQuanState type)
    {
        int indexVal = 0;
        ShangHuData[] dataArray = null;
        switch (type)
        {
            case SSCaiPiaoDataManage.GameCaiPiaoData.DaiJinQuanState.ZhanCheDaiJinQuan_01:
                {
                    indexVal = m_IndexShangHuZhanChe01;
                    dataArray = m_ShangHuDtZhanChe01;
                    if (indexVal >= dataArray.Length)
                    {
                        indexVal = 0;
                    }
                }
                break;
            case SSCaiPiaoDataManage.GameCaiPiaoData.DaiJinQuanState.ZhanCheDaiJinQuan_02:
                {
                    indexVal = m_IndexShangHuZhanChe02;
                    dataArray = m_ShangHuDtZhanChe02;
                    if (indexVal >= dataArray.Length)
                    {
                        indexVal = 0;
                    }
                }
                break;
        }

        if (dataArray == null)
        {
            SSDebug.LogWarning("GetShangHuMingDt -> dataArray was null");
            return null;
        }
        //SSDebug.LogWarning("GetShangHuMingDt -> ============================== " + dataArray[indexVal].ToString());
        return dataArray[indexVal];
    }

    /// <summary>
    /// 更新游戏随机道具奖品4的代金券信息.
    /// </summary>
    internal void UpdateSuiJiDaoJuShangHuInfo(string[] infoArray)
    {
        if (infoArray.Length >= 3)
        {
            m_ShangHuDtSuiJiDaoJu.ShangHuJiangPinName = infoArray[0];
            m_ShangHuDtSuiJiDaoJu.DaiJinQuanName = infoArray[1];
            m_ShangHuDtSuiJiDaoJu.XiangQingInfo = infoArray[2];
        }
    }

    /// <summary>
    /// 获取随机道具商户信息.
    /// </summary>
    internal ShangHuData GetSuiJiDaoJuShangHuInfo()
    {
        //SSDebug.LogWarning("GetSuiJiDaoJuShangHuInfo -> " + m_ShangHuDtSuiJiDaoJu.ToString());
        return m_ShangHuDtSuiJiDaoJu;
    }

    #region 从配置文件读取商户信息
    /// <summary>
    /// 商户配置信息.
    /// </summary>
    string ShangHuFileName = "../config/ShangHuConfig.xml";
    /**
     * <!-- DaJiangBoss -> ShangHuMing 该属性用于jpBoss出场时的商户信息            最多5个字 -->
     * <!-- ShangHuData -> ShangHuMing 该属性用于战车出场时的商户信息              最多5个字 -->
     * <!-- ShangHuData -> ShangHuDanMuInfo 该属性用于循环动画时的商户代金券信息   最多9个字 -->
     */
    void InitReadConfig()
    {
        bool isFix = false;
        //读取jpBoss商户名信息.
        string[] daJiangBossShangHuMingArray = ReadArrayFromFileXml(ShangHuFileName, "DaJiangBoss", "ShangHuMing");
        if (daJiangBossShangHuMingArray == null || daJiangBossShangHuMingArray.Length < 4)
        {
            isFix = true;
            //daJiangBossShangHuMingArray = "盛世网络";
            //WriteToFileXml(ShangHuFileName, "DaJiangBoss", "ShangHuMing", daJiangBossShangHuMingArray);
        }
        else if (daJiangBossShangHuMingArray.Length >= 4)
        {
            for (int i = 0; i < daJiangBossShangHuMingArray.Length; i++)
            {
                if (daJiangBossShangHuMingArray[i] == "")
                {
                    isFix = true;
                }
            }
        }

        if (isFix == true)
        {
            //初始化信息.
            daJiangBossShangHuMingArray = new string[4] { "盛世网络", "陕西纷腾", "西安纷腾", "三角犀" };
            WriteToFileXml(ShangHuFileName, "DaJiangBoss", "ShangHuMing", daJiangBossShangHuMingArray);
        }

        //商户名信息.
        string[] shangHuMingArray = ReadArrayFromFileXml(ShangHuFileName, "ShangHuData", "ShangHuMing");
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
            WriteToFileXml(ShangHuFileName, "ShangHuData", "ShangHuMing", shangHuMingArray);
        }

        //商户代金券信息,在游戏循环动画弹幕中展示.
        isFix = false;
        string[] shangHuDaiJinQuanArray = ReadArrayFromFileXml(ShangHuFileName, "ShangHuData", "ShangHuDanMuInfo");
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
            WriteToFileXml(ShangHuFileName, "ShangHuData", "ShangHuDanMuInfo", shangHuDaiJinQuanArray);
        }
        
        UpdateDaJiangBossShangHuInfo(daJiangBossShangHuMingArray);
        UpdateShangHuInfo(shangHuMingArray);
        UpdateShangHuDanMuInfo(shangHuDaiJinQuanArray);
    }

    /// <summary>
    /// 创建商户数据信息.
    /// </summary>
    void CreatShangHuData(string filepath)
    {
        XmlDocument xmlDoc = new XmlDocument();
        XmlElement root = xmlDoc.CreateElement("ConfigShangHuData");
        for (int i = 0; i < 4; i++)
        {
            //JP大奖商户信息.
            XmlElement elmNewJPDaJiang = xmlDoc.CreateElement("DaJiangBoss");
            root.AppendChild(elmNewJPDaJiang);
        }

        for (int i = 0; i < 4; i++)
        {
            //游戏弹幕中商户信息和游戏里战车代金券信息.
            XmlElement elmNew = xmlDoc.CreateElement("ShangHuData");
            root.AppendChild(elmNew);
        }
        xmlDoc.AppendChild(root);
        xmlDoc.Save(filepath);
        File.SetAttributes(filepath, FileAttributes.Normal);
    }

    /// <summary>
    /// 写入单条数据.
    /// </summary>
    void WriteToFileXml(string fileName, string elementName, string attribute, string valueStr)
    {
        string filepath = Application.dataPath + "/" + fileName;
#if UNITY_ANDROID
		filepath = Application.persistentDataPath + "//" + fileName;
#endif

        //create file
        if (!File.Exists(filepath))
        {
            CreatShangHuData(filepath);
        }

        //update value
        if (File.Exists(filepath))
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(filepath);
            XmlNodeList nodeList = xmlDoc.SelectSingleNode("ConfigShangHuData").ChildNodes;

            foreach (XmlElement xe in nodeList)
            {
                if (xe.Name == elementName)
                {
                    xe.SetAttribute(attribute, valueStr);
                    break;
                }
            }
            File.SetAttributes(filepath, FileAttributes.Normal);
            xmlDoc.Save(filepath);
        }
    }

    /// <summary>
    /// 写入多条数据.
    /// </summary>
    void WriteToFileXml(string fileName, string elementName, string attribute, string[] valueStr)
    {
        string filepath = Application.dataPath + "/" + fileName;
#if UNITY_ANDROID
		filepath = Application.persistentDataPath + "//" + fileName;
#endif

        //create file
        if (!File.Exists(filepath))
        {
            CreatShangHuData(filepath);
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
                if (xe.Name == elementName)//"ShangHuData")
                {
                    xe.SetAttribute(attribute, valueStr[countNum]);
                    countNum++;
                }
            }
            File.SetAttributes(filepath, FileAttributes.Normal);
            xmlDoc.Save(filepath);
        }
    }

    /// <summary>
    /// 读取单条数据.
    /// </summary>
    string ReadFromFileXml(string fileName, string elementName, string attribute)
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
                XmlNodeList nodeList = xmlDoc.SelectSingleNode("ConfigShangHuData").ChildNodes;
                foreach (XmlElement xe in nodeList)
                {
                    if (xe.Name == elementName)
                    {
                        valueStr = xe.GetAttribute(attribute);
                        break;
                    }
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

    /// <summary>
    /// 读取多条数据.
    /// </summary>
    string[] ReadArrayFromFileXml(string fileName, string elementName, string attribute)
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
                    if (xe.Name == elementName)
                    {
                        valueStr[countNum] = xe.GetAttribute(attribute);
                        countNum++;
                    }
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
