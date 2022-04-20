using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ObstacleRotator : MonoBehaviour
{
    public ObjectType objectType;

    public enum ObjectType
    {
        LeftBarbed,
        RightBarbed,
        RightCylinder,
        LefttCylinder,
        BigCylinder,
        Saw
    }

    void Start()
    {
        DOTween.Init();
        StartRotate();
    }

    public void StartRotate()
    {
        if (objectType == ObjectType.BigCylinder)
        {
            transform.DORotate(new Vector3(0, 180, 0), 2).SetEase(Ease.Linear).SetLoops(-1, LoopType.Restart);
        }

        if (objectType == ObjectType.RightBarbed)
        {
            transform.DORotate(new Vector3(-90, 0, 180), 1).SetEase(Ease.Linear).SetLoops(-1, LoopType.Restart);
        }

        if (objectType == ObjectType.LeftBarbed)
        {
            transform.DORotate(new Vector3(-90, 0, 180), 1).SetEase(Ease.Linear).SetLoops(-1, LoopType.Restart);
        }

        if (objectType == ObjectType.RightCylinder)
        {
            transform.DORotate(new Vector3(0, 360, 0), 3).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
        }

        if (objectType == ObjectType.LefttCylinder)
        {
            transform.DORotate(new Vector3(0, 180, 0), 3).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
        }

        if (objectType == ObjectType.Saw)
        {
            transform.DOLocalMove(new Vector3(0, 3.4f, 0), 1.5f).SetLoops(-1, LoopType.Yoyo);
            transform.DOLocalRotate(new Vector3(180, 0, 0), 0.1f).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
        }
    }

    public void StopRotate()
    {

    }
}
