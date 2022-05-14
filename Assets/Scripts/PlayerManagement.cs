using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerManagement : MonoSingleton<PlayerManagement>
{
    [SerializeField] private RunnerScript runnerScript;

    [Header("Player Stats")]
    [SerializeField] private float minHealth = 100;
    [SerializeField] private float maxHealth = 600;
    [SerializeField] private float slapPower = 75;
    [SerializeField] private GameObject localMover;

    [Header("Boss Settings")]
    [SerializeField] private GameObject boss;
    [SerializeField] private BossManager bossManager;

    [Header("Mini Game Uthilities")]
    [SerializeField] private GameObject multiplierBar;
    private Vector3 finishLocation;
    [Space]
    [SerializeField] private Transform mainCamera;
    [SerializeField] private CamFollower camFollower;

    private bool canRun = true;
    private float currentHealth;
    private bool canSlap = false;
    private bool isItFirstSlap = true;

    Sequence sequence;
    Sequence sequenceLocalMover;
    Sequence sequenceCamAndBar;

    void Start()
    {
        currentHealth = minHealth;
        DOTween.Init();
    }

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
        if (Input.GetMouseButton(0) && canSlap)
        {
            UIManager.Instance.SlapButton();
            StartSlapping();
            canSlap = false;
        }
    }

    public void AddHealth(int collectedHealth)
    {
        currentHealth += collectedHealth;

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            canRun = false;
            runnerScript.StartToRun(false);
            runnerScript.CanSwerve();
            runnerScript.PlayAnimation("Death");
            UIManager.Instance.RestartButtonUI();
        }

        if (currentHealth > 600)
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
        UIManager.Instance.SetActiveProgressBar(false);
        runnerScript.PlayAnimation("StructWalk");
        RingWalk();
    }

    private void RingWalk()
    {
        finishLocation = GameObject.FindGameObjectWithTag("FinalTargetLoc").transform.position;
        sequence = DOTween.Sequence();
        sequenceLocalMover = DOTween.Sequence();
        runnerScript.PlayAnimation("StructWalk");

        sequenceLocalMover.Append(localMover.transform.DOMove(finishLocation, 8));
        sequence.Append(transform.DOMove(finishLocation, 8));
    }

    public void RingJump()
    {
        runnerScript.PlayAnimation("Jump");

        sequence.Kill();
        sequence = DOTween.Sequence();
        sequence.Insert(1.1f, transform.DOJump(new Vector3(0, .25f, transform.position.z + 3.5f), 3, 1, 1.1f)
            .OnComplete(() => { runnerScript.PlayAnimation("StructWalk"); }));

        sequence.Append(transform.DOMove(finishLocation, 2.5f));
    }

    public void RingIdle()
    {
        sequence.Kill();
        runnerScript.PlayAnimation("Idle");
        SetBarAndCam();
        CallTheBoss();
        canSlap = true;
    }

    private void SetBarAndCam()
    {
        multiplierBar.SetActive(true);
        sequenceCamAndBar.Append(multiplierBar.transform.DOMoveY(multiplierBar.transform.position.y - 7.5f , 1.5f));
        sequenceCamAndBar.Join(multiplierBar.transform.DORotate(new Vector3(0, -60, 0), 1.5f, RotateMode.FastBeyond360)
            .SetEase(Ease.Linear).SetLoops(2, LoopType.Restart));
        sequenceCamAndBar.Join(camFollower.transform.DORotate(new Vector3(0, -45, 0), 1.5f)
            .OnComplete(() => { UIManager.Instance.MoveMultiplierArrow(); }));
    }

    public void SetBossFollowCam()
    {
        camFollower.SwitchTarget(boss.transform);
        camFollower.transform.DORotate(new Vector3(0, 0, 0), 1f);
    }

    private void CallTheBoss()
    {
        bossManager.SpawnTheBoss();
    }

    public void StartSlapping()
    {
        StartCoroutine(SlapTheBoss());
    }

    public IEnumerator SlapTheBoss()
    {
        canSlap = false;

        //hit boss
        yield return new WaitForSeconds(1.1f);
        runnerScript.PlayAnimation("Slap");
        yield return new WaitForSeconds(1.1f); //wait for the exact hit moment
        bossManager.BossTookHit(UIManager.Instance.multiplier * slapPower);

        yield return new WaitForSeconds(1.574f);
        runnerScript.PlayAnimation("Idle");
    }

    IEnumerator TakeSlapRoutine()
    {
        yield return new WaitForSeconds(1.01f);
        runnerScript.PlayAnimation("Idle");
    }

    public void PlayerTookHit(float damage)
    {
        if (currentHealth - damage > 0)
        {
            StartCoroutine(TakeSlapRoutine());
            currentHealth -= damage;
            UIManager.Instance.SetHealthUIs();
            runnerScript.PlayAnimation("TakeHit");
            canSlap = true;

            UIManager.Instance.MoveMultiplierArrow();
        }
        else
        {
            UIManager.Instance.TurnOnOffUIs(false);
            runnerScript.PlayAnimation("Death");
            UIManager.Instance.RestartButtonUI();
        }
    }

    public void WinStuation()
    {
        StopCoroutine(SlapTheBoss());
        camFollower.SwitchTarget(boss.transform);
        camFollower.transform.DORotate(new Vector3(0, 0, 0), 1f);
    }

    public void PlayAnimation(string animName)
    {
        runnerScript.PlayAnimation(animName);
    }


    public float ReturnHealth()
    {
        return currentHealth / maxHealth;
    }

    public float ReturnPower()
    {
        return slapPower;
    }

    public void ResetCharachter()
    {
        canSlap = false;
        canRun = true;
        currentHealth = minHealth;
        isItFirstSlap = true;

        runnerScript.ResetCharacter();
        camFollower.SwitchTarget(transform);
        boss.transform.position = new Vector3(0, 0, -20);
        multiplierBar.transform.position = new Vector3(0, 0, -20);

        UIManager.Instance.SetProgress(0);
        UIManager.Instance.SetActiveProgressBar(true);
    }
}
