using UnityEngine;

public class SSGameLogoData : MonoBehaviour {

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
        XKGlobalData.GetInstance().m_GameLogo = m_GameLogo;
    }
}
