using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EnemyMovement))]
[RequireComponent(typeof(EnemyRigidBodyController))]
public class Enemy : MonoBehaviour
{
    private EnemyMovement _movement;
    private EnemyRigidBodyController _rbController;

    public int Health { get { return _health; } }
    [SerializeField] int _health = 25;

    private float _damageTimer = 0f;
    private const float _damageTimerMax = 0.15f;

    // Start is called before the first frame update
    void Start()
    {
        _rbController = GetComponent<EnemyRigidBodyController>();
        _movement = GetComponent<EnemyMovement>();
    }

    private void Update()
    {
        _damageTimer += Time.deltaTime;
    }

    public void TakeDamage(int amount)
    {
        if(_damageTimer < _damageTimerMax)
        {
            return;
        }

        _damageTimer = 0f;

        _health -= amount;
        if (_health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        _rbController.MarkAsDead();
        _movement.enabled = false;
        GetComponent<Collider>().enabled = false;
        Destroy(gameObject, 5f);
        //Destroy(gameObject);
    }
}
