using System.Collections;
using System.Collections.Generic;

namespace MikoSramek {

	[System.Serializable]
	public class TalkingPoint {
		public string text;
		public NPCProfile npc;
		public int currentPortrait;
		public List<TalkingResponse> responses;
		public List<TalkingEvent> events;
		public bool startingPoint;
		public float x, y;
		public float h, w;

		public int m_ID;

		public TalkingPoint() {
			responses = new List<TalkingResponse>();
			events = new List<TalkingEvent>();
		}

		public TalkingPoint(float x, float y, float h, float w) {
			this.x = x;
			this.y = y;
			this.h = h;
			this.w = w;
			startingPoint = false;
			responses = new List<TalkingResponse>();
			events = new List<TalkingEvent>();
		}

		public void SetID(int newID) {
			m_ID = newID;
		}
	}

	[System.Serializable]
	public class TalkingResponse {
		public string text;
		public int responseID;

		public TalkingResponse() {
			responseID = -1;
		}
	}

	[System.Serializable]
	public class TalkingEvent {
		public string EventType = "Implement This";
	}
}
