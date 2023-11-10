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
 

    void Start()
    {
      
        target = GameObject.FindGameObjectWithTag("Target").transform;
    }

    void Update()
    {
        if (canMove)
        {
            Move();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Target"))
        {
            canMove = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Target"))
        {
            canMove = true;
        }
    }

    private void Move()
    {
        if (target == null)
            return;

        transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

        transform.LookAt(target);

        transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
    }
}