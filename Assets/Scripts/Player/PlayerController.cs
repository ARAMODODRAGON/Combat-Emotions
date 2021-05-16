using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	// movement
	[SerializeField] float verticalSpeed;
	[SerializeField] float horizontalSpeed;
	Vector2 moveDir;

	public Rigidbody2D rb { get; private set; }

	public CameraFollow cam;

	private void Awake()
	{

	}

	public void Start()
	{
		// get the RB
		rb = GetComponent<Rigidbody2D>();
	}

	public void Update()
	{
		HandleInput();
	}

	//physics stuff
	public void FixedUpdate()
	{
		HandleMovement();
	}

	void HandleInput()
	{
		moveDir = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
	}

	void HandleMovement()
	{
		// beat em ups usually have some sort of difference between vert and horiz speed so we account for that here
		// make sure the movement direction is normalized


		Vector2 finalMovement;

		finalMovement.x = moveDir.x * horizontalSpeed * Time.fixedDeltaTime;
		finalMovement.y = moveDir.y * verticalSpeed * Time.fixedDeltaTime;

		// null check
		if (rb != null)
		{
			rb.velocity = finalMovement;
		}
		else
		{
			Debug.Log("Rigidbody is null");
		}
	}

	private void OnTriggerEnter2D(Collider2D col)
	{
		if (col.CompareTag("Gate"))
		{
			Gate g = col.GetComponent<Gate>();

			if (g != null)
			{
				cam.HandleGateCollision(g);
			}
		}
	}

}
