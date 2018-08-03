using UnityEngine;
using System.Collections;

/*************************************
	Fire1, Fire2, Fire3瞄准空中.
	Fire4, Fire5, Fire6瞄准地面.
	TurnLeft, TurnRight火车的转弯动画.
 *************************************/
public enum AnimatorNameNPC
{
	Null,
	Root1,
//	Root2,
//	Root3,
//	Root4,
	Run1,
	Run2,
	Run3,
	Run4, //边跑边开火
	Fire1,
	Fire2,
//	Fire3,
//	Fire4,
//	Fire5,
//	Fire6,
//	TurnLeft,
//	TurnRight,
//	HuanDan1,
//	HuanDan2,
//	HuanDan3,
//	HuanDan4,
//	HuanDan5,
//	HuanDan6,
}

public class NpcMark : MonoBehaviour {

	public AnimatorNameNPC AniName;
	/// <summary>
	/// AnimatorTime > 0f时, npc停在该路径点做AniName动画.
	/// </summary>
	[Range(0f, 20f)] public float AnimatorTime = 0f;
	[Range(0.1f, 200f)] public float MvSpeed = 1f;
	public bool IsDoFireAction;
	[Range(1, 100)] public int FireCount = 3;
	public bool IsWuDi;
	public bool IsFireFeiJiNpc;

	void Start()
	{
		CheckBoxCollider();
		CheckAniName();
		this.enabled = IsFireFeiJiNpc;
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
		CheckBoxCollider();
		CheckAniName();
		DrawPath();
	}
#endif

    void CheckAniName()
	{
		if (AniName == AnimatorNameNPC.Fire1 || AniName == AnimatorNameNPC.Fire2) {
			AniName = AnimatorNameNPC.Null;
		}
//		if (AniName == AnimatorNameNPC.Fire1 || AniName == AnimatorNameNPC.Fire2 || AniName == AnimatorNameNPC.Fire3
//		    || AniName == AnimatorNameNPC.Fire4 || AniName == AnimatorNameNPC.Fire5 || AniName == AnimatorNameNPC.Fire6) {
//			AniName = AnimatorNameNPC.Null;
//		}
	}

	void CheckBoxCollider()
	{
		BoxCollider boxCol = GetComponent<BoxCollider>();
		if (boxCol == null) {
			boxCol = gameObject.AddComponent<BoxCollider>();
		}
		
		boxCol.isTrigger = true;
		Vector3 boxSize = new Vector3(1f, 1f, 1f);
		if (!IsFireFeiJiNpc) {
			if (boxCol.size != boxSize) {
				boxCol.size = boxSize;
			}
		}
		
		//"Ignore Raycast"
		gameObject.layer = LayerMask.NameToLayer("TransparentFX");
	}
	
//	void OnTriggerEnter(Collider other)
//	{
//		//Debug.Log("Unity:"+"OnTriggerEnter...name "+other.name);
//		if (Network.peerType == NetworkPeerType.Client) {
//			return;
//		}
//
//		XkNpcZaiTiCtrl script = other.GetComponent<XkNpcZaiTiCtrl>();
//		if (script == null) {
//			return;
//		}
//		script.SetNpcIsDoFire(this);
//	}

	public void DrawPath()
	{
		Transform parTran = transform.parent;
		NpcPathCtrl pathScript = parTran.GetComponent<NpcPathCtrl>();
		if(pathScript != null)
		{
			pathScript.DrawPath();
		}
	}
}

public enum NpcRunState
{
	NULL,
	RUN_1,
	RUN_2
}