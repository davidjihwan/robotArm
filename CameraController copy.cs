using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class CameraController : MonoBehaviour
{
    [SerializeField]
    private GameObject pivot;
    [SerializeField]
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MoveAround(float angle){
        transform.RotateAround(pivot.transform.position, Vector3.up, angle);
    }
}
