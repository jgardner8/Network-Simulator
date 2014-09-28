namespace NetworkSimulator.Model
{
	struct PacketContext
	{
		public Packet Packet { get; private set; }
		public Interface Sender { get; private set; }

		/// <summary>
		/// Groups together a packet and the interface it came from.
		/// Useful for NetworkingEquipment to know which of its interfaces a packet came from,
		/// and for interfaces to know whether a packet came from its NetworkingEquipment or the wire.
		/// 
		/// Do not provide a sender if sending from owning equipment to owned interface. 
		/// The equipment itself does not have an address. Besides being semantically incorrect, 
		/// the packet will just be sent right back because the interface will think it came from the wire.
		/// </summary>
		public PacketContext(Packet packet, Interface sender=null) : this()
		{
			Packet = packet;
			Sender = sender;
		}

		//A sender of null denotes that the packet came from the device itself. (owning equipment > interface)
		public bool CameFromOwningEquipment() { return Sender == null; }
		public bool CameFromInterface() { return !CameFromOwningEquipment(); }
	}
}
