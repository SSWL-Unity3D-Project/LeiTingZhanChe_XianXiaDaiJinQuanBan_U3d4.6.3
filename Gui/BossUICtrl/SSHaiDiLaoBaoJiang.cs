using System;
using System.IO;
using System.Xml;
using UnityEngine;

/// <summary>
/// 海底捞爆奖率控制.
/// </summary>
public class SSHaiDiLaoBaoJiang : MonoBehaviour
{
	[System.Serializable]
	public class BaoJiangData
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
		/// 几个人里出一次奖.
		/// </summary>
		int maxPlayer = 1;
		/// <summary>
		/// 当前已经有几个人进行游戏(对同一微信玩家不进行去重).
		/// </summary>
		int currentNum = 0;
        internal void SetCurrentNum(int num)
        {
            currentNum = num;
        }
        internal int GetCurrentNum()
        {
            return currentNum;
        }
        /// <summary>
        /// 是否已经击爆了Npc.
        /// </summary>
        bool IsHaveJiBaoNpc = false;
		internal void SetIsHaveJiBaoNpc(bool isJiBaoNpc)
		{
			IsHaveJiBaoNpc = isJiBaoNpc;
        }
        internal bool GetIsHaveJiBaoNpc()
        {
            return IsHaveJiBaoNpc;
        }

        /// <summary>
        /// 没有人可以爆奖.
        /// </summary>
        int ZeroPlayer = 999999999;
		/// <summary>
		/// 更新游戏在几个人里出一次奖.
		/// </summary>
		internal void UpdateMaxPlayer(int baoJiangLv)
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
            //SSDebug.LogWarning("UpdateMaxPlayer -> maxPlayer ==== " + maxPlayer + ", baoJiangLv ==== " + baoJiangLv);
		}

		internal bool AddCurrentNum()
		{
            bool isReset = false;
			currentNum++;
			if (IsHaveJiBaoNpc == true)
			{
				if (currentNum > maxPlayer)
				{
                    isReset = true;
					ResetCurrentNum();
				}
			}
            return isReset;
		}

		void ResetCurrentNum()
		{
			IsHaveJiBaoNpc = false;
            if (currentNum > maxPlayer)
            {
                currentNum = currentNum - maxPlayer;
            }
            else
            {
                currentNum = 1;
            }
		}

		/// <summary>
		/// 获取是否可以击爆Npc.
		/// </summary>
		internal bool GetIsCanJiBaoNpc()
        {
            if (XkGameCtrl.GetInstance().m_GamePlayerAiData.IsActiveAiPlayer == true)
            {
                //没有玩家激活游戏时,Ai都是可以击爆JPBoss和战车的.
                return true;
            }

            if (IsHaveJiBaoNpc == true)
			{
				//已经有玩家击爆过npc,其余玩家不能在击爆npc了.
				return false;
			}

            if (ZeroPlayer == maxPlayer)
            {
                //游戏后台配置爆奖率为0时不允许爆奖.
                return false;
            }

			if (currentNum >= maxPlayer)
			{
				//当前玩家人数已经积累的足够多了,新来的玩家都可以击爆npc.
				return true;
			}

			bool isCanJiBao = true;
			int randNum = (UnityEngine.Random.Range(0, 1000) % 100) + 1;
            //该玩家在游戏中的爆奖概率 = 累计人头数×该奖池爆奖率.(累计人头数 <= 最大人头数)
            int baoJiangGaiLv = currentNum * m_BaoJiangLv;
            if (randNum > baoJiangGaiLv)
			{
				isCanJiBao = false;
			}
			return isCanJiBao;
		}

        /// <summary>
        /// 获取人数是否已经积累足够.
        /// </summary>
        internal bool GetIsEnoughPlayerNum()
        {
            if (ZeroPlayer == maxPlayer)
            {
                //游戏后台配置爆奖率为0时不允许爆奖.
                return false;
            }

            if (currentNum >= maxPlayer)
            {
                //当前玩家人数已经积累的足够多了,新来的玩家都可以击爆npc.
                return true;
            }
            //人数不够.
            return false;
        }
	}
	/// <summary>
	/// JPBoss爆奖数据控制.
	/// </summary>
	public BaoJiangData m_BaoJiangDtJPBoss = new BaoJiangData();
	/// <summary>
	/// 战车01爆奖数据控制.
	/// </summary>
	public BaoJiangData m_BaoJiangDtZhanChe01 = new BaoJiangData();
	/// <summary>
	/// 战车02爆奖数据控制.
	/// </summary>
	public BaoJiangData m_BaoJiangDtZhanChe02 = new BaoJiangData();
    /// <summary>
    /// 随即道具爆奖数据控制.
    /// </summary>
    public BaoJiangData m_BaoJiangDtSuiJiDaoJu = new BaoJiangData();
    static SSHaiDiLaoBaoJiang _Instance = null;
    public static SSHaiDiLaoBaoJiang GetInstance()
    {
        return _Instance;
    }

    private void Awake()
    {
        _Instance = this;
        Init();
    }

    void Init()
    {
        int num = 0;
        bool isHaveBaoJiang = false;
        if (m_BaoJiangDtJPBoss != null)
        {
            num = GetCurrentNumPlayer(SSCaiPiaoDataManage.GameCaiPiaoData.DaiJinQuanState.JPBossDaiJinQuan);
            m_BaoJiangDtJPBoss.SetCurrentNum(num);

            isHaveBaoJiang = GetIsHaveBaoJiang(SSCaiPiaoDataManage.GameCaiPiaoData.DaiJinQuanState.JPBossDaiJinQuan);
            m_BaoJiangDtJPBoss.SetIsHaveJiBaoNpc(isHaveBaoJiang);
        }

        if (m_BaoJiangDtZhanChe01 != null)
        {
            num = GetCurrentNumPlayer(SSCaiPiaoDataManage.GameCaiPiaoData.DaiJinQuanState.ZhanCheDaiJinQuan_01);
            m_BaoJiangDtZhanChe01.SetCurrentNum(num);

            isHaveBaoJiang = GetIsHaveBaoJiang(SSCaiPiaoDataManage.GameCaiPiaoData.DaiJinQuanState.ZhanCheDaiJinQuan_01);
            m_BaoJiangDtZhanChe01.SetIsHaveJiBaoNpc(isHaveBaoJiang);
        }

        if (m_BaoJiangDtZhanChe02 != null)
        {
            num = GetCurrentNumPlayer(SSCaiPiaoDataManage.GameCaiPiaoData.DaiJinQuanState.ZhanCheDaiJinQuan_02);
            m_BaoJiangDtZhanChe02.SetCurrentNum(num);

            isHaveBaoJiang = GetIsHaveBaoJiang(SSCaiPiaoDataManage.GameCaiPiaoData.DaiJinQuanState.ZhanCheDaiJinQuan_02);
            m_BaoJiangDtZhanChe02.SetIsHaveJiBaoNpc(isHaveBaoJiang);
        }

        if (m_BaoJiangDtSuiJiDaoJu != null)
        {
            num = GetCurrentNumPlayer(SSCaiPiaoDataManage.GameCaiPiaoData.DaiJinQuanState.SuiJiDaoJuDaiJinQuan);
            m_BaoJiangDtSuiJiDaoJu.SetCurrentNum(num);

            isHaveBaoJiang = GetIsHaveBaoJiang(SSCaiPiaoDataManage.GameCaiPiaoData.DaiJinQuanState.SuiJiDaoJuDaiJinQuan);
            m_BaoJiangDtSuiJiDaoJu.SetIsHaveJiBaoNpc(isHaveBaoJiang);
        }
    }

    /// <summary>
    /// 更新爆奖率数据信息.
    /// </summary>
	internal void UpdateBaoJiangDt(SSCaiPiaoDataManage.GameCaiPiaoData.DaiJinQuanState type, int baoJiangLv)
	{
		BaoJiangData baoJiangDt = null;
		switch (type)
        {
            case SSCaiPiaoDataManage.GameCaiPiaoData.DaiJinQuanState.JPBossDaiJinQuan:
                {
                    baoJiangDt = m_BaoJiangDtJPBoss;
                }
                break;
            case SSCaiPiaoDataManage.GameCaiPiaoData.DaiJinQuanState.ZhanCheDaiJinQuan_01:
                {
                    baoJiangDt = m_BaoJiangDtZhanChe01;
                }
                break;
            case SSCaiPiaoDataManage.GameCaiPiaoData.DaiJinQuanState.ZhanCheDaiJinQuan_02:
                {
                    baoJiangDt = m_BaoJiangDtZhanChe02;
                }
                break;
            case SSCaiPiaoDataManage.GameCaiPiaoData.DaiJinQuanState.SuiJiDaoJuDaiJinQuan:
                {
                    baoJiangDt = m_BaoJiangDtSuiJiDaoJu;
                }
                break;
        }

        if (baoJiangDt != null)
        {
            baoJiangDt.UpdateMaxPlayer(baoJiangLv);
        }
	}

    /// <summary>
    /// 设置是否已经击爆Npc.
    /// </summary>
    internal void SetIsHaveJiBaoNpc(bool isHaveJiBao, SSCaiPiaoDataManage.GameCaiPiaoData.DaiJinQuanState type)
    {
        if (XkGameCtrl.GetInstance().m_GamePlayerAiData.IsActiveAiPlayer == true)
        {
            //没有玩家激活游戏时不允许设置该属性.
            return;
        }

        BaoJiangData baoJiangDt = null;
        switch (type)
        {
            case SSCaiPiaoDataManage.GameCaiPiaoData.DaiJinQuanState.JPBossDaiJinQuan:
                {
                    baoJiangDt = m_BaoJiangDtJPBoss;
                }
                break;
            case SSCaiPiaoDataManage.GameCaiPiaoData.DaiJinQuanState.ZhanCheDaiJinQuan_01:
                {
                    baoJiangDt = m_BaoJiangDtZhanChe01;
                }
                break;
            case SSCaiPiaoDataManage.GameCaiPiaoData.DaiJinQuanState.ZhanCheDaiJinQuan_02:
                {
                    baoJiangDt = m_BaoJiangDtZhanChe02;
                }
                break;
            case SSCaiPiaoDataManage.GameCaiPiaoData.DaiJinQuanState.SuiJiDaoJuDaiJinQuan:
                {
                    baoJiangDt = m_BaoJiangDtSuiJiDaoJu;
                }
                break;
        }

        if (baoJiangDt != null)
        {
            SetIsHaveBaoJiang(type, isHaveJiBao);
            baoJiangDt.SetIsHaveJiBaoNpc(isHaveJiBao);
        }
    }

    /// <summary>
    /// 获取是否已经击爆了某种类型的奖品npc.
    /// </summary>
    internal bool GetIsHaveJiBaoNpc(SSCaiPiaoDataManage.GameCaiPiaoData.DaiJinQuanState type)
    {
        BaoJiangData baoJiangDt = null;
        switch (type)
        {
            case SSCaiPiaoDataManage.GameCaiPiaoData.DaiJinQuanState.JPBossDaiJinQuan:
                {
                    baoJiangDt = m_BaoJiangDtJPBoss;
                }
                break;
            case SSCaiPiaoDataManage.GameCaiPiaoData.DaiJinQuanState.ZhanCheDaiJinQuan_01:
                {
                    baoJiangDt = m_BaoJiangDtZhanChe01;
                }
                break;
            case SSCaiPiaoDataManage.GameCaiPiaoData.DaiJinQuanState.ZhanCheDaiJinQuan_02:
                {
                    baoJiangDt = m_BaoJiangDtZhanChe02;
                }
                break;
            case SSCaiPiaoDataManage.GameCaiPiaoData.DaiJinQuanState.SuiJiDaoJuDaiJinQuan:
                {
                    baoJiangDt = m_BaoJiangDtSuiJiDaoJu;
                }
                break;
        }

        bool isHaveJiBao = true;
        if (baoJiangDt != null)
        {
            isHaveJiBao = baoJiangDt.GetIsHaveJiBaoNpc();
        }
        return isHaveJiBao;
    }

    /// <summary>
    /// 获取是否可以击爆奖品Npc.
    /// </summary>
    internal bool GetIsCanJiBaoNpc(SSCaiPiaoDataManage.GameCaiPiaoData.DaiJinQuanState type)
    {
        BaoJiangData baoJiangDt = null;
        switch (type)
        {
            case SSCaiPiaoDataManage.GameCaiPiaoData.DaiJinQuanState.JPBossDaiJinQuan:
                {
                    baoJiangDt = m_BaoJiangDtJPBoss;
                }
                break;
            case SSCaiPiaoDataManage.GameCaiPiaoData.DaiJinQuanState.ZhanCheDaiJinQuan_01:
                {
                    baoJiangDt = m_BaoJiangDtZhanChe01;
                }
                break;
            case SSCaiPiaoDataManage.GameCaiPiaoData.DaiJinQuanState.ZhanCheDaiJinQuan_02:
                {
                    baoJiangDt = m_BaoJiangDtZhanChe02;
                }
                break;
            case SSCaiPiaoDataManage.GameCaiPiaoData.DaiJinQuanState.SuiJiDaoJuDaiJinQuan:
                {
                    baoJiangDt = m_BaoJiangDtSuiJiDaoJu;
                }
                break;
        }

        bool isCanJiBao = false;
        if (baoJiangDt != null)
        {
            isCanJiBao = baoJiangDt.GetIsCanJiBaoNpc();
        }
        return isCanJiBao;
    }

    /// <summary>
    /// 获取人数是否积累足够.
    /// </summary>
    internal bool GetIsEnoughPlayerNum(SSCaiPiaoDataManage.GameCaiPiaoData.DaiJinQuanState type)
    {
        BaoJiangData baoJiangDt = null;
        switch (type)
        {
            case SSCaiPiaoDataManage.GameCaiPiaoData.DaiJinQuanState.JPBossDaiJinQuan:
                {
                    baoJiangDt = m_BaoJiangDtJPBoss;
                }
                break;
            case SSCaiPiaoDataManage.GameCaiPiaoData.DaiJinQuanState.ZhanCheDaiJinQuan_01:
                {
                    baoJiangDt = m_BaoJiangDtZhanChe01;
                }
                break;
            case SSCaiPiaoDataManage.GameCaiPiaoData.DaiJinQuanState.ZhanCheDaiJinQuan_02:
                {
                    baoJiangDt = m_BaoJiangDtZhanChe02;
                }
                break;
            case SSCaiPiaoDataManage.GameCaiPiaoData.DaiJinQuanState.SuiJiDaoJuDaiJinQuan:
                {
                    baoJiangDt = m_BaoJiangDtSuiJiDaoJu;
                }
                break;
        }

        bool isEnough = false;
        if (baoJiangDt != null)
        {
            isEnough = baoJiangDt.GetIsEnoughPlayerNum();
        }
        return isEnough;
    }

    internal void AddPlayerNum()
    {
        bool isReset = false;
        if (m_BaoJiangDtJPBoss != null)
        {
            isReset = m_BaoJiangDtJPBoss.AddCurrentNum();
            SetCurrentNumPlayer(SSCaiPiaoDataManage.GameCaiPiaoData.DaiJinQuanState.JPBossDaiJinQuan, m_BaoJiangDtJPBoss.GetCurrentNum());
            if (isReset == true)
            {
                //重置击爆信息
                SetIsHaveBaoJiang(SSCaiPiaoDataManage.GameCaiPiaoData.DaiJinQuanState.JPBossDaiJinQuan, false);
            }
        }

        if (m_BaoJiangDtZhanChe01 != null)
        {
            isReset = m_BaoJiangDtZhanChe01.AddCurrentNum();
            SetCurrentNumPlayer(SSCaiPiaoDataManage.GameCaiPiaoData.DaiJinQuanState.ZhanCheDaiJinQuan_01, m_BaoJiangDtZhanChe01.GetCurrentNum());
            if (isReset == true)
            {
                //重置击爆信息
                SetIsHaveBaoJiang(SSCaiPiaoDataManage.GameCaiPiaoData.DaiJinQuanState.ZhanCheDaiJinQuan_01, false);
            }
        }

        if (m_BaoJiangDtZhanChe02 != null)
        {
            isReset = m_BaoJiangDtZhanChe02.AddCurrentNum();
            SetCurrentNumPlayer(SSCaiPiaoDataManage.GameCaiPiaoData.DaiJinQuanState.ZhanCheDaiJinQuan_02, m_BaoJiangDtZhanChe02.GetCurrentNum());
            if (isReset == true)
            {
                //重置击爆信息
                SetIsHaveBaoJiang(SSCaiPiaoDataManage.GameCaiPiaoData.DaiJinQuanState.ZhanCheDaiJinQuan_02, false);
            }
        }

        if (m_BaoJiangDtSuiJiDaoJu != null)
        {
            isReset = m_BaoJiangDtSuiJiDaoJu.AddCurrentNum();
            SetCurrentNumPlayer(SSCaiPiaoDataManage.GameCaiPiaoData.DaiJinQuanState.SuiJiDaoJuDaiJinQuan, m_BaoJiangDtSuiJiDaoJu.GetCurrentNum());
            if (isReset == true)
            {
                //重置击爆信息
                SetIsHaveBaoJiang(SSCaiPiaoDataManage.GameCaiPiaoData.DaiJinQuanState.SuiJiDaoJuDaiJinQuan, false);
            }
        }
    }

    #region 爆奖数据的读写操作
    /// <summary>
    /// 爆奖信息.
    /// </summary>
    string FileName = "../config/BaoJiangConfig.xml";
    /// <summary>
    /// 获取当前奖池已经有几个玩家了.
    /// </summary>
    int GetCurrentNumPlayer(SSCaiPiaoDataManage.GameCaiPiaoData.DaiJinQuanState type)
    {
        int num = 0;
        string ele = "BaoJiangDt";
        string info = ReadFromFileXml(FileName, ele, type.ToString());
        if (info != null && info != "")
        {
            num = Convert.ToInt32(info);
        }
        else
        {
            WriteToFileXml(FileName, ele, type.ToString(), num.ToString());
        }
        return num;
    }

    /// <summary>
    /// 设置当前奖池已经有几个玩家了.
    /// </summary>
    void SetCurrentNumPlayer(SSCaiPiaoDataManage.GameCaiPiaoData.DaiJinQuanState type, int num)
    {
        WriteToFileXml(FileName, "BaoJiangDt", type.ToString(), num.ToString());
    }

    /// <summary>
    /// 获取当前奖池是否已经爆奖.
    /// </summary>
    bool GetIsHaveBaoJiang(SSCaiPiaoDataManage.GameCaiPiaoData.DaiJinQuanState type)
    {
        int num = 0;
        string ele = "BaoJiangDt";
        string attribute = "IsBaoJiang" + type.ToString();
        string info = ReadFromFileXml(FileName, ele, attribute);
        if (info != null && info != "")
        {
            num = Convert.ToInt32(info);
        }
        else
        {
            WriteToFileXml(FileName, ele, attribute, num.ToString());
        }
        return num == 0 ? false : true;
    }

    /// <summary>
    /// 设置当前奖池是否已经爆奖.
    /// </summary>
    void SetIsHaveBaoJiang(SSCaiPiaoDataManage.GameCaiPiaoData.DaiJinQuanState type, bool isHaveBaoJiang)
    {
        int num = isHaveBaoJiang == true ? 1 : 0;
        string ele = "BaoJiangDt";
        string attribute = "IsBaoJiang" + type.ToString();
        WriteToFileXml(FileName, ele, attribute, num.ToString());
    }

    /// <summary>
    /// 创建商户数据信息.
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
