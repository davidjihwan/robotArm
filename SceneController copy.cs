using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    
    // public Animator transition;
    [SerializeField]
    private float transitionTime;
    [SerializeField]
    private GameObject winText;
    // private int lastReached = 0;

    // public void Reset(){
    //     // TODO: Shader 
    //     StartCoroutine(LoadLevel(SceneManager.GetActiveScene().buildIndex));
    //     // TODO: Shader
    // }

    // public void Exit(){
    //     // TODO: Shader 
    //     StartCoroutine(LoadLevel(0));
    //     // TODO: Shader
    // }

    // public void Continue(){
    //     // TODO: Shader 
    //     StartCoroutine(LoadLevel(lastReached));
    //     // TODO: Shader
    // }

    public void LoadNextLevel(){
        StartCoroutine(LoadLevel(SceneManager.GetActiveScene().buildIndex + 1));
    }

    IEnumerator LoadLevel(int index){ // coroutine
        // play animation
        // if (index > lastReached){
        //     lastReached = index;
        // }
        // transition.SetTrigger("Start");

        if (index < SceneManager.sceneCountInBuildSettings){
            yield return new WaitForSeconds(transitionTime);
            SceneManager.LoadScene(index);
        } else {
            winText.SetActive(true);
        }
    }

}
