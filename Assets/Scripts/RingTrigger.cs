using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingTrigger : MonoBehaviour
{
    [SerializeField] private bool isFirstTrigger;
    // Start is called before the first frame update
    private void OnTriggerEnter(Collider other)
    {
        if (isFirstTrigger)
        {
            PlayerManagement.Instance.RingJump();
        }
        else
        {
            PlayerManagement.Instance.RingIdle();
        }
    }
}
