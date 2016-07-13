using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MenuController : MonoBehaviour {

	private GameController controller;
	public GameObject menu;
	
	void Start()
	{
		controller = GameObject.FindObjectOfType<GameController>();
	}
	
	public void showMenu()
	{
		menu.SetActive(true);
	}
	
	public void hideMenu()
	{
		menu.SetActive(false);
	}
	
	public void quit()
	{
		controller.gameOver();
		hideMenu();
	}
	
	public void toggleMenu()
	{
		if(menu.activeSelf)
		{
			hideMenu();
		}
		else
		{
			showMenu();
		}
	}
}
