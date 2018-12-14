using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace MikoSramek {

	public class DialogueEditor : EditorWindow {
		DialogueHolder currentDialogue;
		TalkingPoint tempTP;
		object selectedGameObject;
		NPCProfile[] availableProfiles;
		float menuBarHeight = 20;

		float tpWidth = 250;
		float tpHeight = 185;
		float heightChangePerResponse = 40;

		bool clickedOnWindow = false;
		int windowID = -1;
		bool linkingTalkingPoints = false;
		TalkingResponse currentlyLinkingResponse;

		[MenuItem("Dialogue/Open Editor")]
		public static void CreateWindow() {
			DialogueEditor window = CreateInstance<DialogueEditor>();
			window.Show();
		}

		void RefreshProfiles() {
			string[] guids = AssetDatabase.FindAssets("_Profile");
			availableProfiles = new NPCProfile[guids.Length];
			for (int i = 0; i < guids.Length; i++) {
				string path = AssetDatabase.GUIDToAssetPath(guids[i]);
				availableProfiles[i] = AssetDatabase.LoadAssetAtPath<NPCProfile>(path);
			}
		}

		void RefreshSelectedDialogue() {
			if (Selection.activeObject != null) {
				selectedGameObject = Selection.activeObject;
				if (selectedGameObject is DialogueHolder) {
					currentDialogue = (DialogueHolder)selectedGameObject;
				}
				else {
					currentDialogue = null;
				}
			}
		}

		Rect MenuBar {
			get { return new Rect(0, 0, position.width, menuBarHeight); }
		}

		Rect DialogueBoxes {
			get { return new Rect(0, menuBarHeight, position.width, position.height); }
		}

		void OnGUI() {
			var e = Event.current;

			if (e.button == 0) { //LEFT
				if (e.type == EventType.MouseUp) {
					//Debug.Log("Left Click @ " + e.mousePosition.ToString());
					LeftClick(e);
				}
			}
			else if (e.button == 1) { //RIGHT
				if (e.type == EventType.MouseUp) {
					//Debug.Log("Right Click " + e.mousePosition.ToString());
					RightClick(e);
				}
			}
			TopBar();
			//Debug();
			DrawLinks();
			Nodes();

			if (currentDialogue == null || (UnityEngine.Object)selectedGameObject != Selection.activeObject) {
				RefreshSelectedDialogue();
				Repaint();
			}
		}

		void TopBar() {
			GUILayout.BeginHorizontal(EditorStyles.toolbar);

			GUILayout.Label(currentDialogue == null ? "Null" : currentDialogue.name, EditorStyles.miniLabel);
			if (GUILayout.Button("Scan", EditorStyles.toolbarButton)) {
				RefreshSelectedDialogue();
			}
			if (GUILayout.Button("Save", EditorStyles.toolbarButton) && currentDialogue != null) {
				EditorUtility.SetDirty(currentDialogue);
			}
			if (GUILayout.Button("Find Profiles", EditorStyles.toolbarButton)) {
				RefreshProfiles();
			}
			GUILayout.EndHorizontal();
		}

		void Debug() {
			GUILayout.Label(linkingTalkingPoints.ToString());
			GUILayout.Label(windowID.ToString());
		}

		void Nodes() {
			GUILayout.BeginArea(DialogueBoxes);
			if (currentDialogue != null && currentDialogue.talkingPoints.Count > 0) {
				BeginWindows();
				for (int i = 0; i < currentDialogue.talkingPoints.Count; i++) {
					TalkingPoint tp = currentDialogue.talkingPoints[i];

					Rect rect = new Rect(tp.x, tp.y, tp.w, tp.h);

					rect = GUI.Window(i, rect, DisplayTalkingPoint, "");
					tp.x = rect.x;
					tp.y = rect.y;
				}
				EndWindows();
			}
			GUILayout.EndArea();
		}

		void DisplayTalkingPoint(int id) {
			if (id >= currentDialogue.talkingPoints.Count) {
				return;
			}
			TalkingPoint tp = currentDialogue.talkingPoints[id];

			GUILayout.BeginHorizontal();

			//NPC name
			GUILayout.Label(tp.npc == null ? "Select Profile" : tp.npc.characterName /*+ "(" + tp.m_ID.ToString() + ")"*/, EditorStyles.boldLabel);

			//Starting point toggle
			bool sp = tp.startingPoint;
			tp.startingPoint = GUILayout.Toggle(sp, "Starting Point");
			if (sp != tp.startingPoint) {
				ChangeStartingPoint(tp);
			}

			//Profile Change Dropdown
			if (GUILayout.Button("V", GUILayout.Width(25))) {
				GenericMenu profiles = new GenericMenu();
				if (availableProfiles == null) {
					RefreshProfiles();
				}
				foreach (NPCProfile p in availableProfiles) {
					profiles.AddItem(new GUIContent(p.name.ToString()), false, AssignProfile, p);
				}
				tempTP = tp;
				profiles.ShowAsContext();
			}

			GUILayout.EndHorizontal();

			//GUILayout.Space(7);
			GUILayout.BeginHorizontal();

			//Portrais and text
			if (tp.npc != null && tp.npc.portraits != null && tp.npc.portraits[tp.currentPortrait].image != null) {
				GUILayout.Label(tp.npc.portraits[tp.currentPortrait].image.texture, GUILayout.Width(64), GUILayout.Height(64));
				tp.text = GUILayout.TextArea(tp.text, GUILayout.Width(250 - 64 - 14), GUILayout.Height(64));
			}
			else {
				GUILayout.Label("NPC Profiles are broken again...");
			}
			GUILayout.EndHorizontal();

			//Portrait Changer
			if (tp.npc != null) {
				if (GUILayout.Button("V", GUILayout.Width(25))) {
					GenericMenu portraits = new GenericMenu();
					for (int i = 0; i < tp.npc.portraits.Count; i++) {
						if (tp.currentPortrait == i) {
							portraits.AddDisabledItem(new GUIContent(i.ToString() + ". " + tp.npc.portraits[i].mood.ToString()));
						}
						else {
							portraits.AddItem(new GUIContent(i.ToString() + ". " + tp.npc.portraits[i].mood.ToString()), false, ChangePortrait, i);
						}
					}
					tempTP = tp;
					portraits.ShowAsContext();
				}
			}
			//Events
			/*
			GUILayout.BeginHorizontal();
			GUILayout.Button("+", GUILayout.Width(25));
			GUILayout.Label("Event");
			GUILayout.FlexibleSpace();
			GUILayout.Button("-", GUILayout.Width(25));
			GUILayout.EndHorizontal();
			*/
			//Responses
			GUILayout.BeginHorizontal();
			if (GUILayout.Button("+", GUILayout.Width(25))) {
				if (tp.responses == null) {
					tp.responses = new List<TalkingResponse>();
				}
				tp.responses.Add(new TalkingResponse());
				tp.h += heightChangePerResponse;
			}
			GUILayout.BeginVertical();
			if (tp.responses != null) {
				foreach (var link in tp.responses) {
					GUILayout.BeginHorizontal();
					//text
					link.text = GUILayout.TextField(link.text, GUILayout.Width(250 - 64 - 14));
					GUILayout.FlexibleSpace();
					GUILayout.BeginVertical();
					if (GUILayout.Button("-", GUILayout.Width(25))) {
						tp.responses.Remove(link);
						tp.h -= heightChangePerResponse;
						break;
					}
					if (GUILayout.Button("L", GUILayout.Width(25))) {
						linkingTalkingPoints = true;
						currentlyLinkingResponse = link;
					}
					GUILayout.EndVertical();
					GUILayout.EndHorizontal();
					//GUILayout.Label(link.responseID.ToString());
				}
			}
			GUILayout.EndVertical();
			GUILayout.EndHorizontal();
			GUI.DragWindow();
		}

		void AssignProfile(object p) {
			NPCProfile a = (NPCProfile)p;
			if (tempTP != null) {
				tempTP.npc = a;
				tempTP = null;
			}
		}

		void ChangePortrait(object p) {
			if (tempTP != null) {
				tempTP.currentPortrait = (int)p;
				tempTP = null;
			}
		}

		void DrawLinks() {
			if (currentDialogue == null || currentDialogue.talkingPoints == null) {
				return;
			}
			Vector3 end = Vector3.zero;
			for (int a = 0; a < currentDialogue.talkingPoints.Count; a++) {
				TalkingPoint tp = currentDialogue.talkingPoints[a];
				if (tp.responses == null) {
					return;
				}
				for (int b = 0; b < tp.responses.Count; b++) {
					Vector3 start = new Vector3(tp.x + tp.w, tp.y + 185 + 40 * b);
					TalkingPoint link = currentDialogue.FindViaID(tp.responses[b].responseID);
					if (link != null) {
						end = new Vector3(link.x + link.w / 2, link.y + link.h / 2);
						Handles.DrawLine(start, end);
					}
				}
			}
		}

		void LeftClick(Event e) {
			clickedOnWindow = false;
			windowID = CheckForWindowClick(e.mousePosition);
			if (linkingTalkingPoints && clickedOnWindow && windowID > -1) {
				//currentlyLinkingResponse.link = currentDialogue.talkingPoints[windowID];
				currentlyLinkingResponse.responseID = currentDialogue.talkingPoints[windowID].m_ID;
			}
			currentlyLinkingResponse = null;
			linkingTalkingPoints = false;
		}

		void RightClick(Event e) {
			clickedOnWindow = false;
			currentlyLinkingResponse = null;
			linkingTalkingPoints = false;
			windowID = CheckForWindowClick(e.mousePosition);
			GenericMenu newMenu = new GenericMenu();
			if (clickedOnWindow) {
				newMenu.AddItem(new GUIContent("Delete Talking Point"), false, DeleteTalkingPoint);
				newMenu.AddItem(new GUIContent("Reset Height"), false, ResetWindowHeight);
			}
			else {
				newMenu.AddItem(new GUIContent("New Talking Point"), false, AddTalkingPoint);
			}
			newMenu.ShowAsContext();
		}

		void AddTalkingPoint() {
			if (currentDialogue != null) {
				currentDialogue.AddTalkingPoint(0, 0, tpWidth, tpHeight);
			}
		}

		void DeleteTalkingPoint() {
			if (currentDialogue != null && windowID > -1) {
				currentDialogue.talkingPoints.Remove(currentDialogue.talkingPoints[windowID]);
				windowID = -1;
			}
		}

		void ChangeStartingPoint(TalkingPoint tp) {
			foreach (TalkingPoint t in currentDialogue.talkingPoints) {
				if (t != tp) {
					t.startingPoint = false;
				}
			}
		}

		void ResetWindowHeight() {
			if (currentDialogue != null && windowID > -1) {
				currentDialogue.talkingPoints[windowID].h = tpHeight;
				currentDialogue.talkingPoints[windowID].w = tpWidth;
				windowID = -1;
			}
		}

		int CheckForWindowClick(Vector2 pos) {
			if (currentDialogue != null && currentDialogue.talkingPoints.Count > 0) {
				for (int i = 0; i < currentDialogue.talkingPoints.Count; i++) {
					TalkingPoint tp = currentDialogue.talkingPoints[i];
					Rect rect = new Rect(tp.x, tp.y, tp.w, tp.h);
					if (rect.Contains(pos)) {
						clickedOnWindow = true;
						return i;
					}
				}
			}
			return -1;
		}
	}
}
