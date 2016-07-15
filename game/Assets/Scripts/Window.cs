using UnityEngine;
using System.Collections;

public class Window : MonoBehaviour {

    public ParticleSystem wind;
    ParticleSystem.ShapeModule shape;
    
    public void breakWindow()
    {
        wind.Play();
    }   
}
