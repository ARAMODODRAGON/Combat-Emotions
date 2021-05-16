using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetStatsOnLoad : MonoBehaviour {

	[SerializeField] private int m_scoreValue;
	[SerializeField] private IntValue m_score;
	[SerializeField] private int m_healthValue;
	[SerializeField] private IntValue m_health;
	[SerializeField] private int m_levelValue;
	[SerializeField] private IntValue m_level;

	private void Awake() {
		if (m_score) m_score.Value = m_scoreValue;
		if (m_health) m_health.Value = m_healthValue;
		if (m_level) m_level.Value = m_levelValue;
	}
}
