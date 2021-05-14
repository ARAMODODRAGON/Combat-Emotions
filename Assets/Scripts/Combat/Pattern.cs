using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Pattern", menuName = "Scriptable Objects/Pattern")]
public class Pattern : ScriptableObject {

	// access the number of emotions in this pattern
	public int Count => m_emotionPattern.Count;

	// access the emotion at that index
	public Emotion this[int index] => m_emotionPattern[index];

	// private list of emotions for this pattern
	[SerializeField] private List<Emotion> m_emotionPattern;
}
