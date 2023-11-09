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
    //[SerializeField] private float force = 1f;

    public bool IsRagdoll
        => _animator.GetBool("CanWalk") == false;

    private void Awake()
    {
        _hipsBone = _animator.GetBoneTransform(HumanBodyBones.Hips);
    }
    void Start()
    {
        GetRagdollBits();
        RagdollModeOff();
        _animator.SetBool("CanWalk", true);
    }


    void Update()
    {

    }

    //private void OnCollisionEnter(Collision other)
    //{
    //    if (other.gameObject.tag == ("Player"))
    //    {
    //        Debug.Log(other.gameObject);
    //        PushBack(other.transform.position - transform.position);
    //    }
    //}

    public void AllowGetUp()
    {
        StartCoroutine(MoveAgain());
    }

    IEnumerator MoveAgain()
    {
        var rb = GetComponent<Rigidbody>();
        while (rb.velocity.magnitude > 0.1f)
        {
            yield return new WaitForSeconds(1);
        }
        yield return new WaitForSeconds(Random.Range(3.0f, 4.0f));
        RagdollModeOff();
        AllignPositionToHips();
        _animator.SetTrigger("GetUp");
    }

    private void AllignPositionToHips()
    {
        Vector3 originalHipsPosition = _hipsBone.position;
        transform.position = _hipsBone.position;
        _hipsBone.position = originalHipsPosition;
    }

    public void AddRagdollForce(Vector3 force)
    {
        foreach (Rigidbody rigid in _RagdollRigidbodies)
        {
            rigid.AddForce(force, ForceMode.Impulse);
        }
    }

    private void RagdollModeOn(Vector3 direction)
    {
        _animator.enabled = false;

        foreach (Collider col in _ragdollColliders)
        {
            col.enabled = true;
        }

        foreach (Rigidbody rigid in _RagdollRigidbodies)
        {
            rigid.isKinematic = false;
            //rigid.AddForce(direction * force, ForceMode.Impulse);
        }

        GetComponent<Enemy>().TakeDamage(Mathf.RoundToInt(direction.magnitude));

        GetComponent<Collider>().enabled = false;
        GetComponent<Rigidbody>().isKinematic = true;
    }

    private void RagdollModeOff()
    {
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
    }

    private void GetRagdollBits()
    {
        _ragdollColliders = Rig.GetComponentsInChildren<Collider>();
        _RagdollRigidbodies = Rig.GetComponentsInChildren<Rigidbody>();
    }

    public void SetRagdoll()
    {
        //StartCoroutine(MoveAgain());
        RagdollModeOn(Vector3.zero);
    }

    public void SetRagdollWithForce(Vector3 force)
    {
        StartCoroutine(MoveAgain());
        RagdollModeOn(force);
    }

    public void PushBack(Vector3 playerDirection)
    {
        //StartCoroutine(MoveAgain());
        RagdollModeOn(playerDirection);
    }
}
