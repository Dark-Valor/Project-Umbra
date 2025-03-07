using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class PauseMenuManager : MonoBehaviour
{
    //Fields to be set in the inspector
    [SerializeField]
    private GameObject _MainMenuCanvasGO;
    [SerializeField]
    private GameObject _LoreMenuCanvasGO;

    private bool isPaused = false;

    [Header("First Selected Options")]
    [SerializeField] private GameObject _mainMenuFirst;
    [SerializeField] private GameObject _loreMenuFirst;

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

    //Action Methods

    private void OpenMainMenu(){
        _MainMenuCanvasGO.SetActive(true);
        _LoreMenuCanvasGO.SetActive(false);

        EventSystem.current.SetSelectedGameObject(_mainMenuFirst);
    }

    private void CloseAllMenus(){
        _MainMenuCanvasGO.SetActive(false);
        _LoreMenuCanvasGO.SetActive(false);

        EventSystem.current.SetSelectedGameObject(null);
    }

    //Main Menu Actions Methods
    public void OnResumePress(){
        Unpause();
    }

    //Exit Game Button
    public void OnExitPress(){
        //Changes the scene to the main menu
        // SceneLoader.instance.LoadMainMenu();
        Debug.Log("Exit Game - SHOULD CHANGE SCENE TO MAIN MENU");
    }

    public void OnLorePress(){
        OpenLoreMenuHandle();
    }

    private void OpenLoreMenuHandle(){
        _MainMenuCanvasGO.SetActive(false);
        _LoreMenuCanvasGO.SetActive(true);

        EventSystem.current.SetSelectedGameObject(_loreMenuFirst);
    }

    public void OnQuitPress(){
        Application.Quit();
    }


    public void onBackPress(){
        OpenMainMenu();
    }

}