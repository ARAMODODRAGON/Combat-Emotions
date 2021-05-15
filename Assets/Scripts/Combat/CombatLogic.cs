using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatLogic {

	enum CombatState : byte {

	}

	// references
	private PlayerController m_player = null;
	private EnemyController m_enemy = null;
	private EnemyCombatInfo m_eci = null;
	public Transform playerTargetPos = null;
	public Transform enemyTargetPos = null;

	public void Init(PlayerController player, EnemyController enemy) {
		m_player = player;
		m_enemy = enemy;
		m_eci = m_enemy.GetComponent<EnemyCombatInfo>();
		if (!m_eci) Debug.LogError($"Cannot start battle because enemy did not have a component of type {typeof(EnemyCombatInfo).Name}");
	}

	public void Step() {

	}

}
