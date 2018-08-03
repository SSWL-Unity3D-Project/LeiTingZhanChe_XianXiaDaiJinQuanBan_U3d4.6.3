using UnityEngine;
using System.Collections;

public class AiMark : MonoBehaviour
{
	public static bool IsMoveSpeedByAiMark = true;
    AiPathCtrl m_AiPath;
	[Range(0.001f, 100f)]public float MvSpeed = 5f;
	/**************************************************************
	 * PlayerAni是ZhiShengJiAction.null并且TimePlayerAni > 0f时,
	 * 主角会在该产生点停留TimePlayerAni时间之后在运动.
	**************************************************************/
	public ZhiShengJiAction PlayerAni;
	[Range(0f, 100f)]public float TimePlayerAni = 0f;
	public Transform PlayerCamAimTran;
	//public Transform[] CameraAimArray;
	[Range(0.01f, 100f)]public float SpeedIntoAim = 1f;
	[Range(0.01f, 100f)]public float SpeedOutAim = 1f;
	public bool IsAimPlayer;
	public bool IsTestDrawPath;
	private Transform _mNextMark = null;
	public Transform mNextMark
	{
		get
		{
			return _mNextMark;
		}
		set
		{
			_mNextMark = value;
		}
	}
	
	private int mMarkCount = 0;
	bool IsInitMarkInfo;
	void Start()
	{
        m_AiPath = gameObject.GetComponentInParent<AiPathCtrl>();
        if (m_AiPath.m_AiPathGroup != null)
        {
            //统一将主角镜头移动速度修改为普通状态移动速度.
            MvSpeed = m_AiPath.m_AiPathGroup.m_MoveSpeed;
        }

        //MvSpeed = XkGameCtrl.GetInstance().MvSpeed;
        bool isOutputError = false;
		if (PlayerAni == ZhiShengJiAction.Null && TimePlayerAni > 0f && MvSpeed > 1f) {
			Debug.Log("Unity:"+"PlayerAni is null, but MvSpeed is greater than 1f");
			isOutputError = true;
		}

		if (isOutputError) {
			GameObject obj = null;
			obj.name = "null";
		}
		IsInitMarkInfo = true;
		IsTestDrawPath = false;
        
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
        CheckPathMarkScale();
    }
	
	public void setMarkCount( int count )
	{
		mMarkCount = count;
	}
	
	public int getMarkCount()
	{
		return mMarkCount;
	}

	public float GetMvSpeed()
	{
        float speed = MvSpeed;
        if (m_AiPath.m_AiPathGroup != null)
        {
            switch (m_AiPath.m_AiPathGroup.m_CameraMoveType)
            {
                case AiPathGroupCtrl.MoveState.Boss:
                    {
                        speed = m_AiPath.m_AiPathGroup.m_BossMoveSpeed;
                        break;
                    }
                case AiPathGroupCtrl.MoveState.YuLe:
                    {
                        //娱乐(镜头在场景中转弯阶段)
                        speed = m_AiPath.m_AiPathGroup.m_YuLeMoveSpeed;
                        break;
                    }
            }
        }
        return speed;
	}
	
	// Use this for initialization
//	void Start()
//	{
//		this.enabled = false;
//	}
	
	void OnTriggerEnter(Collider other)
	{
//		PSZiYouMoveCamera script = other.GetComponent<PSZiYouMoveCamera>();
//		if (script != null) {
//			script.SetCameraMarkInfo(this);
//			return;
//		}

		XkPlayerCtrl playerScript = other.GetComponent<XkPlayerCtrl>();
		if (playerScript == null) {
			return;
		}
//		Debug.Log("Unity:"+"AiMark::OnTriggerEnter -> AniName "+PlayerAni);
//		Debug.Log("Unity:"+"AiMark::OnTriggerEnter -> MarkName "+gameObject.name);
		playerScript.PlayZhuJiaoMarkAction(this);
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
		//CheckPathMarkScale();

		Transform parTran = transform.parent;
		if (parTran == null) {
			return;
		}

		AiPathCtrl pathScript = parTran.GetComponent<AiPathCtrl>();
		if (!pathScript.enabled) {
			if (!IsTestDrawPath) {
				if (!IsInitMarkInfo) {
					pathScript.enabled = true;
				}
				else {
					return;
				}
			}
			else {
				pathScript.enabled = true;
			}
		}
		else {
			if (!IsTestDrawPath) {
				if (IsInitMarkInfo) {
					pathScript.enabled = false;
				}
			}
		}
		pathScript.DrawPath();
	}
#endif

    void CheckPathMarkScale()
	{
		Vector3 scale = new Vector3(1f, 1f, 1f);
		if (transform.localScale != scale) {
			transform.localScale = scale;
		}
		transform.localScale = scale;
	}

	void CheckBoxCollider()
	{
		BoxCollider boxCol = GetComponent<BoxCollider>();
		if (boxCol == null) {
			boxCol = gameObject.AddComponent<BoxCollider>();
		}

		boxCol.isTrigger = true;
		Vector3 boxSize = new Vector3(1f, 1f, 1f);
		if (boxCol.size != boxSize) {
			boxCol.size = boxSize;
		}

		//"Ignore Raycast"
		gameObject.layer = LayerMask.NameToLayer("TransparentFX");
	}
}