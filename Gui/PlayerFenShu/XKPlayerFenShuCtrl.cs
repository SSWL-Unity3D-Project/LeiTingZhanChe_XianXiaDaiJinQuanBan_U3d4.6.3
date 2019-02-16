using UnityEngine;
using System.Collections.Generic;

public class XKPlayerFenShuCtrl : MonoBehaviour
{
	public GameObject PlayerFenShuPre;
    public UIAtlas m_FenShuAtlasP1;
    public UIAtlas m_FenShuAtlasP2;
    public UIAtlas m_FenShuAtlasP3;
    List<XKPlayerFenShuMove> FenShuListP1;
	List<XKPlayerFenShuMove> FenShuListP2;
	List<XKPlayerFenShuMove> FenShuListP3;
    int MaxPlayerFS = 8;
	static XKPlayerFenShuCtrl _Instance;
	public static XKPlayerFenShuCtrl GetInstance()
	{
		return _Instance;
	}
	// Use this for initialization
	void Start()
	{
		_Instance = this;
		FenShuListP1 = new List<XKPlayerFenShuMove>();
		FenShuListP2 = new List<XKPlayerFenShuMove>();
		FenShuListP3 = new List<XKPlayerFenShuMove>();
        GameObject obj = null;
        XKPlayerFenShuMove fenShuMoveCom = null;
        for (int i = 0; i < MaxPlayerFS; i++)
        {
            obj = (GameObject)Instantiate(PlayerFenShuPre);
            obj.transform.parent = transform;
            obj.transform.localScale = new Vector3(1f, 1f, 1f);
            obj.transform.localPosition = Vector3.zero;
            fenShuMoveCom = obj.GetComponent<XKPlayerFenShuMove>();
            if (fenShuMoveCom != null)
            {
                fenShuMoveCom.Init(m_FenShuAtlasP1);
                fenShuMoveCom.SetActive(false);
            }
            FenShuListP1.Add(fenShuMoveCom);
        }

        for (int i = 0; i < MaxPlayerFS; i++)
        {
            obj = (GameObject)Instantiate(PlayerFenShuPre);
            obj.transform.parent = transform;
            obj.transform.localScale = new Vector3(1f, 1f, 1f);
            obj.transform.localPosition = Vector3.zero;
            fenShuMoveCom = obj.GetComponent<XKPlayerFenShuMove>();
            if (fenShuMoveCom != null)
            {
                fenShuMoveCom.Init(m_FenShuAtlasP2);
                fenShuMoveCom.SetActive(false);
            }
            FenShuListP2.Add(fenShuMoveCom);
        }

        for (int i = 0; i < MaxPlayerFS; i++)
        {
            obj = (GameObject)Instantiate(PlayerFenShuPre);
            obj.transform.parent = transform;
            obj.transform.localScale = new Vector3(1f, 1f, 1f);
            obj.transform.localPosition = Vector3.zero;
            fenShuMoveCom = obj.GetComponent<XKPlayerFenShuMove>();
            if (fenShuMoveCom != null)
            {
                fenShuMoveCom.Init(m_FenShuAtlasP3);
                fenShuMoveCom.SetActive(false);
            }
            FenShuListP3.Add(fenShuMoveCom);
        }
    }
	
//	public Transform TestPlayerTr;
//	public Transform TestFenShuTr;
//	[Range(1, 999999)]public int FenShuTest = 1;
//	public Vector3 TestPos;
//	void OnGUI()
//	{
//		GUI.Box(new Rect(0f, 0f, 500f, 25f), TestPos.ToString());
//	}
	// Update is called once per frame
//	void Update()
//	{
//		if (Camera.main == null) {
//			return;
//		}
//		TestPlayerTr = XkPlayerCtrl.GetInstanceFeiJi().TestCubeTr;
//		Vector3 startPos = Camera.main.WorldToScreenPoint(TestPlayerTr.position);
//		startPos.z = 0f;
//		startPos.x = startPos.x < 0f ? 0f : startPos.x;
//		startPos.x = startPos.x > Screen.width ? Screen.width : startPos.x;
//		startPos.y = startPos.y < 0f ? 0f : startPos.y;
//		startPos.y = startPos.y > Screen.height ? Screen.height : startPos.y;
//
//		startPos.x *= (XkGameCtrl.ScreenWidth / Screen.width);
//		startPos.y *= (XkGameCtrl.ScreenHeight / Screen.height);
//		
//		startPos.x = startPos.x < 0f ? 0f : startPos.x;
//		startPos.x = startPos.x > XkGameCtrl.ScreenWidth ? XkGameCtrl.ScreenWidth : startPos.x;
//		startPos.y = startPos.y < 0f ? 0f : startPos.y;
//		startPos.y = startPos.y > XkGameCtrl.ScreenHeight ? XkGameCtrl.ScreenHeight : startPos.y;
//
//		int fenShuLen = FenShuTest.ToString().Length;
//		startPos.x += 9f * (fenShuLen - 1);
//		TestPos = startPos;
//		TestFenShuTr.localPosition = startPos;

//		if (Input.GetKeyDown(KeyCode.P)) {
//			//ShowPlayerFenShu(PlayerEnum.Null, FenShuTest);
//			ShowPlayerFenShu(PlayerEnum.PlayerOne, Random.Range(1, 999999));
//		}
//	}

	XKPlayerFenShuMove GetXKPlayerFenShuMove(PlayerEnum indexPlayer)
	{
        int indexVal = (int)indexPlayer - 1;
        if (indexVal < 0 || indexVal > 2)
        {
            return null;
        }

        List<XKPlayerFenShuMove> fenShuList = null;
        switch (indexPlayer)
        {
            case PlayerEnum.PlayerOne:
                {
                    fenShuList = FenShuListP1;
                }
                break;
            case PlayerEnum.PlayerTwo:
                {
                    fenShuList = FenShuListP2;
                }
                break;
            case PlayerEnum.PlayerThree:
                {
                    fenShuList = FenShuListP3;
                }
                break;
        }

        GameObject obj = null;
		int valTmp = 0;
		for (int i = 0; i < MaxPlayerFS; i++) {
			obj = fenShuList[i].gameObject;
			if (obj.activeSelf) {
				continue;
			}
			valTmp = i;
			break;
		}
		return fenShuList[valTmp];
	}

	public void ShowPlayerFenShu(PlayerEnum indexVal, int fenShuVal)
	{
		int indexPlayer = (int)indexVal - 1;
		indexPlayer = (indexPlayer < 0 || indexPlayer > 3) ? 0 : indexPlayer;
		int fenShuValTmp = (fenShuVal * XKDaoJuGlobalDt.FenShuBeiLv[indexPlayer]);
		if (fenShuValTmp <= 0) {
			return;
		}

		XkGameCtrl.PlayerJiFenArray[indexPlayer] += fenShuValTmp;
		XKPlayerScoreCtrl.ChangePlayerScore(indexVal);

		XKPlayerFenShuMove fenShuMoveCom = GetXKPlayerFenShuMove(indexVal);
		if (fenShuMoveCom == null) {
			return;
		}
		Transform playerTr = XKPlayerMoveCtrl.GetXKPlayerMoveCom(indexVal).PiaoFenPoint;
//		playerTr = TestPlayerTr; //test

		Vector3 startPos = XkGameCtrl.GetInstance().GetWorldObjToScreenPos(playerTr.position);
		int fenShuLen = fenShuVal.ToString().Length;
		startPos.x += 9f * (fenShuLen - 1);
		fenShuMoveCom.SetPlayerFenShuVal(fenShuVal, startPos, indexPlayer);
	}
}