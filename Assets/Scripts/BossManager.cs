using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BossManager : MonoBehaviour
{
    [SerializeField] private int bossHealth = 600;
    [SerializeField] private Bermuda.Animation.SimpleAnimancer bossAnimancer;

    Sequence sequence;

    void Start()
    {
        DOTween.Init();
    }

    public void CallTheBoss()
    {
        bossHealth *= PlayerPrefs.GetInt("HCLevel") * 50 ;
        sequence = DOTween.Sequence();

        sequence.Append(transform.DOJump(new Vector3(0, .25f, transform.position.z - 5.5f), 3, 1, 2.5f)
            .OnStart(() => { bossAnimancer.PlayAnimation("Fall"); }) );
        StartCoroutine(LandingSequence());

        PlayerManagement.Instance.SlapTheBoss();
    }

    IEnumerator LandingSequence()
    {
        Debug.Log("1");
        yield return new WaitForSeconds(2.4f);
        bossAnimancer.PlayAnimation("Landing");
        Debug.Log("2");

        yield return new WaitForSeconds(1.45f);
        bossAnimancer.PlayAnimation("Idle");
        Debug.Log("3");
        yield return null;
    }
}
