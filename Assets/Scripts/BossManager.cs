using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BossManager : MonoBehaviour
{
    [SerializeField] private float bossMaxHealth = 600;
    [SerializeField] private float bossCurrentHealth;
    [SerializeField] private float bossSlapPower = 200;
    [SerializeField] private Bermuda.Animation.SimpleAnimancer bossAnimancer;

    Sequence sequence;

    void Start()
    {
        DOTween.Init();
    }

    public void SpawnTheBoss()
    {
        gameObject.SetActive(true);
        bossMaxHealth += PlayerPrefs.GetInt("HCLevel") * 50;
        bossCurrentHealth = bossMaxHealth;
        sequence = DOTween.Sequence();

        Vector3 targetLocation = new Vector3(0, .25f, transform.position.z - 5.5f);
        sequence.Append(transform.DOJump(targetLocation, 3, 1, 2.5f)
            .OnStart(() => { bossAnimancer.PlayAnimation("Fall"); }));
        StartCoroutine(LandingSequence());
    }

    IEnumerator LandingSequence()
    {
        yield return new WaitForSeconds(2.4f);
        bossAnimancer.PlayAnimation("Landing");

        yield return new WaitForSeconds(1.45f);
        bossAnimancer.PlayAnimation("Idle");
        UIManager.Instance.TurnOnOffUIs(true);
        UIManager.Instance.SetHealthUIs();
    }

    IEnumerator HitRoutine()
    {
        yield return new WaitForSeconds(1.58f);

        bossAnimancer.PlayAnimation("Slap");
        yield return new WaitForSeconds(1.1f); //wait for the exact hit moment
        PlayerManagement.Instance.PlayerTookHit(bossSlapPower);       

        yield return new WaitForSeconds(1.574f);
        bossAnimancer.PlayAnimation("Idle");
    }
     IEnumerator TakeSlapRoutine()
    {
        yield return new WaitForSeconds(1.01f);
        bossAnimancer.PlayAnimation("Idle");
    }

    public void BossTookHit(float damage)
    {
        if (bossCurrentHealth - damage > 0)
        {
            StartCoroutine(TakeSlapRoutine());
            bossCurrentHealth -= damage;
            UIManager.Instance.SetHealthUIs();
            bossAnimancer.PlayAnimation("TakeHit");

            StartCoroutine(HitRoutine());
        }
        else
        {
            sequence = DOTween.Sequence();
            float pathRange = (damage - bossCurrentHealth) / 10;
            UIManager.Instance.TurnOnOffUIs(false);
            PlayerManagement.Instance.SetBossFollowCam();
            bossAnimancer.PlayAnimation("Fly");
            sequence.Append(transform.DOJump(new Vector3(0, 0.125f, (120 + (7.5f * (int)pathRange))), 10, 1, 5).SetSpeedBased()
                .OnComplete(() =>
                {
                    bossAnimancer.PlayAnimation("FallImpact");
                    DragTheBoss();
                }));
        }
    }

    private void DragTheBoss()
    {
        sequence = DOTween.Sequence();
        Vector3 slidePosition = new Vector3(transform.position.x, transform.position.y, transform.position.z + Random.Range(2, 10));
        sequence.Append(transform.DOMove(slidePosition, 1)
                .OnComplete(() => { BossLastLocation(); }));
    }

    public void BossLastLocation()
    {
        UIManager.Instance.SetTotalMoney();
        UIManager.Instance.NextLvUI();
    }

    public void PlayAnimation(string animName)
    {
        bossAnimancer.PlayAnimation(animName);
    }

    public float ReturnHealth()
    {
        return bossCurrentHealth / bossMaxHealth;
    }
}
