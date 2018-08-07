using UnityEngine;
using System.Collections;

public class XKNpcDamageCtrl : MonoBehaviour
{
	[Range(0.01f, 3f)]public float DamageTime = 0.1f;
	public XKMeshColorCtrl[] MeshColorArray;
    // Update is called once per frame
    //void Update()
    //{
    //    if (Input.GetKeyUp(KeyCode.P))
    //    {
    //        PlayNpcDamageEvent();
    //    }
    //}

	public void PlayNpcDamageEvent()
	{
		for (int i = 0; i < MeshColorArray.Length; i++) {
			if (MeshColorArray[i] != null) {
				MeshColorArray[i].MakeMeshToNewColor(DamageTime);
			}
		}
	}
}