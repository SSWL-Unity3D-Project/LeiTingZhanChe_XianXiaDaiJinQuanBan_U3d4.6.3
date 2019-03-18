using UnityEngine;

public class SSGameLogoData : MonoBehaviour
{
    public enum GameDaiJinQuanMode
    {
        /// <summary>
        /// 代金券游戏UI模式.
        /// </summary>
        DaiJinQuan = 0,
        /// <summary>
        /// 海底捞菜品券游戏UI模式.
        /// 当发放游戏代金券信息时,代金券面值信息写为0.
        /// </summary>
        HDL_CaiPinQuan = 1,
    }
    /// <summary>
    /// 游戏代金券模式.
    /// </summary>
    public static GameDaiJinQuanMode m_GameDaiJinQuanMode = GameDaiJinQuanMode.HDL_CaiPinQuan;

    public enum GameLogo
    {
        /// <summary>
        /// 默认为公司Logo
        /// </summary>
        Default = 0,
        /// <summary>
        /// 海底捞火锅Logo
        /// </summary>
        HaiDiLao = 1,
    }
    /// <summary>
    /// 游戏展示的Logo图片数据.
    /// </summary>
    public GameLogo m_GameLogoInfo = GameLogo.HaiDiLao;
    /// <summary>
    /// 游戏Logo枚举.
    /// </summary>
    public static GameLogo m_GameLogo = GameLogo.Default;

    public enum GameVersionHddServer
    {
        /// <summary>
        /// 测试版.
        /// </summary>
        CeShiBan = 0,
        /// <summary>
        /// 正式版.
        /// </summary>
        ZhengShiBan = 1,
    }
    /// <summary>
    /// 游戏红点点服务器版本控制接口.
    /// </summary>
    public GameVersionHddServer m_GameVersionHddServer = GameVersionHddServer.CeShiBan;

    public enum GameVersionState
    {
        /// <summary>
        /// 海底捞版本游戏.
        /// </summary>
        HaiDiLao = 0,
        /// <summary>
        /// KTV智能桌面版本游戏.
        /// </summary>
        KTV = 1,
    }
    /// <summary>
    /// 游戏版本状态.
    /// </summary>
    public static GameVersionState m_GameVersionState = GameVersionState.HaiDiLao;

    private void Awake()
    {
        m_GameLogo = m_GameLogoInfo;
        XKGlobalData.m_GameVersionHddServer = m_GameVersionHddServer;
        switch (m_GameVersionHddServer)
        {
            case GameVersionHddServer.CeShiBan:
                {
                    //设置游戏为红点点测试服务器版本信息.
                    XKGameVersionCtrl.SetTestGameVersion();
                    break;
                }
            case GameVersionHddServer.ZhengShiBan:
                {
                    //设置红点点正式服务器版本信息.
                    XKGameVersionCtrl.SetReleaseGameVersion();
                    break;
                }
        }
    }
}
