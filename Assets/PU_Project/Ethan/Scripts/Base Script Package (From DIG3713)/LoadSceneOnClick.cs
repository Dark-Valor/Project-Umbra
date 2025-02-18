using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneOnClick : MonoBehaviour {

    [Tooltip("Check this if you want to load the scene by name")]
    public bool loadByName = true;
    [Tooltip("Name of scene to load - must check loadByName to use this")]
    public string sceneName;
    [Tooltip("Scene index from the build settings - only needed if not using name")]
    public int sceneIndex = 0;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            LoadScene();
        }
    }

    public void LoadScene()
    {
        if (loadByName)
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            SceneManager.LoadScene(sceneIndex);
        }
    }
}
