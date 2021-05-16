using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCombatInfo : MonoBehaviour {

	[Tooltip("the emotion pattern to be used when attacking")]
	public Pattern pattern = null;

	[Tooltip("the speed that each attack in the pattern will come at (multiplier)")]
	public float attackSpeed = 1f;

	[Tooltip("the delay at the end of the pattern before the next pattern starts")]
	public float delay = 2f;

	[Tooltip("the health of the enemy (obviously)")]
	public int health = 1;

	[Tooltip("the points that are given for killing this enemy")]
	public int points = 100;

	[Tooltip("the type of enemy")]
	public int targetType;

}
