using UnityEngine;
using System.Collections;

public class XKTriggerOpenStage : MonoBehaviour
{
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
		XkPlayerCtrl playerScript = other.GetComponent<XkPlayerCtrl>();
		if (playerScript == null) {
			return;
		}
		XKGameStageCtrl.GetInstance().MoveIntoStageUI();
		gameObject.SetActive(false);
	}
}