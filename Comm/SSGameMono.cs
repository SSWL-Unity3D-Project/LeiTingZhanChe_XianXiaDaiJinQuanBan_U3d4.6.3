using UnityEngine;

public class SSGameMono : MonoBehaviour
{
    /// <summary>
    /// 产生预制.
    /// </summary>
    public Object Instantiate(GameObject prefab, Transform parent, Transform trPosRot = null)
    {
        GameObject obj = (GameObject)Instantiate(prefab);
        if (parent != null)
        {
            obj.transform.SetParent(parent);
            if (trPosRot == null)
            {
                obj.transform.localScale = prefab.transform.localScale;
                obj.transform.localPosition = prefab.transform.localPosition;
            }
            else
            {
                obj.transform.position = trPosRot.position;
                obj.transform.rotation = trPosRot.rotation;
            }
        }
        return obj;
    }

    public void UnityLog(object msg)
    {
        Debug.Log("Unity: " + msg);
    }

    public void UnityLogWarning(object msg)
    {
        Debug.LogWarning("Unity: " + msg);
    }

    public void UnityLogError(object msg)
    {
        Debug.LogError("Unity: " + msg);
    }
}