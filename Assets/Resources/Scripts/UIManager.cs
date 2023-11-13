using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEditor.SearchService;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Spawner spawner;
    [SerializeField] private TextMeshProUGUI waveCounterText;
    void Start()
    {
        
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
}
