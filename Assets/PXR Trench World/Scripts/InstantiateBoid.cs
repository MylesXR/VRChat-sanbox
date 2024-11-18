using UdonSharp;
using UnityEngine;

////////////////////////////////////////////////////////////////////////////////////////////////////////////

public class InstantiateBoid : UdonSharpBehaviour
{
    [Header("Boid Settings")]

    [Tooltip("Defines the volume within which the boids can wander. " +
     "Specifies the range for the random positioning of each boid in the flock.")]
    [SerializeField] public Vector3 wanderSize = new Vector3(7, 7, 7);

    [Tooltip("The rotation of the wander area in Euler angles.")]
    [SerializeField] public Vector3 wanderRotationEuler = Vector3.zero;

    [Tooltip("The prefab used to spawn each boid. This should be a pre-designed GameObject " +
     "containing the Boid script and any associated visuals.")]
    [SerializeField] GameObject prefab;

    [Tooltip("The number of boids to spawn within the wander area. " +
     "This value determines the size of the flock and can be adjusted to create larger or smaller groups.")]
    [SerializeField] int numberOfBoids = 10;

    [Tooltip("The color of the wander area drawn in the scene view.")]
    [SerializeField] private Color gizmoColor = Color.blue;

    // Array to store all the boids; allows easy access to all spawned boids
    private GameObject[] boidArray;

    // Quaternion to represent the rotation of the wander area
    public Quaternion wanderRotation;

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////

    void Start()
    {
        // Initialize the wanderRotation based on the Euler angles provided
        wanderRotation = Quaternion.Euler(wanderRotationEuler);

        // Initialize the array with the specified number of boids
        boidArray = new GameObject[numberOfBoids];

        // Calculate once to avoid repetitive calculations; improves performance by avoiding redundant operations
        Vector3 halfWanderSize = wanderSize / 2;

        // Loop to spawn each boid
        for (int i = 0; i < numberOfBoids; i++)
        {
            // Random spawn position within the defined wander area; helps create a natural and
            // dynamic flock appearance
            Vector3 spawnPos = new Vector3(
                Random.Range(-halfWanderSize.x, halfWanderSize.x),
                Random.Range(-halfWanderSize.y, halfWanderSize.y),
                Random.Range(-halfWanderSize.z, halfWanderSize.z)
            );

            // Apply rotation to the spawn position
            spawnPos = wanderRotation * spawnPos;

            // Make the spawn position relative to the flock GameObject; allows the entire flock
            // to be moved as a single unit
            spawnPos += transform.position;

            // Instantiate boid at the spawn position
            boidArray[i] = Instantiate(prefab, spawnPos, Quaternion.identity);

            // Get reference to the Boid script to set properties
            Boid boidScript = boidArray[i].GetComponent<Boid>();

            // Set reference to this script; enables coordination between individual boids and the flock controller
            if (boidScript != null)
            {
                boidScript.boids = this;
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////

    public GameObject[] GetAllBoids()
    {
        // Function to access all boids; allows external scripts to interact with the flock
        return boidArray; 
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////

    private void OnDrawGizmos()
    {
        // Set Gizmo color to blue
        Gizmos.color = gizmoColor;

        // Set the Gizmo matrix for rotation and translation
        Gizmos.matrix = Matrix4x4.TRS(transform.position, Quaternion.Euler(wanderRotationEuler), Vector3.one);

        // Draw the wander area as a wire cube
        Gizmos.DrawWireCube(Vector3.zero, wanderSize);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////
    
}