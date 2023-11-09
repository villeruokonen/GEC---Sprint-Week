using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundMarkFader : MonoBehaviour
{
    private MeshRenderer _quadRend;
    private float _fadeSpeed = 0.5f;
    private float _fadeDelay = 0.8f;
    private float _fadeTimer = 0.0f;
    // Start is called before the first frame update
    void Start()
    {
        _quadRend = GetComponentInChildren<MeshRenderer>();   
    }

    // Update is called once per frame
    void Update()
    {
        _fadeTimer += Time.deltaTime;
        if (_fadeTimer > _fadeDelay)
        {
            Color newColor = _quadRend.material.color;
            newColor.a -= _fadeSpeed * Time.deltaTime;
            _quadRend.material.color = newColor;
            if (newColor.a <= 0.0f)
            {
                Destroy(gameObject);
            }
        }
    }
}
