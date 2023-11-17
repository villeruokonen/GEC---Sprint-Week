using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    [SerializeField] private GameObject _failScreen;
    [SerializeField] private GameObject _totem;
    [SerializeField] private float _totemFailY = -10.0f;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (_totem.transform.position.y < _totemFailY)
        {
            ShowFailScreen();
        }
    }

    void ShowFailScreen()
    {
        _failScreen?.SetActive(true);
        StartCoroutine(RestartGame());
    }

    IEnumerator RestartGame()
    {
        yield return new WaitForSeconds(2.0f);
        SceneManager.LoadScene(0);
    }

   
}
