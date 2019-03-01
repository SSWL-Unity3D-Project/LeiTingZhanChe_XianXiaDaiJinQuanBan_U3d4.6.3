using UnityEngine;

public class XKPlayerMvFanWei : MonoBehaviour {
	public PointState FanWeiState = PointState.Qian;
	static XKPlayerMvFanWei _InstanceQian;
	public static XKPlayerMvFanWei GetInstanceQian()
	{
		return _InstanceQian;
	}
	static XKPlayerMvFanWei _InstanceHou;
	public static XKPlayerMvFanWei GetInstanceHou()
	{
		return _InstanceHou;
	}
	static XKPlayerMvFanWei _InstanceZuo;
	public static XKPlayerMvFanWei GetInstanceZuo()
	{
		return _InstanceZuo;
	}
	static XKPlayerMvFanWei _InstanceYou;
	public static XKPlayerMvFanWei GetInstanceYou()
	{
		return _InstanceYou;
	}
	void Awake()
	{
		switch (FanWeiState) {
		case PointState.Qian:
			_InstanceQian = this;
			break;
		case PointState.Hou:
			_InstanceHou = this;
			break;
		case PointState.Zuo:
			_InstanceZuo = this;
			break;
		case PointState.You:
			_InstanceYou = this;
			break;
		}

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
}