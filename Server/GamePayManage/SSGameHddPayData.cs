using System;
using System.Xml;
using UnityEngine;

namespace Assets.XKGame.Script.Server.GamePayManage
{
    public class SSGameHddPayData : SSGameMono
    {
        public enum ShangHuInfo
        {
            /// <summary>
            /// 海底捞火锅商户.
            /// </summary>
            ShangHu1 = 1,
            ShangHu2 = 2,
            ShangHu3 = 3,
            ShangHu4 = 4,
            ShangHu5 = 5,
            ShangHu6 = 6,
            ShangHu7 = 7,
            ShangHu8 = 8,
            ShangHu9 = 9,
            ShangHu10 = 10,
        }
        /// <summary>
        /// 游戏商户信息配置.
        /// </summary>
        public ShangHuInfo m_ShangHuInfo = ShangHuInfo.ShangHu1;

        [Serializable]
        public class GameShangHuData
        {
            /// <summary>
            /// 商户枚举信息.
            /// </summary>
            public ShangHuInfo ShangHuEnum = ShangHuInfo.ShangHu1;
            /// <summary>
            /// 商户id.
            /// </summary>
            public string ShangHuId;
            /// <summary>
            /// 商户名称.
            /// </summary>
            public string ShangHuName;
            public GameShangHuData(ShangHuInfo info, string id, string name)
            {
                ShangHuEnum = info;
                ShangHuId = id;
                ShangHuName = name;
            }
            public GameShangHuData()
            {
            }
        }

        private void Start()
        {
            Init();
        }

        /// <summary>
        /// 初始化.
        /// </summary>
        internal void Init()
        {
            if (XKGlobalData.GetInstance().m_ShangHuDt == null)
            {
                int shangHuKey = (int)m_ShangHuInfo;
                GameShangHuData data = ReadShangHuConfigInfo(shangHuKey.ToString());
                if (data == null)
                {
                    data = new GameShangHuData(ShangHuInfo.ShangHu1, "888888", "海底捞火锅");
                    UnityLogWarning("not find game shangHu info!");
                }
                XKGlobalData.GetInstance().m_ShangHuDt = data;
            }
        }

        /// <summary>
        /// 读取游戏商家的配置信息.
        /// </summary>
        GameShangHuData ReadShangHuConfigInfo(string key)
        {
            TextAsset configAsset = (TextAsset)Resources.Load("GameConfig/GameShangHuData");
            string attribute = "ShangHuEnum";
            GameShangHuData config = null;

            if (configAsset != null)
            {
                try
                {
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.LoadXml(configAsset.text);
                    XmlNodeList nodeList = xmlDoc.SelectSingleNode("Config").ChildNodes;
                    foreach (XmlElement xe in nodeList)
                    {
                        if (xe.GetAttribute(attribute) == key)
                        {
                            config = new GameShangHuData();
                            config.ShangHuEnum = (ShangHuInfo)Convert.ToInt32(key);
                            config.ShangHuId = xe.GetAttribute("ShangHuId");
                            config.ShangHuName = xe.GetAttribute("ShangHuName");
                            //UnityLog("ShangHuEnum == " + key + ", ShangHuId == " + config.ShangHuId
                            //    + ", ShangHuName == " + config.ShangHuName);
                            break;
                        }
                    }
                }
                catch (Exception exception)
                {
                    UnityLogError("Unity:" + "error: xml was wrong! " + exception);
                }
            }
            else
            {
                UnityLogWarning("configAsset was null!!!!!!");
            }
            return config;
        }
    }
}
