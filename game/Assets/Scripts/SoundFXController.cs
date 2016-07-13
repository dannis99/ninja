using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class MusicController : MonoBehaviour {

	public AudioSource audioSource;

	private enum Fade {In, Out};
	private float fadeTime = .5f;
	private AudioClip nextClip;
	
	private static MusicController _instance;
	public static MusicController instance
	{
		get
		{
			if(_instance == null)
			{
				_instance = GameObject.FindObjectOfType<MusicController>();
				DontDestroyOnLoad(_instance.gameObject);
			}
			return _instance;
		}
	}
	
	void Awake()
	{
		if(_instance == null)
		{
			_instance = this;
			DontDestroyOnLoad(this);
		}
		else
		{
			if(this != _instance)
				DestroyImmediate(this.gameObject);
		}
	}
	
	void Start()
	{
		
	}
	
	public void playMusic(AudioClip audioClip)
	{
		nextClip = audioClip;
		if(audioSource.isPlaying)
		{
			StartCoroutine(FadeAudio(fadeTime, Fade.Out));
			Invoke("setAudio", fadeTime);
		}
		else
		{
			audioSource.clip = nextClip;
			audioSource.Play();
		}
	}
	
	private void setAudio()
	{	
		audioSource.clip = nextClip;
		audioSource.Play();
		StartCoroutine(FadeAudio(fadeTime, Fade.In));
	}
	
	IEnumerator FadeAudio (float timer, Fade fadeType) {
		float start = fadeType == Fade.In? 0.0f : 1.0f;
		float end = fadeType == Fade.In? 1.0f : 0.0f;
		float i = 0.0f;
		float step = 1.0f/timer;
		
		while (i <= 1.0) {
			i += step * Time.deltaTime;
			audioSource.volume = Mathf.Lerp(start, end, i);
			yield return null;
		}
	}
}
