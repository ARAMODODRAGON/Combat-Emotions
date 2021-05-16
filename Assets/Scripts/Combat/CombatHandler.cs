using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// enters and exits battles, contains a logic handler to handle the battle logic itself
public class CombatHandler : MonoBehaviour {

	// this handles all the logic for the battle 
	[SerializeField] private CombatLogic m_logic = new CombatLogic();

	enum HandlerState : byte {
		Waiting,
		EnteringCombat,
		PlayingCombat,
		LeavingCombat
	}

	// current state
	private HandlerState m_handlerState = HandlerState.Waiting;
	private float m_currentLerpValue = 0.0f;

	// references to player and enemy
	private PlayerController m_player = null;
	private SpriteRenderer m_playerSprite = null;
	private EnemyController m_enemy = null;
	private SpriteRenderer m_enemySprite = null;
	private EnemyAnimator m_enemyAnimator = null;

	// positions of stuff
	private Vector3 m_playerOldPos = Vector3.zero;
	private Vector3 m_enemyOldPos = Vector3.zero;
	[SerializeField] private Transform m_playerTargetPos = null;
	[SerializeField] private Transform m_enemyTargetPos = null;

	// UI stuff
	[SerializeField] private UnityEngine.UI.Image m_fadeOverlay = null;
	[SerializeField] [Range(0.0f, 1.0f)] private float m_fadeOverlayTargetAlpha = 0.0f;
	[SerializeField] private UnityEngine.UI.Text m_scoreText = null;
	[SerializeField] private IntValue m_scoreValue;
	private int m_lastScore = 0;

	// timings of stuff
	[Header("Timings")]
	[SerializeField] private float m_timeToToggleCombat;

	private void Awake() {
		m_lastScore = m_scoreValue.Value;
		m_logic.enemyTargetPos = m_enemyTargetPos;
		m_logic.playerTargetPos = m_playerTargetPos;
		m_logic.scoreValue = m_scoreValue;
	}

	// starts combat between the given player and enemy
	public void StartCombat(PlayerController player, EnemyController enemy) {
		if (!player || !enemy) {
			Debug.LogError("missing player or enemy reference");
			return;
		}

		if (!m_logic.Init(player, enemy)) return;

		// step 0: set enemy and player references also change the enemy and player sprite sorting layer index
		m_player = player;
		m_enemy = enemy;
		m_playerSprite = player.GetComponent<SpriteRenderer>();
		m_enemySprite = enemy.GetComponent<SpriteRenderer>();
		m_logic.enemyAnimator = m_enemyAnimator = enemy.GetComponent<EnemyAnimator>();
		m_player.anim.ToggleIdle();
		m_playerSprite.sortingOrder = 2;
		m_enemySprite.sortingOrder = 2;

		// step 1: freeze all other enemies and stop the rigidbodies from taking effect
		EnemyManager.s_enmInstance.bEngagedInBattle = true;
		for (int i = 0; i < EnemyManager.countEnemies; i++) {
			EnemyController ec = EnemyManager.GetEnemy(i);
			if (ec != enemy) {
				ec.enabled = false;
				ec.GetComponent<EnemyAnimator>().anim.speed = 0.0f;
			}
			if (ec.body) ec.body.simulated = false;
		}
		player.enabled = false;
		Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
		if (rb) rb.simulated = false;


		// step 2: begin the animation of zooming in
		m_handlerState = HandlerState.EnteringCombat;
		m_currentLerpValue = 0.0f;
		m_playerOldPos = m_player.transform.position;
		m_enemyOldPos = m_enemy.transform.position;
		m_enemyAnimator.ToggleIdle();

	}

	private void Update() {
		switch (m_handlerState) {
			case HandlerState.Waiting: break;
			case HandlerState.EnteringCombat: UpdateEnterCombat(); break;
			case HandlerState.PlayingCombat: UpdatePlayingCombat(); break;
			case HandlerState.LeavingCombat: UpdateExitCombat(); break;
			default: Debug.LogError("Combat Handler unsupported state: " + m_handlerState); break;
		}

		if (m_lastScore != m_scoreValue.Value) {
			m_lastScore = m_scoreValue.Value;
			m_scoreText.text = m_lastScore.ToString().PadLeft(9, '0');
		}
	}

	private void UpdateEnterCombat() {
		// step 0: calculate delta
		m_currentLerpValue += Time.deltaTime;
		if (m_currentLerpValue > m_timeToToggleCombat) {
			m_currentLerpValue = m_timeToToggleCombat;
			m_handlerState = HandlerState.PlayingCombat;
			m_logic.BeginCombat();
		}
		float delta = m_currentLerpValue / m_timeToToggleCombat;

		// step 1: move player and enemy towards the target positions
		m_player.transform.position = Vector3.Lerp(m_playerOldPos, m_playerTargetPos.position, delta);
		m_enemy.transform.position = Vector3.Lerp(m_enemyOldPos, m_enemyTargetPos.position, delta);

		// step 2: lerp the alpha of the fade overlay
		m_fadeOverlay.color = Color.Lerp(Color.clear, new Color(0f, 0f, 0f, m_fadeOverlayTargetAlpha), delta);
	}

	private void UpdatePlayingCombat() {
		/* TMP */
		//if (Input.GetKeyDown(KeyCode.F)) m_handlerState = HandlerState.LeavingCombat;

		//// reset player and enemy locations
		//m_player.transform.position = m_playerTargetPos.position;
		//m_enemy.transform.position = m_enemyTargetPos.position;

		if (!m_logic.IsDone) m_logic.Step();
		else {
			m_scoreValue.Value += m_enemy.GetComponent<EnemyCombatInfo>().points;
			m_handlerState = HandlerState.LeavingCombat;
		}
	}

	private void UpdateExitCombat() {
		// step 0: calculate delta & change state
		m_currentLerpValue -= Time.deltaTime;
		if (m_currentLerpValue < 0f) {
			m_currentLerpValue = 0f;
			m_handlerState = HandlerState.Waiting;

			// change sorting layer
			m_playerSprite.sortingOrder = 0;
			m_enemySprite.sortingOrder = 0;
			EnemyManager.s_enmInstance.bEngagedInBattle = false;
			m_enemyAnimator.ToggleDie();

			// reenable stuff
			for (int i = 0; i < EnemyManager.countEnemies; i++) {
				EnemyController ec = EnemyManager.GetEnemy(i);
				if (ec != m_enemy) {
					ec.enabled = true;
					ec.GetComponent<EnemyAnimator>().anim.speed = 1.0f;
					if (ec.body) ec.body.simulated = true;
				}
			}
			m_player.enabled = true;
			m_player.GetComponent<Rigidbody2D>().simulated = true;
		}
		float delta = m_currentLerpValue / m_timeToToggleCombat;

		// step 1: move player and enemy away from the target positions
		m_player.transform.position = Vector3.Lerp(m_playerOldPos, m_playerTargetPos.position, delta);
		m_enemy.transform.position = Vector3.Lerp(m_enemyOldPos, m_enemyTargetPos.position, delta);

		// step 2: lerp the alpha of the fade overlay
		m_fadeOverlay.color = Color.Lerp(Color.clear, new Color(0f, 0f, 0f, m_fadeOverlayTargetAlpha), delta);
	}

}
