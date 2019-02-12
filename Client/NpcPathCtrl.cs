using UnityEngine;
using System.Collections.Generic;

public class NpcPathCtrl : MonoBehaviour {
	public bool IsMoveEndFire;
	public bool IsAutoMarkName;
	public bool IsDrawLine;
    internal float m_PathLength = 0f;
	void Start()
	{
		CheckNpcPathScript();
		IsAutoMarkName = false;
		this.enabled = false;
		NpcMark[] markScript = GetComponentsInChildren<NpcMark>();
		if (markScript.Length != transform.childCount) {
			Debug.LogWarning("Unity:"+"NpcPathScript was wrong!");
			GameObject obj = null;
			obj.name = "null";
		}
        gameObject.SetActive(false);
        GetPathLength();
    }

    void GetPathLength()
    {
        float pathLength = 0f;
        if (transform.childCount > 1)
        {
            for (int i = 0; i < transform.childCount - 1; i++)
            {
                Vector3 startPos = transform.GetChild(i).position;
                Vector3 endPos = transform.GetChild(i + 1).position;
                pathLength += Vector3.Distance(startPos, endPos);
            }
        }
        m_PathLength = pathLength;
    }

#if UNITY_EDITOR
    public bool IsDrawGizmos = false;
    void OnDrawGizmosSelected()
	{
        if (!IsDrawGizmos)
        {
            return;
        }

		if (!XkGameCtrl.IsDrawGizmosObj) {
			return;
		}

		if (!enabled) {
			return;
		}
		ChangeMarkName();

		Transform parTran = transform;
		if(parTran.childCount > 1)
		{
			List<Transform> nodesTran = new List<Transform>(parTran.GetComponentsInChildren<Transform>()){};
			nodesTran.Remove(parTran);
			if (IsDrawLine) {
				iTween.DrawLine(nodesTran.ToArray(), Color.blue);
			}
			else {
				iTween.DrawPath(nodesTran.ToArray(), Color.blue);
			}
		}
	}
#endif

    public void DrawPath ()
	{
#if UNITY_EDITOR
		OnDrawGizmosSelected();
#endif
	}

	void CheckNpcPathScript()
	{
		NpcMark[] markScript = GetComponentsInChildren<NpcMark>();
		if (markScript.Length != transform.childCount) {
			Debug.LogWarning("Unity:"+"NpcPath was wrong! markLen "+markScript.Length);
			GameObject obj = null;
			obj.name = "null";
		}
	}

	void ChangeMarkName()
	{
		if (!IsAutoMarkName) {
			return;
		}
		
		NpcMark [] tranArray = transform.GetComponentsInChildren<NpcMark>();
		for (int i = 0; i < tranArray.Length; i++) {
			tranArray[i].name = "Mark_" + i;
		}
	}
}