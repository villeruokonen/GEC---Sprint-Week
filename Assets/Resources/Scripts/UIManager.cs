using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Spawner spawner;
    [SerializeField] private TextMeshProUGUI waveCounterText;
    void Start()
    {
        
    }

    
    void Update()
    {
        waveCounterText.text = "WAVE "+spawner._currentWave.ToString(); 
    }
}
