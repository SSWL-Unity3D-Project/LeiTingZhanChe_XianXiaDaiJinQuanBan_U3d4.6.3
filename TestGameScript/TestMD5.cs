using UnityEngine;
using System.Text;
using System.Security.Cryptography;
using System;
using System.IO;

public class TestMD5 : MonoBehaviour
{
	// Use this for initialization
	void Start ()
    {
        string text = "12345";
        string jiaMiText = Md5Sum(text);
        Debug.Log("text ==== " + text);
        Debug.Log("jiaMiText ==== " + jiaMiText);

        jiaMiText = Md5Encrypt(text);
        Debug.Log("text ==== " + text);
        Debug.Log("jiaMiText ==== " + jiaMiText);
        
        text = Md5Decrypt(jiaMiText);
        Debug.Log("jiaMiText ==== " + jiaMiText);
        Debug.Log("text ==== " + text);
    }

    /// <summary>
    /// MD5加密数据算法.
    /// </summary>
    public string Md5Sum(string strToEncrypt)
    {
        byte[] bs = UTF8Encoding.UTF8.GetBytes(strToEncrypt);
        System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5CryptoServiceProvider.Create();

        byte[] hashBytes = md5.ComputeHash(bs);

        string hashString = "";
        for (int i = 0; i < hashBytes.Length; i++)
        {
            hashString += System.Convert.ToString(hashBytes[i], 16).PadLeft(2, '0');
        }
        return hashString.PadLeft(32, '0');
    }

    #region MD5秘钥
    //建立加密对象的密钥和偏移量           
    static byte[] iv = { 102, 66, 93, 156, 78, 4, 253, 32 };//定义偏移量   
    static byte[] key = { 55, 36, 246, 128, 36, 99, 89, 3 };//定义密钥     
    #endregion

    #region MD5加密
    /// <summary>   
    /// MD5加密   
    /// </summary>   
    /// <param name="strSource">需要加密的字符串</param>   
    /// <returns>MD5加密后的字符串</returns>   
    public static string Md5Encrypt(string strSource)
    {
        //把字符串放到byte数组中   
        byte[] bytIn = System.Text.Encoding.Default.GetBytes(strSource);
        //实例DES加密类   
        DESCryptoServiceProvider mobjCryptoService = new DESCryptoServiceProvider();
        mobjCryptoService.Key = iv;
        mobjCryptoService.IV = key;
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
    public static string Md5Decrypt(string Source)
    {
        string val = "0";
        try
        {
            //将解密字符串转换成字节数组   
            byte[] bytIn = System.Convert.FromBase64String(Source);
            //给出解密的密钥和偏移量，密钥和偏移量必须与加密时的密钥和偏移量相同   
            DESCryptoServiceProvider mobjCryptoService = new DESCryptoServiceProvider();
            mobjCryptoService.Key = iv;
            mobjCryptoService.IV = key;
            //实例流进行解密   
            System.IO.MemoryStream ms = new System.IO.MemoryStream(bytIn, 0, bytIn.Length);
            ICryptoTransform encrypto = mobjCryptoService.CreateDecryptor();
            CryptoStream cs = new CryptoStream(ms, encrypto, CryptoStreamMode.Read);
            StreamReader strd = new StreamReader(cs, Encoding.Default);
            val = strd.ReadToEnd();
        }
        catch (Exception ex)
        {
            Debug.Log("Md5Decrypt -> ex == " + ex);
        }
        return val;
    }
    #endregion
}
