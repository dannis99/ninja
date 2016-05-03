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

	float tScale;
	Color scaledColor;
	Vector3 startScale = new Vector3(1.0f, 1.0f, 1.0f);
	Vector3 endScale = new Vector3(2.0f, 2.0f, 1.0f);

	// Use this for initialization
	void Start () {
		headShield.color = secondColor;
		bodyShield.color = secondColor;
		scaledColor = new Color(firstColor.r, firstColor.g, firstColor.b, 0f);
	}
	
	// Update is called once per frame
	void Update () {
		//checking shields
		if(shielded)
		{
			headShield.enabled = true;
			bodyShield.enabled = true;
			headShield.transform.localScale = startScale;
			bodyShield.transform.localScale = startScale;

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
			if(headShield.transform.localScale != endScale)
			{
				tScale += Time.deltaTime;
				headShield.transform.localScale = Vector3.Lerp(startScale, endScale, tScale);
				bodyShield.transform.localScale = Vector3.Lerp(startScale, endScale, tScale);
				headShield.color = Color.Lerp(firstColor,scaledColor, tScale);
				bodyShield.color = Color.Lerp(firstColor,scaledColor, tScale);
			}
			else
			{
				tScale = 0;
				headShield.enabled = false;
				bodyShield.enabled = false;
			}
		}
	}
}
