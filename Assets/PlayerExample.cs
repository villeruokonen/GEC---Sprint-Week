using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerExample : MonoBehaviour
{
    [SerializeField] private GameObject _model;
    [SerializeField] private GameObject _tornadoModel;
    [SerializeField] private ParticleSystem _tornadoParticles;
    private Vector3 _tornadoScale;
    private readonly Vector3 _tornadoFullScale = Vector3.one;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 input = new (Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        
        transform.position += input * 5.0f * Time.deltaTime;


        if (input.magnitude > 0.01f)
        {
            _tornadoModel.transform.localScale 
                = Vector3.Lerp(_tornadoModel.transform.localScale, _tornadoFullScale, Time.deltaTime * 5f);
            _tornadoParticles.Play();
            _tornadoModel.SetActive(true);
            _model.transform.Rotate(Vector3.up * 18_000 * Time.deltaTime);
        }
        else
        {
            _tornadoModel.transform.localScale
                = Vector3.Lerp(_tornadoModel.transform.localScale, Vector3.zero, Time.deltaTime * 5f);
            _tornadoParticles.Stop();
            _tornadoModel.SetActive(false);
        }
    }
}
