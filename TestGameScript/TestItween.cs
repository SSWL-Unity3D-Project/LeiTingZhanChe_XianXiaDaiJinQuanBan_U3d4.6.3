using UnityEngine;
using System.Collections;

public class TestItween : MonoBehaviour {

	// Use this for initialization
	void Start()
    {
        TestMove();
    }

    void TestMove()
    {
        SSDebug.LogWarning("****************************** time ===== " + Time.time);
        Vector3[] pathArray = new Vector3[2];
        pathArray[0] = transform.position;
        pathArray[1] = transform.position + Vector3.up;
        float time = 10f;
        float offset = 3f;
        float speed = Vector3.Distance(pathArray[0], pathArray[1]) / time;
        speed = offset * speed;
        iTween.MoveTo(gameObject, iTween.Hash("path", pathArray,
                                          "speed", speed,
                                          "orienttopath", false,
                                          "easeType", iTween.EaseType.linear,
                                          "oncomplete", "MoveNpcOnCompelteITween"));
    }

    void MoveNpcOnCompelteITween()
    {
        SSDebug.LogWarning("****************************** time ===== " + Time.time);
    }
}
