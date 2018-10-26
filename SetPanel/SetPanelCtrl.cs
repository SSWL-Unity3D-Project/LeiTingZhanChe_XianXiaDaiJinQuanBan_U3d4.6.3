using System;
using UnityEngine;

public class SetPanelCtrl : MonoBehaviour
{
	static private SetPanelCtrl Instance = null;
	static public SetPanelCtrl GetInstance()
	{
		if(Instance == null)
		{
			GameObject obj = new GameObject("_SetPanelCtrl");
			Instance = obj.AddComponent<SetPanelCtrl>();
		}
		return Instance;
	}

	// Use this for initialization
	void Start()
	{
        //Debug.Log("Unity:"+"SetPanelCtrl::init...");
        InputEventCtrl.GetInstance().ClickSetMoveBtEvent += ClickSetMoveBtEvent;
        InputEventCtrl.GetInstance().ClickSetEnterBtEvent += ClickSetEnterBtEvent;
    }

    private void ClickSetMoveBtEvent(pcvr.ButtonState val)
    {
        if (val == pcvr.ButtonState.UP)
        {
            return;
        }
        
        if (SSUIRoot.GetInstance().m_GameUIManage != null)
        {
            SSUIRoot.GetInstance().m_GameUIManage.CreatGamePayDataPanel();
        }
    }

    void ClickSetEnterBtEvent(pcvr.ButtonState val)
	{
		if (HardwareCheckCtrl.IsTestHardWare) {
			return;
		}

		if (val == pcvr.ButtonState.UP) {
			return;
		}

        if (Application.loadedLevel == (int)GameLevel.SetPanel) {
			return;
		}
		loadLevelSetPanel();
	}

    void loadLevelSetPanel()
	{
		if (XkGameCtrl.IsLoadingLevel) {
//			Debug.Log("Unity:"+"*************Loading...");
			return;
		}
		
		if (!XkGameCtrl.IsGameOnQuit) {
			GC.Collect();
			Application.LoadLevel( (int)GameLevel.SetPanel );
		}
	}
}
