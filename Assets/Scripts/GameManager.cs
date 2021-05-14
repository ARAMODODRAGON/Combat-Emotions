using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameManager", menuName = "Managers/GameManager")]
public class GameManager : ScriptableObject {

	// Singleton
	public GameManager Get() {
		// untested
		if (!__instance) {
			__instance = FindObjectOfType<GameManager>();
			if (!__instance) __instance = CreateInstance<GameManager>();
		}
		return __instance;
	}
	private GameManager __instance = null;

	// game state
	public bool IsPaused { get; private set; } = false;

	// pause event
	public delegate void PauseChangeHandler();

	public PauseChangeHandler OnPause;
	public PauseChangeHandler OnUnpause;

	// toggles the pause state and returns the new pause value
	public bool TogglePause() {
		return IsPaused = !IsPaused;
	}

}
