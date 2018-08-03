using UnityEngine;

/// <summary>
/// 主角路径摄像机速度控制触发器.
/// </summary>
public class SSTriggerCameraSpeed : MonoBehaviour
{
    public enum TriggerEnum
    {
        /// <summary>
        /// 打开娱乐阶段.
        /// 关闭战车npc的刷怪逻辑.
        /// </summary>
        Open,
        /// <summary>
        /// 关闭娱乐阶段.
        /// 打开战车npc的刷怪逻辑.
        /// </summary>
        Close,
    }
    public TriggerEnum TriggerSt = TriggerEnum.Open;
    /// <summary>
    /// 摄像机运动速度.
    /// </summary>
    public float m_CameraSpeed = 5f;
    public AiPathCtrl TestPlayerPath;
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

    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<XkPlayerCtrl>() == null)
        {
            return;
        }
        
        if (XkGameCtrl.GetInstance() != null && XkGameCtrl.GetInstance().m_AiPathGroup != null)
        {
            //修改娱乐阶段主角镜头运动速度.
            XkGameCtrl.GetInstance().m_AiPathGroup.m_YuLeMoveSpeed = m_CameraSpeed;
        }

        switch (TriggerSt)
        {
            case TriggerEnum.Open:
                {
                    //打开娱乐阶段.
                    //关闭战车npc的刷怪逻辑.
                    if (XkGameCtrl.GetInstance() != null && XkGameCtrl.GetInstance().m_AiPathGroup != null)
                    {
                        XkGameCtrl.GetInstance().m_AiPathGroup.SetCameraMoveType(AiPathGroupCtrl.MoveState.YuLe);
                    }
                    break;
                }
            case TriggerEnum.Close:
                {
                    //关闭娱乐阶段.
                    //打开战车npc的刷怪逻辑.
                    if (XkGameCtrl.GetInstance() != null && XkGameCtrl.GetInstance().m_AiPathGroup != null)
                    {
                        XkGameCtrl.GetInstance().m_AiPathGroup.SetCameraMoveType(AiPathGroupCtrl.MoveState.Default);
                    }
                    break;
                }
        }
    }
}