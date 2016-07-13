using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class SoundFXController : MonoBehaviour {

	public AudioSource audioSource;
	public AudioClip buttonClickClip;
	public AudioClip turnTransitionClip;
	public AudioClip stairsClip;
	public AudioClip spellActiveClip;
	public AudioClip spellDeactiveClip;
	public AudioClip levelUpClip;
	public AudioClip chestOpenClip;
	public AudioClip chestLockedClip;

	private static SoundFXController _instance;
	public static SoundFXController instance
	{
		get
		{
			if(_instance == null)
			{
				_instance = GameObject.FindObjectOfType<SoundFXController>();
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
	
	public void playSound(AudioClip audioClip)
	{
		audioSource.PlayOneShot(audioClip);
	}
	
	public void buttonClick()
	{
		audioSource.PlayOneShot(buttonClickClip);
	}
	
	public void turnTransition()
	{
		audioSource.PlayOneShot(turnTransitionClip);
	}
	
	public void stairs()
	{
		audioSource.PlayOneShot(stairsClip);
	}
	
	public void spellActive()
	{
		audioSource.PlayOneShot(spellActiveClip);
	}
	
	public void spellDeactive()
	{
		audioSource.PlayOneShot(spellDeactiveClip);
	}
	
	public void levelUp()
	{
		audioSource.PlayOneShot(levelUpClip);
	}
	
	public void chestOpen()
	{
		audioSource.PlayOneShot(chestOpenClip);
	}
	
	public void chestLocked()
	{
		audioSource.PlayOneShot(chestLockedClip);
	}
}
