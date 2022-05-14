using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingTrigger : MonoBehaviour
{
    [SerializeField] private bool isFirstTrigger;
    private Collider col;
    // Start is called before the first frame update

    private void Start()
    {
        col = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        col.enabled = false;
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
