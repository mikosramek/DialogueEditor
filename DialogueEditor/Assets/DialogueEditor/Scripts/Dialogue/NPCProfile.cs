using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MikoSramek {

	[System.Serializable]
	public class NPCProfile : ScriptableObject {
		public string characterName;
		public List<Portrait> portraits;
		public int currentPortrait;

		public void Save() {
#if UNITY_EDITOR
			UnityEditor.EditorUtility.SetDirty(this);
#endif
		}
	}

	[System.Serializable]
	public class Portrait {
		public Mood mood;

		[SerializeField]
		public Sprite image;
	}

	[System.Serializable]
	public enum Mood {
		idle,
		happy,
		scared,
		angry,
		sad
	}
}
