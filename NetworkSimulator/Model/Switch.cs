using System;
using System.Collections.Generic;

namespace NetworkSimulator.Model
{
	class Switch : Hub
	{
		//a switch stores the address of devices that are connected to more efficiently forward packets
		private List<Tuple<Interface, Address>> _connectedAddresses = new List<Tuple<Interface, Address>>();

		//Switch interfaces don't need addresses
		public Switch(IEventHandler eventHandler, uint numInterfaces) : base(eventHandler, numInterfaces) 
		{ }

		protected override void Run()
		{
			while (!_shutdownRequested)
			{
				var context = Input.Take();
				//when a client sends something, they're added to _connectedAddresses so future packets
				//can go straight to them. This is how a switch works. 
				if (!_connectedAddresses.Exists(t => t.Item1 == context.Sender && t.Item2 == context.Packet.From))
					_connectedAddresses.Add(Tuple.Create(context.Sender, context.Packet.From));
				SendTo(context.Packet.To, context.Packet, context.Packet.From.IP);
			}
		}

		protected override void SendTo(Address addr, Packet packet, uint receivedFromIP)
		{
			//If client isn't in _connectedAddresses, then the switch cannot know
			//where to send it. It will instead broadcast the packet. 
			var sendTo = _connectedAddresses.Find(context => context.Item2 == addr);
			if (sendTo != null) //known address (client has sent something on this network before)
			{
				_eventHandler.Handle(EventType.Forward, "Packet is addressed to known client: forwarding", this);
				SendOutOf(sendTo.Item1, packet);
			}
			else			    //don't know which interface to send to, broadcast it
			{
				_eventHandler.Handle(EventType.Broadcast, "Don't know which port they're on: broadcasting.", this);
				Broadcast(packet, receivedFromIP);
			}
		}
	}
}
