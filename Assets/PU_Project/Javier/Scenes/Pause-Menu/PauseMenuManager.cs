using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Unity.VisualScripting;
using UnityEngine;

public class PauseMenuManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _MainMenuCanvasGO;
    [SerializeField]
    private GameObject _LoreMenuCanvasGO;

    private bool isPaused = false;

    private void Start()
    {
        _MainMenuCanvasGO.SetActive(false);
        _LoreMenuCanvasGO.SetActive(false);
    }

    private void Update(){
        if(InputManager.instance.MenuOpenCloseInput){
            Debug.Log("MenuOpenCloseInput");
            if(!isPaused){
                Pause();
            }else{
                Unpause();
            }
        }
    }

    private void Pause(){
        isPaused = true;
        Time.timeScale = 0f;

        OpenMainMenu();
    }

    private void Unpause(){
        isPaused = false;
        Time.timeScale = 1f;

        CloseAllMenus();
    }

    private void OpenMainMenu(){
        _MainMenuCanvasGO.SetActive(true);
        _LoreMenuCanvasGO.SetActive(false);
    }

    private void CloseAllMenus(){
        _MainMenuCanvasGO.SetActive(false);
        _LoreMenuCanvasGO.SetActive(false);
    }

    //Main Menu Actions Methods
    public void OnResumePress(){
        Unpause();
    }

    public void OnLorePress(){
        OpenLoreMenuHandle();
    }

    private void OpenLoreMenuHandle(){
        _MainMenuCanvasGO.SetActive(false);
        _LoreMenuCanvasGO.SetActive(true);
    }

    public void OnQuitPress(){
        Application.Quit();
    }

}