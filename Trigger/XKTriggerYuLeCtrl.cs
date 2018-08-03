using UnityEngine;

public class XKTriggerYuLeCtrl : MonoBehaviour
{
    public enum TriggerEnum
    {
        Null,
        Open,
        Close,
    }
    public TriggerEnum TriggerSt = TriggerEnum.Null;
	/**
	 * 娱乐环节配置信息.
	 */
	[Range(1f, 100f)]public float YuLeSpeedVal = 60f;
	[Range(1f, 500f)]public float YuLeSpeedYGVal = 60f;
	[Range(0f, 500f)]public float HorizontalSpeedYGBL = 0.5f;
	public static bool IsActiveYuLeTrigger;
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
		if (!XkGameCtrl.IsDrawGizmosObj) {
			return;
		}
		
		if (!enabled) {
			return;
		}
		
		if (TestPlayerPath != null) {
			TestPlayerPath.DrawPath();
		}
	}
#endif

    void OnTriggerEnter(Collider other)
	{	
		if (other.GetComponent<XkPlayerCtrl>() == null) {
			return;
		}

		switch (TriggerSt) {
		case TriggerEnum.Open:
			IsActiveYuLeTrigger = true;
			XKPlayerMoveCtrl.SetPlayerYuLeInfo(this);
			break;
		default:
			IsActiveYuLeTrigger = false;
			XKPlayerMoveCtrl.SetPlayerJiSuMoveSpeed(PlayerEnum.Null);
			break;
		}
	}
}