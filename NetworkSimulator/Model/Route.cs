namespace NetworkSimulator.Model
{
	/// <summary>
	/// Represents a route in a routing table.
	/// </summary>
	struct Route
	{
		public Address Dest { get; private set; }
		public Address NextHop { get; private set; }
		public uint NumHops { get; private set; }

		public Route(Address dest, Address nextHop, uint numHops) : this()
		{
			Dest = dest;
			NextHop = nextHop;
			NumHops = numHops;
		}

		/// <summary>
		/// Returns true if the route can be used for the address given
		/// 
		/// For evaluation of which routes are better than others, compare
		/// the lengths of the subnet masks, and the number of hops.
		/// </summary>
		public bool Matches(Address addr)
		{
			return (addr.IP & Dest.Mask) == (Dest.IP & Dest.Mask);
		}
	}
}
