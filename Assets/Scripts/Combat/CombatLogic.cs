using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CombatLogic {

	enum CombatState : byte {
		None = 0,
		NormalPause,
		EnemyCombo_ShowEmotion,
		EnemyCombo_ShortPauseA,
		EnemyCombo_Attack,
		EnemyCombo_ShortPauseB,
		PostComboPause
	}

	// input
	[SerializeField] private KeyCode m_emoteHappyKey;
	[SerializeField] private KeyCode m_emoteSadKey;
	[SerializeField] private KeyCode m_emoteAngryKey;
	private bool HappyKeyDown => Input.GetKeyDown(m_emoteHappyKey);
	private bool SadKeyDown => Input.GetKeyDown(m_emoteSadKey);
	private bool AngryKeyDown => Input.GetKeyDown(m_emoteAngryKey);

	// references
	[SerializeField] private IntValue m_playerHealth;
	[SerializeField] private IntValue m_scoreValue;

	private PlayerController m_player = null;
	private EnemyController m_enemy = null;
	private EnemyCombatInfo m_eci = null;
	[HideInInspector] public Transform playerTargetPos = null;
	[HideInInspector] public Transform enemyTargetPos = null;
	[HideInInspector] public EnemyAnimator enemyAnimator = null;

	[SerializeField] private UnityEngine.UI.Image m_playerHappyIcon = null;
	[SerializeField] private UnityEngine.UI.Image m_playerSadIcon = null;
	[SerializeField] private UnityEngine.UI.Image m_playerAngryIcon = null;
	[SerializeField] private UnityEngine.UI.Image m_enemyHappyIcon = null;
	[SerializeField] private UnityEngine.UI.Image m_enemySadIcon = null;
	[SerializeField] private UnityEngine.UI.Image m_enemyAngryIcon = null;
	[SerializeField] private UnityEngine.UI.Text m_playerComboText = null;
	[SerializeField] private UnityEngine.UI.Text m_enemyComboText = null;

	[SerializeField] private UnityEngine.UI.Image[] m_playerHealthIndicator = null;
	[SerializeField] private UnityEngine.UI.Image[] m_enemyHealthIndicator = null;

	[SerializeField] private Pattern[] m_playerCombos = null;

	// state
	private CombatState m_state = CombatState.None;
	private float m_timer = 0f; // general use timer
	private float m_timerB = 0f; // general use timer
	private int m_curpatpos = 0;
	private Emotion m_playerEmote = Emotion.Happy;
	private bool m_canSwitchEmote = false;
	private bool m_switchOnce = false;
	private float m_playerShake = 0f;
	private float m_enemyShake = 0f;
	private List<Emotion> m_currentCombo = new List<Emotion>();
	public bool IsDone { get; private set; } = false;

	// values
	[SerializeField] private float m_playerComboDropDelay;
	[SerializeField] private float m_shakeLength;
	[SerializeField] private float m_normalDelay;
	[SerializeField] private float m_shortPauseLength;

	// should be called at the begining of a battle to confirm if the battle can start and to init some values
	public bool Init(PlayerController player, EnemyController enemy) {
		m_playerHealthIndicator[0].gameObject.SetActive(false);
		m_enemyHealthIndicator[0].gameObject.SetActive(false);
		m_player = player;
		m_enemy = enemy;
		m_eci = m_enemy.GetComponent<EnemyCombatInfo>();
		m_playerEmote = Emotion.Happy;
		// following two checks to make sure the combat can start
		if (!m_eci) {
			Debug.LogError($"Cannot start battle because enemy did not have a component of type {typeof(EnemyCombatInfo).Name}");
			return false;
		} else if (!m_eci.pattern) {
			Debug.LogError($"Cannot start battle because {typeof(EnemyCombatInfo).Name} did not have an attached pattern");
			return false;
		}
		UpdatePlayerHealth();
		UpdateEnemyHealth();
		// success
		return true;
	}

	// takes damage and determines if should switch to game over state
	private void PlayerTakeDamage() {
		m_playerShake = 0.65f;
		m_playerHealth.Value -= 1;
		if (m_playerHealth.Value == 0) {
			Debug.LogError("No death state!");
			SwitchState(CombatState.None);
		}
		UpdatePlayerHealth();
	}

	private bool DoSwitchPlayerEmote() {
		int keypresses = 0;
		if (HappyKeyDown) keypresses++; if (SadKeyDown) keypresses++; if (AngryKeyDown) keypresses++;
		// can only press one
		if (keypresses == 1) {
			if (HappyKeyDown) SetPlayerEmote(Emotion.Happy);
			if (SadKeyDown) SetPlayerEmote(Emotion.Sad);
			if (AngryKeyDown) SetPlayerEmote(Emotion.Angry);
			return true;
		}
		return false;
	}

	// updates the player emote icon
	private void SetPlayerEmote(Emotion emotion) {
		m_playerEmote = emotion;
		m_playerHappyIcon.enabled = false;
		m_playerSadIcon.enabled = false;
		m_playerAngryIcon.enabled = false;
		switch (m_playerEmote) {
			case Emotion.Happy:
				m_playerHappyIcon.enabled = true;
				break;
			case Emotion.Sad:
				m_playerSadIcon.enabled = true;
				break;
			case Emotion.Angry:
				m_playerAngryIcon.enabled = true;
				break;
			default: break;
		}
	}

	// updates the enemy emote icon
	private void SetEnemyEmoteIcon(Emotion emotion) {
		m_enemyHappyIcon.enabled = false;
		m_enemySadIcon.enabled = false;
		m_enemyAngryIcon.enabled = false;
		switch (emotion) {
			case Emotion.None: break;
			case Emotion.Happy:
				m_enemyHappyIcon.enabled = true;
				break;
			case Emotion.Sad:
				m_enemySadIcon.enabled = true;
				break;
			case Emotion.Angry:
				m_enemyAngryIcon.enabled = true;
				break;
			default: Debug.LogError($"Invalid state {emotion}"); break;
		}
	}

	private void UpdateEnemyHealth() {
		m_enemyHealthIndicator[1].enabled = false;
		m_enemyHealthIndicator[2].enabled = false;
		m_enemyHealthIndicator[3].enabled = false;
		switch (m_eci.health) {
			case 3: m_enemyHealthIndicator[3].enabled = true; goto case 2;
			case 2: m_enemyHealthIndicator[2].enabled = true; goto case 1;
			case 1: m_enemyHealthIndicator[1].enabled = true; break;
			case 0: break;
			default: break;
		}
	}

	private void UpdatePlayerHealth() {
		m_playerHealthIndicator[1].enabled = false;
		m_playerHealthIndicator[2].enabled = false;
		m_playerHealthIndicator[3].enabled = false;
		switch (m_playerHealth.Value) {
			case 3: m_playerHealthIndicator[3].enabled = true; goto case 2;
			case 2: m_playerHealthIndicator[2].enabled = true; goto case 1;
			case 1: m_playerHealthIndicator[1].enabled = true; break;
			case 0: break;
			default: break;
		}
	}

	// sets the state and starts combat
	public void BeginCombat() {

		m_playerHealthIndicator[0].gameObject.SetActive(true);
		m_enemyHealthIndicator[0].gameObject.SetActive(true);

		// set the first state
		SwitchState(CombatState.NormalPause);

		// enable player emote icon
		m_canSwitchEmote = true;
		SetPlayerEmote(m_playerEmote);
	}

	public void Step() {
		switch (m_state) {
			case CombatState.None: break;
			case CombatState.NormalPause:
				StateNormalPause(); break;
			case CombatState.EnemyCombo_ShowEmotion:
				StateEnemyCombo_ShowEmotion(); break;
			case CombatState.EnemyCombo_ShortPauseA:
				StateEnemyCombo_ShortPauseA(); break;
			case CombatState.EnemyCombo_Attack:
				StateEnemyCombo_Attack(); break;
			case CombatState.EnemyCombo_ShortPauseB:
				StateEnemyCombo_ShortPauseB(); break;
			case CombatState.PostComboPause:
				StatePostComboPause(); break;
			default: Debug.LogError($"Combat logic invalid state {m_state}"); break;
		}

		// switching emotions and icons
		if (m_canSwitchEmote) {
			bool didSwitch = DoSwitchPlayerEmote();
			if (didSwitch && m_switchOnce) {
				m_switchOnce = m_canSwitchEmote = false;
			}
		}

		// update shake

		if (m_playerShake > 0f) {
			m_playerShake -= Time.deltaTime;
			Vector3 offset = new Vector3(Random.Range(-m_shakeLength, m_shakeLength), Random.Range(-m_shakeLength, m_shakeLength), 0f);
			if (m_playerShake < 0.5f) offset *= m_playerShake / 0.5f;
			m_player.transform.position = offset + playerTargetPos.position;
		} else m_player.transform.position = playerTargetPos.position;

		if (m_enemyShake > 0f) {
			m_enemyShake -= Time.deltaTime;
			Vector3 offset = new Vector3(Random.Range(-m_shakeLength, m_shakeLength), Random.Range(-m_shakeLength, m_shakeLength), 0f);
			if (m_enemyShake < 0.5f) offset *= m_enemyShake / 0.5f;
			m_enemy.transform.position = offset + enemyTargetPos.position;
		} else m_enemy.transform.position = enemyTargetPos.position;
	}

	private void Log(string s) {
		//Debug.Log(s);
	}

	private void SwitchState(CombatState state) {
		m_state = state;
		m_timer = 0f;
		m_timerB = 0f;
		m_currentCombo.Clear();
	}

	private void StateNormalPause() {
		m_timer += Time.deltaTime;
		if (m_timer > m_normalDelay) {
			// enemy not dead start next combo
			if (m_eci.health != 0) {
				SwitchState(CombatState.EnemyCombo_ShowEmotion);
				m_curpatpos = 0;
				m_playerComboText.text = null;
			}
			// enemy dead
			else {
				SwitchState(CombatState.None);
				m_playerComboText.text = null;
				m_enemyComboText.text = null;
				IsDone = true;
				m_playerHealthIndicator[0].gameObject.SetActive(false);
				m_enemyHealthIndicator[0].gameObject.SetActive(false);
			}
		}
	}

	private void StateEnemyCombo_ShowEmotion() {
		// stop player from switching
		m_switchOnce = true;

		// get emotion
		Emotion emotion = m_eci.pattern[m_curpatpos];
		SetEnemyEmoteIcon(emotion);
		Log(emotion.ToString());
		// update enemy combo text
		m_enemyComboText.text += $"{emotion}\n";

		// switch
		SwitchState(CombatState.EnemyCombo_ShortPauseA);
	}

	private void StateEnemyCombo_ShortPauseA() {
		// wait for delay
		m_timer += Time.deltaTime * m_eci.attackSpeed;
		if (m_timer > m_shortPauseLength) {
			SwitchState(CombatState.EnemyCombo_Attack);
			//enemyAnimator.Attack();
		}
	}

	private void StateEnemyCombo_Attack() {
		// allow player to switch again
		m_canSwitchEmote = true;

		// get enemy emotion
		Emotion enemyemote = m_eci.pattern[m_curpatpos];

		// TODO: play enemy attack animation

		// hide icon
		SetEnemyEmoteIcon(Emotion.None);

		// player has advantage
		if (Emote.Advantage(m_playerEmote, enemyemote) > 0) {

			Log("Blocked");
			// move on next emote
			m_curpatpos++;
			if (m_curpatpos < m_eci.pattern.Count)
				SwitchState(CombatState.EnemyCombo_ShortPauseB);
			// move onto the post combo delay
			else {
				SwitchState(CombatState.PostComboPause);
				m_enemyComboText.text = null;
				m_curpatpos = 0;
				m_timerB = -1f;
			}

		}
		// equal or losing has same result
		else {
			// animate
			enemyAnimator.Attack();
			Log("Hurt");
			// return to begining of combo
			SwitchState(CombatState.NormalPause);
			// player takes damage
			PlayerTakeDamage();

			m_enemyComboText.text = null;
		}
	}

	private void StateEnemyCombo_ShortPauseB() {
		// wait for delay (no scaling this time)
		m_timer += Time.deltaTime;
		if (m_timer > m_shortPauseLength) {
			SwitchState(CombatState.EnemyCombo_ShowEmotion);
		}
	}

	private void StatePostComboPause() {
		m_enemyComboText.text = null;
		m_canSwitchEmote = false;

		// wait for delay and immediately restart if the player doesnt combo 
		m_timer += Time.deltaTime;
		if (m_timer > m_eci.delay) {
			m_canSwitchEmote = true;
			SwitchState(CombatState.NormalPause);
			m_playerComboText.text = null;
			return;
		}

		// look at the player combo and decide if failed or successfull

		bool didswitch = DoSwitchPlayerEmote();
		if (m_timerB >= 0f) m_timerB += Time.deltaTime;

		// input an emote, confirm the emote
		if (didswitch) {
			m_timerB = 0f;

			// add to list and confirm if it matches any combos
			m_currentCombo.Add(m_playerEmote);

			// check combo
			bool matchany = false;
			Pattern exactmatch = null;
			foreach (Pattern pattern in m_playerCombos) {
				// early exit
				if (pattern.Count < m_currentCombo.Count) continue;

				// check if this combo matches the enemy we are targeting
				if (pattern.TargetType != m_eci.targetType) continue;

				bool matched = true;

				// check if the current combo matches
				for (int i = 0; i < m_currentCombo.Count; i++) {
					// check if matches pattern
					if (pattern[i] != m_currentCombo[i]) {
						matched = false;
						break;
					}
					if ((i + 1) == pattern.Count) {
						exactmatch = pattern;
					}
				}

				matchany |= matched;

			}

			// update combo text
			if (matchany) m_playerComboText.text += $"<color=green>{m_playerEmote}</color>\n";
			else m_playerComboText.text += $"<color=red>{m_playerEmote}</color>\n";

			// we did a combo
			if (matchany && exactmatch) {
				// hit enemy
				m_eci.health--;
				enemyAnimator.Hurt();
				UpdateEnemyHealth();

				m_enemyShake = 0.65f;
				SwitchState(CombatState.NormalPause);
				m_playerComboText.text = null;
				m_canSwitchEmote = true;

			}
			// we are doing a combo
			else if (matchany) {
				// animate
				enemyAnimator.Hurt();
			}
			// we fucked up the combo
			else {
				PlayerTakeDamage();
				SwitchState(CombatState.NormalPause);
				m_canSwitchEmote = true;
			}

		}
		// took too long, take damage
		else if (m_timerB > m_playerComboDropDelay) {
			m_canSwitchEmote = true;
			SwitchState(CombatState.NormalPause);
			PlayerTakeDamage();
			enemyAnimator.Attack();
		}
	}



}
