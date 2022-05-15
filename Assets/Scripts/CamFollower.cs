using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamFollower : MonoBehaviour
{
    private Transform targetTransform = null;
    [SerializeField] private Transform player;
    [SerializeField] private Transform boss;
    [SerializeField] private Vector3 offset;
    [SerializeField] private float playerFollowSpeed = 0.5f;

    // Update is called once per frame
    void LateUpdate()
    {
        if(targetTransform == null)
        {
            targetTransform = player;
        }

        if(targetTransform == player)
        {
            Vector3 targetPosition = player.position + offset;
            transform.position = Vector3.Lerp(transform.position, targetPosition, playerFollowSpeed);
        }
        else
        {
            boss = GameObject.FindGameObjectWithTag("Boss").transform;
            Vector3 targetPosition = boss.position + offset;
            transform.position = Vector3.Lerp(transform.position, targetPosition, playerFollowSpeed);
        }
        
    }

    public void SwitchTarget(Transform newTargetTransform)
    {
        targetTransform = newTargetTransform;
    }
}
