using System.Collections;
using System.Collections.Generic;
using Bermuda.Runner;
using Bermuda.Animation;
using UnityEngine;
using PathCreation;
using DG.Tweening;

public class RunnerScript : MonoBehaviour
{
    [SerializeField] private Transform model;
    [SerializeField] private Transform localMoverTarget;
    [SerializeField] private PathCreator pathCreator;
    [SerializeField] private SimpleAnimancer animancer;
    [SerializeField] private PlayerSwerve playerSwerve;
    [Space]

    [SerializeField] private string StartIdleAnimName = "StartIdle";
    [SerializeField] private float StarIdleAnimSpeed = 1f;

    [SerializeField] private string idleAnimName = "Idle";
    [SerializeField] private float idleAnimSpeed = 1f;

    [SerializeField] private string walkingAnimName = "Walking";
    [SerializeField] private float walkingAnimSpeed = 1f;

    [SerializeField] private string hitAnimName = "Hit";
    [SerializeField] private float hitAnimSpeed = 1f;

    [SerializeField] private string danceAnimName = "Dance";
    [SerializeField] private float danceAnimSpeed = 1f;

    [SerializeField] private string currentAnimName = "Walking";
    [Space]

    [SerializeField] private float distance = 0;
    [SerializeField] private float startDistance = 0;
    [SerializeField] private float clampLocalX = 2f;
    [Space]

    [SerializeField] private float runSpeed = 2;
    [SerializeField] private float localTargetswipeSpeed = 2f;
    [SerializeField] private float swipeLerpSpeed = 2f;
    [SerializeField] private float swipeRotateLerpSpeed = 2f;

    private Vector3 oldPosition;
    private bool canRun = false;
    private bool canSwerve = false;
    private bool moveEnabled = false;


    void Awake()
    {
        playerSwerve.OnSwerve += PlayerSwipe_OnSwerve;
        distance = startDistance;
        oldPosition = localMoverTarget.localPosition;
    }

    public void Init()
    {
        pathCreator = FindObjectOfType<PathCreator>();
    }

    private void Start()
    {
        PlayAnimation(StartIdleAnimName, 1f);
    }

    // Update is called once per frame
    void Update()
    {
        UpdatePath();
        FollowLocalMoverTarget();
        oldPosition = model.localPosition;
    }

    public void StartToRun(bool checkRun)
    {
        moveEnabled = checkRun;

        if (moveEnabled)
        {
            canRun = true;
            canSwerve = true;
            PlayAnimation(walkingAnimName, 1f);
            currentAnimName = walkingAnimName;

            distance += runSpeed * Time.deltaTime;
        }
        else
        {
            canRun = false;
            canSwerve = false;
            PlayAnimation(idleAnimName, 1f);
            currentAnimName = idleAnimName;
        }
    }

    void UpdatePath()
    {
        if (canRun)
        {
            transform.position = pathCreator.path.GetPointAtDistance(distance);
            transform.eulerAngles = pathCreator.path.GetRotationAtDistance(distance).eulerAngles + new Vector3(0f, 0f, 90f);
        }
    }

    private void PlayerSwipe_OnSwerve(Vector2 direction)
    {
        if (canRun && canSwerve)
        {
            localMoverTarget.localPosition = localMoverTarget.localPosition + Vector3.right * direction.x * localTargetswipeSpeed * Time.deltaTime;
            ClampLocalPosition();
        }
    }

    void ClampLocalPosition()
    {
        Vector3 pos = localMoverTarget.localPosition;
        pos.x = Mathf.Clamp(pos.x, -clampLocalX, clampLocalX);
        localMoverTarget.localPosition = pos;

    }

    void FollowLocalMoverTarget()
    {
        if (canSwerve)
        {
            Vector3 direction = localMoverTarget.localPosition - oldPosition;
            animancer.GetAnimatorTransform().forward = Vector3.Lerp(animancer.GetAnimatorTransform().forward, direction, swipeRotateLerpSpeed * Time.deltaTime);

            Vector3 nextPos = new Vector3(localMoverTarget.localPosition.x, model.localPosition.y, model.localPosition.z); ;
            model.localPosition = Vector3.Lerp(model.localPosition, nextPos, swipeLerpSpeed * Time.deltaTime);
        }
    }

    public void PlayAnimation(string animName, float animSpeed)
    {
        animancer.PlayAnimation(animName);
        animancer.SetStateSpeed(animSpeed);

        currentAnimName = walkingAnimName;
    }

    public void DodgeBack()
    {
        StartCoroutine(DodgeBackProcess());
    }

    IEnumerator DodgeBackProcess()
    {
        canSwerve = false;
        canRun = false;
        animancer.PlayAnimation("Hit");

        yield return new WaitForSeconds(0.933f);

        animancer.PlayAnimation(currentAnimName);
        canRun = true;
        canSwerve = true;
    }

    public void ResetCharacter()
    {
        PlayAnimation(StartIdleAnimName, 1f);
        distance = 0;
        localMoverTarget.localPosition = new Vector3 (0, 0, 1f);
        canRun = false;
        canSwerve = false;
    }
}
