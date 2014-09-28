namespace NetworkSimulator.Model
{
	class Hub : NetworkingEquipment
	{
		public Hub(IEventHandler eventHandler, uint numInterfaces) : base(eventHandler)
		{
			//Hub interfaces don't need addresses
			for (int i = 0; i < numInterfaces; i++)
				Interfaces.Add(new Interface(new Address(0, 0), this));
		}

		protected override void Run()
		{
			while (!_shutdownRequested)
			{
				var context = Input.Take();
				_eventHandler.Handle(EventType.Broadcast, "\"" + context.Packet.Message + "\" passed through Hub", this);
				SendTo(context.Packet.To, context.Packet, context.Packet.From.IP);
			}
		}

		protected virtual void SendTo(Address addr, Packet packet, uint receivedFromIP)
		{
			Broadcast(packet, receivedFromIP); //for better or worse, this is how a hub works
		}
	}
}