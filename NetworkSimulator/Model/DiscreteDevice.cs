using ConcurrencyConstructs;
using System.Threading;

namespace NetworkSimulator.Model
{
	/// <summary>
	/// A device that runs concurrently to all other devices
	/// Manages its own thread. 
	/// All networking equipment and interfaces are built on this.
	/// </summary>
	abstract class DiscreteDevice
	{
		private Thread _thread;
		private Channel<PacketContext> _input = new Channel<PacketContext>();
		protected bool _shutdownRequested = false;

		public DiscreteDevice()
		{
			_thread = new Thread(Run); 
		}

		//What the thread runs when started.
		//Used to run an infinite loop, taking data from a channel
		//and processing it.
		protected abstract void Run();

		//Not possible to start up again after shutdown
		//(shutdown is already hacky enough, don't have 
		//enough confidence in it to allow restart)
		public virtual void Start()
		{
			if (!_shutdownRequested)
				_thread.Start();
		}

		public virtual void Shutdown()
		{
			_shutdownRequested = true;
		}

		protected Channel<PacketContext> Input { get { return _input; } }
	}
}
