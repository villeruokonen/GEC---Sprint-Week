using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionReporter : MonoBehaviour
{
    public Collision LastCollision => _lastCollision;
    private Collision _lastCollision;

    private float _timer = 0f;
    private const float _timerMax = 0.25f;

    private void Update()
    {
        _timer += Time.deltaTime;

        if(_timer >= _timerMax)
        {
            _timer = 0;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if(_lastCollision == null)
        {
            _lastCollision = collision;
        }

        if(collision.relativeVelocity.magnitude < _lastCollision.relativeVelocity.magnitude)
        {
            return;
        }

        if(_timer >= _timerMax)
        {
            _lastCollision = collision;
        }
    }
}
