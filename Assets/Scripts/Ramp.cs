using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ramp : MonoBehaviour
{
    [SerializeField] private Material black;
    [SerializeField] private Material green;
    [SerializeField] private GameObject boss;
    [SerializeField] private float range;

    private void OnTriggerEnter(Collider other)
    {
        TurnGreen();
        CalculateRange();
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

    private void CalculateRange()
    {
        int range = (int)(transform.position.z / 7.5f);
        int money = range * ((PlayerPrefs.GetInt("HCLevel") + 1) * 10);
        PlayerPrefs.SetInt("TotalMoney", money);
        Debug.Log(money);
        Debug.Log(range);
    }
}
