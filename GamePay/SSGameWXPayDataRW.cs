using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;

namespace Assets.XKGame.Script.GamePay
{
    /// <summary>
    /// 游戏通过微信支付的营收和返券数据信息读写组件.
    /// </summary>
    public class SSGameWXPayDataRW
    {
        /// <summary>
        /// 数据保存路径信息
        /// </summary>
        private string m_FileName = "../config/GameWXPay.db";

        /// <summary>
        /// 写数据.
        /// </summary>
        public void WriteToFileXml(SSGameWXPayData payData)
        {
#if UNITY_STANDALONE_WIN
            string filePath = Application.dataPath + "/" + m_FileName;
#endif

#if UNITY_ANDROID
		    string filePath = Application.persistentDataPath + "//" + m_FileName;
#endif

            //create file
            if (!File.Exists(filePath))
            {
                XmlDocument xmlDoc = new XmlDocument();
                XmlElement root = xmlDoc.CreateElement("GamePayData");
                xmlDoc.AppendChild(root);
                xmlDoc.Save(filePath);
                File.SetAttributes(filePath, FileAttributes.Normal);
            }

            //update value
            if (File.Exists(filePath))
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(filePath);
                XmlNodeList nodeList = xmlDoc.SelectSingleNode("GamePayData").ChildNodes;

                string key01 = "Time";
                string val01 = payData.Time;
                string key02 = "Revenue";
                string val02 = payData.Revenue;
                string key03 = "Rebate";
                string val03 = payData.Rebate;
                if (nodeList.Count > 0)
                {
                    bool isAddNewData = true;
                    bool isRemoveData = false;
                    List<XmlNode> nodeListTmp = new List<XmlNode>();
                    foreach (XmlElement xe in nodeList)
                    {
                        if (xe.GetAttribute(key01) == payData.Time)
                        {
                            if (isAddNewData == true)
                            {
                                //修改信息
                                isAddNewData = false;
                                xe.SetAttribute(key02, val02);
                                xe.SetAttribute(key03, val03);
                            }
                            else
                            {
                                //删除信息.
                                isRemoveData = true;
                                nodeListTmp.Add(xe);
                            }
                        }
                    }

                    if (isRemoveData == true)
                    {
                        //删除冗余信息.
                        for (int i = 0; i < nodeListTmp.Count; i++)
                        {
                            xmlDoc.DocumentElement.RemoveChild(nodeListTmp[i]);
                        }
                        nodeListTmp.Clear();
                    }

                    if (isAddNewData == true)
                    {
                        //添加信息.
                        XmlElement root = xmlDoc.DocumentElement;
                        XmlElement elmNew = xmlDoc.CreateElement("PayData");
                        root.AppendChild(elmNew);
                        elmNew.SetAttribute(key01, val01);
                        elmNew.SetAttribute(key02, val02);
                        elmNew.SetAttribute(key03, val03);
                        xmlDoc.AppendChild(root);
                    }
                }
                else
                {
                    //添加信息.
                    XmlElement root = xmlDoc.DocumentElement;
                    XmlElement elmNew = xmlDoc.CreateElement("PayData");
                    root.AppendChild(elmNew);
                    elmNew.SetAttribute(key01, val01);
                    elmNew.SetAttribute(key02, val02);
                    elmNew.SetAttribute(key03, val03);
                    xmlDoc.AppendChild(root);
                }

                File.SetAttributes(filePath, FileAttributes.Normal);
                xmlDoc.Save(filePath);
                //UnityEngine.Debug.Log("WriteToFileXml -----------> Time: " + payData.Time
                //    + ", Revenue: " + payData.Revenue
                //    + ", Rebate: " + payData.Rebate);
            }
        }
        
        /// <summary>
        /// 读数据.
        /// </summary>
        internal SSGameWXPayData[] ReadFromFileXml()
        {
#if UNITY_STANDALONE_WIN
            string filePath = Application.dataPath + "/" + m_FileName;
#endif

#if UNITY_ANDROID
		    string filePath = Application.persistentDataPath + "//" + m_FileName;
#endif

            List<SSGameWXPayData> payDataList = new List<SSGameWXPayData>();
            if (File.Exists(filePath))
            {
                try
                {
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.Load(filePath);

                    string key01 = "Time";
                    string val01 = "";
                    string key02 = "Revenue";
                    string val02 = "";
                    string key03 = "Rebate";
                    string val03 = "";
                    SSGameWXPayData payData = null;
                    XmlNodeList nodeList = xmlDoc.SelectSingleNode("GamePayData").ChildNodes;
                    foreach (XmlElement xe in nodeList)
                    {
                        val01 = xe.GetAttribute(key01);
                        val02 = xe.GetAttribute(key02);
                        val03 = xe.GetAttribute(key03);
                        payData = new SSGameWXPayData();
                        payData.Time = val01;
                        payData.Revenue = val02;
                        payData.Rebate = val03;
                        if (payDataList.Contains(payData) == false)
                        {
                            payDataList.Add(payData);
                        }
                    }
                    File.SetAttributes(filePath, FileAttributes.Normal);
                    xmlDoc.Save(filePath);
                }
                catch (Exception exception)
                {
                    File.SetAttributes(filePath, FileAttributes.Normal);
                    File.Delete(filePath);
                    Debug.LogError("Unity:" + "error: xml was wrong! " + exception);
                    Debug.LogError("Unity:" + "error: filePath == " + filePath);
                }
            }

            payDataList.Reverse();
            SSGameWXPayData[] dataArray = payDataList.ToArray();
            payDataList.Clear();
            return dataArray;
        }
    }
}
