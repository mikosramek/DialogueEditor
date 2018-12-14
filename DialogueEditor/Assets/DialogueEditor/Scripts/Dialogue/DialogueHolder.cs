using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MikoSramek {

	public class DialogueHolder : ScriptableObject {

		[SerializeField]
		public List<TalkingPoint> talkingPoints;

		[HideInInspector]
		[SerializeField]
		public List<int> usedIDS;

		public void AddTalkingPoint(float x, float y, float w, float h) {
			TalkingPoint tp = new TalkingPoint(x, y, h, w);
			tp.SetID(GetNewID());
			talkingPoints.Add(tp);
		}

		int GetNewID() {
			int a = Random.Range(0, 100);
			while (usedIDS.Contains(a)) {
				a = Random.Range(0, 100);
			}
			if (usedIDS == null) {
				usedIDS = new List<int>();
			}
			usedIDS.Add(a);
			return a;
		}

		public TalkingPoint FindFirstTalkingPoint() {
			foreach (TalkingPoint t in talkingPoints) {
				if (t.startingPoint) {
					return t;
				}
			}
			Debug.LogWarning("No starting point set on " + this.name + ".");
			return null;
		}

		public TalkingPoint FindViaID(int id) {
			foreach (TalkingPoint t in talkingPoints) {
				if (t.m_ID == id) {
					return t;
				}
			}
			Debug.LogWarning("No talking point of that ID exists.");
			return null;
		}
	}
}
