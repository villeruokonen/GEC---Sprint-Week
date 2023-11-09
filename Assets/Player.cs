using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyState
{
    Idle,
    Chasing,
    Attacking,
    TornadoState,
    Dead
}

[RequireComponent(typeof(PlayerMovement), typeof(CharacterController))]
public class Player : MonoBehaviour
{
    private List<GameObject> _physicsEnemies = new List<GameObject>();

    private PlayerMovement _playerMovement;

    private bool IsTornado => _playerMovement.IsTornado;

    [SerializeField] private float _kickForce = 10f;

    // Start is called before the first frame update
    void Start()
    {
        _playerMovement = GetComponent<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        if (IsTornado)
        {
            RotateEnemiesInTornado();

            CheckForEnemiesToPickUp();
        }
        else
        {
            if (_physicsEnemies.Count > 0)
                RemoveAllEnemiesFromTornado();
        }
    }

    void OnCollisionEnter(Collision col)
    {
        if (IsTornado)
            return;

        if (!_playerMovement.IsMoving)
            return;

        if (col.gameObject.CompareTag("Enemy"))
        {
            var rbController = col.gameObject.GetComponent<EnemyRigidBodyController>();

            if (rbController == null)
                return;

            Vector3 force = (col.transform.position - transform.position).normalized;

            force *= _kickForce;

            rbController.SetRagdollWithForce(force);
        }
    }

    void RotateEnemiesInTornado()
    {
        if (_physicsEnemies.Count == 0)
        {
            return;
        }

        foreach (GameObject enemy in _physicsEnemies)
        {
            if (enemy == null)
                continue;

            RotateAroundPlayer(enemy.transform, _playerMovement.TornadoAPS / 2);
        }
    }

    void CheckForEnemiesToPickUp()
    {
        var enemies = Physics.OverlapSphere(transform.position, 8, LayerMask.GetMask("Enemy"));

        foreach (var enemy in enemies)
        {
            if (_physicsEnemies.Contains(enemy.gameObject))
                continue;

            _physicsEnemies.Add(enemy.gameObject);
            ParentToPlayer(enemy.transform);
            SignalEnemyRagdoll(enemy.gameObject);
        }
    }

    void OnTriggerEnter(Collider col)
    {
        if (_playerMovement.IsTornado)
        {
            if (col.CompareTag("Enemy"))
            {
                _physicsEnemies.Add(col.gameObject);
                //col.GetComponent<Enemy>().Ragdoll();
                ParentToPlayer(col.transform);
                SignalEnemyRagdoll(col.gameObject);
            }
        }
    }

    void ParentToPlayer(Transform t)
    {
        t.SetParent(transform);
    }

    void DeparentFromPlayer(Transform t)
    {
        t.SetParent(null);

        var rbController = t.GetComponent<EnemyRigidBodyController>();

        if (rbController == null)
            return;

        rbController.AllowGetUp();
    }

    void RemoveAllEnemiesFromTornado()
    {
        foreach (GameObject enemy in _physicsEnemies)
        {
            if (enemy == null)
                continue;

            DeparentFromPlayer(enemy.transform);

            ApplyForceToEnemy(enemy);
        }

        _physicsEnemies.Clear();
    }

    void ApplyForceToEnemy(GameObject enemy)
    {
        var rbController = enemy.GetComponent<EnemyRigidBodyController>();
        if (rbController == null)
            return;

        Vector3 force = enemy.transform.position - transform.position;
        force.y = 0;
        force.Normalize();

        rbController.AddRagdollForce(force * _playerMovement.TornadoAPS / 4);
    }

    void RotateAroundPlayer(Transform t, int angles)
    {
        Vector3 tPos = t.position;
        Vector3 center = transform.position + 5 * Vector3.up;
        Vector3 dir = tPos - center;

        Vector3 newPos = center + dir.normalized * 6;

        Vector3 newDir = newPos - center;

        t.position = newPos;
        
        Vector3 rotationDir = Vector3.Cross(dir, newDir);

        t.RotateAround(center, Vector3.up, angles * Time.deltaTime);

        var rbController = t.GetComponent<EnemyRigidBodyController>();
        if (rbController == null)
            return;

        rbController.AddRagdollForce(rotationDir / 4);
        
    }

    void SignalEnemyRagdoll(GameObject enemy)
    {
        var rbController = enemy.GetComponent<EnemyRigidBodyController>();
        if (rbController == null)
            return;

        rbController.SetRagdoll();
    }
}
