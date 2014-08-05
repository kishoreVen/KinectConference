using UnityEngine;
using System.Collections;

public class FPSMove : MonoBehaviour 
{
	public float speed;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 movementDirection = Vector3.zero;

		if(Input.GetKey(KeyCode.W))
		{
			movementDirection.z = 1.0f;
		}
		if(Input.GetKey(KeyCode.S))
		{
			movementDirection.z = -1.0f;
		}
		if(Input.GetKey(KeyCode.A))
		{
			movementDirection.x = -1.0f;
		}
		if(Input.GetKey(KeyCode.D))
		{
			movementDirection.x = 1.0f;
		}
		if(Input.GetKey(KeyCode.LeftShift))
		{
			movementDirection.y = 1.0f;
		}
		if(Input.GetKey(KeyCode.LeftControl))
		{
			movementDirection.y = -1.0f;
		}

        if (Input.GetKey(KeyCode.UpArrow))
        {
            transform.Rotate(Vector3.right, 0.25f);
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            transform.Rotate(Vector3.right, -0.25f);
        }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            transform.Rotate(Vector3.up, -0.25f);
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            transform.Rotate(Vector3.up, 0.25f);
        }

		transform.position += movementDirection * Time.deltaTime * speed;
	}
}
