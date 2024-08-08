using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class MeshBreak : UdonSharpBehaviour
{
    public Rigidbody[] rbs;
    public bool[] isBroken;
    public int breakerLayer; // The layer to identify the breaker

    void Start()
    {
        foreach (Rigidbody rb in rbs)
        {
            rb.isKinematic = true;
        }
        isBroken = new bool[rbs.Length];
    }

    private void Update()
    {
        for (int i = 0; i < rbs.Length; i++)
        {
            if (isBroken[i])
            {
                rbs[i].isKinematic = false;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == breakerLayer)
        {
            for (int i = 0; i < isBroken.Length; i++)
            {
                isBroken[i] = true;
            }
        }
    }
}
