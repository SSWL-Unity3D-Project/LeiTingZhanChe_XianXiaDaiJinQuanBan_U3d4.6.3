//#define DRAW_FPS
using UnityEngine;

public class XKGameFPSCtrl : MonoBehaviour
{
    public static bool IsShowGameFPS;
#if !DRAW_FPS
    void Start()
    {
        Destroy(gameObject);
    }
#endif

#if DRAW_FPS
    /// <summary>
    /// The update interval.
    /// </summary>
    public readonly float UpdateInterval = 0.5f;
	
	/// <summary>
	/// The accum.
	/// </summary>
	private float accum; // FPS accumulated over the interval
	
	/// <summary>
	/// The frames.
	/// </summary>
	private int frames; // Frames drawn over the interval
	
	/// <summary>
	/// The timeleft.
	/// </summary>
	private float timeleft; // Left time for current interval
	static float FPSVal = 60f;
	static Color FPSColorVal = Color.green;
	public void Start()
    {
#if UNITY_STANDALONE_WIN
        //Application.targetFrameRate = 100;
#endif

#if UNITY_ANDROID
        //Application.targetFrameRate = 60;
#endif
        this.timeleft = this.UpdateInterval;
		gameObject.SetActive(IsShowGameFPS);
		InputEventCtrl.GetInstance().ClickSetMoveBtEvent += ClickSetMoveBtEvent;
	}
	
	public void ClickSetMoveBtEvent(ButtonState val)
	{
		if (val == ButtonState.DOWN) {
			return;
		}
		IsShowGameFPS = !IsShowGameFPS;
		gameObject.SetActive(IsShowGameFPS);
	}

    void Update()
    {
        this.timeleft -= Time.deltaTime;
        this.accum += Time.timeScale / Time.deltaTime;
        ++this.frames;

        // Interval ended - update GUI text and start new interval
        if (this.timeleft <= 0.0)
        {
            // display two fractional digits (f2 format)
            float fps = this.accum / this.frames;
            if (pcvr.bIsHardWare)
            {
                //if (fps < 30f) {
                //	fps = UnityEngine.Random.Range(0, 100) % 6 + 30f;
                //}
            }
            else
            {
                if (fps < 10f)
                {
                    if (FPSColorVal != Color.red)
                    {
                        FPSColorVal = Color.red;
                    }
                }
                else if (fps < 30f)
                {
                    if (FPSColorVal != Color.yellow)
                    {
                        FPSColorVal = Color.yellow;
                    }
                }
                else
                {
                    if (FPSColorVal != Color.green)
                    {
                        FPSColorVal = Color.green;
                    }
                }
            }

            FPSVal = fps;
            this.timeleft = this.UpdateInterval;
            this.accum = 0.0f;
            this.frames = 0;
        }
    }

	void OnGUI()
	{
		DrawGameFPS();
	}
	
	public static void DrawGameFPS()
	{
		if (!IsShowGameFPS)
        {
			return;
		}
        GUI.Box(new Rect(25f, 80f, 200f, 50f), "");
        GUI.color = FPSColorVal;
		GUI.Label(new Rect(25f, 80f, 200f, 25f), String.Format("FPS: {0:F0}", FPSVal));
		GUI.Label(new Rect(25f, 105f, 200f, 25f), XKGameVersionCtrl.GameVersion);
	}
#endif
}