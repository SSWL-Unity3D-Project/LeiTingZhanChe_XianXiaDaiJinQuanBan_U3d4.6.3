using UnityEngine;

public class SSUIRoot : SSGameMono
{
    static SSUIRoot _Instance;
    public static SSUIRoot GetInstance()
    {
        if (_Instance == null)
        {
            GameObject obj = new GameObject("_SSUIRoot");
            _Instance = obj.AddComponent<SSUIRoot>();
        }
        return _Instance;
    }

    /// <summary>
    /// 游戏UI管理组件.
    /// </summary>
    internal SSGameUICtrl m_GameUIManage;
    /// <summary>
    /// 退出游戏UI界面控制脚本.
    /// </summary>
    internal SSExitGameUI m_ExitUICom;
    
    /// <summary>
    /// 获取是否有玩家显示了彩票不足的信息.
    /// </summary>
    internal bool GetIsActiveCaiPiaoBuZuPanel(PlayerEnum indexPlayer)
    {
        int index = (int)indexPlayer - 1;
        if (index < 0 || index > 2)
        {
            UnityLogWarning("GetIsActiveCaiPiaoBuZuPanel -> index was wrong! index == " + index);
            return false;
        }

        if (m_GameUIManage != null)
        {
            if (m_GameUIManage.m_CaiPiaoBuZuArray.Length <= index)
            {
                UnityLogWarning("GetIsActiveCaiPiaoBuZuPanel -> m_CaiPiaoBuZuArray.Lenth was wrong! index == " + index);
                return false;
            }
            return m_GameUIManage.m_CaiPiaoBuZuArray[index] == null ? false : true;
        }
        return false;
    }
}