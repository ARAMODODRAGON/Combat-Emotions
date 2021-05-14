using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour {

	private static List<EnemyController> s_enemyControllers;

	public static int CountEnemies => s_enemyControllers.Count;
	public static EnemyController GetEnemy(int index) => s_enemyControllers[index];

	public Rigidbody2D body { get; private set; } = null;

	private void Awake() {
		s_enemyControllers.Add(this);
		body = GetComponent<Rigidbody2D>();
	}

	private void OnDestroy() {
		s_enemyControllers.Remove(this);
	}

}
