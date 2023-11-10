using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    // Accessed by the Spawner script
    public float speed;
    private Transform target;
    public bool canMove = true;

    [SerializeField] private int _damage = 1;
    [SerializeField] private float _damageRate = 1;

    private float _damageTimer;

    private TotemController _totem;

    private bool _damagesTotem;
 
    void Start()
    {
      
        target = GameObject.FindGameObjectWithTag("Target").transform;
        _totem = target.GetComponentInChildren<TotemController>();
    }

    void Update()
    {
        if (canMove)
        {
            Move();
        }

        if (_damagesTotem)
        {
            _damageTimer += Time.deltaTime;

            if (_damageTimer >= _damageRate)
            {
                _damageTimer = 0;
                DamageTotem();
            }
        }
    }

    private void DamageTotem()
    {
        if(_totem == null)
            return;
        _totem.TakeDamage(_damage);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Target"))
        {
            canMove = false;
            _damagesTotem = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Target"))
        {
            canMove = true;
        }

        _damagesTotem = false;
    }

    private void Move()
    {
        if (target == null || !canMove)
            return;

        transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

        transform.LookAt(target);

        transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
    }
}