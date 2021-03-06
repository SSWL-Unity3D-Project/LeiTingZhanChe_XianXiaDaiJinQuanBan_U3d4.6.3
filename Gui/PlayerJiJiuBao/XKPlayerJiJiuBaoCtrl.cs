﻿using UnityEngine;

public class XKPlayerJiJiuBaoCtrl : MonoBehaviour
{
	//public GameObject PlayerJiJiuBaoPre;
	//List<XKPlayerJiJiuBaoMove> JiJiuBaoList;
	//const int MaxPlayerFS = 12;
	static  XKPlayerJiJiuBaoCtrl _Instance;
	public static XKPlayerJiJiuBaoCtrl GetInstance()
	{
		return _Instance;
	}
	// Use this for initialization
	void Start()
	{
		_Instance = this;
		//JiJiuBaoList = new List<XKPlayerJiJiuBaoMove>();
		//GameObject obj = null;
		//for (int i = 0; i < MaxPlayerFS; i++) {
		//	obj = (GameObject)Instantiate(PlayerJiJiuBaoPre);
		//	obj.transform.parent = transform;
		//	obj.transform.localScale = new Vector3(1f, 1f, 1f);
		//	obj.transform.localPosition = Vector3.zero;
		//	JiJiuBaoList.Add(obj.GetComponent<XKPlayerJiJiuBaoMove>());
		//	obj.SetActive(false);
		//}
	}
	
	XKPlayerJiJiuBaoMove GetXKPlayerJiJiuBaoMove()
	{
		//GameObject obj = null;
		//int valTmp = 0;
		//for (int i = 0; i < MaxPlayerFS; i++) {
		//	obj = JiJiuBaoList[i].gameObject;
		//	if (obj.activeSelf) {
		//		continue;
		//	}
		//	valTmp = i;
		//	break;
		//}
		//return JiJiuBaoList[valTmp];

        GameObject gmDataPrefab = (GameObject)Resources.Load("Prefabs/GUI/DaoJuMaoZi/PlayerJiJiuBao");
        if (gmDataPrefab == null)
        {
            SSDebug.LogWarning("GetXKPlayerJiJiuBaoMove -> gmDataPrefab was null");
            return null;
        }

        GameObject obj = (GameObject)Instantiate(gmDataPrefab);
        obj.transform.parent = transform;
        obj.transform.localScale = new Vector3(1f, 1f, 1f);
        obj.transform.localPosition = Vector3.zero;
        return obj.GetComponent<XKPlayerJiJiuBaoMove>();
    }
	
	public void ShowPlayerJiJiuBao(PlayerEnum indexVal)
	{
		XKPlayerJiJiuBaoMove jiJiuBaoMoveCom = GetXKPlayerJiJiuBaoMove();
		if (jiJiuBaoMoveCom == null) {
			return;
		}
		
		Transform playerTr = XKPlayerMoveCtrl.GetInstance(indexVal).PiaoFenPoint;
		Vector3 startPos = XkGameCtrl.GetInstance().GetWorldObjToScreenPos(playerTr.position);
		jiJiuBaoMoveCom.SetPlayerJiJiuBaoVal(startPos);
	}
}