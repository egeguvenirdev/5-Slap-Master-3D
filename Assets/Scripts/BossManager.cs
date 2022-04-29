using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BossManager : MonoBehaviour
{
    [SerializeField] private int bossHealth = 600;
    [SerializeField] private int bossSlapPower = 200;
    [SerializeField] private Bermuda.Animation.SimpleAnimancer bossAnimancer;

    Sequence sequence;

    void Start()
    {
        DOTween.Init();
    }

    public void SpawnTheBoss()
    {
        bossHealth += PlayerPrefs.GetInt("HCLevel") * 50 ;
        sequence = DOTween.Sequence();

        sequence.Append(transform.DOJump(new Vector3(0, .25f, transform.position.z - 5.5f), 3, 1, 2.5f)
            .OnStart(() => { bossAnimancer.PlayAnimation("Fall"); }) );
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
        yield return new WaitForSeconds(1.58f);
        bossAnimancer.PlayAnimation("Idle");
        yield return new WaitForSeconds(1f);
        bossAnimancer.PlayAnimation("Slap");
        yield return new WaitForSeconds(1.1f);
        PlayerManagement.Instance.PlayerTookHit(bossSlapPower);
        yield return new WaitForSeconds(1.783f);
        bossAnimancer.PlayAnimation("Idle");
        //playerManager.PlayerTookHit();
    }

    public void BossTookHit(int damage)
    {
        if (bossHealth - damage > 0)
        {
            bossHealth -= damage;
            bossAnimancer.PlayAnimation("TakeHit");
        
            StartCoroutine(HitRoutine());
        }
        else
        {
            //burda geber
        }

    }
}
