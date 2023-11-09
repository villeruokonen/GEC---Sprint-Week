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
        }
        else
        {
            if (_physicsEnemies.Count > 0)
                RemoveAllEnemiesFromTornado();
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

            RotateAroundPlayer(enemy.transform, _playerMovement.TornadoAPS / 4);
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
    }

    void RemoveAllEnemiesFromTornado()
    {
        foreach (GameObject enemy in _physicsEnemies)
        {
            if (enemy == null)
                continue;

            DeparentFromPlayer(enemy.transform);

            //enemy.GetComponent<Enemy>().Unragdoll();

            ApplyForceToEnemy(enemy);
        }

        _physicsEnemies.Clear();
    }

    void ApplyForceToEnemy(GameObject enemy)
    {
        Rigidbody rb = enemy.GetComponent<Rigidbody>();
        if (rb == null)
            return;

        Vector3 force = enemy.transform.position - transform.position;
        force.y = 0;
        force.Normalize();
        force *= 10;
        rb.AddForce(force, ForceMode.Impulse);
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

        Rigidbody rb;
        if ((rb = t.GetComponent<Rigidbody>()) == null)
        {
            t.position = newPos;
            return;
        }

        //rb.AddForce(rotationDir, ForceMode.Impulse);
        
    }

    void SignalEnemyRagdoll(GameObject enemy)
    {

    }
}
