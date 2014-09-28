using System;
using System.Collections.Generic;
using System.Linq;

namespace NetworkSimulator.Model
{
	class Router : NetworkingEquipment
	{
		private List<Route> _routingTable = new List<Route>();
		private Action<PacketContext> _doWithInput;

		public Router(IEventHandler eventHandler) : base(eventHandler)
		{ }

		/// <summary>
		/// Returns false if packet doesn't require routing because it is coming from the network
		/// it was addressed to (and therefore came here by mistake/broadcast).
		/// </summary>
		private bool IsToOutsideNetwork(PacketContext context)
		{
			return (context.Packet.To.IP & context.Sender.Addr.Mask) 
				!= (context.Sender.Addr.IP & context.Sender.Addr.Mask);
		}

		protected override void Run()
		{
			while (!_shutdownRequested)
			{
				var context = Input.Take();
				if (context.Sender == null) //can happen when calling shutdown as things get garbage collected
					continue;

				if (Interfaces.Exists(i => i.Addr == context.Packet.To))
					_eventHandler.Handle(EventType.Receive, context.Packet.From.FormattedIPMask + " -> " + 
						context.Packet.To.FormattedIPMask + ": " + context.Packet.Message, this);
				else if (!IsToOutsideNetwork(context))
					_eventHandler.Handle(EventType.Dispose, "Packet came from network it was addressed to: throwing away.", this);
				else
				{
					context.Packet.DecrementTTL();
					if (context.Packet.TTL == 0)
					{
						_eventHandler.Handle(EventType.Dispose, "Packet \"" + context.Packet.Message + "\" has expired.", this);
						continue;
					}
					SendTo(context.Packet.To, context.Packet);
				}
			}
		}

		/// <summary>
		/// Recursive function used to find interface to forward packet out of to get
		/// to specified address.
		/// </summary>
		public void SendTo(Address addr, Packet packet)
		{
			//Check if next-hop is directly connected
			var sendTo = Interfaces.Find(i => (i.Addr.IP & i.Addr.Mask) == (addr.IP & i.Addr.Mask));
			if (sendTo != null)
			{
				_eventHandler.Handle(EventType.Forward, "Network is directly connected, sending to host.", this);
				sendTo.Put(new PacketContext(packet));
				return;
			}
			
			//Next-hop is not directly connected, search for a route to it
			var matchingRoutes = _routingTable.Where(route => route.Matches(addr)).ToArray();
			if (matchingRoutes.Count() == 0)
			{
				_eventHandler.Handle(EventType.Dispose, "No route to network, throwing packet away.", this);
				return;
			}
			_eventHandler.Handle(EventType.Forward, "Found a route to next hop: forwarding.", this);

			//Routers with more specific (larger) subnet masks are considered superior
			uint maxSubnetMask = matchingRoutes.Max(route => route.Dest.Mask);
			var routesWithMaxMask = matchingRoutes.Where(route => route.Dest.Mask == maxSubnetMask);
			//Now sort by hops, to get the route with the least hops
			var orderedRoutes = routesWithMaxMask.OrderBy(route => route.NumHops);
			//And finally forward packet to the one with the least hops
			SendTo((orderedRoutes.ToArray())[0].NextHop, packet);	
		}

		public void AddRoute(Route r) { _routingTable.Add(r); }
		public void DeleteRoute(Route r) { _routingTable.Remove(r); }
		public void DeleteRouteAt(int idx) { _routingTable.RemoveAt(idx); }
	}
}
