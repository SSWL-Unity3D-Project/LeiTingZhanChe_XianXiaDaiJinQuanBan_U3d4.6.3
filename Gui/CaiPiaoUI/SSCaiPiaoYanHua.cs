using UnityEngine;

/// <summary>
/// 彩票烟花组件.
/// </summary>
public class SSCaiPiaoYanHua : SSGameMono
{
    /// <summary>
    /// 最大时间.
    /// </summary>
    float m_MaxTime = 3f;
    float m_LastTime = 0f;
    /// <summary>
    /// 产生烟花的间隔时间.
    /// </summary>
    float m_TimeYanHua = 0.2f;
    float m_LastTimeYanHua = 0f;
    bool IsCreatYanHua = false;
    public void Init(float timeMax)
    {
        m_MaxTime = timeMax;
        m_LastTimeYanHua = m_LastTime = Time.time;
        IsCreatYanHua = true;
        CreatYanHuaLiZi();
    }

    void Update()
    {
        if (IsCreatYanHua == true)
        {
            if (Time.time - m_LastTime <= m_MaxTime)
            {
                if (Time.time - m_LastTimeYanHua >= m_TimeYanHua)
                {
                    CreatYanHuaLiZi();
                }
            }
            else
            {
                //结束烟花的产生.
                IsCreatYanHua = false;
                Destroy(this);
            }
        }
    }

    void CreatYanHuaLiZi()
    {
        int randVal = 0;
        Transform[] trPointArray = XKPlayerCamera.GetInstanceFeiJi().GetDaoJuSpawnPoint();
        GameObject[] liZiArray = XkGameCtrl.GetInstance().m_CaiPiaoFlyData.m_JPBossCaiPiaoFlyDt.m_LiZiPrefabArray;

        randVal = Random.Range(0, 100) % trPointArray.Length;
        Transform point = trPointArray[randVal];

        randVal = Random.Range(0, 100) % liZiArray.Length;
        GameObject liZiPrefab = liZiArray[randVal];

        if (liZiPrefab != null && point != null)
        {
            Instantiate(liZiPrefab, XkGameCtrl.NpcAmmoArray, point);
        }
    }
}