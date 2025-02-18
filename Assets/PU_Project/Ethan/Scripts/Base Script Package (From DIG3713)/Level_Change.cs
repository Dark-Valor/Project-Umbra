using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// Make sure to add this, or you can't use SceneManager
using UnityEngine.SceneManagement;


public class Level_Change : MonoBehaviour
{
    [Tooltip("Check this if you want to load the scene by name")]
    public bool loadByName = true;
    [Tooltip("Name of scene to load - must check loadByName to use this")]
    public string sceneName;
    [Tooltip("Scene index from the build settings - only needed if not using name")]
    public int sceneIndex = 0;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.name == "Player" || other.CompareTag("Player"))
        {
            if (loadByName)
            {
                SceneManager.LoadScene(sceneName);
            }
            else
            {
                // The scene number to load (in File->Build Settings)
                SceneManager.LoadScene(sceneIndex);
            }
        }
    }
}