using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerManagement : MonoSingleton<PlayerManagement>
{
    [SerializeField] private RunnerScript runnerScript;

    //player stats
    [SerializeField] private float minHealth = 100;
    [SerializeField] private float maxHealth = 600;
    [SerializeField] private float finishLocation = 99;
    private bool canRun = true;
    private float currentHealth;

    Sequence sequence;

    void Start()
    {
        currentHealth = minHealth;
        DOTween.Init();
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

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            //burda geber
        }

        if (currentHealth + collectedHealth > 600)
        {
            currentHealth = 600;
        }

        SetUIProcess();
    }

    private void SetUIProcess()
    {
        float floatHealth = currentHealth / 600;
        UIManager.Instance.SetProgress(floatHealth);
    }

    public void FinishedAction()
    {
        canRun = false;
        runnerScript.StartToRun(false);
        runnerScript.PlayAnimation("StructWalk", 1);
        RingWalk();
    }

    private void RingWalk()
    {
        sequence = DOTween.Sequence();
        runnerScript.PlayAnimation("StructWalk", 1);
        sequence.Append(transform.DOMoveZ(finishLocation, 4));
    }

    public void RingJump()
    {
        Debug.Log("UCAMAAZSIN TOMBIK");
        runnerScript.PlayAnimation("Jump", 1);

        sequence.Kill();
        sequence = DOTween.Sequence();
        sequence.Insert(1.1f, transform.DOJump(new Vector3(0, 0, transform.position.z + 3.5f), 3, 1, 1.1f)
            .OnComplete(() => { runnerScript.PlayAnimation("StructWalk", 1); }) );

        sequence.Append(transform.DOMoveZ(finishLocation, 2.5f));
    }

    public void RingIdle()
    {
        sequence.Kill();
        runnerScript.PlayAnimation("Idle", 1);
        CallTheBoss();
    }

    private void CallTheBoss()
    {

    }

    public void ResetCharachter()
    {
        canRun = true;
        currentHealth = minHealth;
        runnerScript.ResetCharacter();
    }
}
