//#define TEST_YULE_TRIGER
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

#if TEST_YULE_TRIGER
        IndexVal = CountTrigger;
        CountTrigger++;
#endif
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

#if TEST_YULE_TRIGER
    static int CountTrigger = 0;
    int IndexVal = 0;
    bool IsHit = false;
    private void OnGUI()
    {
        GUI.Box(new Rect(15f, IndexVal * 25f, 350f, 25f), "IndexVal " + IndexVal + ", TriggerSt " + TriggerSt + ", IsHit " + IsHit);
    }
#endif

    void OnTriggerEnter(Collider other)
    {
#if TEST_YULE_TRIGER
        SSDebug.Log("SSTriggerCameraSpeed::OnTriggerEnter -> 11111 m_CameraSpeed == " + m_CameraSpeed + ", TriggerSt ====== " + TriggerSt);
#endif
        if (other.GetComponent<XkPlayerCtrl>() == null)
        {
            return;
        }

#if TEST_YULE_TRIGER
        IsHit = true;
        SSDebug.Log("SSTriggerCameraSpeed::OnTriggerEnter -> 22222 m_CameraSpeed == " + m_CameraSpeed + ", TriggerSt ====== " + TriggerSt);
#endif
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