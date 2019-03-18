using System;
using System.IO;
using System.Net.Mail;
using System.Net.Mime;
using System.Xml;
using UnityEngine;

namespace XKGame.Script.GameEmail
{
    public class SSGameEmail
    {
        public class MyEmail
        {
            private MailMessage mMailMessage;   //主要处理发送邮件的内容（如：收发人地址、标题、主体、图片等等）
            private SmtpClient mSmtpClient; //主要处理用smtp方式发送此邮件的配置信息（如：邮件服务器、发送端口号、验证方式等等）
            private int mSenderPort;   //发送邮件所用的端口号（htmp协议默认为25）
            private string mSenderServerHost;    //发件箱的邮件服务器地址（IP形式或字符串形式均可）
            private string mSenderPassword;    //发件箱的密码
            private string mSenderUsername;   //发件箱的用户名（即@符号前面的字符串，例如：hello@163.com，用户名为：hello）
            private bool mEnableSsl;    //是否对邮件内容进行socket层加密传输
            private bool mEnablePwdAuthentication;  //是否对发件人邮箱进行密码验证

            ///<summary>
            /// 构造函数
            ///</summary>
            ///<param name="server">发件箱的邮件服务器地址</param>
            ///<param name="toMail">收件人地址（可以是多个收件人，程序中是以“;"进行区分的）</param>
            ///<param name="fromMail">发件人地址</param>
            ///<param name="subject">邮件标题</param>
            ///<param name="emailBody">邮件内容（可以以html格式进行设计）</param>
            ///<param name="username">发件箱的用户名（即@符号前面的字符串，例如：hello@163.com，用户名为：hello）</param>
            ///<param name="password">发件人邮箱密码</param>
            ///<param name="port">发送邮件所用的端口号（htmp协议默认为25）</param>
            ///<param name="sslEnable">true表示对邮件内容进行socket层加密传输，false表示不加密</param>
            ///<param name="pwdCheckEnable">true表示对发件人邮箱进行密码验证，false表示不对发件人邮箱进行密码验证</param>
            public MyEmail(string server, string toMail, string fromMail, string subject, string emailBody, string username, string password, string port, bool sslEnable, bool pwdCheckEnable)
            {
                try
                {
                    mMailMessage = new MailMessage();
                    mMailMessage.To.Add(toMail);
                    mMailMessage.From = new MailAddress(fromMail);
                    mMailMessage.Subject = subject;
                    mMailMessage.Body = emailBody;
                    mMailMessage.IsBodyHtml = true;
                    mMailMessage.BodyEncoding = System.Text.Encoding.UTF8;
                    mMailMessage.Priority = MailPriority.Normal;
                    this.mSenderServerHost = server;
                    this.mSenderUsername = username;
                    this.mSenderPassword = password;
                    this.mSenderPort = Convert.ToInt32(port);
                    this.mEnableSsl = sslEnable;
                    this.mEnablePwdAuthentication = pwdCheckEnable;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }

            ///<summary>
            /// 添加附件
            ///</summary>
            ///<param name="attachmentsPath">附件的路径集合，以分号分隔</param>
            public void AddAttachments(string attachmentsPath)
            {
                try
                {
                    string[] path = attachmentsPath.Split(';'); //以什么符号分隔可以自定义
                    Attachment data;
                    ContentDisposition disposition;
                    for (int i = 0; i < path.Length; i++)
                    {
                        data = new Attachment(path[i], MediaTypeNames.Application.Octet);
                        disposition = data.ContentDisposition;
                        disposition.CreationDate = File.GetCreationTime(path[i]);
                        disposition.ModificationDate = File.GetLastWriteTime(path[i]);
                        disposition.ReadDate = File.GetLastAccessTime(path[i]);
                        mMailMessage.Attachments.Add(data);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }

            ///<summary>
            /// 邮件的发送
            ///</summary>
            public void Send()
            {
                try
                {
                    if (mMailMessage != null)
                    {
                        mSmtpClient = new SmtpClient();
                        //mSmtpClient.Host = "smtp." + mMailMessage.From.Host;
                        mSmtpClient.Host = this.mSenderServerHost;
                        mSmtpClient.Port = this.mSenderPort;
                        mSmtpClient.UseDefaultCredentials = false;
                        mSmtpClient.EnableSsl = this.mEnableSsl;
                        if (this.mEnablePwdAuthentication)
                        {
                            System.Net.NetworkCredential nc = new System.Net.NetworkCredential(this.mSenderUsername, this.mSenderPassword);
                            //mSmtpClient.Credentials = new System.Net.NetworkCredential(this.mSenderUsername, this.mSenderPassword);
                            //NTLM: Secure Password Authentication in Microsoft Outlook Express
                            mSmtpClient.Credentials = nc.GetCredential(mSmtpClient.Host, mSmtpClient.Port, "NTLM");
                        }
                        else
                        {
                            mSmtpClient.Credentials = new System.Net.NetworkCredential(this.mSenderUsername, this.mSenderPassword);
                        }
                        mSmtpClient.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
                        mSmtpClient.Send(mMailMessage);
                    }
                }
                catch (Exception ex)
                {
                    SSDebug.LogWarning(ex.ToString());
                }
            }
        }
        
        string m_FileName = "GameEmailInfo.db";
        /// <summary>
        /// 创建配置文件.
        /// </summary>
        void CreatGamePlayerData(string filepath)
        {
            XmlDocument xmlDoc = new XmlDocument();
            XmlElement root = xmlDoc.CreateElement("ConfigData");
            XmlElement elmNew = xmlDoc.CreateElement("Config");
            root.AppendChild(elmNew);
            xmlDoc.AppendChild(root);
            xmlDoc.Save(filepath);
            File.SetAttributes(filepath, FileAttributes.Normal);
        }

        /// <summary>
        /// 从配置文件读取游戏发送邮件的时间信息.
        /// </summary>
        string ReadGameSendEmailTime()
        {
            string filepath = Application.dataPath + "/" + m_FileName;
#if UNITY_ANDROID
		    filepath = Application.persistentDataPath + "//" + m_FileName;
#endif
            //create file
            if (!File.Exists(filepath))
            {
                CreatGamePlayerData(filepath);
            }

            string time = "";
            if (File.Exists(filepath))
            {
                try
                {
                    string elementName = "Config";
                    string attribute1 = "TimeVal";
                    string valueStr1 = "";
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.Load(filepath);
                    XmlNodeList nodeList = xmlDoc.SelectSingleNode("ConfigData").ChildNodes;
                    foreach (XmlElement xe in nodeList)
                    {
                        if (xe.Name == elementName)
                        {
                            valueStr1 = xe.GetAttribute(attribute1);
                            if (valueStr1 != null && valueStr1 != "")
                            {
                                time = valueStr1;
                                //SSDebug.Log("ReadGamePlayerData -> userId == " + valueStr1 + ", timeVal == " + valueStr2);
                                //AddFreePlayGamePlayerInfo(System.Convert.ToInt32(valueStr1), System.Convert.ToDateTime(valueStr1));
                            }
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
            return time;
        }

        /// <summary>
        /// 向配置文件写入游戏发送邮件的时间信息.
        /// </summary>
        void WriteGameSendEmailTime(DateTime time)
        {
            string filepath = Application.dataPath + "/" + m_FileName;
#if UNITY_ANDROID
		    filepath = Application.persistentDataPath + "//" + m_FileName;
#endif

            //create file
            if (!File.Exists(filepath))
            {
                CreatGamePlayerData(filepath);
            }

            //update value
            if (File.Exists(filepath))
            {
                try
                {
                    string elementName = "Config";
                    string attribute1 = "TimeVal";
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.Load(filepath);
                    XmlNodeList nodeList = xmlDoc.SelectSingleNode("ConfigData").ChildNodes;
                    foreach (XmlElement xe in nodeList)
                    {
                        if (xe.Name == elementName)
                        {
                            xe.SetAttribute(attribute1, time.ToString("G"));
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
        }

        /// <summary>
        /// 发送邮件.
        /// </summary>
        internal void SendGameEmail(string msg)
        {
            string time = ReadGameSendEmailTime();
            if (time == null || time == "")
            {
            }
            else
            {
                DateTime timeNow = DateTime.Now;
                DateTime timeRecord = Convert.ToDateTime(time);

                TimeSpan ts1 = new TimeSpan(timeNow.Ticks);
                TimeSpan ts2 = new TimeSpan(timeRecord.Ticks);
                TimeSpan ts = ts2.Subtract(ts1).Duration();

                int dTime = ts.Hours * 3600 + ts.Minutes * 60 + ts.Seconds; //秒.
                int minTime = 10 * 60 * 60; //秒.
                if (dTime > minTime)
                {
                    //间隔时间大于可以发送邮件的间隔时间.
                }
                else
                {
                    //间隔时间小于可以发送邮件的间隔时间.
                    //不允许发送邮件.
                    //SSDebug.LogWarning("SendGameEmail -> time buZu!");
                    return;
                }
            }
            //将发送游戏的时间信息保存到配置文件.
            WriteGameSendEmailTime(DateTime.Now);

            ThreadSendOpenGameMsgToEmail threadSendEmail = new ThreadSendOpenGameMsgToEmail(msg);
            if (threadSendEmail != null)
            {
                System.Threading.Thread threadEmail = new System.Threading.Thread(new System.Threading.ThreadStart(threadSendEmail.Run));
                threadEmail.Start();
            }
        }
        
        /// <summary>
        /// 通过线程发送游戏登录信息到邮箱.
        /// </summary>
        public class ThreadSendOpenGameMsgToEmail
        {
            string m_Msg = "";
            public ThreadSendOpenGameMsgToEmail(string msg)
            {
                m_Msg = msg;
            }

            ~ThreadSendOpenGameMsgToEmail()
            {
                //SSDebug.Log("~ThreadSendOpenGameMsgToEmail -> destory this thread*********************************");
            }

            internal void Run()
            {
                try
                {
                    //smtp.163.com
                    string senderServerIp = "123.125.50.133";
                    //smtp.gmail.com
                    //string senderServerIp = "74.125.127.109";
                    //smtp.qq.com
                    //string senderServerIp = "58.251.149.147";
                    //string senderServerIp = "smtp.sina.com";
                    //string toMailAddress = "mingmingruyuedlut@163.com";
                    //string fromMailAddress = "mingmingruyuedlut@163.com";
                    string toMailAddress = "shenyongqiang2008@126.com";
                    string fromMailAddress = "shenyongqiang2008@163.com";
                    string subjectInfo = "LeiTingZhanChe_" + SSGameLogoData.m_GameVersionState + " sending e_mail";
                    string bodyInfo = m_Msg;
                    string mailUsername = "shenyongqiang2008";
                    double num = Math.Pow(2, 10) + Math.Pow(2, 9) + Math.Pow(2, 8) + Math.Pow(2, 7) + 66;
                    string tmp = "shen";
                    string mailPassword = num.ToString() + tmp; //发送邮箱的密码（）
                    string mailPort = "25";
                    MyEmail email = new MyEmail(senderServerIp, toMailAddress, fromMailAddress, subjectInfo, bodyInfo, mailUsername, mailPassword, mailPort, false, false);
                    //string attachPath = "E:\\123123.txt; E:\\haha.pdf";
                    //email.AddAttachments(attachPath);
                    email.Send();
                }
                catch (Exception ex)
                {
                    SSDebug.LogWarning(ex.ToString());
                }
            }
        }
    }
}
