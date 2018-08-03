using UnityEngine;

public class SSCreatNpcData : MonoBehaviour
{
    /// <summary>
    /// Npc路径组.
    /// </summary>
    public NpcPathCtrl[] m_NpcPathGp;
    /// <summary>
    /// npc产生点组件.
    /// </summary>
    [HideInInspector]
    public XKSpawnNpcPoint m_SpawnPoint;
    void Start()
    {
        m_SpawnPoint = gameObject.GetComponent<XKSpawnNpcPoint>();
    }

    /// <summary>
    /// 获取npc的路径数据.
    /// </summary>
    public NpcPathCtrl GetNpcPahtData()
    {
        NpcPathCtrl[] comGp = m_NpcPathGp;
        if (comGp == null || comGp.Length <= 0)
        {
            Debug.LogWarning("Unity: not find NpcPathData!");
            return null;
        }

        NpcPathCtrl com = null;
        int rv = Random.Range(0, 100) % comGp.Length;
        com = comGp[rv];
        if (com == null)
        {
            Debug.LogWarning("Unity: com was null! rv ============ " + rv);
        }
        return com;
    }
}