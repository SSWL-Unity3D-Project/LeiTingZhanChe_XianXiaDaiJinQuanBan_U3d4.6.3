using UnityEngine;
using System.Collections;

public enum AppGameType
{
	Null,
	DanJiTanKe,
	DanJiFeiJi,
	LianJiTanKe,
	LianJiFeiJi,
	LianJiServer,
}

public class GameTypeCtrl : MonoBehaviour {
	public TKMoveState TKMoveSt = TKMoveState.U_FangXiangPan;
	public static bool IsSetTKMoveSt;

	AppGameType AppType = AppGameType.DanJiFeiJi;
	public GameObject NetCtrlObj;
	public static AppGameType AppTypeStatic = AppGameType.Null;
	public static bool IsServer;
	public static GameTypeCtrl Instance;
	void Awake()
	{
		Instance = this;
		//pcvr.TKMoveSt = TKMoveSt;
		IsSetTKMoveSt = true;

		AppTypeStatic = AppType;
		if (AppType == AppGameType.LianJiServer) {
			IsServer = true;
		}
	}
}