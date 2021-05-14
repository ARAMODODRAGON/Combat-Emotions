using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatHandler : MonoBehaviour {
	
	enum State : byte {
		Waiting,
		EnteringCombat,
		PlayingCombat,
		LeavingCombat
	}

	// starts combat between the given player and enemy
	public void StartCombat(PlayerController player, EnemyController enemy) {

		// step 1: freeze all other enemies


		// step 2: begin the animation of zooming in


	}

}
