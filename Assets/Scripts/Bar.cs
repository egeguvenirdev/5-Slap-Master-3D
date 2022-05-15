using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class Bar : MonoBehaviour
{
    [Header("Bar")] 
    [SerializeField] private GameObject multiplierBar;
    [SerializeField] private TMP_Text slapBoardText;
    [SerializeField] private Transform arrow;
   
    private bool clickBonusCheck = false;
    public float multiplier;

    Sequence sequenceCamAndBar;

    private void Start()
    {
        DOTween.Init();
    }

    public void MoveMultiplierArrow()
    {
        arrow.DOLocalMoveX(-4.4f, 0);
        arrow.DOLocalMoveX(4.4f, 2).SetEase(Ease.InOutQuint).SetLoops(-1, LoopType.Yoyo);
        StartCoroutine(HandTransform());
    }

    IEnumerator HandTransform()
    {
        float arrowXpos = arrow.localPosition.x;

        if (arrowXpos > -4.5f && arrowXpos < -3.5f)
        {
            SetMuiltiplier(1);
        }

        if (arrowXpos > -3.5f && arrowXpos < -2.5f)
        {
            SetMuiltiplier(2);
        }

        if (arrowXpos > -2.5f && arrowXpos < -1.5f)
        {
            SetMuiltiplier(3);
        }

        if (arrowXpos > -1.5f && arrowXpos < -0.5f)
        {
            SetMuiltiplier(4);
        }

        if (arrowXpos > -0.5f && arrowXpos < 0.5f)
        {
            SetMuiltiplier(5);
        }

        if (arrowXpos > 0.5f && arrowXpos < 1.5f)
        {
            SetMuiltiplier(4);
        }

        if (arrowXpos > 1.5f && arrowXpos < 2.5f)
        {
            SetMuiltiplier(3);
        }

        if (arrowXpos > 2.5f && arrowXpos < 3.5f)
        {
            SetMuiltiplier(2);
        }

        if (arrowXpos > 3.5f && arrowXpos < 4.5f)
        {
            SetMuiltiplier(1);
        }

        if (!clickBonusCheck)
        {
            yield return new WaitForFixedUpdate();
            StartCoroutine(HandTransform());
        }
        else
        {
            SlapButton();
        }
    }

    //RENKLERI AYARLAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA
    private void SetMuiltiplier(float multiplierInt)
    {
        slapBoardText.text = "POWER ->" + PlayerManagement.Instance.ReturnPower() * multiplierInt;
        multiplier = multiplierInt;

        if (multiplierInt == 1)
        {
            slapBoardText.color = new Color32(231, 12, 12, 255);
        }

        if (multiplierInt == 2)
        {
            slapBoardText.color = new Color32(255, 153, 21, 255);
        }

        if (multiplierInt == 3)
        {
            slapBoardText.color = new Color32(250, 205, 51, 255);
        }

        if (multiplierInt == 4)
        {
            slapBoardText.color = new Color32(105, 179, 76, 255);
        }

        if (multiplierInt == 5)
        {
            slapBoardText.color = new Color32(105, 179, 76, 255);
        }
    }

    public void SlapButton()
    {
        clickBonusCheck = true;
        StopCoroutine(HandTransform());
        DOTween.Kill(arrow.transform);
        ResSlapButton();
    }

    public void ResSlapButton()
    {
        clickBonusCheck = false;
        slapBoardText.text = "POWER -> 50";
    }

    public void InstantiateTheBar()
    {
        multiplierBar.SetActive(true);
        sequenceCamAndBar.Append(multiplierBar.transform.DOMoveY(multiplierBar.transform.position.y - 7.5f, 1.5f));
        sequenceCamAndBar.Join(multiplierBar.transform.DORotate(new Vector3(0, -60, 0), 1.5f, RotateMode.FastBeyond360)
            .SetEase(Ease.Linear).SetLoops(2, LoopType.Restart));
    }
}
