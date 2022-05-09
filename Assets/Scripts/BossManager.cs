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
    private bool isItFirstSlap = true;

    Sequence sequence;

    void Start()
    {
        DOTween.Init();
    }

    public void SpawnTheBoss()
    {
        bossMaxHealth += PlayerPrefs.GetInt("HCLevel") * 50 ;
        bossCurrentHealth = bossMaxHealth;
        sequence = DOTween.Sequence();

        sequence.Append(transform.DOJump(new Vector3(0, .25f, PlayerManagement.Instance.bossSpawnPosition.position.z - 5.5f), 3, 1, 2.5f)
            .OnStart(() => { bossAnimancer.PlayAnimation("Fall"); })
            .OnComplete(() => { UIManager.Instance.TurnOnOffUIs(true); }));
        StartCoroutine(LandingSequence());

    }

    IEnumerator LandingSequence()
    {
        yield return new WaitForSeconds(2.4f);
        bossAnimancer.PlayAnimation("Landing");

        yield return new WaitForSeconds(1.45f);
        bossAnimancer.PlayAnimation("Idle");

        yield return null;
    }

    IEnumerator HitRoutine()
    {
        if (isItFirstSlap)
        {
            isItFirstSlap = false;
        }
        else
        {
            yield return new WaitForSeconds(1.58f);
        }
        bossAnimancer.PlayAnimation("Idle");
        yield return new WaitForSeconds(1f);
        bossAnimancer.PlayAnimation("Slap");
        yield return new WaitForSeconds(1.1f);
        PlayerManagement.Instance.PlayerTookHit(bossSlapPower);
        yield return new WaitForSeconds(1.783f);
        bossAnimancer.PlayAnimation("Idle");
        //playerManager.PlayerTookHit();
    }

    public void BossTookHit(float damage)
    {
        if (bossCurrentHealth - damage > 0)
        {
            UIManager.Instance.SetSlapUIs("Boss");
            bossCurrentHealth -= damage;
            bossAnimancer.PlayAnimation("TakeHit");
        
            StartCoroutine(HitRoutine());
        }
        else
        {
            UIManager.Instance.TurnOnOffUIs(false);
        }

    }
    public float ReturnHealth()
    {
        return bossCurrentHealth / bossMaxHealth;
    }
}
