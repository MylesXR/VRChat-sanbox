using UdonSharp;
using UnityEngine;

public class Boid : UdonSharpBehaviour
{
    [Header("Boid Settings")]

    [Tooltip("The speed at which each boid moves.")]
    [SerializeField] private float speed = 7.0f;

    [Tooltip("The distance at which boids will repel from each other to avoid collision.")]
    [SerializeField] private float repulsionDistance = 1.0f;

    [Tooltip("The minimum scale size each boid can have.")]
    [Range(0, 100), SerializeField] private float minScale = 0.8f;

    [Tooltip("The maximum scale size each boid can have.")]
    [Range(0, 100), SerializeField] private float maxScale = 1.2f;

    [Tooltip("Reference to the InstantiateBoid script, which manages the entire flock.")]
    [SerializeField] public InstantiateBoid boids;

    // Random target position for the boid to move towards.
    private Vector3 randomTarget;

    // The Transform component of this boid, cached for performance.
    private Transform myTransform;

    // Array containing the Transform components of all boids, for easier data manipulation.
    private Transform[] boidTransforms;

    // The turning speed of the boid.
    private float turnSpeed;

    // The index of this particular boid in the array of all boids.
    private int myIndex;

    // Square of the repulsion distance, cached for performance.
    private float sqrRepulsionDistance;

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////

    void Start()
    {
        InitializeVariables();
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////

    private void InitializeVariables()
    {
        // Cache the Transform component for quicker access
        myTransform = transform;

        // Pre-compute squared repulsion distance for optimization
        sqrRepulsionDistance = repulsionDistance * repulsionDistance;

        // If there's no boids manager (Instantiate Boid Script), no need to proceed further
        if (boids == null) return;

        // Initialize various boid attributes
        PickRandomTarget();
        turnSpeed = TurnSpeed();
        SetRandomScale();

        // Populate the array of boid Transforms
        InitializeBoidArray();
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////

    private void InitializeBoidArray()
    {
        // Get an array of all boids from the Instantiate Boid Script
        GameObject[] boidArray = boids.GetAllBoids();

        // If there are no other boids, return
        if (boidArray == null) return;

        // Initialize the array for Transform components
        boidTransforms = new Transform[boidArray.Length];

        // Populate Transform array and find the index for this boid
        for (int i = 0; i < boidTransforms.Length; i++)
        {
            boidTransforms[i] = boidArray[i].transform;
            if (boidArray[i] == gameObject) myIndex = i;
        }
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////

    private void CalculatePositionAndRotation()
    {
        // Calculate and store the current boid position
        Vector3 myPosition = myTransform.position;

        // Calculate the vector towards the random target
        Vector3 toTarget = randomTarget - myPosition;

        // Check and update the random target if needed
        if (ShouldPickNewTarget(myPosition))
        {
            PickRandomTarget();
            toTarget = randomTarget - myPosition;
        }

        // Apply repulsion to avoid collisions
        ApplyRepulsion(myPosition);

        // Update the boid's position
        myTransform.position = myPosition;

        // Update the boid's rotation based on the new position
        UpdateRotation(toTarget);

        // Move the boid forward
        myTransform.Translate(Vector3.forward * Time.deltaTime * speed);
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////

    void Update()
    {

        // Conditional check to optimize update cycle
        if (boids == null || Time.frameCount % 6 != myIndex % 6) return;

        // Update boid's position and rotation
        CalculatePositionAndRotation();
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////

    private bool ShouldPickNewTarget(Vector3 myPosition)
    {
        // Transform the current position into the coordinate system of the boid manager
        Vector3 localMyPosition = boids.transform.InverseTransformPoint(myPosition);

        // Get half the dimensions of the wandering area
        Vector3 localHalfWanderSize = boids.wanderSize * 0.5f;

        // Check if the boid is out of the wandering area or reached its target
        return (Mathf.Abs(localMyPosition.x) > localHalfWanderSize.x ||
                Mathf.Abs(localMyPosition.y) > localHalfWanderSize.y ||
                Mathf.Abs(localMyPosition.z) > localHalfWanderSize.z) ||
               (randomTarget - myPosition).sqrMagnitude < 1f;
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////

    private void UpdateRotation(Vector3 toTarget)
    {
        // Calculate the rotation needed to look at the target
        Quaternion targetRotation = Quaternion.LookRotation(toTarget);

        // Smoothly rotate towards the target
        myTransform.rotation = Quaternion.Lerp(myTransform.rotation, targetRotation, turnSpeed * Time.deltaTime);
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////

    private void PickRandomTarget()
    {
        // Get half the dimensions of the wandering area
        Vector3 localHalfWanderSize = boids.wanderSize * 0.5f;

        // Pick a random point within the wandering area
        Vector3 localRandomTarget = new Vector3(
            Random.Range(-localHalfWanderSize.x, localHalfWanderSize.x),
            Random.Range(-localHalfWanderSize.y, localHalfWanderSize.y),
            Random.Range(-localHalfWanderSize.z, localHalfWanderSize.z)
        );

        // Transform the random point into world coordinates
        randomTarget = boids.transform.TransformPoint(localRandomTarget);
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////

    private float TurnSpeed()
    {
        // Calculate a random turn speed based on the boid's current speed
        return Random.Range(0.2f, 0.4f) * speed;
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////

    private void SetRandomScale()
    {
        // Pick a random scale within the allowed range
        float randomScale = Random.Range(minScale, maxScale);

        // Apply the random scale to the boid
        myTransform.localScale = new Vector3(randomScale, randomScale, randomScale);
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////

    private void ApplyRepulsion(Vector3 myPosition)
    {
        // Calculate the repulsion factor based on distance and time
        float repulsionFactor = repulsionDistance * Time.deltaTime;

        // Limit the number of boids to check for performance
        int checkCount = boidTransforms.Length / 12;

        // Initialize the direction in which to repel the boid
        Vector3 repulsionDirection = Vector3.zero;

        // Loop through nearby boids to apply repulsion
        for (int i = myIndex % 6; i < boidTransforms.Length; i += 6)
        {
            if (--checkCount < 0) break;

            // Calculate the vector to the other boid
            Vector3 toFish = myPosition - boidTransforms[i].position;

            // Calculate the squared distance to the other boid
            float distance = toFish.sqrMagnitude;

            // If the other boid is too close, apply repulsion
            if (distance < sqrRepulsionDistance)
            {
                repulsionDirection = toFish / Mathf.Sqrt(distance);
                myPosition += repulsionDirection * repulsionFactor;
            }
        }
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////

}