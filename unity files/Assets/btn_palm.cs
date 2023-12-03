using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class btn_palm : MonoBehaviour
{
    public Sprite[] palm_sprites;
    public int index = 2;
    // public Sprite[] fingerSprites;
    public void change_palm()
    {
        // public GameObject palm = GameObject.Find("/Canvas/palm");
        Image image = GameObject.Find("/Canvas/palm").GetComponent<Image> ();
        Sprite sprite = Resources.Load<Sprite>("Sprites/finger_transparent4.png");
        Debug.Log("Button Clicked!"+" Old image instance ID: "+image.GetInstanceID());
        int n = palm_sprites.Length;
        index = (index+1)%n;
        image.sprite = palm_sprites[index];
        Debug.Log("New image instance ID: "+image.GetInstanceID());
    }
    
}
