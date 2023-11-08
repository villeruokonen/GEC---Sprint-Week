using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    private Transform _hipsBone;
    [SerializeField] private float _hipsOffset;
    private void Awake()
    {
        _hipsBone = _animator.GetBoneTransform(HumanBodyBones.Hips);
    }
    void Start()
    {
        _animator.SetBool("CanWalk", true);
    }

    
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            Debug.Log("Hit");

            _animator.enabled = false;
            StartCoroutine(MoveAgain());
        }
    }

    IEnumerator MoveAgain()
    {
        yield return new WaitForSeconds(3f);
        _animator.enabled = true;
        AllignPositionToHips();
        _animator.SetTrigger("GetUp");

        // Wait for the "GetUp" animation to finish
        yield return new WaitForSeconds(_animator.GetCurrentAnimatorStateInfo(0).length);

        // Set the character's position to the hip bone's position
        //transform.position = _hipsBone.position;
    }

    private void AllignPositionToHips()
    {
        Vector3 originalHipsPosition = _hipsBone.position;
        transform.position = _hipsBone.position;

        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hitInfo))
        {
            //transform.position = new Vector3(transform.position.x, hitInfo.point.y, transform.position.z);
        }

        _hipsBone.position = originalHipsPosition;
    }
}
