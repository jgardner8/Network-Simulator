using System;

namespace NetworkSimulator.Model
{
	enum EventType 
	{ 
		Receive, //Received a packet that was addressed to me
		Dispose, //Threw a packet away for some reason (TTL expired, no route, broadcasted to me but not addressed to me, etc)
		Forward, //Forwarded a packet in the intended direction in the hope it will find its recipient soon
		Broadcast //Sent a packet in all directions in the absense of better knowledge/technology. Usually followed by many disposals.
	}

	/// <summary>
	/// Handles events occurring within the model, not interactions with the user.
	/// Only really used for events related to receiving or sending packets.
	/// </summary>
	interface IEventHandler
	{
		void Handle(EventType type, string desc, Object sender);
	}
}
