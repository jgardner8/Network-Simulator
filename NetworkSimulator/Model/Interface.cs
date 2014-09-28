namespace NetworkSimulator.Model
{
	class Interface : DiscreteDevice
	{
		private Address _addr;
		private NetworkingEquipment _owningEquipment; //used to send packets to the device which contains this interface
		private Interface _connectedTo; //used to send packets across the wire

		public Interface(Address address, NetworkingEquipment owningEquipment)
		{
			_addr = address;
			_owningEquipment = owningEquipment;
		}

		protected override void Run()
		{
			while (!_shutdownRequested)
			{
				var context = Input.Take();
				if (_connectedTo != null)
				{
					if (context.CameFromOwningEquipment())
						_connectedTo.Put(new PacketContext(context.Packet, this));
					else
						_owningEquipment.AddToProcessingQueue(new PacketContext(context.Packet, this));
				}
			}
		}

		public override void Shutdown()
		{
			base.Shutdown();
			Put(new PacketContext()); //gets the thread off blocking call Take()
		}

		public void ConnectTo(Interface connectTo)
		{
			connectTo._connectedTo = this;
			_connectedTo = connectTo;
		}

		public void Put(PacketContext context)
		{
			Input.Put(context);
		}

		public Address Addr { get { return _addr; } }
		public Interface ConnectedTo { get { return _connectedTo; } }
	}
}
