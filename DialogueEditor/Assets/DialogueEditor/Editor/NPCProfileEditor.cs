using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

namespace MikoSramek {

	[CustomEditor(typeof(NPCProfile))]
	public class NPCProfileEditor : Editor {

		public override void OnInspectorGUI() {
			NPCProfile profile = (NPCProfile)target;
			profile.characterName = GUILayout.TextField(profile.characterName);
			if (profile.portraits == null) {
				if (GUILayout.Button("Add Portraits")) {
					profile.portraits = new List<Portrait>();
				}
			}
			else {
				if (profile.portraits.Count > 0) {
					for (int i = 0; i < profile.portraits.Count; i++) {
						GUILayout.BeginHorizontal();
						bool pic = profile.portraits[i].image;
						if (pic) {
							GUILayout.Label(profile.portraits[i].image.texture, GUILayout.Width(128), GUILayout.Height(128));
						}
						GUILayout.BeginVertical();
						if (GUILayout.Button(pic ? "Change Image" : "Add Image", GUILayout.Width(100))) {
							string path = EditorUtility.OpenFilePanel("Select Image", "", "png");
							if (path.Length != 0) {
								path = path.Replace(Application.dataPath, "Assets");
								profile.portraits[i].image = AssetDatabase.LoadAssetAtPath<Sprite>(path);
							}
						}

						profile.portraits[i].mood = (Mood)(EditorGUILayout.EnumPopup(profile.portraits[i].mood, GUILayout.Width(100)));
						if (GUILayout.Button("-", GUILayout.Width(25), GUILayout.Height(25))) {
							profile.portraits.Remove(profile.portraits[i]);
							break;
						}
						GUILayout.EndVertical();

						GUILayout.EndHorizontal();
					}
				}
				if (GUILayout.Button("New Portrait")) {
					profile.portraits.Add(new Portrait());
					EditorUtility.SetDirty((NPCProfile)target);
					profile.Save();
				}
				if (GUILayout.Button("Save Profile")) {
					EditorUtility.SetDirty((NPCProfile)target);
					profile.Save();
				}
			}
		}
	}
}
