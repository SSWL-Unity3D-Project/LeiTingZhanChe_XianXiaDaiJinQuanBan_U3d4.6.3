using UnityEngine;

public class AiPathGroupCtrl : MonoBehaviour
{
	public PlayerTypeEnum PathState = PlayerTypeEnum.FeiJi;
    /// <summary>
    /// 平常阶段镜头运动速度..
    /// </summary>
    public float m_MoveSpeed = 3f;
    /// <summary>
    /// boss阶段镜头运动速度..
    /// </summary>
    public float m_BossMoveSpeed = 1f;
    public enum MoveState
    {
        /// <summary>
        /// 平常运动状态.
        /// </summary>
        Default = 0,
        /// <summary>
        /// Boss产生时的运动速度.
        /// </summary>
        Boss = 1,
        /// <summary>
        /// 娱乐(镜头在场景中转弯阶段)
        /// </summary>
        YuLe = 2,
    }
    internal MoveState m_CameraMoveType = MoveState.Default;
    /// <summary>
    /// 娱乐(镜头在场景中转弯阶段)镜头运动速度.
    /// </summary>
    internal float m_YuLeMoveSpeed = 0f;
    /// <summary>
    /// 设置主角镜头运动状态.
    /// </summary>
    internal void SetCameraMoveType(MoveState type)
    {
        m_CameraMoveType = type;
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
	{
		if (!XkGameCtrl.IsDrawGizmosObj) {
			return;
		}

		if (!enabled) {
			return;
		}

		AiPathCtrl[] PathArray = transform.GetComponentsInChildren<AiPathCtrl>();
		for (int i = 0; i < PathArray.Length; i++) {
			PathArray[i].name = PathState + "AiPath_" + (i+1);
		}
	}
#endif
}
