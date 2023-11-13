using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyRigidBodyController : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    private Transform _hipsBone;
    [SerializeField] private GameObject Rig;
    private Collider[] _ragdollColliders;
    private Rigidbody[] _RagdollRigidbodies;

    [SerializeField] private CollisionReporter _collisionReporter;
    private Collision _lastCollision 
        => _collisionReporter.LastCollision;

    private bool CanTakeRagdollDamage;

    public Animator Animator => _animator;

    private float _getUpTimer = 0f;
    private const float _getUpTimerMax = 0.33f;

    private bool _tryingToGetUp = false;

    public bool IsRagdoll
    => !_animator.enabled && !_animator.GetBool("CanWalk");

    public bool AttackAnimationPlaying
    => _animator.GetCurrentAnimatorStateInfo(0).IsName("Lumbering");

    private void Awake()
    {
        _hipsBone = _animator.GetBoneTransform(HumanBodyBones.Hips);
    }
    void Start()
    {
        GetRagdollBits();
        RagdollModeOff();
        _animator.SetBool("CanWalk", true);
        _collisionReporter = GetComponentInChildren<CollisionReporter>();
    }

    public void MarkAsDead()
    {
        _animator.enabled = false;
        StopAllCoroutines();
    }

    private void Update()
    {
        if(_lastCollision != null)
        {
            if(CanTakeRagdollDamage)
            {
                int damage = Mathf.RoundToInt(_lastCollision.relativeVelocity.magnitude / 6);
                Debug.Log("Damage: " + damage);
                GetComponent<Enemy>().TakeDamage(damage);
            }
        }

        if(_getUpTimer < _getUpTimerMax)
        {
            _getUpTimer += Time.deltaTime;
        }

        // Should this be done?
        //if (IsRagdoll)
        //{
        //    transform.position = _hipsBone.position;
        //}
    }

    IEnumerator MoveAgain()
    {
        if(_tryingToGetUp)
            yield break;

        _tryingToGetUp = true;

        var rb = GetComponent<Rigidbody>();
        while (rb.velocity.magnitude > 0.04f)
        {
            yield return new WaitForSeconds(1);
        }
        yield return new WaitForSeconds(Random.Range(3.0f, 4.0f));

        RagdollModeOff();
        AllignPositionToHips();
        _animator.SetTrigger("GetUp");
        StartCoroutine(TimerGetUp());

        while(_animator.GetCurrentAnimatorStateInfo(0).IsName("GetUp"))
        {
            yield return new WaitForSeconds(0.1f);
        }

        _tryingToGetUp = false;
    }

    IEnumerator TimerGetUp()
    {
        GetComponent<EnemyMovement>().canMove=false;
        yield return new WaitForSeconds(2.2f);
        GetComponent<EnemyMovement>().canMove=true;
    }

    private void AllignPositionToHips()
    {
        Vector3 originalHipsPosition = _hipsBone.position;
        transform.position = _hipsBone.position;
        _hipsBone.position = originalHipsPosition;
    }

    public void AddRagdollForce(Vector3 force)
    {
        RagdollModeOn(Vector3.zero);

        foreach (Rigidbody rigid in _RagdollRigidbodies)
        {
            rigid.AddForce(force, ForceMode.Impulse);
        }
    }

    private void RagdollModeOn(Vector3 direction)
    {
        _animator.enabled = false;
        GetComponent<EnemyMovement>().canMove = false;

        foreach (Collider col in _ragdollColliders)
        {
            col.enabled = true;
        }

        foreach (Rigidbody rigid in _RagdollRigidbodies)
        {
            rigid.isKinematic = false;
        }

        // Take damage equal to force
        GetComponent<Enemy>().TakeDamage(Mathf.RoundToInt(direction.magnitude));

        GetComponent<Collider>().enabled = false;
        GetComponent<Rigidbody>().isKinematic = true;
    }

    private void RagdollModeOff()
    {
        _animator.SetBool("CanAttack", false);
        _animator.SetBool("CanWalk", true);
        GetComponent<EnemyMovement>().canMove = true;

        foreach (Collider col in _ragdollColliders)
        {
            col.enabled = false;
        }

        foreach (Rigidbody rigid in _RagdollRigidbodies)
        {
            rigid.isKinematic = true;
        }
        _animator.enabled = true;
        GetComponent<Collider>().enabled = true;
        GetComponent<Rigidbody>().isKinematic = false;
        CanTakeRagdollDamage = false;
    }

    private void GetRagdollBits()
    {
        _ragdollColliders = Rig.GetComponentsInChildren<Collider>();
        _RagdollRigidbodies = Rig.GetComponentsInChildren<Rigidbody>();
    }

    public void SetRagdoll()
    {
        StartCoroutine(MoveAgain());
        RagdollModeOn(Vector3.zero);

        CanTakeRagdollDamage = false;
    }

    public void SetRagdollWithForce(Vector3 force)
    {
        StartCoroutine(MoveAgain());
        RagdollModeOn(force);

        CanTakeRagdollDamage = true;
    }

    public void PushBack(Vector3 playerDirection)
    {
        StartCoroutine(MoveAgain());
        RagdollModeOn(playerDirection);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Target"))
        {
           _animator.SetBool("CanWalk", false);
            _animator.SetBool("CanAttack", true);
        }
        
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Target"))
        {
            _animator.SetBool("CanWalk", true);
            _animator.SetBool("CanAttack", false);
        }
    }
}
