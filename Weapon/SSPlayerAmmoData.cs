using UnityEngine;

public class SSPlayerAmmoData : SSGameMono
{
    /// <summary>
    /// 子弹真实展示对象.
    /// </summary>
    public GameObject[] AmmoCoreArray = new GameObject[3];
    internal GameObject GetAmmoCore(int index)
    {
        GameObject obj = null;
        if (index >= 0 && index < AmmoCoreArray.Length)
        {
            obj = AmmoCoreArray[index];
        }
        return obj;
    }

    internal void SetActiveAmmoCore(int index)
    {
        if (index >= 0 && index <= AmmoCoreArray.Length)
        {
            for (int i = 0; i < AmmoCoreArray.Length; i++)
            {
                if (AmmoCoreArray[i] != null)
                {
                    AmmoCoreArray[i].SetActive(index == i);
                }
            }
        }
    }
}
