using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(PlayerMovement), typeof(CharacterController))]
public class Player : MonoBehaviour
{
    private List<GameObject> _physicsEnemies = new List<GameObject>();

    private PlayerMovement _playerMovement;

    private bool IsTornado => _playerMovement.IsTornado;

    public float TornadoPower => _tornadoPower;

    [SerializeField] private float _kickForce = 10.0f;

    [SerializeField] private float _tornadoRadius = 4.5f;

    [SerializeField] private float _maxTornadoPower = 3.0f;

    private float _tornadoPower = 2.0f;
    private readonly float _tornadoDecayRate = 0.5f;

    private Image _tornadoPowerSlider;

    // Start is called before the first frame update
    void Start()
    {
        _playerMovement = GetComponent<PlayerMovement>();

        _tornadoPowerSlider = transform.Find("Canvas/TornadoBG/TornadoPowerSlider").GetComponent<Image>();

        _tornadoPower = _maxTornadoPower;
    }

    // Update is called once per frame
    void Update()
    {
        if (IsTornado)
        {
            _tornadoPower -= _tornadoDecayRate * Time.deltaTime;
            RotateEnemiesInTornado();

            CheckForEnemiesToPickUp();

            if (_tornadoPower <= 0)
            {
                _playerMovement.StopTornado();
            }
        }
        else
        {
            _tornadoPower += _tornadoDecayRate / 2.0f * Time.deltaTime;
            if (_physicsEnemies.Count > 0)
                RemoveAllEnemiesFromTornado();

            if (_tornadoPower > _maxTornadoPower)
                _tornadoPower = _maxTornadoPower;
        }

        _tornadoPowerSlider.fillAmount = _tornadoPower / _maxTornadoPower;
    }

    void OnTriggerEnter(Collider col)
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

            Vector3 force = (-col.transform.position + transform.position).normalized;

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
        var enemies = Physics.OverlapSphere(transform.position, _tornadoRadius, LayerMask.GetMask("Enemy"));

        foreach (var enemy in enemies)
        {
            if (_physicsEnemies.Contains(enemy.gameObject))
                continue;

            Transform root = enemy.transform.root;

            _physicsEnemies.Add(root.gameObject);
            ParentToPlayer(root);
            SetEnemyRagdollNoDamage(enemy.gameObject);
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
    }

    void RemoveAllEnemiesFromTornado()
    {
        foreach (GameObject enemy in _physicsEnemies)
        {
            if (enemy == null)
                continue;

            DeparentFromPlayer(enemy.transform);

            ApplyForceToEnemy(enemy);

            // Hack: zero vector for no damage but sets damage flag in ragdoll
            SetEnemyRagdollWithForceDamage(enemy, Vector3.zero);
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
        
        rbController.AddRagdollForce(rotationDir.normalized);
        
    }

    void SetEnemyRagdollNoDamage(GameObject enemy)
    {
        var rbController = enemy.GetComponent<EnemyRigidBodyController>();
        if (rbController == null)
            return;

        rbController.SetRagdoll();
    }

    void SetEnemyRagdollWithForceDamage(GameObject enemy, Vector3 force)
    {
        var rbController = enemy.GetComponent<EnemyRigidBodyController>();
        if (rbController == null)
            return;

        rbController.SetRagdollWithForce(force);
    }
}
