using System.Collections.Generic;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class BoidManager : MonoBehaviour
{
    [SerializeField] private List<Boid> boids = new();
    [SerializeField] private GameObject boidPrefab;
    [SerializeField] private int amountOfBoids = 20;

    [SerializeField] private float cohesion = .5f;
    [SerializeField] private float seperation = 1.0f;
    [SerializeField] private float seperationRadius = 1.0f;
    [SerializeField] private float allignment = 4.0f;

    [SerializeField] private float boundaryRadius = 50.0f;

    [SerializeField] private float maxBoidVelocity = 20.0f;

    [SerializeField] private float BoidFieldOfView = 5.0f;

    private void Start()
    {
        InitializeBoids();
    }

    private Vector3 GetAverageVector(Boid currentBoid, bool velocity)
    {
        Vector3 average = Vector3.zero;
        int amountOfNeighbourBoids = 0;

        foreach (Boid boid in boids)
        {
            if (boid == currentBoid || boid.Rb == null) { continue; }

            if (Vector3.Distance(boid.transform.position, currentBoid.transform.position) <= BoidFieldOfView)
            {
                average += velocity ? boid.Rb.velocity : boid.transform.position;
                amountOfNeighbourBoids++;
            }
        }

        if (average.magnitude == 0 && velocity)
        {
            return Vector3.zero;
        }

        return average /= amountOfNeighbourBoids;
    }

    private void FixedUpdate()
    {
        if (boids.Count < amountOfBoids) { return; }

        foreach (Boid boid in boids)
        {
            boid.UpdateBoidPosition(ref boids, boundaryRadius, seperation, cohesion, seperationRadius, allignment, maxBoidVelocity, transform);
            boid.SetFlockCenterAndVelocity(GetAverageVector(boid, false), GetAverageVector(boid, true));
        }
    }

    private void InitializeBoids()
    {
        for (int i = 0; i < amountOfBoids; i++)
        {
            GameObject obj = Instantiate(boidPrefab);
            obj.TryGetComponent(out Boid boid);
            boids.Add(boid);

            Vector3 randomVector = new(Random.Range(-20, 20), Random.Range(-20, 20), Random.Range(-20, 20));
            boid.SetPosition(randomVector);
        }
    }
}
