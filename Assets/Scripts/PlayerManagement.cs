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
    [SerializeField] private float slapPower = 50;
    [SerializeField] private Vector3 finishLocation = new Vector3(0, 0.25f, 99);
    [SerializeField] private GameObject localMover;
    [Space]

    [SerializeField] private GameObject boss;
    [SerializeField] private BossManager bossManager;
    [Space]

    [SerializeField] private GameObject multiplierBar;
    [SerializeField] private Vector3 multiplierBarFinishLocation = new Vector3(0, 3.25f, 0.25f);
    private Vector3 multiplierBarFirstLocation = new Vector3(0, 10f, 0.25f);

    private bool canRun = true;
    private float currentHealth;

    Sequence sequence;
    Sequence sequenceLocalMover;
    Sequence sequenceCamAndBar;

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
        sequenceLocalMover = DOTween.Sequence();
        runnerScript.PlayAnimation("StructWalk", 1);

        sequenceLocalMover.Append(localMover.transform.DOMove(finishLocation, 8));
        sequence.Append(transform.DOMove(finishLocation, 8));
    }

    public void RingJump()
    {
        runnerScript.PlayAnimation("Jump", 1);

        sequence.Kill();
        sequence = DOTween.Sequence();
        sequence.Insert(1.1f, transform.DOJump(new Vector3(0, .25f, transform.position.z + 3.5f), 3, 1, 1.1f)
            .OnComplete(() => { runnerScript.PlayAnimation("StructWalk", 1); }) );

        sequence.Append(transform.DOMove(finishLocation, 2.5f));
    }

    public void RingIdle()
    {
        sequence.Kill();
        runnerScript.PlayAnimation("Idle", 1);
        SetCamAndBar();
        CallTheBoss();
    }

    private void SetCamAndBar()
    {
        //bar
        multiplierBar.SetActive(true);
        sequenceCamAndBar.Append(multiplierBar.transform.DOLocalMove(multiplierBarFinishLocation, 1.5f));
        sequenceCamAndBar.Join(multiplierBar.transform.DOLocalRotate(new Vector3(0, 120, 0), 1.5f, RotateMode.FastBeyond360)
            .SetEase(Ease.Linear).SetLoops(2, LoopType.Restart));

        //cam
    }

    private void CallTheBoss()
    {
        Instantiate(boss, new Vector3(0, 15, 105), Quaternion.Euler(0, 180, 0));
        bossManager = GameObject.FindGameObjectWithTag("Boss").GetComponent<BossManager>();
        bossManager.CallTheBoss();
    }

    public void SlapTheBoss()
    {

    }

    public void ResetCharachter()
    {
        canRun = true;
        currentHealth = minHealth;
        runnerScript.ResetCharacter();
    }
}
