using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 动态下载玩家微信头像的控制脚本.
/// </summary>
public class AsyncImageDownload : MonoBehaviour
{
    /// <summary>
    /// 玩家微信头像数据.
    /// </summary>
    public class PlayerWXDt
    {
        /// <summary>
        /// 游戏中4个玩家的微信头像资源.
        /// </summary>
        public Texture2D playerWXHead;
        /// <summary>
        /// 玩家微信头像的Url.
        /// </summary>
        public string url;
        public PlayerWXDt(string url, Texture2D playerWXHead)
        {
            this.url = url;
            this.playerWXHead = playerWXHead;
        }
    }
    /// <summary>
    /// 玩家微信头像数据列表.
    /// </summary>
    List<PlayerWXDt> PlayerWXList = new List<PlayerWXDt>();
    /// <summary>
    /// 查找玩家微信数据.
    /// </summary>
    PlayerWXDt FindPlayerWXDt(string url)
    {
        PlayerWXDt playerDt = PlayerWXList.Find((dt) => {
            return dt.url.Equals(url);
        });
        return playerDt;
    }

    /// <summary>
    /// 添加玩家微信数据.
    /// </summary>
    void AddPlayerWXDt(PlayerWXDt playerDt)
    {
        if (playerDt != null && FindPlayerWXDt(playerDt.url) == null)
        {
            //SSDebug.LogWarning("AddPlayerWXDt -> headUrl == " + playerDt.url);
            PlayerWXList.Add(playerDt);
        }

        if (PlayerWXList != null && PlayerWXList.Count > 6)
        {
            //当玩家微信头像数据个数缓存大于一定数值后,清理第一条数据.
            RemovePlayerWXDt(0);
        }
    }

    /// <summary>
    /// 删除玩家微信数据按照索引信息.
    /// </summary>
    internal void RemovePlayerWXDt(int indexVal)
    {
        if (indexVal >= 0 && indexVal < PlayerWXList.Count)
        {
            if (PlayerWXList != null)
            {
                PlayerWXList.RemoveAt(indexVal);
            }
        }
    }

    /// <summary>
    /// 下载玩家微信头像.
    /// </summary>
    public void LoadPlayerHeadImg(string url, UITexture image)
    {
        if (url != null && url != "" && url.Length > 5)
        {
            //SSDebug.LogWarning("LoadPlayerHeadImg::UITexture -> url == " + url);
            PlayerWXDt playerDt = FindPlayerWXDt(url);
            if (playerDt != null)
            {
                //找到玩家微信数据.
                image.mainTexture = playerDt.playerWXHead;
                //SSDebug.LogWarning("LoadPlayerHeadImg::UITexture -> find headImg from list");
            }
            else
            {
                //没有找到玩家微信数据.
                StartCoroutine(DownloadImage(url, image));
            }
        }
    }

    /// <summary>
    /// 下载玩家微信头像.
    /// </summary>
    IEnumerator DownloadImage(string url, UITexture image)
    {
        Texture2D tex2d = null;
        //Debug.Log("Unity:"+"downloading new image:" + url.GetHashCode());//url转换HD5作为名字.
        WWW www = null;
        try
        {
            www = new WWW(url);
        }
        catch (System.Exception)
        {
        }
        yield return www;

        try
        {
            if (www != null)
            {
                if (www.error == null)
                {
                    tex2d = www.texture;
                }
                else
                {
                    if (www.error.Contains("404") == false)
                    {
                        tex2d = www.texture;
                    }
                    else
                    {
                        SSDebug.LogWarning("DownloadImage -> www.error ===== " + www.error);
                    }
                }
            }
            else
            {
                SSDebug.LogWarning("DownloadImage -> www was null");
            }
        }
        catch (System.Exception ex)
        {
            SSDebug.LogWarning("DownloadImage -> ex ======= " + ex);
        }

        if (tex2d != null && image != null)
        {
            //Debug.Log("Unity: DownloadImage...");
            image.mainTexture = tex2d;
            //添加玩家微信数据.
            AddPlayerWXDt(new PlayerWXDt(url, tex2d));
        }
    }
    
    /// <summary>
    /// 下载玩家微信头像.
    /// </summary>
    public void LoadPlayerHeadImg(string url, Material image)
    {
        if (url != null && url != "" && url.Length > 5)
        {
            //SSDebug.LogWarning("LoadPlayerHeadImg::Material -> url == " + url);
            PlayerWXDt playerDt = FindPlayerWXDt(url);
            if (playerDt != null)
            {
                //找到玩家微信数据.
                image.mainTexture = playerDt.playerWXHead;
                //SSDebug.LogWarning("LoadPlayerHeadImg::Material -> find headImg from list");
            }
            else
            {
                //没有找到玩家微信数据.
                StartCoroutine(DownloadImage(url, image));
            }
        }
    }

    /// <summary>
    /// 下载玩家微信头像.
    /// </summary>
    IEnumerator DownloadImage(string url, Material image)
    {
        Texture2D tex2d = null;
        //Debug.Log("Unity:"+"downloading new image:" + url.GetHashCode());//url转换HD5作为名字.
        WWW www = null;
        try
        {
            www = new WWW(url);
        }
        catch (System.Exception)
        {
        }
        yield return www;

        try
        {
            if (www != null)
            {
                if (www.error == null)
                {
                    tex2d = www.texture;
                }
                else
                {
                    if (www.error.Contains("404") == false)
                    {
                        tex2d = www.texture;
                    }
                    else
                    {
                        SSDebug.LogWarning("DownloadImage -> www.error ===== " + www.error);
                    }
                }
            }
            else
            {
                SSDebug.LogWarning("DownloadImage -> www was null");
            }
        }
        catch (System.Exception ex)
        {
            SSDebug.LogWarning("DownloadImage -> ex ======= " + ex);
        }

        if (tex2d != null && image != null)
        {
            //Debug.Log("Unity: DownloadImage...");
            image.mainTexture = tex2d;
            //添加玩家微信数据.
            AddPlayerWXDt(new PlayerWXDt(url, tex2d));
        }
    }
}
