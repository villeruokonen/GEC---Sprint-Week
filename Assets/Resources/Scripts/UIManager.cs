using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEditor.SearchService;
using UnityEngine.SceneManagement;
using Autodesk.Fbx;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Spawner spawner;
    [SerializeField] private TextMeshProUGUI waveCounterText;
    [SerializeField] private GameObject CreditsMenu;
    [SerializeField] private GameObject MainMenu;
    [SerializeField] private GameObject MenuButton;
    void Start()
    {
        if (CreditsMenu == null)
            return;
        CreditsMenu.SetActive(false);
    }

    
    void Update()
    {
        if (waveCounterText)
        {
            waveCounterText.text = "WAVE " + spawner._currentWave.ToString();
        }
        
    }

    public void StartGame()
    {
        
        SceneManager.LoadScene(1);
    }

    public void Credits()
    { 
        CreditsMenu.SetActive(true);
        MainMenu.SetActive(false);
        MenuButton.SetActive(true);
    }

    public void Back()
    {
        CreditsMenu.SetActive(false);
        MainMenu.SetActive(true);
        MenuButton.SetActive(false);
    }
}
