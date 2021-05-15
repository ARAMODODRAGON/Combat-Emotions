using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour 
{

	public Rigidbody2D body { get; private set; } = null;
	public SpriteRenderer spriteRend { get; private set; } = null;
	private int enemyIndex = -1;
	[HideInInspector]
	public bool b_Moving = false;
	public float speed = 10.0f;

	private void Start() 
	{
		enemyIndex = EnemyManager.s_enmInstance.AddEnemyController(this);
		body = gameObject.GetComponent<Rigidbody2D>();
		spriteRend = gameObject.GetComponent<SpriteRenderer>();
	}

	private void FixedUpdate()
    {
		if (b_Moving)
		{
			PlayerController playerRef = EnemyManager.s_enmInstance.playerRef;
			Vector2 directionToPlayer = (playerRef.transform.position - gameObject.transform.position);
			directionToPlayer.Normalize();
			if(body)
            {
				Vector2 movementVec = new Vector2(gameObject.transform.position.x, gameObject.transform.position.y) + directionToPlayer * speed * Time.deltaTime;
				body.MovePosition(movementVec);
            }
			if (spriteRend)
			{
				if (playerRef.transform.position.x > gameObject.transform.position.x && !spriteRend.flipX) //Sprites all face right by default
					spriteRend.flipX = false;
				else if (playerRef.transform.position.x > gameObject.transform.position.x && spriteRend.flipX)
					spriteRend.flipX = true;
			}

		}

	}

	private void OnDestroy() 
	{
		EnemyManager.s_enmInstance.RemoveEnemyContoller(enemyIndex);
	}

}
