using UnityEngine;

public class SSPlayerAmmoData : SSGameMono
{
    /// <summary>
    /// 子弹真实展示对象.
    /// </summary>
    public GameObject[] AmmoCoreArray = new GameObject[3];
    /// <summary>
    /// 玩家子弹数据信息列表.
    /// </summary>
    public SSPlayerAmmoCore[] AmmoCoreDtArray = new SSPlayerAmmoCore[3];
    internal GameObject GetAmmoCore(int index)
    {
        GameObject obj = null;
        if (index >= 0 && index < AmmoCoreArray.Length)
        {
            obj = AmmoCoreArray[index];
        }
        return obj;
    }

    /// <summary>
    /// 设置子弹不同等级的显示状态.
    /// </summary>
    internal void SetActiveAmmoCore(int index, PlayerEnum indexPlayer)
    {
        if (index >= 0 && index <= AmmoCoreArray.Length)
        {
            for (int i = 0; i < AmmoCoreArray.Length; i++)
            {
                if (AmmoCoreArray[i] != null)
                {
                    AmmoCoreArray[i].SetActive(index == i);
                    if (index == i)
                    {
                        //处理各个玩家的子弹.
                        if (AmmoCoreDtArray.Length > i && AmmoCoreDtArray[i] != null)
                        {
                            AmmoCoreDtArray[i].InitPlayerAmmo(indexPlayer);
                        }
                    }
                }
            }
        }
    }
}
