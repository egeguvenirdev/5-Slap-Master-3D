using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ramp : MonoBehaviour
{
    public RampType objectType;
    [SerializeField] private Material black;
    [SerializeField] private Material green;
    [SerializeField] private float degisken;
    public enum RampType
    {
        ten = 1,
        tenOne = 11,
        tenTwo = 12,
        tenThree = 13,
        tenFour = 14,
        tenFive = 15,
        tenSix = 16,
        tenSeven = 17,
        tenEight = 18,
        tenNine = 19,

        twenty = 2,
        twentyOne = 21,
        twentyTwo = 22,
        twentyThree = 23,
        twentyFour = 24,
        twentyFive = 25,
        twentySix = 26,
        twentySeven = 27,
        twentyEight = 28,
        twentyNine = 29,

        thirty = 3,
        thirtyOne = 31,
        thirtyTwo = 32,
        thirtyThree = 33,
        thirtyFour = 34,
        thirtyFive = 35,
        thirtySix = 36,
        thirtySeven = 37,
        thirtyEight = 38,
        thirtyNine = 39,
        fourty = 40
    }

    private void OnTriggerEnter(Collider other)
    {
        TurnGreen();
    }

    private void OnTriggerExit(Collider other)
    {
        TurnBlack();
    }

    public void TurnGreen()
    {
        gameObject.GetComponent<Renderer>().material = green;
    }

    public void TurnBlack()
    {
        gameObject.GetComponent<Renderer>().material = black;
    }
}
