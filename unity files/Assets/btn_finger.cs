using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class btn_finger : MonoBehaviour
{
    public Sprite[] finger_sprites;
    public int index = 2;
    public void change_finger()
    {
        int n = finger_sprites.Length;
        index = (index+1)%n;
        for(int i=0;i<5;i++){
            string location = "/Canvas/finger"+i;
            Debug.Log(location);
            Image finger_img = GameObject.Find(location).GetComponent<Image> ();
            finger_img.sprite = finger_sprites[index];
        }
    }
}
