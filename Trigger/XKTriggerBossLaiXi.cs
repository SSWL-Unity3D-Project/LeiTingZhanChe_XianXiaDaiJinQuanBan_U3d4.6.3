using UnityEngine;
using System.Collections;

public class XKTriggerBossLaiXi : MonoBehaviour
{
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
		if (other.GetComponent<XkPlayerCtrl>() == null) {
			return;
		}
		XKBossLXCtrl.GetInstance().StartPlayBossLaiXi();
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
}