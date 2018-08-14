using UnityEngine;
using System.Collections;

public class AudioBeiJingCtrl : MonoBehaviour
{
	[Range(0f, 100f)] public float TimeNextBeiJiAudio = 1f;
	public static int IndexBeiJingAd = 0;
	float TimeDuration = 1f;
	void Awake()
	{
		TweenVolume tVol = gameObject.GetComponent<TweenVolume>();
		if (tVol != null) {
			TimeDuration = tVol.duration;
			DestroyObject(tVol);
		}

        if (audio != null)
        {
            m_VolumeStart = audio.volume;
        }
    }

    internal float m_VolumeStart = 1f;
	public void MakeBeiJiAudioDownVolum()
	{
        TweenVolume tVol = gameObject.GetComponent<TweenVolume>();
        if (tVol != null)
        {
            DestroyObject(tVol);
        }

        tVol = gameObject.AddComponent<TweenVolume>();
		if (tVol != null) {
			tVol.from = m_VolumeStart;
			tVol.to = 0f;
			tVol.duration = TimeDuration;
			EventDelegate.Add(tVol.onFinished, delegate{
				AudioVolumeDownEnd();
			});
			tVol.enabled = true;
		}
	}

	public static void StopGameBeiJingAudio()
	{
		AudioListCtrl.StopLoopAudio(AudioListCtrl.GetInstance().ASGuanKaBJ[IndexBeiJingAd], 2);
	}

	void AudioVolumeDownEnd()
	{
		CancelInvoke("DelayPlayNextBeiJingAudio");
		Invoke("DelayPlayNextBeiJingAudio", TimeNextBeiJiAudio);
	}

	void DelayPlayNextBeiJingAudio()
    {
        TweenVolume tVol = gameObject.GetComponent<TweenVolume>();
        if (tVol != null)
        {
            DestroyObject(tVol);
        }

        AudioListCtrl.StopLoopAudio(AudioListCtrl.GetInstance().ASGuanKaBJ[IndexBeiJingAd]);
        AudioListCtrl.GetInstance().ASGuanKaBJ[IndexBeiJingAd].volume = m_VolumeStart;

        IndexBeiJingAd++;
		if (IndexBeiJingAd >= AudioListCtrl.GetInstance().ASGuanKaBJ.Length) {
			IndexBeiJingAd = 0;
		}
		//Debug.Log("Unity:"+"DelayPlayNextBeiJingAudio -> IndexBeiJingAd "+IndexBeiJingAd);
		XKGlobalData.GetInstance().PlayGuanKaBeiJingAudio(IndexBeiJingAd);
	}
}