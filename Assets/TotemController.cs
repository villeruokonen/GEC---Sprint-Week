using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TotemController : MonoBehaviour
{
    [SerializeField]
    private int _health = 100;

    private int _maxHealth;

    private float _initialYPos;

    [SerializeField]
    private float _endYOffset;

    private float _healthPercentage => (float)_health / _maxHealth;

    // Start is called before the first frame update
    void Start()
    {
        _maxHealth = _health;
        _initialYPos = transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3()
        {
            x = transform.position.x,
            y = Mathf.Lerp(_initialYPos + _endYOffset, _initialYPos, _healthPercentage),
            z = transform.position.z
        };
    }

    public void TakeDamage(int amount)
    {
        _health -= amount;

        if (_health <= 0)
        {
            //GameOver();
        }
    }
}
