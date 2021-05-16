using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour {

	private static List<EnemyController> s_enemyControllers = new List<EnemyController>();

<<<<<<< HEAD
	public static int CountEnemies => s_enemyControllers.Count;
	public static EnemyController GetEnemy(int index) => s_enemyControllers[index];
=======
	private void FixedUpdate()
    {
		if (b_Moving && !EnemyManager.s_enmInstance.bEngagedInBattle)
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
>>>>>>> Development

	public Rigidbody2D body { get; private set; } = null;

	private void Awake() {
		s_enemyControllers.Add(this);
		body = GetComponent<Rigidbody2D>();
	}

	private void OnDestroy() {
		s_enemyControllers.Remove(this);
	}

	public void StartMoving()
    {
		Debug.Log("Start moving");
		b_Moving = true;
		gameObject.GetComponent<EnemyAnimator>().ToggleMove();

	}

	private void OnTriggerEnter2D(Collider2D col_)
    {
		if(col_!=this && col_!=null)
        {
			PlayerController pc_ = col_.GetComponent<PlayerController>();
			if(pc_)
            {
				//Add code to begin fight
            }
        }
    }

}
