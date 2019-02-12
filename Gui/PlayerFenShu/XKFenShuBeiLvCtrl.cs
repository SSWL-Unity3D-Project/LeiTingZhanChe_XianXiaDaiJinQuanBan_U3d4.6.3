using UnityEngine;

public class XKFenShuBeiLvCtrl : MonoBehaviour
{
	//public GameObject PlayerFenShuBeiLvPre;
	//List<XKFenShuBeiLvMove> FenShuBeiLvList;
	//const int MaxPlayerFS = 12;
	static  XKFenShuBeiLvCtrl _Instance;
	public static XKFenShuBeiLvCtrl GetInstance()
	{
		return _Instance;
	}
	// Use this for initialization
	void Start()
	{
		_Instance = this;
		//FenShuBeiLvList = new List<XKFenShuBeiLvMove>();
		//GameObject obj = null;
		//for (int i = 0; i < MaxPlayerFS; i++) {
		//	obj = (GameObject)Instantiate(PlayerFenShuBeiLvPre);
		//	obj.transform.parent = transform;
		//	obj.transform.localScale = new Vector3(1f, 1f, 1f);
		//	obj.transform.localPosition = Vector3.zero;
		//	FenShuBeiLvList.Add(obj.GetComponent<XKFenShuBeiLvMove>());
		//	obj.SetActive(false);
		//}
	}
	
	XKFenShuBeiLvMove GetXKFenShuBeiLvMove()
	{
		//GameObject obj = null;
		//int valTmp = 0;
		//for (int i = 0; i < MaxPlayerFS; i++) {
		//	obj = FenShuBeiLvList[i].gameObject;
		//	if (obj.activeSelf) {
		//		continue;
		//	}
		//	valTmp = i;
		//	break;
		//}
		//return FenShuBeiLvList[valTmp];

        GameObject gmDataPrefab = (GameObject)Resources.Load("Prefabs/GUI/DaoJuMaoZi/FenShuBeiLv");
        if (gmDataPrefab == null)
        {
            SSDebug.LogWarning("GetXKFenShuBeiLvMove -> gmDataPrefab was null");
            return null;
        }

        GameObject obj = (GameObject)Instantiate(gmDataPrefab);
        obj.transform.parent = transform;
        obj.transform.localScale = new Vector3(1f, 1f, 1f);
        obj.transform.localPosition = Vector3.zero;
        return obj.GetComponent<XKFenShuBeiLvMove>();
    }

    public void ShowPlayerFenShuBeiLv(PlayerEnum indexVal)
	{
		XKFenShuBeiLvMove fenShuBeiLvMoveCom = GetXKFenShuBeiLvMove();
		if (fenShuBeiLvMoveCom == null) {
			return;
		}
		
		Transform playerTr = XKPlayerMoveCtrl.GetXKPlayerMoveCom(indexVal).PiaoFenPoint;
		Vector3 startPos = XkGameCtrl.GetInstance().GetWorldObjToScreenPos(playerTr.position);
		fenShuBeiLvMoveCom.SetPlayerFenShuBeiLvVal(startPos);
	}
}