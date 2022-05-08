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
    [SerializeField] private float slapPower = 50;
    [SerializeField] private Vector3 finishLocation = new Vector3(0, 0.25f, 99);
    [SerializeField] private GameObject localMover;


    [Header("Boss Settings")]
    [SerializeField] private GameObject boss;
    [SerializeField] private BossManager bossManager;


    [Header("Mini Game Uthilities")]
    [SerializeField] private GameObject multiplierBar;
    [SerializeField] private Vector3 multiplierBarFinishLocation = new Vector3(0, 3.25f, 0.25f);
    private Vector3 multiplierBarFirstLocation = new Vector3(-3.8f, 10f, 2.5f);
    [Space]
    [SerializeField] private Transform mainCamera;
    [SerializeField] private CamFollower camFollower;


    private bool canRun = true;
    private float currentHealth;
    private bool canSlap = false;

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
        runnerScript.PlayAnimation("StructWalk");
        RingWalk();
    }

    private void RingWalk()
    {
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
        SetCamAndBar();
        CallTheBoss();
        canSlap = true;
    }

    private void SetCamAndBar()
    {
        //bar
        multiplierBar.SetActive(true);

        sequenceCamAndBar.Append(multiplierBar.transform.DOLocalMove(multiplierBarFinishLocation, 1.5f));
        sequenceCamAndBar.Join(multiplierBar.transform.DOLocalRotate(new Vector3(0, -60, 0), 1.5f, RotateMode.FastBeyond360)
            .SetEase(Ease.Linear).SetLoops(2, LoopType.Restart));
        sequenceCamAndBar.Join(camFollower.transform.DORotate(new Vector3(0, -30, 0), 1.5f)
            .OnComplete(() => { UIManager.Instance.MoveMultiplierArrow(); }));

    }

    private void CallTheBoss()
    {
        Instantiate(boss, new Vector3(0, 15, 105), Quaternion.Euler(0, 180, 0));
        bossManager = GameObject.FindGameObjectWithTag("Boss").GetComponent<BossManager>(); //AMELELIGI DUZETL "OFTYPE"
        bossManager.SpawnTheBoss();
    }

    public void StartSlapping()
    {
        StartCoroutine(SlapTheBoss());
       // multiplierBar.SetActive(false);
    }

    public IEnumerator SlapTheBoss()
    {
        canSlap = false;
        yield return new WaitForSeconds(1.58f); 
        runnerScript.PlayAnimation("Idle");

        yield return new WaitForSeconds(1f);
        runnerScript.PlayAnimation("Slap");

        yield return new WaitForSeconds(1.1f);
        Debug.Log(UIManager.Instance.multiplier);
        bossManager.BossTookHit(UIManager.Instance.multiplier * 50);
        yield return new WaitForSeconds(1.783f);
        runnerScript.PlayAnimation("Idle");
    }

    public void PlayerTookHit(int damage)
    {
        if (currentHealth - damage > 0)
        {
            currentHealth -= damage;
            runnerScript.PlayAnimation("TakeHit");
            canSlap = true;

            //multiplierBar.SetActive(true);
            UIManager.Instance.MoveMultiplierArrow();
        }
        else
        {
            //burda geber
        }
    }

    public void ResetCharachter()
    {
        canSlap = false;
        canRun = true;
        currentHealth = minHealth;
        runnerScript.ResetCharacter();
        multiplierBar.transform.DOLocalMove(multiplierBarFirstLocation, 1f);
        multiplierBar.SetActive(true);
    }
}
