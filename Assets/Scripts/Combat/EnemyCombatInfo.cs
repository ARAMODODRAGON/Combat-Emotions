using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCombatInfo : MonoBehaviour {

	[Tooltip("the emotion pattern to be used when attacking")]
	public Pattern pattern = null;

	[Tooltip("the speed that each attack in the pattern will come at")]
	public float attackSpeed = 1f;

	[Tooltip("the delay at the end of the pattern before the next pattern starts")]
	public float delay = 2f;

}
