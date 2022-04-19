using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManagement : MonoSingleton<PlayerManagement>
{
    [SerializeField] private RunnerScript runnerScript;

    //player stats
    [SerializeField] private int minHealth = 100;
    [SerializeField] private int maxHealth = 100;
    private int currentHealth;

    void Start()
    {
        currentHealth = minHealth;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            runnerScript.StartToRun(true);
        }
        if (Input.GetMouseButtonUp(0))
        {
            runnerScript.StartToRun(false);
        }
    }

    public void AddHealth(int collectedHealth)
    {
        currentHealth += collectedHealth;
    }

    public void ResetCharachter()
    {
        currentHealth = minHealth;
        runnerScript.ResetCharacter();
    }
}
