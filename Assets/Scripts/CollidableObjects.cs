using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollidableObjects : MonoBehaviour
{
    [SerializeField] RunnerScript runnerScript;
    public ObjectType objectType;

    public enum ObjectType
    {
        Fastfood,
        Fruit,
        Obstacle,
        FinishLine,
    }

    private void Start()
    {
        runnerScript = FindObjectOfType<RunnerScript>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (objectType == ObjectType.Fastfood)
        {
            var particle = ObjectPooler.Instance.GetPooledObject("FoodParticle");
            particle.transform.position = other.gameObject.transform.position + new Vector3(0f, 0.75f, 0.5f);
            particle.transform.rotation = gameObject.transform.rotation;
            particle.SetActive(true);
            particle.GetComponent<ParticleSystem>().Play();

            PlayerManagement.Instance.AddHealth(10);
            gameObject.SetActive(false);
        }

        if (objectType == ObjectType.Fruit)
        {
            var particle = ObjectPooler.Instance.GetPooledObject("FoodParticle");
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
            var particle = ObjectPooler.Instance.GetPooledObject("ObstacleParticle");
            particle.transform.position = transform.position;
            particle.transform.rotation = transform.rotation;
            particle.SetActive(true);
            particle.GetComponent<ParticleSystem>().Play();

            PlayerManagement.Instance.AddHealth(-50);
            runnerScript.DodgeBack(); //DUZENLEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEE
        }

        if (objectType == ObjectType.FinishLine)
        {
            Invoke("FinishedAction", 0.1f);
        }
    }

    private void FinishedAction()
    {
        PlayerManagement.Instance.FinishedAction();
    }

}
