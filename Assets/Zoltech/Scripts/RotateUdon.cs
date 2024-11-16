
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class RotateUdon : UdonSharpBehaviour
{
	public float x;
	public float y;
	public float z;
	void Start()
	{
		transform.Rotate(x + 0f, y + 0f, z + 0f);
	}

	// Update is called once per frame
	void Update()
	{

		//transform.Rotate(x + 0f, y + 0f, z + 0f);
	}
}
