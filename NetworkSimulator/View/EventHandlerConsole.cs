using System;
using NetworkSimulator.Model;

namespace NetworkSimulator.View
{
	class EventHandlerConsole : IEventHandler
	{
		public void Handle(EventType type, string desc, Object sender)
		{
			string prefix;
			if      (sender is Hub)    prefix = "\t"; //both switches and hubs are hubs, both have one tab prefix
			else if (sender is Router) prefix = "\t\t";
			else                       prefix = "";

			Console.WriteLine(prefix + desc);
		}
	}
}
