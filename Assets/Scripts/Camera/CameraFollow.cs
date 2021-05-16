using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// This doesn't need to be an FSM but I'm gonna make it one in case we need to add more states or have something done in the 
public enum CameraStates
{
	Follow,
	Locked,
	MoveLock, // move to a set position and then lock the camera
	MoveRight, // 
}


public class CameraFollow : MonoBehaviour
{
	// playerref
	public PlayerController player;
	public CameraStates state;

	// how long does it take for the camera to reach target
	[SerializeField] float cameraTime;

	// how close can the camera get to a set location before it stops
	[SerializeField] float tolerance;

	// used and modified by the smooth dampening function, just set it to 0 and let the function take care of it
	Vector3 cameraVelocity = Vector3.zero;
	Vector3 moveLocPosition;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void LateUpdate()
    {
		switch (state)
		{
			case CameraStates.Follow:
				{
					Follow();
					break;
				}
			case CameraStates.Locked:
				{

					break;
				}
			case CameraStates.MoveLock:
				{
					MoveLock();
					break;
				}
			case CameraStates.MoveRight:
				{
					Follow();
					break;
				}
		}
    }


	//lock the camera in it's current position
	public void Lock()
	{
		state = CameraStates.Locked;
	}

	public void Unlock()
	{
		state = CameraStates.Follow;
	}

	public void MoveRight()
	{
		state = CameraStates.MoveRight;
	}

	//move the camera to a set location before locking it
	void MoveLock(Vector3 pos_)
	{
		moveLocPosition = pos_;
		state = CameraStates.MoveLock;

		
	}

	void MoveLock()
	{
		Vector3 smoothTarget = Vector3.SmoothDamp(transform.position, moveLocPosition + new Vector3(0.0f, 0.0f, -10.0f), ref cameraVelocity, cameraTime);

		gameObject.transform.position = smoothTarget;


		if (Vector3.Distance(transform.position, moveLocPosition) < tolerance)
		{
			state = CameraStates.Locked;
		}
	}

	// smoothly follow the player
	void Follow()
	{
		Vector3 target = player.gameObject.transform.position + new Vector3(0.0f,0.0f, -10.0f);

		//Vector3 smoothTarget = Vector3.Lerp(transform.position, target, cameraSpeed * Time.deltaTime);
		Vector3 smoothTarget = Vector3.SmoothDamp(transform.position, target, ref cameraVelocity, cameraTime);

		if (state == CameraStates.Follow || (state == CameraStates.MoveRight && smoothTarget.x > gameObject.transform.position.x))
		{
			gameObject.transform.position = smoothTarget;
		}
	}

	public void HandleGateCollision(Gate g_)
	{ 
		if (g_ != null)
		{
			if (g_.triggered == false)
			{
				state = g_.state;
				if (g_.combat == true)
				{
					//EnemyManager.s_enmInstance.BeginEncounter(g_.encounterIndex);
					Debug.Log("Combat Called");
					Debug.Log(g_.encounterIndex);
				}

				g_.triggered = true;
			}
		}
	}
}
