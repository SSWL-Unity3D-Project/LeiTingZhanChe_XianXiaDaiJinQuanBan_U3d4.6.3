using UnityEngine;

public class SSChouJiangTiShiManage : SSGameMono
{
    static SSChouJiangTiShiManage _Instance;
    public static SSChouJiangTiShiManage GetInstance()
    {
        return _Instance;
    }
    // Use this for initialization
    void Awake()
    {
        _Instance = this;
    }

    /// <summary>
    /// 创建抽奖提示UI.
    /// </summary>
    SSChouJiangTiShi CreateChouJiangTiShi()
    {
        SSChouJiangTiShi tiShi = null;
        string prefabPath = "Prefabs/GUI/PingJiChouJiang/ChouJiangTiShi";
        GameObject gmDataPrefab = (GameObject)Resources.Load(prefabPath);
        if (gmDataPrefab != null)
        {
            GameObject obj = (GameObject)Instantiate(gmDataPrefab, transform);
            tiShi = obj.GetComponent<SSChouJiangTiShi>();
        }
        else
        {
            SSDebug.LogWarning("GetChouJiangTiShi -> gmDataPrefab was null, prefabPath == " + prefabPath);
        }
        return tiShi;
    }

    /// <summary>
    /// 显示抽奖提示UI.
    /// </summary>
    internal void ShowChouJiangTiShi(PlayerEnum indexVal)
    {
        SSChouJiangTiShi tiShiCom = CreateChouJiangTiShi();
        if (tiShiCom == null)
        {
            return;
        }
        Transform playerTr = XKPlayerMoveCtrl.GetInstance(indexVal).PiaoFenPoint;
        Vector3 startPos = XkGameCtrl.GetInstance().GetWorldObjToScreenPos(playerTr.position);
        tiShiCom.ShowChouJiangTiShi(startPos);
    }
}
