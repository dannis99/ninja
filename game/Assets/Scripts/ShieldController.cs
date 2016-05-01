using UnityEngine;
using System.Collections;

public class ShieldController : MonoBehaviour {

	public bool shielded;
	public SpriteRenderer headShield;
	public SpriteRenderer bodyShield;

	public Color firstColor;
	public Color secondColor;
	Color fromColor;
	Color toColor;
	bool movingToFirst;
	float t;

	// Use this for initialization
	void Start () {
		headShield.color = secondColor;
		bodyShield.color = secondColor;
	}
	
	// Update is called once per frame
	void Update () {
		//checking shields
		if(shielded)
		{
			headShield.enabled = true;
			bodyShield.enabled = true;

			if (headShield.color == firstColor && movingToFirst) 
			{
				t = 0;
				movingToFirst = false;
				fromColor = firstColor;
				toColor = secondColor;
			}
			else if (headShield.color == secondColor && !movingToFirst) 
			{
				t = 0;
				movingToFirst = true;
				fromColor = secondColor;
				toColor = firstColor;
			}

			t += Time.deltaTime;
			headShield.color = Color.Lerp(fromColor,toColor, t);
			bodyShield.color = Color.Lerp(fromColor,toColor, t);
		}
		else
		{
			headShield.enabled = false;
			bodyShield.enabled = false;
		}
	}
}
