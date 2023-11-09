using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public float speed;
    private Transform target;

    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Target").transform;
    }

    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, target.position, 1f * speed * Time.deltaTime);
    }
}