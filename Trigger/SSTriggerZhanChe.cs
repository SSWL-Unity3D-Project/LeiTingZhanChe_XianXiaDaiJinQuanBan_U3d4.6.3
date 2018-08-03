using UnityEngine;

public class SSTriggerZhanChe : MonoBehaviour
{
    /// <summary>
    /// 创建战车npc的状态.
    /// </summary>
    public SpawnNpcManage.CreatZhanCheState m_CreatZhanCheState;
    void Start()
    {
        MeshRenderer mesh = gameObject.GetComponent<MeshRenderer>();
        if (mesh != null)
        {
            Destroy(mesh);
        }

        MeshFilter meshFt = gameObject.GetComponent<MeshFilter>();
        if (meshFt != null)
        {
            Destroy(meshFt);
        }
    }

    public AiPathCtrl TestPlayerPath;
    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<XkPlayerCtrl>() == null)
        {
            return;
        }

        if (XkPlayerCtrl.GetInstanceFeiJi() != null
            && XkPlayerCtrl.GetInstanceFeiJi().m_SpawnNpcManage != null)
        {
            XkPlayerCtrl.GetInstanceFeiJi().m_SpawnNpcManage.m_CreatZhanCheState = m_CreatZhanCheState;
        }
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        if (!XkGameCtrl.IsDrawGizmosObj)
        {
            return;
        }

        if (!enabled)
        {
            return;
        }

        if (TestPlayerPath != null)
        {
            TestPlayerPath.DrawPath();
        }
    }
#endif
}