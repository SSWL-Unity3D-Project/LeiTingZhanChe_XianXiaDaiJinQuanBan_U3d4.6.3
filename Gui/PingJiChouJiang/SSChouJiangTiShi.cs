using UnityEngine;

public class SSChouJiangTiShi : MonoBehaviour
{
    /// <summary>
    /// 位移时间.
    /// </summary>
    [Range(0.1f, 10f)]
    public float PiaoZiTime = 0.5f;
    /// <summary>
    /// 位移的高度.
    /// </summary>
    [Range(10f, 500f)]
    public float PiaoZiPY = 100f;
    /// <summary>
    /// 设定大小.
    /// </summary>
    public Vector2 LocalScale = new Vector2(1f, 1f);
    /// <summary>
    /// 显示抽奖提示.
    /// </summary>
    public void ShowChouJiangTiShi(Vector3 startPos)
    {
        startPos.y += XKDaoJuGlobalDt.GetInstance().DaoJuMaoZiPY;
        transform.localPosition = startPos;
        transform.localEulerAngles = Vector3.zero;
        transform.localScale = new Vector3(LocalScale.x, LocalScale.y, 1f);
        gameObject.SetActive(true);
        TweenPosition twPos = gameObject.AddComponent<TweenPosition>();
        twPos.from = startPos;
        twPos.to = startPos + new Vector3(0f, PiaoZiPY, 0f);
        twPos.duration = PiaoZiTime;
        twPos.PlayForward();

        EventDelegate.Add(twPos.onFinished, delegate {
            StartPlayTweenAlpha();
        });

        StartCoroutine(RemoveSelf());
    }

    /// <summary>
    /// 开始播放淡化效果.
    /// </summary>
    void StartPlayTweenAlpha()
    {
        TweenAlpha twAlp = gameObject.AddComponent<TweenAlpha>();
        twAlp.from = 1f;
        twAlp.to = 0f;
        twAlp.duration = 3f;
        twAlp.PlayForward();
    }

     System.Collections.IEnumerator RemoveSelf()
    {
        float time = 3f + PiaoZiTime;
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }
}
