using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class PlayerManagement : MonoSingleton<PlayerManagement>
{
    [SerializeField] private RunnerScript runnerScript;

    [Header("Player Stats")]
    [SerializeField] private float minHealth = 100;
    [SerializeField] private float maxHealth = 600;
    [SerializeField] private float slapPower = 75;
    [SerializeField] private GameObject localMover;
    Vector3 finalPosition;

    [Header("Boss Settings")]
    [SerializeField] private GameObject boss;
    [SerializeField] private BossManager bossManager;

    [Header("Mini Game Uthilities")]
    [SerializeField] private GameObject multiplierBar;
    [SerializeField] private GameObject playerHealtUI;
    [SerializeField] private Image playerHealthImage;
    [SerializeField] private Transform mainCamera;
    [SerializeField] private CamFollower camFollower;
    private Bar bar;

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
        runnerScript.Init();
        bar = FindObjectOfType<Bar>();
        boss = GameObject.FindGameObjectWithTag("Boss");
        bossManager = FindObjectOfType<BossManager>();
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
            bar.SlapButton();
            StartSlapping();
            canSlap = false;
        }
    }

    public void AddHealth(int collectedHealth)
    {
        currentHealth += collectedHealth;

        if (currentHealth <= 0)
        {
            canRun = false;
            runnerScript.PlayAnimation("Death");
            runnerScript.enabled = false;
            UIManager.Instance.SetActiveProgressBar(false);
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
        sequence = DOTween.Sequence();
        sequenceLocalMover = DOTween.Sequence();
        runnerScript.PlayAnimation("StructWalk");

        finalPosition = new Vector3(0, 0, (transform.position.z + 20));
        sequenceLocalMover.Append(localMover.transform.DOMove(finalPosition, 6));
        sequence.Append(transform.DOMove(finalPosition, 6));
    }

    public void RingJump()
    {
        runnerScript.PlayAnimation("Jump");

        sequence.Kill();
        sequence = DOTween.Sequence();
        sequence.Insert(1.1f, transform.DOJump(new Vector3(0, .25f, transform.position.z + 3.5f), 3, 1, 1.1f)
            .OnComplete(() => { runnerScript.PlayAnimation("StructWalk"); }));

        sequence.Append(transform.DOMove(finalPosition + new Vector3(0, 0.25f, 0), 2.5f));
    }

    public void RingIdle()
    {
        sequence.Kill();
        runnerScript.PlayAnimation("Idle");
        SetCam();
        bar.InstantiateTheBar();
        CallTheBoss();
    }

    private void SetCam()
    { 
        camFollower.transform.DORotate(new Vector3(0, -45, 0), 1.5f)
            .OnComplete(() => { bar.MoveMultiplierArrow(); });
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
        bossManager.BossTookHit(bar.multiplier * slapPower);
        var particle = ObjectPooler.Instance.GetPooledObject("PlayerParticle");
        particle.transform.position = transform.position + new Vector3(0, 1.5f, 1f);
        particle.transform.rotation = Quaternion.identity;
        particle.SetActive(true);
        particle.GetComponent<ParticleSystem>().Play();

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
            SetUI(ReturnHealth());
            runnerScript.PlayAnimation("TakeHit");
            canSlap = true;

            bar.MoveMultiplierArrow();
        }
        else
        {
            OpenUI(false);
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

    public void OpenUI(bool check)
    {
        if (check)
        {
            playerHealtUI.SetActive(true);
        }
        else
        {
            playerHealtUI.SetActive(false);
        }
    }

    public void SetUI(float health)
    {
        playerHealthImage.fillAmount = health;
    }

    public float ReturnHealth()
    {
        return currentHealth / maxHealth;
    }

    public float ReturnPower()
    {
        return slapPower;
    }

    public void CanSlap(bool check)
    {
        canSlap = check;
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
