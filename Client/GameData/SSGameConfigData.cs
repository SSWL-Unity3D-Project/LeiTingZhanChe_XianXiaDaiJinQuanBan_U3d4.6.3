using System.Xml;
using System.IO;
using System;
using UnityEngine;

/// <summary>
/// 游戏配置信息.
/// </summary>
public class SSGameConfigData
{
    #region 游戏配置信息
    string m_FileName = "GameConfigData.db";
    /// <summary>
    /// 初始化.
    /// </summary>
    internal void Init()
    {
        //InitMianFeiFuHuoCiShu();
        InitPlayerHealthMax();
        InitPingJiFenShu();
        InitZaiWanYiJuGaiLv();
        InitXueBaoJianGeTime();
    }
    
    /// <summary>
    /// 免费复活次数.
    /// </summary>
    //internal int m_MianFeiFuHuoCiShu = 1;
    /// <summary>
    /// 初始化免费复活次数信息.
    /// </summary>
    //void InitMianFeiFuHuoCiShu()
    //{
    //    string info = ReadFromFileXml(m_FileName, "mianFeiFuHuoCiShu");
    //    if (info == null || info == "")
    //    {
    //        //默认值为1.
    //        info = "1";
    //        WriteToFileXml(m_FileName, "mianFeiFuHuoCiShu", info);
    //    }
    //    m_MianFeiFuHuoCiShu = Convert.ToInt32(info);
    //}

    /// <summary>
    /// 更新免费复活次数.
    /// </summary>
    //internal void UpdataMianFeiFuHuoCiShu(int mianFeiFuHuoCiShu)
    //{
    //    if (mianFeiFuHuoCiShu < 0)
    //    {
    //        mianFeiFuHuoCiShu = 0;
    //    }
    //    m_MianFeiFuHuoCiShu = mianFeiFuHuoCiShu;
    //    WriteToFileXml(m_FileName, "mianFeiFuHuoCiShu", mianFeiFuHuoCiShu.ToString());
    //}

    /// <summary>
    /// 玩家最大血值信息.
    /// </summary>
    internal int m_PlayerHealthMax = 45000;
    /// <summary>
    /// 初始化玩家最大血值信息.
    /// </summary>
    void InitPlayerHealthMax()
    {
        string info = ReadFromFileXml(m_FileName, "playerHealthMax");
        if (info == null || info == "")
        {
            //默认值为45000.
            info = "45000";
            WriteToFileXml(m_FileName, "playerHealthMax", info);
        }
        m_PlayerHealthMax = Convert.ToInt32(info);

        if (XkGameCtrl.GetInstance() != null && XkGameCtrl.GetInstance().PlayerXueLiangMax != m_PlayerHealthMax)
        {
            //更新血量最大值.
            XkGameCtrl.GetInstance().UpdatePlayerMaxHealth(m_PlayerHealthMax);
        }
    }

    /// <summary>
    /// 更新玩家最大血值.
    /// </summary>
    internal void UpdataPlayerHealthMax(int playerHealthMax)
    {
        if (playerHealthMax < 0)
        {
            playerHealthMax = 0;
        }

        if (XkGameCtrl.GetInstance() != null && XkGameCtrl.GetInstance().PlayerXueLiangMax != playerHealthMax)
        {
            //更新血量最大值.
            XkGameCtrl.GetInstance().UpdatePlayerMaxHealth(playerHealthMax);
        }

        if (m_PlayerHealthMax != playerHealthMax)
        {
            m_PlayerHealthMax = playerHealthMax;
            WriteToFileXml(m_FileName, "playerHealthMax", playerHealthMax.ToString());
        }
    }

    /// <summary>
    /// 分数评级信息.
    /// </summary>
    int m_PingJi_SSS = 200000;
    int m_PingJi_SS = 120000;
    int m_PingJi_S = 80000;
    int m_PingJi_A = 40000;
    int m_PingJi_B = 20000;
    int m_PingJi_C = 10000;
    int m_PingJi_D = 0;
    /// <summary>
    /// 获取游戏当前评级的最低分数信息.
    /// </summary>
    int GetGamePinJiFenShu(SSPingJiData.PingJiLevel pingJiLevel)
    {
        int fenShu = 0;
        switch (pingJiLevel)
        {
            case SSPingJiData.PingJiLevel.SSS:
                {
                    fenShu = m_PingJi_SSS;
                    break;
                }
            case SSPingJiData.PingJiLevel.SS:
                {
                    fenShu = m_PingJi_SS;
                    break;
                }
            case SSPingJiData.PingJiLevel.S:
                {
                    fenShu = m_PingJi_S;
                    break;
                }
            case SSPingJiData.PingJiLevel.A:
                {
                    fenShu = m_PingJi_A;
                    break;
                }
            case SSPingJiData.PingJiLevel.B:
                {
                    fenShu = m_PingJi_B;
                    break;
                }
            case SSPingJiData.PingJiLevel.C:
                {
                    fenShu = m_PingJi_C;
                    break;
                }
            case SSPingJiData.PingJiLevel.D:
                {
                    fenShu = m_PingJi_D;
                    break;
                }
        }
        return fenShu;
    }

    /// <summary>
    /// 获取游戏评级分数列表信息.
    /// </summary>
    internal int[] GetGamePingJiFenShuArray()
    {
        int[] fenShuArray = new int[7];
        for (int i = 0; i < fenShuArray.Length; i++)
        {
            fenShuArray[i] = GetGamePinJiFenShu((SSPingJiData.PingJiLevel)i);
        }
        return fenShuArray;
    }

    /// <summary>
    /// 初始化玩家评级分数信息.
    /// </summary>
    void InitPingJiFenShu()
    {
        string info_sss = ReadFromFileXml(m_FileName, "PingJi_SSS");
        if (info_sss == null || info_sss == "")
        {
            info_sss = "200000";
            WriteToFileXml(m_FileName, "PingJi_SSS", info_sss);
        }
        m_PingJi_SSS = Convert.ToInt32(info_sss);

        string info_ss = ReadFromFileXml(m_FileName, "PingJi_SS");
        if (info_ss == null || info_ss == "")
        {
            info_ss = "120000";
            WriteToFileXml(m_FileName, "PingJi_SS", info_ss);
        }
        m_PingJi_SS = Convert.ToInt32(info_ss);

        string info_s = ReadFromFileXml(m_FileName, "PingJi_S");
        if (info_s == null || info_s == "")
        {
            info_s = "80000";
            WriteToFileXml(m_FileName, "PingJi_S", info_s);
        }
        m_PingJi_S = Convert.ToInt32(info_s);

        string info_a = ReadFromFileXml(m_FileName, "PingJi_A");
        if (info_a == null || info_a == "")
        {
            info_a = "40000";
            WriteToFileXml(m_FileName, "PingJi_A", info_a);
        }
        m_PingJi_A = Convert.ToInt32(info_a);

        string info_b = ReadFromFileXml(m_FileName, "PingJi_B");
        if (info_b == null || info_b == "")
        {
            info_b = "20000";
            WriteToFileXml(m_FileName, "PingJi_B", info_b);
        }
        m_PingJi_B = Convert.ToInt32(info_b);

        string info_c = ReadFromFileXml(m_FileName, "PingJi_C");
        if (info_c == null || info_c == "")
        {
            info_c = "10000";
            WriteToFileXml(m_FileName, "PingJi_C", info_c);
        }
        m_PingJi_C = Convert.ToInt32(info_c);

        string info_d = ReadFromFileXml(m_FileName, "PingJi_D");
        if (info_d == null || info_d == "")
        {
            info_d = "0";
            WriteToFileXml(m_FileName, "PingJi_D", info_d);
        }
        m_PingJi_D = Convert.ToInt32(info_d);

        if (XkGameCtrl.GetInstance() != null)
        {
            int[] fenShuArray = GetGamePingJiFenShuArray();
            XkGameCtrl.GetInstance().UpdateGamePingJiFenShuInfo(fenShuArray);
        }
    }

    /// <summary>
    /// 更新玩家评级分数信息.
    /// </summary>
    internal void UpdataPingJiFenShu(int pingJi_sss, int pingJi_ss, int pingJi_s,
        int pingJi_a, int pingJi_b, int pingJi_c, int pingJi_d)
    {
        if (pingJi_sss < 0)
        {
            pingJi_sss = 0;
        }
        m_PingJi_SSS = pingJi_sss;
        WriteToFileXml(m_FileName, "PingJi_SSS", pingJi_sss.ToString());
        
        if (pingJi_ss < 0)
        {
            pingJi_ss = 0;
        }
        m_PingJi_SS = pingJi_ss;
        WriteToFileXml(m_FileName, "PingJi_SS", pingJi_ss.ToString());

        if (pingJi_s < 0)
        {
            pingJi_s = 0;
        }
        m_PingJi_S = pingJi_s;
        WriteToFileXml(m_FileName, "PingJi_S", pingJi_s.ToString());

        if (pingJi_a < 0)
        {
            pingJi_a = 0;
        }
        m_PingJi_A = pingJi_a;
        WriteToFileXml(m_FileName, "PingJi_A", pingJi_a.ToString());

        if (pingJi_b < 0)
        {
            pingJi_b = 0;
        }
        m_PingJi_B = pingJi_b;
        WriteToFileXml(m_FileName, "PingJi_B", pingJi_b.ToString());

        if (pingJi_c < 0)
        {
            pingJi_c = 0;
        }
        m_PingJi_C = pingJi_c;
        WriteToFileXml(m_FileName, "PingJi_C", pingJi_c.ToString());

        if (pingJi_d < 0)
        {
            pingJi_d = 0;
        }
        m_PingJi_D = pingJi_d;
        WriteToFileXml(m_FileName, "PingJi_D", pingJi_d.ToString());

        if (XkGameCtrl.GetInstance() != null)
        {
            int[] fenShuArray = GetGamePingJiFenShuArray();
            XkGameCtrl.GetInstance().UpdateGamePingJiFenShuInfo(fenShuArray);
        }
    }
    
    /// <summary>
    /// 玩家再玩一局游戏奖品的概率信息.
    /// </summary>
    internal int m_ZaiWanYiJuGaiLv = 10;
    /// <summary>
    /// 初始化玩家再玩一局游戏奖品的概率信息.
    /// </summary>
    void InitZaiWanYiJuGaiLv()
    {
        string info = ReadFromFileXml(m_FileName, "ZaiWanYiJuGaiLv");
        if (info == null || info == "")
        {
            info = "10";
            WriteToFileXml(m_FileName, "ZaiWanYiJuGaiLv", info);
        }
        m_ZaiWanYiJuGaiLv = Convert.ToInt32(info);

        if (XkGameCtrl.GetInstance() != null)
        {
            XkGameCtrl.GetInstance().UpdateZaiWanYiJuGaiLv(m_ZaiWanYiJuGaiLv);
        }
    }

    /// <summary>
    /// 更新玩家再玩一局游戏奖品的概率信息.
    /// </summary>
    internal void UpdataZaiWanYiJuGaiLv(int zaiWanYiJuGaiLv)
    {
        if (zaiWanYiJuGaiLv < 0)
        {
            zaiWanYiJuGaiLv = 10;
        }
        m_ZaiWanYiJuGaiLv = zaiWanYiJuGaiLv;
        WriteToFileXml(m_FileName, "ZaiWanYiJuGaiLv", zaiWanYiJuGaiLv.ToString());

        if (XkGameCtrl.GetInstance() != null)
        {
            XkGameCtrl.GetInstance().UpdateZaiWanYiJuGaiLv(zaiWanYiJuGaiLv);
        }
    }

    /// <summary>
    /// 玩家再玩一局游戏奖品的概率信息.
    /// </summary>
    internal int m_XueBaoJianGeTime = 10;
    /// <summary>
    /// 初始化玩家再玩一局游戏奖品的概率信息.
    /// </summary>
    void InitXueBaoJianGeTime()
    {
        string info = ReadFromFileXml(m_FileName, "XueBaoJianGeTime");
        if (info == null || info == "")
        {
            info = "10";
            WriteToFileXml(m_FileName, "XueBaoJianGeTime", info);
        }
        m_XueBaoJianGeTime = Convert.ToInt32(info);
    }

    /// <summary>
    /// 更新玩家再玩一局游戏奖品的概率信息.
    /// </summary>
    internal void UpdataXueBaoJianGeTime(int xueBaoJianGeTime)
    {
        if (xueBaoJianGeTime <= 0)
        {
            xueBaoJianGeTime = 10;
        }
        m_XueBaoJianGeTime = xueBaoJianGeTime;
        WriteToFileXml(m_FileName, "XueBaoJianGeTime", xueBaoJianGeTime.ToString());

        if (XkGameCtrl.GetInstance() != null)
        {
            XkGameCtrl.GetInstance().m_NpcDiaoDaoJuTimeLengQue = xueBaoJianGeTime;
        }
    }
    #endregion

    #region 数据读写管理
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
}
