using UnityEngine;
using System;
using System.Xml;
using System.IO;

public class SSChouJiangData : MonoBehaviour
{
    /// <summary>
    /// 再玩一局游戏的人次概率控制.
    /// 最大人数没有达到时随机放奖,如果最大人数已经够了但是奖品仍没有放出时就100%给最后的这个人发奖(同时清除当前记录的人数信息)
    /// </summary>
    public class ZaiWanYiJuJiangPinData
    {
        /// <summary>
        /// 爆奖率.
        /// </summary>
        [Range(0, 100)]
        int m_BaoJiangLv = 50;
        /// <summary>
        /// 设置爆奖率.
        /// </summary>
        void SetBaoJiangLv(int baoJiangLv)
        {
            m_BaoJiangLv = baoJiangLv;
        }
        /// <summary>
        /// 最大人数.
        /// </summary>
        internal int maxPlayer = 10;
        /// <summary>
        /// 当前人数.
        /// </summary>
        int curPlayer = 10;
        internal void SetCurrentPlayerNum(int num)
        {
            curPlayer = num;
        }
        internal int GetCurrentPlayerNum()
        {
            return curPlayer;
        }
        /// <summary>
        /// 随机概率.
        /// </summary>
        //int suiJiGaiLv = 30;
        /// <summary>
        /// 没有人可以爆奖.
        /// </summary>
        int ZeroPlayer = 999999999;
        /// <summary>
        /// 更新游戏在几个人里出一次奖.
        /// </summary>
        internal void SetMaxPlayer(int baoJiangLv)
        {
            if (baoJiangLv < 0 || baoJiangLv > 100)
            {
                baoJiangLv = 50;
            }
            SetBaoJiangLv(baoJiangLv);

            if (baoJiangLv == 0)
            {
                maxPlayer = ZeroPlayer;
            }
            else
            {
                int addVal = 0;
                if (100 % baoJiangLv != 0)
                {
                    addVal = 1;
                }
                maxPlayer = (100 / baoJiangLv) + addVal;
            }
        }

        /// <summary>
        /// 是否已经爆奖.
        /// </summary>
        bool IsHaveBaoJiang = false;
        internal void SetIsHaveJiBaoNpc(bool isJiBaoNpc)
        {
            IsHaveBaoJiang = isJiBaoNpc;
        }

        internal bool GetIsHaveJiBaoNpc()
        {
            return IsHaveBaoJiang;
        }

        /// <summary>
        /// 获取是否可以爆奖.
        /// </summary>
        internal bool GetIsCanBaoJiang()
        {
            if (IsHaveBaoJiang == true)
            {
                //已经有玩家击爆奖,其余玩家不能在出该奖品了.
                return false;
            }

            if (ZeroPlayer == maxPlayer)
            {
                //游戏后台配置爆奖率为0时不允许爆奖.
                return false;
            }

            if (curPlayer >= maxPlayer)
            {
                //当前玩家人数已经积累的足够多了,新来的玩家都可以击爆npc.
                return true;
            }

            bool isCanJiBao = true;
            int randNum = (UnityEngine.Random.Range(0, 1000) % 100) + 1;
            //该玩家在游戏中的爆奖概率 = 累计人头数×该奖池爆奖率.(累计人头数 <= 最大人头数)
            int baoJiangGaiLv = curPlayer * m_BaoJiangLv;
            //SSDebug.LogWarning("baoJiangGaiLv chouJiangDt == " + baoJiangGaiLv + ", curPlayer == " + curPlayer
            //    + ", m_BaoJiangLv == " + m_BaoJiangLv + ", randNum == " + randNum);
            if (randNum > baoJiangGaiLv)
            {
                isCanJiBao = false;
            }
            return isCanJiBao;
        }

        /// <summary>
        /// 获取人数是否足够.
        /// </summary>
        internal bool GetIsEnoughPlayerNum()
        {
            if (ZeroPlayer == maxPlayer)
            {
                //游戏后台配置爆奖率为0时不允许爆奖.
                return false;
            }

            if (curPlayer >= maxPlayer)
            {
                //当前玩家人数已经积累的足够多了,新来的玩家都可以击爆npc.
                return true;
            }
            //人数不足.
            return false;
        }

        internal bool AddCurrentNum()
        {
            bool isReset = false;
            curPlayer++;
            if (IsHaveBaoJiang == true)
            {
                if (curPlayer > maxPlayer)
                {
                    isReset = true;
                    ResetCurrentNum();
                }
            }
            return isReset;
        }

        void ResetCurrentNum()
        {
            IsHaveBaoJiang = false;
            if (curPlayer > maxPlayer)
            {
                curPlayer = curPlayer - maxPlayer;
            }
            else
            {
                curPlayer = 1;
            }
        }
    }
    ZaiWanYiJuJiangPinData m_ZaiWanYiJuJiangPinDt = new ZaiWanYiJuJiangPinData();

    /// <summary>
    /// 初始化.
    /// </summary>
    internal void Init(int gaiLv)
    {
        SetZaiWanYiJuJiangPinDtMaxPlayer(gaiLv);
        int num = ReadCurrentNumPlayer();
        bool isHaveBaoJiang = ReadIsHaveBaoJiang();
        if (m_ZaiWanYiJuJiangPinDt != null)
        {
            m_ZaiWanYiJuJiangPinDt.SetCurrentPlayerNum(num);
            m_ZaiWanYiJuJiangPinDt.SetIsHaveJiBaoNpc(isHaveBaoJiang);
        }
    }

    /// <summary>
    /// 设置再玩一局游戏奖品的最大人数.
    /// </summary>
    void SetZaiWanYiJuJiangPinDtMaxPlayer(int gaiLv)
    {
        if (m_ZaiWanYiJuJiangPinDt != null)
        {
            m_ZaiWanYiJuJiangPinDt.SetMaxPlayer(gaiLv);
        }
    }

    /// <summary>
    /// 获取是否可以出再玩一局游戏奖品.
    /// </summary>
    internal bool GetIsCanOutZaiWanYiJuJiangPin()
    {
        bool isCanOut = false;
        if (m_ZaiWanYiJuJiangPinDt != null)
        {
            isCanOut = m_ZaiWanYiJuJiangPinDt.GetIsCanBaoJiang();
        }
        return isCanOut;
    }

    /// <summary>
    /// 获取是否可以出再玩一局游戏奖品.
    /// </summary>
    internal bool GetIsEnoughPlayerNum()
    {
        bool isEnough = false;
        if (m_ZaiWanYiJuJiangPinDt != null)
        {
            isEnough = m_ZaiWanYiJuJiangPinDt.GetIsEnoughPlayerNum();
        }
        return isEnough;
    }

    /// <summary>
    /// 设置是否已经爆奖.
    /// </summary>
    internal void SetIsHaveBaoJiang(bool isHaveBaoJiang)
    {
        if (m_ZaiWanYiJuJiangPinDt != null)
        {
            WriteIsHaveBaoJiang(isHaveBaoJiang);
            m_ZaiWanYiJuJiangPinDt.SetIsHaveJiBaoNpc(isHaveBaoJiang);
        }
    }

    /// <summary>
    /// 添加玩家数量.
    /// </summary>
    internal void AddPlayerNum()
    {
        if (m_ZaiWanYiJuJiangPinDt != null)
        {
            bool isReset = m_ZaiWanYiJuJiangPinDt.AddCurrentNum();
            WriteCurrentNumPlayer(m_ZaiWanYiJuJiangPinDt.GetCurrentPlayerNum());
            if (isReset == true)
            {
                //重置爆奖信息
                WriteIsHaveBaoJiang(false);
            }
        }
    }


    #region 爆奖数据的读写操作
    /// <summary>
    /// 爆奖信息.
    /// </summary>
    string FileName = "../config/ZaiWanYiJuJiangPingConfig.xml";
    /// <summary>
    /// 读取当前奖品已经有几个玩家了.
    /// </summary>
    int ReadCurrentNumPlayer()
    {
        int num = 0;
        string ele = "BaoJiangDt";
        string atr = "CurrentPlayer";
        string info = ReadFromFileXml(FileName, ele, atr);
        if (info != null && info != "")
        {
            num = Convert.ToInt32(info);
        }
        else
        {
            WriteToFileXml(FileName, ele, atr, num.ToString());
        }
        return num;
    }

    /// <summary>
    /// 写入当前奖品已经有几个玩家了.
    /// </summary>
    void WriteCurrentNumPlayer(int num)
    {
        string ele = "BaoJiangDt";
        string atr = "CurrentPlayer";
        WriteToFileXml(FileName, ele, atr, num.ToString());
    }

    /// <summary>
    /// 读取当前奖池是否已经爆奖.
    /// </summary>
    bool ReadIsHaveBaoJiang()
    {
        int baoJiangInfo = 0;
        string ele = "BaoJiangDt";
        string attribute = "IsHaveBaoJiang";
        string info = ReadFromFileXml(FileName, ele, attribute);
        if (info != null && info != "")
        {
            baoJiangInfo = Convert.ToInt32(info);
        }
        else
        {
            WriteToFileXml(FileName, ele, attribute, baoJiangInfo.ToString());
        }
        return baoJiangInfo == 0 ? false : true;
    }

    /// <summary>
    /// 写入当前奖池是否已经爆奖.
    /// </summary>
    void WriteIsHaveBaoJiang(bool isHaveBaoJiang)
    {
        int num = isHaveBaoJiang == true ? 1 : 0;
        string ele = "BaoJiangDt";
        string attribute = "IsHaveBaoJiang";
        WriteToFileXml(FileName, ele, attribute, num.ToString());
    }

    /// <summary>
    /// 创建爆奖数据信息.
    /// </summary>
    void CreatBaoJiangData(string filepath)
    {
        XmlDocument xmlDoc = new XmlDocument();
        XmlElement root = xmlDoc.CreateElement("ConfigBaoJiangData");
        XmlElement elmNew = xmlDoc.CreateElement("BaoJiangDt");
        root.AppendChild(elmNew);
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
            CreatBaoJiangData(filepath);
        }

        //update value
        if (File.Exists(filepath))
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(filepath);
            XmlNodeList nodeList = xmlDoc.SelectSingleNode("ConfigBaoJiangData").ChildNodes;

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
                XmlNodeList nodeList = xmlDoc.SelectSingleNode("ConfigBaoJiangData").ChildNodes;
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
    #endregion
}
