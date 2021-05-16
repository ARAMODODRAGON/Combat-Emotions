using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	// movement
	[SerializeField] float verticalSpeed;
	[SerializeField] float horizontalSpeed;
	Vector2 moveDir;

	private bool wasmoving = false;
	public EnemyAnimator anim { get; private set; } = null;

	public Rigidbody2D rb { get; private set; }

	[SerializeField] CameraFollow cam;

	private void Awake()
	{
		anim = GetComponent<EnemyAnimator>();
		anim.destroyOnDie = false;
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

	private void OnEnable() {
		if (wasmoving) anim.ToggleMove();
		else anim.ToggleIdle();
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

		// update animation
		if (finalMovement.sqrMagnitude > 0.001f != wasmoving) {
			if (wasmoving) anim.ToggleIdle();
			else anim.ToggleMove();
			wasmoving = finalMovement.sqrMagnitude > 0.001f;
		}

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

	public CameraFollow GetCamera()
	{
		return cam;
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
