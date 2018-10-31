public class SSGameHddSreenId : SSGameMono
{
    public SSGameNumUI m_GameNumUI;
    internal void Init(int screenId)
    {
        UnityLog("Init -> screenId =================== " + screenId);
        if (m_GameNumUI != null)
        {
            m_GameNumUI.ShowNumUI(screenId);
        }
    }
}
