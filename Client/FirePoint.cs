using UnityEngine;
using System.Collections;

public enum NpcFireAction
{
	Fire1_4,
	Fire2_5,
	Fire3_6,
}

public class FirePoint : MonoBehaviour {
	[Range(1, 100)]public int CountFire = 5;
	public NpcFireAction AniFireName = NpcFireAction.Fire1_4;
	void Start()
	{
		enabled = false;
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
		SetFirePointName();
	}
#endif

    void SetFirePointName()
	{
		FirePointCtrl script = transform.parent.GetComponent<FirePointCtrl>();
		if (script == null) {
			return;
		}
		script.SetFirePointName();
	}
}