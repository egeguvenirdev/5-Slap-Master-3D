using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollidableObjects : MonoBehaviour
{
    private Bermuda.Runner.BermudaRunnerCharacter bermudaRunnerCharacter;

    public ObjectType objectType;

    public enum ObjectType
    {
        Fastfood,
        Fruit,
        Cheese,
        Obstacle,
        FinishLine,
    }

    private void Start()
    {
        bermudaRunnerCharacter = FindObjectOfType<Bermuda.Runner.BermudaRunnerCharacter>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (objectType == ObjectType.Fastfood)
        {
            var particle = ObjectPooler.Instance.GetPooledObject("MoneyParticle");
            particle.transform.position = other.gameObject.transform.position + new Vector3(0f, 0.75f, 0.5f);
            particle.transform.rotation = gameObject.transform.rotation;
            particle.SetActive(true);
            particle.GetComponent<ParticleSystem>().Play();

            PlayerManagement.Instance.AddHealth(10);
            gameObject.SetActive(false);
        }

        if (objectType == ObjectType.Fruit)
        {
            var particle = ObjectPooler.Instance.GetPooledObject("DiceParticle");
            particle.transform.position = transform.position;
            particle.transform.rotation = transform.rotation;
            particle.SetActive(true);
            particle.GetComponent<ParticleSystem>().Play();

            PlayerManagement.Instance.AddHealth(20);
            gameObject.SetActive(false);
        }

        if (objectType == ObjectType.Obstacle)
        {
            gameObject.SetActive(false);
            var particle = ObjectPooler.Instance.GetPooledObject("AtmParticle");
            particle.transform.position = transform.position;
            particle.transform.rotation = transform.rotation;
            particle.SetActive(true);
            particle.GetComponent<ParticleSystem>().Play();

            PlayerManagement.Instance.AddHealth(-10);
            bermudaRunnerCharacter.DodgeBack();
        }

        if (objectType == ObjectType.FinishLine)
        {
            Invoke("FinishedAction", 1.75f);
        }
    }

    private void FinishedAction()
    {

    }

}
