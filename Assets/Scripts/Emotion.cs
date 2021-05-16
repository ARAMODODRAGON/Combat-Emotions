using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Emotion : byte {
	None = 0,
	Happy,
	Sad,
	Angry
}

public static class Emote {
	// checks if 'a' has the advantage over 'b'
	// returns  1 if a > b
	// returns  0 if a == b or if a or b is invalid
	// returns -1 if a < b
	public static int Advantage(Emotion a, Emotion b) {
		if (a == b) return 0;
		switch (a) {
			case Emotion.Happy:
				if (b == Emotion.Angry) return 1;
				else if (b == Emotion.Sad) return -1;
				break;
			case Emotion.Sad:
				if (b == Emotion.Happy) return 1;
				else if (b == Emotion.Angry) return -1;
				break;
			case Emotion.Angry:
				if (b == Emotion.Sad) return 1;
				else if (b == Emotion.Happy) return -1;
				break;
			default: break;
		}
		return 0;
	}
}