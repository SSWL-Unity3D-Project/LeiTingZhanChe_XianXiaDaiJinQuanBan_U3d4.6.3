using UnityEngine;

public class XKPlayerJiSuCtrl : MonoBehaviour
{
	//public GameObject PlayerJiSuPre;
	//List<XKPlayerJiSuMove> JiSuList;
	//const int MaxPlayerFS = 12;
	static  XKPlayerJiSuCtrl _Instance;
	public static XKPlayerJiSuCtrl GetInstance()
	{
		return _Instance;
	}
	// Use this for initialization
	void Start()
	{
		_Instance = this;
		//JiSuList = new List<XKPlayerJiSuMove>();
		//GameObject obj = null;
		//for (int i = 0; i < MaxPlayerFS; i++) {
		//	obj = (GameObject)Instantiate(PlayerJiSuPre);
		//	obj.transform.parent = transform;
		//	obj.transform.localScale = new Vector3(1f, 1f, 1f);
		//	obj.transform.localPosition = Vector3.zero;
		//	JiSuList.Add(obj.GetComponent<XKPlayerJiSuMove>());
		//	obj.SetActive(false);
		//}
	}
	
	XKPlayerJiSuMove GetXKPlayerJiSuMove()
	{
		//GameObject obj = null;
		//int valTmp = 0;
		//for (int i = 0; i < MaxPlayerFS; i++) {
		//	obj = JiSuList[i].gameObject;
		//	if (obj.activeSelf) {
		//		continue;
		//	}
		//	valTmp = i;
		//	break;
		//}
		//return JiSuList[valTmp];

        GameObject gmDataPrefab = (GameObject)Resources.Load("Prefabs/GUI/DaoJuMaoZi/PlayerJiSu");
        if (gmDataPrefab == null)
        {
            SSDebug.LogWarning("GetXKPlayerJiSuMove -> gmDataPrefab was null");
            return null;
        }

        GameObject obj = (GameObject)Instantiate(gmDataPrefab);
        obj.transform.parent = transform;
        obj.transform.localScale = new Vector3(1f, 1f, 1f);
        obj.transform.localPosition = Vector3.zero;
        return obj.GetComponent<XKPlayerJiSuMove>();
    }
	
	public void ShowPlayerJiSu(PlayerEnum indexVal)
	{
		XKPlayerJiSuMove jiSuMoveCom = GetXKPlayerJiSuMove();
		if (jiSuMoveCom == null) {
			return;
		}

		Transform playerTr = XKPlayerMoveCtrl.GetInstance(indexVal).PiaoFenPoint;
//		playerTr = TestPlayerTr; //test
		
		Vector3 startPos = XkGameCtrl.GetInstance().GetWorldObjToScreenPos(playerTr.position);
		jiSuMoveCom.SetPlayerJiSuVal(startPos);
	}
}