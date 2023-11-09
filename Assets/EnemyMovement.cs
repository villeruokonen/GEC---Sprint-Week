using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] private float _speed = 5f;

    private EnemyRigidBodyController _rbController;

    private Transform _target;

    private Rigidbody _rb;

    private bool IsRagdoll
        => _rbController.IsRagdoll || _rb.isKinematic;
    // Start is called before the first frame update
    void Start()
    {
        _rbController = GetComponent<EnemyRigidBodyController>();
        _rb = GetComponent<Rigidbody>();
        _target = GameObject.FindWithTag("Totem").transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsRagdoll)
        {
            Vector3 newPos = Vector3.MoveTowards(transform.position, _target.position, _speed * Time.deltaTime);
            _rb.MovePosition(newPos);
            Vector3 rot = transform.rotation.eulerAngles;
            transform.LookAt(_target);
            transform.rotation = Quaternion.Euler(rot.x, transform.rotation.eulerAngles.y, rot.z);
        }
    }
}
