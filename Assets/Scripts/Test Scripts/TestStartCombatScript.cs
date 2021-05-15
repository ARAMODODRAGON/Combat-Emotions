using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestStartCombatScript : MonoBehaviour {

	public CombatHandler handler;
	public PlayerController player;
	public EnemyController enemy;

	public enum StartSettings {
		OnAwake,
		OnKeyPress
	}

	public StartSettings startSetting;
	public KeyCode key;

	private void Awake() {
		if (startSetting == StartSettings.OnAwake)
			StartCombat();
	}

	private void Update() {
		if (startSetting == StartSettings.OnKeyPress) {
			if (Input.GetKeyDown(key))
				StartCombat();
		}
	}


	private void StartCombat() {
		handler.StartCombat(player, enemy);
		this.enabled = false;
	}
}