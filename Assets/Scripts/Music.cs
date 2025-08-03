using Seb.Helpers;
using UnityEngine;

public class Music : MonoBehaviour
{
	public float crossfadeDuration = 1.5f;

	AudioSource sourceA;
	AudioSource sourceB;
	bool usingSourceA = true;
	bool muted = false;
	public float maxVol = 1;
	bool isCrossFade;
	[HideInInspector] public float volFac = 1;
	float blendFromVal;
	Coroutine x;

	void Awake()
	{
		sourceA = gameObject.AddComponent<AudioSource>();
		sourceB = gameObject.AddComponent<AudioSource>();

		sourceA.loop = true;
		sourceB.loop = true;
	}

	public void PlaySong(AudioClip newClip, float vol)
	{
		if (isCrossFade)
		{
			StopCoroutine(x);
		}
		
		AudioSource active = usingSourceA ? sourceA : sourceB;
		blendFromVal =active.volume;

		volFac = vol;

		//	Debug.Log(newClip);
		x = StartCoroutine(CrossfadeTo(newClip));
	}

	public void ToggleMute()
	{
		muted = !muted;
	}

	void Update()
	{
		if (!isCrossFade)
		{
			sourceA.volume = Mathf.Lerp(sourceA.volume, volCurrMax, Time.unscaledDeltaTime * 8);
			sourceB.volume =  Mathf.Lerp(sourceB.volume, volCurrMax, Time.unscaledDeltaTime * 8);
		}
	}

	private System.Collections.IEnumerator CrossfadeTo(AudioClip newClip)
	{
		AudioSource active = usingSourceA ? sourceA : sourceB;
		AudioSource next = usingSourceA ? sourceB : sourceA;

		next.clip = newClip;
		next.volume = 0f;
		next.Play();

		float t = 0f;
		isCrossFade = true;
		while (t < crossfadeDuration)
		{
			t += Time.unscaledDeltaTime;
			float blend = Mathf.Clamp01(t / crossfadeDuration);
			active.volume = Maths.EaseQuadIn(1f - blend) * blendFromVal;
			next.volume = Maths.EaseQuadIn(blend) * volCurrMax;
			yield return null;
		}

		isCrossFade = false;
		active.Stop();
		usingSourceA = !usingSourceA;
	}

	float volCurrMax => muted ? 0 : maxVol * volFac;
}