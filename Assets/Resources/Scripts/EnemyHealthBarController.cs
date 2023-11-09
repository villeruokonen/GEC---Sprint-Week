using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBarController : MonoBehaviour
{
    [SerializeField] private Enemy _owner;
    [SerializeField] private Slider _slider;
    
    [SerializeField] private float _yOffset;

    private Transform _origin;

    private int _maxHealth;
    private int _health;
    private Vector3 TargetPos 
        => _origin.position + Vector3.up * _yOffset;

    private bool _isDead => _health <= 0;

    // Start is called before the first frame update
    void Start()
    {
        Transform root = transform.root;
        _owner = root.GetComponentInChildren<Enemy>();
        _maxHealth = _owner.Health;
        _health = _maxHealth;

        _slider =
        _slider != null ? _slider : GetComponentInChildren<Slider>();

        _slider.maxValue = 1;
        _slider.minValue = 0;

        _origin = transform.parent;

        UpdateHealthbar();
    }

    // Update is called once per frame
    void Update()
    {
        if (_isDead)
            return;

        UpdateHealthbar();
    }

    public void UpdateHealthbar()
    {
        if(_owner == null)
        {
            Destroy(gameObject);
            return;
        }

        _health = _owner.Health;

        if(_health <= 0)
        {
            _slider.gameObject.SetActive(false);
            return;
        }

        float healthPercent = (float)_health / _maxHealth;

        _slider.value = healthPercent;

        RecenterHealthbar();
    }

    void RecenterHealthbar()
    {
        transform.position = TargetPos;

        Vector3 lookPos = Camera.main.transform.position;
        lookPos.y = transform.position.y;
        transform.LookAt(lookPos);
    }
}
