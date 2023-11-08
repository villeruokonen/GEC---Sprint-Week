using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    private Transform _hipsBone;
    [SerializeField] private GameObject Rig;
    private Collider[] _ragdollColliders;
    private Rigidbody[] _RagdollRigidbodies;
   

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

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag ==("Ball"))
        {
            
            StartCoroutine(MoveAgain());
            RagdollModeOn();
        }
        
    }

    IEnumerator MoveAgain()
    {
        yield return new WaitForSeconds(3f);
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
    private void RagdollModeOn()
    {
        _animator.enabled = false;

        foreach (Collider col in _ragdollColliders)
        {
            col.enabled = true;
        }

        foreach (Rigidbody rigid in _RagdollRigidbodies)
        {
            rigid.isKinematic = false;
        }
        
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
}
