using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Boid : MonoBehaviour
{
    public Rigidbody Rb { get; private set; }

    [SerializeField] private int boidLayer = 1;

    private Vector3 flockCenter = Vector3.zero;
    private Vector3 flockVelocity = Vector3.zero;

    private void Start()
    {
        Rb = GetComponent<Rigidbody>();
        Rb.useGravity = false;
    }

    public void SetPosition(Vector3 position)
    {
        transform.position = position;
    }

    public void SetFlockCenterAndVelocity(Vector3 centerPosition, Vector3 centerVelocity)
    {
        flockCenter = centerPosition;
        flockVelocity = centerVelocity;
    }

    private void VelocityLimiting(float maxVelocity)
    {
        if (Rb.velocity.magnitude > maxVelocity)
        {
            Vector3 limitedVelocity = Rb.velocity.normalized * maxVelocity;
            Rb.velocity = limitedVelocity;
        }
    }

    private void SeperationHandling(float seperation, float seperationRadius, ref List<Boid> boids)
    {

        foreach (Boid boid in boids)
        {
            if (Vector3.Distance(boid.transform.position, transform.position) > seperationRadius) { continue; }

            Vector3 directionToBoid = boid.transform.position - transform.position;
            Rb.AddForce(10.0f * seperation * -directionToBoid.normalized, ForceMode.Force);
        }
    }

    public void UpdateBoidPosition(ref List<Boid> boids, float flockRadius, float seperation, float coherence, float seperationRadius = 3.0f, float allignment = 3.0f, float maxVelocity = 20.0f, Transform parent = null)
    {
        if (Rb == null) { return; }

        SeperationHandling(seperation, seperationRadius, ref boids);

        if (Vector3.Distance(transform.position, flockCenter) >= seperationRadius && flockCenter != null && Rb != null)
        {
            Vector3 directionToCenter = flockCenter - transform.position;

            Rb.AddForce(10.0f * coherence * directionToCenter.normalized, ForceMode.Force);
        }

        if (parent != null)
        {
            if (Vector3.Distance(transform.position, parent.transform.position) >= flockRadius)
            {
                Vector3 direction = parent.transform.position - transform.position;

                Rb.AddForce(25.0f * coherence * direction.normalized, ForceMode.Force);
            }
        }

        if (flockVelocity != null && Rb != null && flockVelocity != Vector3.zero)
        {
            Rb.velocity += (flockVelocity / allignment);
        }

        VelocityLimiting(maxVelocity);
        transform.LookAt(Rb.velocity.normalized + gameObject.transform.position);
    }
}
