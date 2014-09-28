namespace NetworkSimulator.Model
{
	struct Packet
	{
		public string Message { get; private set; }
		public Address To { get; private set; }
		public Address From { get; private set; }
		public uint TTL { get; private set; }

		public Packet(string message, Address to, Address from) : this()
		{
			Message = message;
			To = to;
			From = from;
			TTL = 10; //max hops before routing loop is assumed and packet is thrown away
		}

		public void DecrementTTL()
		{
			TTL--;
		}
	}
}
