using UnityEngine;
using System.Collections;

public class PSCameraMark : MonoBehaviour {
	public bool IsAimPlayer;
	Transform NextMark;

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
	{
		if (!XkGameCtrl.IsDrawGizmosObj) {
			return;
		}

		if (!enabled) {
			return;
		}

		PSCameraPath pathScript = GetComponentInParent<PSCameraPath>();
		if (pathScript != null) {
			pathScript.DrawPath();
		}
	}
#endif

    void OnTriggerEnter(Collider other)
	{
		//Debug.Log("Unity:"+"OnTriggerEnter...hitName "+other.name+", markName "+name);
//		PSZiYouMoveCamera script = other.GetComponent<PSZiYouMoveCamera>();
//		if (script == null) {
//			return;
//		}
//		script.SetCameraMarkInfo(this);
	}

	public void SetNextMark(Transform mark)
	{
		NextMark = mark;
	}

	public Transform GetNextMark()
	{
		return NextMark;
	}
}