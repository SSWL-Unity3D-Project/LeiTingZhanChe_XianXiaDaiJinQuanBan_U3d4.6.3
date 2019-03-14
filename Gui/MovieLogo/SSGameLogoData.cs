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
    private void Awake()
    {
        m_GameLogo = m_GameLogoInfo;
    }
}
