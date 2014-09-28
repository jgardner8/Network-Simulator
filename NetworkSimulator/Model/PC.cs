namespace NetworkSimulator.Model
{
	class PC : NetworkingEquipment
	{
		public PC(IEventHandler eventHandler) : base(eventHandler)
		{ }

		protected override void Run()
		{
			while (!_shutdownRequested)
			{
				var context = Input.Take();
				if (Interfaces.Exists(i => i.Addr == context.Packet.To)) //if for me
					_eventHandler.Handle(EventType.Receive, context.Packet.From.FormattedIPMask + " -> " + 
						context.Packet.To.FormattedIPMask + ": " + context.Packet.Message, this);
				else //not for me, throw away
					_eventHandler.Handle(EventType.Dispose, "Packet is not for me, throwing away.", this);
			}
		}
	}
}
