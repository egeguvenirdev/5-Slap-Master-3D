using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManagement : MonoSingleton<PlayerManagement>
{
    [SerializeField] private RunnerScript runnerScript;

    //player stats
    [SerializeField] private int minHealth = 100;
    [SerializeField] private int maxHealth = 600;
    private bool canRun = true;
    private int currentHealth;

    void Start()
    {
        currentHealth = minHealth;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0) && canRun)
        {
            runnerScript.StartToRun(true);
        }
        if (Input.GetMouseButtonUp(0) && canRun)
        {
            runnerScript.StartToRun(false);
        }
    }

    public void AddHealth(int collectedHealth)
    {
        currentHealth += collectedHealth;
        
        if(currentHealth <= 0)
        {
            currentHealth = 0;
            //burda geber
        }

        if(currentHealth + collectedHealth > 600)
        {
            currentHealth = 600;
        }

        SetUIProcess();
    }

    private void SetUIProcess()
    {
        float floatHealth = currentHealth / 600;
        UIManager.Instance.SetProgress(floatHealth);
        Debug.Log(floatHealth);
    }

    public void FinishedAction()
    {
        canRun = false;
    }

    public void ResetCharachter()
    {
        canRun = true;
        currentHealth = minHealth;
        runnerScript.ResetCharacter();
    }
}
