using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalSense : MonoBehaviour
{

    public SceneController sc;
    [SerializeField]
    private Material capturedMat;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other) {
        Debug.Log("Entered");
        Debug.Log(other.gameObject.tag);
        if (other.gameObject.tag == "Goal"){
            other.gameObject.GetComponent<Renderer>().material = capturedMat;
            sc.LoadNextLevel();
        }
    }
}
