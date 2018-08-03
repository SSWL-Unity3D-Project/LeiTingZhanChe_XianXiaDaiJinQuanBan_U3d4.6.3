using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class XKTriggerSpawnNpc : MonoBehaviour
{
//	public XKSpawnNpcPointGroup SpawnPointGroup;
	public XKSpawnNpcPoint[] SpawnPointArray;
	bool IsFixedSpawnPoint;
	public AiPathCtrl TestPlayerPath;
	static bool IsDonnotSpawnNpcTest = false;
	void Start()
	{
		bool isWrong = false;
		for (int i = 0; i < SpawnPointArray.Length; i++) {
			if (SpawnPointArray[i] == null) {
				Debug.LogWarning("Unity:"+"SpawnPointArray was null! index "+i);
				GameObject obj = null;
				obj.name = "null";
				break;
			}

			SpawnPointArray[i].SetIsSpawnTrigger();
			isWrong = SpawnPointArray[i].AddTriggerSpawnScript(this);
			if (isWrong) {
				Debug.LogWarning("Unity:"+"SpawnPointArray was overlap! index "+i);
				GameObject obj = null;
				obj.name = "null";
				break;
			}
		}

		for (int i = 0; i < SpawnPointArray.Length; i++) {
			SpawnPointArray[i].ClearSpawnPointCheckList();
		}
		Invoke("DelayChangeBoxColliderSize", 0.2f);

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

	void DelayChangeBoxColliderSize()
	{
		BoxCollider boxCol = GetComponent<BoxCollider>();
		if (boxCol == null) {
			gameObject.SetActive(false);
			return;
		}
		XkGameCtrl.GetInstance().ChangeBoxColliderSize(transform);
	}

	public string TestNpcName;
	void OnTriggerEnter(Collider other)
	{
		if (IsDonnotSpawnNpcTest) {
			return; //test.
		}

		if (JiFenJieMianCtrl.GetInstance().GetIsShowFinishTask()) {
			return;
		}

		if (Network.peerType == NetworkPeerType.Client) {
			return;
		}

		XkPlayerCtrl ScriptPlayer = other.GetComponent<XkPlayerCtrl>();
		if (ScriptPlayer == null) {
			return;
		}

		//Debug.Log("Unity:"+"XKTriggerSpawnNpc::OnTriggerEnter -> hit "+other.name);
		for (int i = 0; i < SpawnPointArray.Length; i++) {
//			if (SpawnPointArray[i].NpcObj.name != TestNpcName || SpawnPointArray[i].NpcFangZhen != null) {
//					continue; //test
//			}
//
//			if (IsDonnotSpawnNpcTest) {
//				return;
//			}
//			IsDonnotSpawnNpcTest = true;
			SpawnPointArray[i].SpawnPointAllNpc();
		}
	}

	void CleanUpSpawnPointInfo()
	{
//		if (SpawnPointGroup == null) {
//			SpawnPointGroup = new GameObject();
//			SpawnPointGroup.transform.parent = transform.parent;
//			SpawnPointGroup.name = "SpawnPointGroup";
//		}
		
		for (int i = 0; i < SpawnPointArray.Length; i++) {
			if (SpawnPointArray[i] != null) {
				//SpawnPointArray[i].transform.parent = SpawnPointGroup.transform;
			}
		}

//		List<XKSpawnNpcPoint> spawnPointList = new List<XKSpawnNpcPoint>(SpawnPointArray){};
//		XKSpawnNpcPoint[] spawnPointArrayTmp = SpawnPointGroup.GetComponentsInChildren<XKSpawnNpcPoint>();
//		for (int i = 0; i < spawnPointArrayTmp.Length; i++) {
//			//Debug.Log("Unity:"+"name "+spawnPointArrayTmp[i].name);
//			if (spawnPointList.Contains(spawnPointArrayTmp[i])) {
//				continue;
//			}
//			spawnPointArrayTmp[i].gameObject.SetActive(false);
//			spawnPointArrayTmp[i].name = "NpcSpawnPoint_XX";
//		}
	}

	public void MoveSpawnPointArrayToSpawnPointList()
	{
		if (TriggerSpawnList == null || TriggerSpawnList.SpawnPointGroup == null) {
			return;
		}

		Transform groupTr = TriggerSpawnList.SpawnPointGroup.transform;
		for (int i = 0; i < SpawnPointArray.Length; i++) {
			if (SpawnPointArray[i] != null) {
				SpawnPointArray[i].transform.parent = groupTr;
			}
		}
	}

	void RenameNpcSpawnPoint()
	{
		int countPoint = 0;
		for (int i = 0; i < SpawnPointArray.Length; i++) {
			if (SpawnPointArray[i] != null) {
				SpawnPointArray[i].name = "NpcSpawnPoint_"+countPoint;
				countPoint++;
			}
		}
	}

	void FixedSpawnPointInfo()
	{
		List<XKSpawnNpcPoint> pointList = new List<XKSpawnNpcPoint>(SpawnPointArray){};
		bool isFindNull = true;
		do {
			isFindNull = false;
			for (int i = 0; i < pointList.Count; i++) {
				if (pointList[i] == null) {
					isFindNull = true;
					pointList.RemoveAt(i);
					break;
				}
			}

			if (!isFindNull) {
				break;
			}
		} while (isFindNull);

		SpawnPointArray = pointList.ToArray();
	}

	public void CheckSpawnPointInfo()
	{
		FixedSpawnPointInfo();
		CleanUpSpawnPointInfo();
		RenameNpcSpawnPoint();
	}

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
	{
		DrawGizmos(true);
	}
#endif

    void DrawGizmos(bool isSelect)
	{
		if (!isSelect) {
			return;
		}
		TriggerSpawnList.TriggerSpawn = this;

		if (!XkGameCtrl.IsDrawGizmosObj) {
			return;
		}
		
		if (!enabled) {
			return;
		}
		
		if (SpawnPointArray != null) {
			for (int i = 0; i < SpawnPointArray.Length; i++) {
				if (SpawnPointArray[i] == null) {
					Debug.LogWarning("Unity:"+"SpawnPointArray was wrong! index "+i);
//					GameObject obj = null;
//					obj.name = "null";
					break;
				}
				SpawnPointArray[i].AddTestTriggerSpawnNpc(this);
			}
		}

		if (IsFixedSpawnPoint) {
			IsFixedSpawnPoint = false;
			CheckSpawnPointInfo();
		}
		
		if (TestPlayerPath != null) {
			TestPlayerPath.DrawPath();
		}
	}
//#endif

	public void CleanEmptySpawnPointGroup()
	{
		Transform trTmp = null;
		XKSpawnNpcPoint[] spawnScriptArray = null;
		XKSpawnNpcPointGroup[] spawnPointGroupArray = GameObject.FindObjectsOfType(typeof(XKSpawnNpcPointGroup)) as XKSpawnNpcPointGroup[];
		if (spawnPointGroupArray == null) {
			//Debug.Log("Unity:"+"CleanEmptySpawnPointGroup -> spawnPointGroupArray is null");
			return;
		}
		//Debug.Log("Unity:"+"CleanEmptySpawnPointGroup -> spawnPointGroupArray.Length "+spawnPointGroupArray.Length);
		List<XKSpawnNpcPointGroup> spawnPointGroupList = new List<XKSpawnNpcPointGroup>(spawnPointGroupArray){};

		bool isFindEmptySpawnPoint = false;
		do {
			isFindEmptySpawnPoint = false;

			for (int i = 0; i < spawnPointGroupList.Count; i++) {
				trTmp = spawnPointGroupList[i].transform;
				trTmp.gameObject.layer = LayerMask.NameToLayer("TransparentFX");
				if (trTmp.childCount <= 0) {
					isFindEmptySpawnPoint = true;
				}
				else {
					Transform[] trArrayTmp = trTmp.GetComponentsInChildren<Transform>();
					List<Transform> trListTmp = new List<Transform>(trArrayTmp){};
					for (int j = 1; j < trListTmp.Count; j++) {
						XKSpawnNpcPoint spawnPointTmp = trListTmp[j].GetComponent<XKSpawnNpcPoint>();
						if (spawnPointTmp == null) {
							trListTmp[j].parent = trTmp.parent;
						}
					}
					
					spawnScriptArray = spawnPointGroupList[i].gameObject.GetComponentsInChildren<XKSpawnNpcPoint>();
					if (spawnScriptArray.Length <= 0) {
						isFindEmptySpawnPoint = true;
					}
				}

				if (isFindEmptySpawnPoint) {
					DestroyImmediate(spawnPointGroupList[i].gameObject);
					spawnPointGroupList.RemoveAt(i);
					break;
				}
			}

		} while(isFindEmptySpawnPoint);
	}

	public void RemoveCartoonSpawnNpc()
	{
		int max = SpawnPointArray.Length;
		for (int i = 0; i < max; i++) {
			SpawnPointArray[i].RemovePointAllNpc();
		}
	}

	public SpawnPointList TriggerSpawnList = new SpawnPointList();
	[Serializable]
	public class SpawnPointList
	{
		public XKTriggerSpawnNpc TriggerSpawn;
		public XKSpawnNpcPointGroup SpawnPointGroup;
		public Transform[] SpawnPointArray = new Transform[0];
	}
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof (XKTriggerSpawnNpc.SpawnPointList))]
public class SpawnPointListDrawer : PropertyDrawer
{
	bool IsTest = false;
	const float lineHeight = 18f;
	const float spacing = 4f;
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		if (!IsTest) {
			return;
		}
		EditorGUI.BeginProperty(position, label, property);
		float x = position.x;
		float y = position.y;
		float inspectorWidth = position.width;
		
		// Draw label
		// Don't make child fields be indented
		var indent = EditorGUI.indentLevel;
		EditorGUI.indentLevel = 0;
		
		var items = property.FindPropertyRelative("SpawnPointArray");
		var titles = new string[] {"SpawnPointArray:", "", "", ""};
		var props = new string[] {"XKSpawnNpcPoint", "-"};
		var widths = new float[] {.7f, .1f, .1f, .1f};
		bool changedLength = false;
		if (items.arraySize > 0)
		{
			for (int i = -1; i < items.arraySize; ++i)
			{
				var item = items.GetArrayElementAtIndex(i);
				
				float rowX = x;
				for (int n = 0; n < props.Length; ++n)
				{
					float w = widths[n]*inspectorWidth;
					
					// Calculate rects
					Rect rect = new Rect(rowX, y, w, lineHeight);
					rowX += w;
					
					if (i == -1)
					{
						EditorGUI.LabelField(rect, titles[n]);
					}
					else
					{
						if (n == 0)
						{
							EditorGUI.ObjectField(rect, item.objectReferenceValue, typeof (Transform), true);
						}
						else
						{
							if (GUI.Button(rect, props[n]))
							{
								switch (props[n])
								{
								case "-":
									int sizeTmp = items.arraySize;
									items.DeleteArrayElementAtIndex(i);
									if (sizeTmp == items.arraySize) {
										items.DeleteArrayElementAtIndex(i);
									}
									changedLength = true;
									break;
								}
							}
						}
					}
				}
				
				y += lineHeight + spacing;
				if (changedLength)
				{
					break;
				}
			}
		}
		else
		{
			// add button
//			var addButtonRect = new Rect(x, y, inspectorWidth, lineHeight);
//			if (GUI.Button(addButtonRect, "Add SpawnPointArray"))
//			{
//				items.InsertArrayElementAtIndex(items.arraySize);
//			}
//			y += lineHeight + spacing;
		}
		var pointGroupSP = property.FindPropertyRelative("SpawnPointGroup");
		if (pointGroupSP != null) {
			float widthVal = 140f;
			Rect rect = new Rect(x, y, widthVal, lineHeight);
			EditorGUI.LabelField(rect, "SpawnPointGroup");

			rect = new Rect(widthVal, y, inspectorWidth - widthVal + 15f, lineHeight);
			EditorGUI.ObjectField(rect, pointGroupSP.objectReferenceValue, typeof (XKSpawnNpcPointGroup), true);
			y += lineHeight + spacing;
		}

		// add all button
		var addAllButtonRect = new Rect(x, y, inspectorWidth, lineHeight);
		if (GUI.Button(addAllButtonRect, "Create SpawnPointGroup"))
		{
			var triggerSpawn = property.FindPropertyRelative("TriggerSpawn").objectReferenceValue as XKTriggerSpawnNpc;
			if (triggerSpawn != null) {
				GameObject pointGroup = null;
				if (triggerSpawn.TriggerSpawnList.SpawnPointGroup == null) {
					bool isFindPointGroup = false;
					for (int i = 0; i < triggerSpawn.SpawnPointArray.Length; i++) {
						if (triggerSpawn.SpawnPointArray[i] != null
						    && triggerSpawn.SpawnPointArray[i].transform.parent != null
						    && triggerSpawn.SpawnPointArray[i].transform.parent.name == "SpawnPointGroup") {
							isFindPointGroup = true;
							pointGroup = triggerSpawn.SpawnPointArray[i].transform.parent.gameObject;
							break;
						}
					}

					if (!isFindPointGroup) {
						pointGroup = new GameObject();
					}
					pointGroup.name = "SpawnPointGroup";
					pointGroup.transform.parent = triggerSpawn.transform.parent;
//					pointGroup.transform.localPosition = Vector3.zero;
//					pointGroup.transform.localEulerAngles = Vector3.zero;
					pointGroup.layer = LayerMask.NameToLayer("TransparentFX");

					XKSpawnNpcPointGroup groupScript = pointGroup.GetComponent<XKSpawnNpcPointGroup>();
					if (groupScript == null) {
						triggerSpawn.TriggerSpawnList.SpawnPointGroup = pointGroup.AddComponent<XKSpawnNpcPointGroup>();
					}
					else {
						triggerSpawn.TriggerSpawnList.SpawnPointGroup = groupScript;
					}
					triggerSpawn.MoveSpawnPointArrayToSpawnPointList();
				}
				else {
					pointGroup = triggerSpawn.TriggerSpawnList.SpawnPointGroup.gameObject;
					pointGroup.layer = LayerMask.NameToLayer("TransparentFX");
					triggerSpawn.MoveSpawnPointArrayToSpawnPointList();
				}

//				if (triggerSpawn.SpawnPointGroup != null) {
//					//Debug.Log("Unity:"+"*********** pointGroup "+triggerSpawn.SpawnPointGroup);
//					pointGroup = triggerSpawn.SpawnPointGroup.gameObject;
//					var children = new Transform[pointGroup.transform.childCount];
//					int n = 0;
//					foreach (Transform child in pointGroup.transform) {
//						children[n++] = child;
//					}
//					
//					triggerSpawn.TriggerSpawnList.SpawnPointArray = new Transform[children.Length];
//					for (n = 0; n < children.Length; ++n) {
//						triggerSpawn.TriggerSpawnList.SpawnPointArray[n] = children[n];
//					}
//				}
			}
		}
		y += lineHeight + spacing;

		addAllButtonRect = new Rect(x, y, inspectorWidth, lineHeight);
		if (GUI.Button(addAllButtonRect, "Clean Empty SpawnPointGroup"))
		{
			var triggerSpawn = property.FindPropertyRelative("TriggerSpawn").objectReferenceValue as XKTriggerSpawnNpc;
			if (triggerSpawn != null) {
				triggerSpawn.CleanEmptySpawnPointGroup();
			}
		}
		y += lineHeight + spacing;
		
		// Set indent back to what it was
		EditorGUI.indentLevel = indent;
		EditorGUI.EndProperty();
	}

	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	{
		if (!IsTest) {
			return 0f;
		}

		SerializedProperty items = property.FindPropertyRelative("SpawnPointArray");
		float lineAndSpace = lineHeight + spacing;
		int btCount = 2;
		float heightUI = (items.arraySize*lineAndSpace)
							+ (btCount*lineAndSpace)
							+ lineAndSpace;
		if (items.arraySize > 0) {
			heightUI += lineAndSpace;
		}
		return heightUI;
	}
}
#endif