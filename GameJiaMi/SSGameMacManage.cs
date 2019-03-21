using UnityEngine;
using System.Net.NetworkInformation;
using System;

public class SSGameMacManage : MonoBehaviour
{
	// Use this for initialization
	void Init()
    {
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
                SSDebug.LogWarning("not find local area connection!");
            }
        }
        catch (Exception ex)
        {
            SSDebug.LogWarning("Mac get error! ex == " + ex);
        }
#endif

        if (boxNum != defaultPcMac)
        {
            //开始进行电脑Mac地址信息校验.
        }
    }
}
