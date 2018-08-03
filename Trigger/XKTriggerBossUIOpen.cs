using UnityEngine;
using System.Collections;

public class XKTriggerBossUIOpen : MonoBehaviour
{
	[Range(1f, 999f)]public int TimeBoss = 90;
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

    void OnTriggerEnter(Collider other)
	{	
		if (other.GetComponent<XkPlayerCtrl>() == null) {
			return;
		}
		XKBossXueTiaoCtrl.GetInstance().OpenBossXueTiao(TimeBoss);
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