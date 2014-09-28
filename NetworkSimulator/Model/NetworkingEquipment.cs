using System.Collections.Generic;

namespace NetworkSimulator.Model
{
	abstract class NetworkingEquipment : DiscreteDevice
	{
		private List<Interface> _interfaces = new List<Interface>();
		protected IEventHandler _eventHandler;

		protected NetworkingEquipment(IEventHandler eventHandler)
		{
			_eventHandler = eventHandler;
		}

		public void SendOutOf(Interface sendOutOf, Packet packet)
		{
			if (_interfaces.Contains(sendOutOf)) //make sure device owns this interface first...
				sendOutOf.Put(new PacketContext(packet, null));
		}

		public void SendOutOf(int interfaceIdx, Packet packet)
		{
			SendOutOf(Interfaces[interfaceIdx], packet);
		}

		/// <summary>
		/// Method will sent out of every interface except the one connected to exceptForInterfaceWithIP.
		/// Often used to avoid broadcasting back to the device you received a packet from.
		/// 
		/// Note that this method does not transform a packet into a broadcast packet
		/// It just forwards it out all interfaces. There is currently no support for broadcast packets.
		/// </summary>
		public void Broadcast(Packet packet, uint exceptForInterfaceWithIP=0)
		{
			foreach (Interface i in _interfaces)
				if (exceptForInterfaceWithIP == 0 || exceptForInterfaceWithIP != i.ConnectedTo.Addr.IP)
					SendOutOf(i, packet);				
		}

		public override void Start()
		{
			if (!_shutdownRequested)
			{
				base.Start();
				foreach (Interface i in _interfaces)
					i.Start();
			}
		}

		public override void Shutdown()
		{
			base.Shutdown();
			foreach (var i in _interfaces)
				i.Shutdown();
			Input.Put(new PacketContext()); //gets the thread off blocking call Take()
		}

		//Used by the interfaces this NetworkingEquipment 
		//owns so they can forward packets to this device.
		public void AddToProcessingQueue(PacketContext context)
		{
			Input.Put(context);
		}

		public Interface this[int idx] { get { return _interfaces[idx]; } }
		public List<Interface> Interfaces { get { return _interfaces; } }
	}
}
