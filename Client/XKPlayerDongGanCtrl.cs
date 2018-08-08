using UnityEngine;
using System.Collections;

public class XKPlayerDongGanCtrl : MonoBehaviour {
	PlayerEnum IndexPlayer = PlayerEnum.Null;
	/**
	 * KeyZYQiNangState = 0 -> 左右气囊放气.
	 * KeyZYQiNangState = 1 -> 左气囊充气,右气囊放气.
	 * KeyZYQiNangState = 2 -> 右气囊充气,左气囊放气.
	 */
	int KeyZYQiNangState;
	/**
	 * KeyQHQiNangState = 0 -> 前后气囊放气.
	 * KeyQHQiNangState = 1 -> 前气囊充气, 后气囊放气.
	 * KeyQHQiNangState = 2 -> 后气囊充气,前气囊放气.
	 */
	int KeyQHQiNangState;
	float TimeLastZY;
	float MinTimeZY = 0.4f;
	// Update is called once per frame
	void TmpUpdate()
	{
		//if (pcvr.DongGanState == 0) {
		//	return;
		//}

		if (!GameTimeCtrl.GetInstance().GetIsCheckTimeSprite()) {
			return;
		}

		if (!XkGameCtrl.GetIsActivePlayer(IndexPlayer)) {
			return;
		}

		if (DaoJiShiCtrl.GetInstance(IndexPlayer).GetIsPlayDaoJishi()) {
			return;
		}
		//pcvr.OpenQiNangQian(IndexPlayer);//test.
		//return;
		
		Vector3 eulerAngle = transform.eulerAngles;
		if (eulerAngle.x > 180f) {
			eulerAngle.x -= 360f;
		}

		if (eulerAngle.z > 180f) {
			eulerAngle.z -= 360f;
		}
		float eulerAngleX = eulerAngle.x;
		float eulerAngleZ = eulerAngle.z;
		float offsetAngle = 0.5f;
		if (Mathf.Abs(eulerAngleX) <= offsetAngle) {
			//前后气囊放气.
			if (KeyQHQiNangState != 0) {
				KeyQHQiNangState = 0;
				if (KeyZYQiNangState == 0) {
					//pcvr.CloseQiNangQian(IndexPlayer);
					//pcvr.CloseQiNangHou(IndexPlayer);
				}
			}
		}
		else if  (eulerAngleX < 0f) {
			//前气囊充气,后气囊放气.
			if (KeyQHQiNangState != 1) {
				KeyQHQiNangState = 1;
				//pcvr.OpenQiNangQian(IndexPlayer);
				//pcvr.CloseQiNangHou(IndexPlayer, KeyZYQiNangState);
			}
		}
		else if (eulerAngleX > 0f) {
			//后气囊充气,前气囊放气.
			if (KeyQHQiNangState != 2) {
				KeyQHQiNangState = 2;
				//pcvr.OpenQiNangHou(IndexPlayer);
				//pcvr.CloseQiNangQian(IndexPlayer, KeyZYQiNangState);
			}
		}

		int indexVal = (int)IndexPlayer - 1;
		eulerAngleZ = -InputEventCtrl.PlayerFX[indexVal];
		if (XKGlobalData.GameVersionPlayer != 0) {
			indexVal -= 2;
			eulerAngleZ = -InputEventCtrl.PlayerFX[indexVal];
		}

		offsetAngle = 0.1f;
		if (Mathf.Abs(eulerAngleZ) <= offsetAngle) {
			//左右气囊放气.
			if (KeyZYQiNangState != 0 && Time.realtimeSinceStartup - TimeLastZY >= MinTimeZY) {
				KeyZYQiNangState = 0;
				TimeLastZY = Time.realtimeSinceStartup;
				if (KeyQHQiNangState == 0) {
					//pcvr.CloseQiNangZuo(IndexPlayer);
					//pcvr.CloseQiNangYou(IndexPlayer);
				}
			}
		}
		else if (eulerAngleZ < 0f) {
			//左气囊充气,右气囊放气.
			if (KeyZYQiNangState != 1 && Time.realtimeSinceStartup - TimeLastZY >= MinTimeZY) {
				KeyZYQiNangState = 1;
				TimeLastZY = Time.realtimeSinceStartup;
				//pcvr.OpenQiNangZuo(IndexPlayer);
				//pcvr.CloseQiNangYou(IndexPlayer, KeyQHQiNangState);
			}
		}
		else if  (eulerAngleZ > 0f) {
			//右气囊充气,左气囊放气.
			if (KeyZYQiNangState != 2 && Time.realtimeSinceStartup - TimeLastZY >= MinTimeZY) {
				KeyZYQiNangState = 2;
				TimeLastZY = Time.realtimeSinceStartup;
				//pcvr.OpenQiNangYou(IndexPlayer);
				//pcvr.CloseQiNangZuo(IndexPlayer, KeyQHQiNangState);
			}
		}
	}

	public void SetPlayerIndex(PlayerEnum playerVal)
	{
		IndexPlayer = playerVal;
		//pcvr.CloseAllQiNangArray(IndexPlayer, 1);
	}
}