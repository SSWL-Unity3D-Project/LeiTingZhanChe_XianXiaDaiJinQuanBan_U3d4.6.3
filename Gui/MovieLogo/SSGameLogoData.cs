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
    public GameDaiJinQuanMode m_DaiJinQuanMode = GameDaiJinQuanMode.HDL_CaiPinQuan;
    /// <summary>
    /// 游戏代金券模式.
    /// </summary>
    public static GameDaiJinQuanMode m_GameDaiJinQuanMode = GameDaiJinQuanMode.HDL_CaiPinQuan;

    public enum GameLogo
    {
        /// <summary>
        /// 默认Logo
        /// </summary>
        Default = 0,
        /// <summary>
        /// 海底捞火锅
        /// </summary>
        HaiDiLao = 1,
    }
    public GameLogo m_GameLogo = GameLogo.Default;

    private void Awake()
    {
        m_GameDaiJinQuanMode = m_DaiJinQuanMode;
        XKGlobalData.GetInstance().m_GameLogo = m_GameLogo;
    }
}
