using UnityEngine;

public class SSCaiPiaoDaJiang : SSGameMono
{
    /// <summary>
    /// 大奖数字信息控制组件.
    /// </summary>
    public SSGameNumUI m_GameNumUI;
    public UITexture m_PlayerIndexUI;
    public Texture[] m_PlayerIndexUIArray = new Texture[3];
    internal void ShowDaJiangCaiPiaoNum(PlayerEnum indexPlayer, int num)
    {
        if (m_GameNumUI != null)
        {
            m_GameNumUI.ShowNumUI(num);
        }
        
        int index = (int)indexPlayer - 1;
        if (index >= 0 && index <= 2)
        {
            if (m_PlayerIndexUI != null && m_PlayerIndexUIArray.Length > index)
            {
                //设置中大奖玩家的UI索引信息.
                m_PlayerIndexUI.mainTexture = m_PlayerIndexUIArray[index];
            }
        }
    }
}