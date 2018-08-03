using UnityEngine;
using System.Collections;

public class XKTriggerOpenObj : MonoBehaviour
{
	public GameObject ObjOpen;
	[Range(0.01f, 100f)] public float TimeOpen = 3f;
	float TimeLast;
	bool IsActiveTrigger;
	public AiPathCtrl TestPlayerPath;

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

    void Start()
	{
		ObjOpen.SetActive(false);

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

	void Update()
	{
		if (!IsActiveTrigger) {
			return;
		}

		if (Time.time - TimeLast < TimeOpen) {
			return;
		}
		ObjOpen.SetActive(false);
		gameObject.SetActive(false);
	}

	void OnTriggerEnter(Collider other)
	{
		XkPlayerCtrl playerScript = other.GetComponent<XkPlayerCtrl>();
		if (playerScript == null) {
			return;
		}
		IsActiveTrigger = true;
		TimeLast = Time.time;
		ObjOpen.SetActive(true);
	}
}