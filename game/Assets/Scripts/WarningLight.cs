using UnityEngine;
using System.Collections;

public class WarningLight : MonoBehaviour {

    public Color onColor;
    public Color offColor;
    public SpriteRenderer warningRenderer;
    public Light warningLight;
    bool inWarningMode = false;
    float t;
    private float timeFluctuation = 2f;

    void Update () {
	    if(inWarningMode)
        {
            t += Time.deltaTime;
            float timeDiff = t % timeFluctuation;
            if (timeDiff > timeFluctuation / 2f)
            {
                warningRenderer.color = Color.Lerp(onColor, offColor, timeDiff - Mathf.Floor(timeDiff));
                warningLight.color = Color.Lerp(onColor, offColor, timeDiff - Mathf.Floor(timeDiff));
            }
            else
            {
                warningRenderer.color = Color.Lerp(offColor, onColor, timeDiff - Mathf.Floor(timeDiff));
                warningLight.color = Color.Lerp(offColor, onColor, timeDiff - Mathf.Floor(timeDiff));
            }
        }
	}

    public void beginWarning()
    {
        inWarningMode = true;
        warningLight.enabled = true;
    }
}
