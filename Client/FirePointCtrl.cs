using UnityEngine;
using System.Collections;

public class FirePointCtrl : MonoBehaviour
{
	FirePoint[] FirePointArray;
	void Start()
	{
		enabled = false;
		FirePointArray = transform.GetComponentsInChildren<FirePoint>();
		if (FirePointArray.Length != transform.childCount || FirePointArray.Length <= 0) {
			Debug.LogWarning("Unity:"+"FirePoint was wrong!");
			GameObject obj = null;
			obj.name = "null";
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
		SetFirePointName();
	}
#endif

    public void SetFirePointName()
	{
		FirePoint[] tranArray = transform.GetComponentsInChildren<FirePoint>();
		for (int i = 0; i < tranArray.Length; i++) {
			tranArray[i].name = "Point_" + i;
		}
	}

	int IndexFirePoint;
	public FirePoint GetFirePoint()
	{
		if (FirePointArray.Length <= 0) {
			return null;
		}

		int indexVal = IndexFirePoint;
		IndexFirePoint++;
		IndexFirePoint = IndexFirePoint >= FirePointArray.Length ? 0 : IndexFirePoint;
		return FirePointArray[indexVal];
	}
}