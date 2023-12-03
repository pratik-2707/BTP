 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    public GameObject cube;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("This is empty game object speaking\n");
        cube = GameObject.Find("Cube");
        
    }

    // Update is called once per frame
    void Update()
    {
        cube.SetActive(false);
        
    }
}
