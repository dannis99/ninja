using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class LoadingController : MonoBehaviour {
	public GameObject loadingPanel;

	private static LoadingController _instance;
	public static LoadingController instance
	{
		get
		{
			if(_instance == null)
			{
				_instance = GameObject.FindObjectOfType<LoadingController>();
				DontDestroyOnLoad(_instance.gameObject);
			}
			return _instance;
		}
	}

	void Awake () {
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

	void Update()
	{
		
	}
	
	public void showLoading()
	{
		loadingPanel.SetActive(true);
	}
	
	public void hideLoading()
	{
		loadingPanel.SetActive(false);
	}
}
