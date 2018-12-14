using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace MikoSramek {

	public class ScriptableObjectEditor : Editor {

		[MenuItem("Dialogue/Create Dialogue Holder")]
		public static void CreateDialogue() {
			DialogueHolder asset = ScriptableObject.CreateInstance<DialogueHolder>();

			AssetDatabase.CreateAsset(asset, "Assets/NewDialogue.asset");
			AssetDatabase.SaveAssets();
		}

		[MenuItem("Dialogue/Create NPC Profile")]
		public static void CreateProfile() {
			NPCProfile asset = ScriptableObject.CreateInstance<NPCProfile>();

			AssetDatabase.CreateAsset(asset, "Assets/NewProfile.asset");
			AssetDatabase.SaveAssets();
		}

		/*
		[MenuItem("Flags/Create Flag List")]
		public static void CreateFlags() {
			EventSystem_GlobalFlags asset = ScriptableObject.CreateInstance<EventSystem_GlobalFlags>();

			AssetDatabase.CreateAsset(asset, "Assets/NewFlagList.asset");
			AssetDatabase.SaveAssets();
		}
		*/
	}
}
