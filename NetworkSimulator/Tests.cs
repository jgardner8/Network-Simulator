using System;
using System.Threading;
using NetworkSimulator.Model;
using NetworkSimulator.View;

namespace NetworkSimulator
{
	static class Tests
	{
		private static IEventHandler _eventHandler = new EventHandlerConsole();

		public static void DirectPcTest()
		{
			PC pc1 = new PC(_eventHandler);
			PC pc2 = new PC(_eventHandler);
			pc1.Interfaces.Add(new Interface(
				new Address("192.168.1.1", "/24"),
				pc1));
			pc2.Interfaces.Add(new Interface(
				new Address("192.168.1.2", "/24"),
				pc2));
			pc2[0].ConnectTo(pc1[0]);

			pc1.Start();
			pc2.Start();

			pc1.SendOutOf(0,
				new Packet("Hello World",
					new Address("192.168.1.2", "255.255.255.0"),
					pc1[0].Addr
				)
			);

			Console.ReadLine();

			pc1.Shutdown();
			pc2.Shutdown();
		}

		public static void HubTest()
		{
			PC pc1 = new PC(_eventHandler);
			PC pc2 = new PC(_eventHandler);
			PC pc3 = new PC(_eventHandler);
			pc1.Interfaces.Add(new Interface(
				new Address("192.168.1.1", "/24"),
				pc1));
			pc2.Interfaces.Add(new Interface(
				new Address("192.168.1.2", "/24"),
				pc2));
			pc3.Interfaces.Add(new Interface(
				new Address("192.168.1.3", "255.255.255.0"),
				pc3));

			Hub h1 = new Hub(_eventHandler, 3);

			h1[0].ConnectTo(pc1[0]);
			h1[1].ConnectTo(pc2[0]);
			h1[2].ConnectTo(pc3[0]);

			pc1.Start();
			pc2.Start();
			pc3.Start();
			h1.Start();

			pc1.SendOutOf(0,
				new Packet("Hello World",
					new Address("192.168.1.2", "255.255.255.0"),
					pc1[0].Addr
				)
			);

			Thread.Sleep(1500);

			pc3.SendOutOf(0,
				new Packet("Hi PC1!",
					new Address("192.168.1.1", "255.255.255.0"),
					pc3[0].Addr)
			);

			Console.ReadLine();

			pc1.Shutdown();
			pc2.Shutdown();
			pc3.Shutdown();
			Thread.Sleep(1000);
			h1.Shutdown();
		}

		public static void SwitchTest()
		{
			PC pc1 = new PC(_eventHandler);
			PC pc2 = new PC(_eventHandler);
			PC pc3 = new PC(_eventHandler);
			pc1.Interfaces.Add(new Interface(
				new Address("192.168.1.1", "/24"),
				pc1));
			pc2.Interfaces.Add(new Interface(
				new Address("192.168.1.2", "/24"),
				pc2));
			pc3.Interfaces.Add(new Interface(
				new Address("192.168.1.3", "255.255.255.0"),
				pc3));

			Switch s1 = new Switch(_eventHandler, 3);

			s1[0].ConnectTo(pc1[0]);
			s1[1].ConnectTo(pc2[0]);
			s1[2].ConnectTo(pc3[0]);

			pc1.Start();
			pc2.Start();
			pc3.Start();
			s1.Start();

			pc1.SendOutOf(0,
				new Packet("Hello World",
					new Address("192.168.1.2", "255.255.255.0"),
					pc1[0].Addr
				)
			);

			Thread.Sleep(1500);

			pc3.SendOutOf(0,
				new Packet("Hi PC1!",
					new Address("192.168.1.1", "255.255.255.0"),
					pc3[0].Addr)
			);

			Console.ReadLine();

			pc1.Shutdown();
			pc2.Shutdown();
			pc3.Shutdown();
			Thread.Sleep(1000);
			s1.Shutdown();
		}

		public static void BasicRouterTest()
		{
			PC pc1 = new PC(_eventHandler);
			pc1.Interfaces.Add(new Interface(
				new Address("192.168.1.2", "/24"),
				pc1));

			Router r1 = new Router(_eventHandler);
			r1.Interfaces.Add(new Interface(
				new Address("192.168.1.1", "/24"),
				r1));
			r1.Interfaces.Add(new Interface(
				new Address("10.1.1.1", "/24"),
				r1));

			Router r2 = new Router(_eventHandler);
			r2.Interfaces.Add(new Interface(
				new Address("10.1.1.2", "255.255.255.0"),
				r2));
			r2.Interfaces.Add(new Interface(
				new Address("110.175.38.220", "/24"),
				r2));

			PC pc2 = new PC(_eventHandler);
			pc2.Interfaces.Add(new Interface(
				new Address("110.175.38.221", "/24"),
				pc2));

			r1[0].ConnectTo(pc1[0]);
			r1[1].ConnectTo(r2[0]);
			r2[1].ConnectTo(pc2[0]);

			r1.AddRoute(new Route(new Address("0.0.0.0", "0.0.0.0"),
								  new Address("10.1.1.2", "/24"), 1));

			r2.AddRoute(new Route(new Address("192.168.1.2", "/24"),
								  new Address("10.1.1.1", "/24"), 1));

			pc1.Start();
			pc2.Start();
			r1.Start();
			r2.Start();

			pc2.SendOutOf(0,
				new Packet("Hello World",
					new Address("192.168.1.2", "255.255.255.0"),
					pc2[0].Addr)
			);

			Console.ReadLine();

			pc1.SendOutOf(0,
				new Packet("Hi PC2!",
				new Address("110.175.38.221", "255.255.255.0"),
				pc1[0].Addr)
			);

			Console.ReadLine();

			pc1.Shutdown();
			pc2.Shutdown();
			Thread.Sleep(1000);
			r1.Shutdown();
			r2.Shutdown();
		}

		public static void ComplexNetworkTest()
		{
			//Network 1
			Router r1 = new Router(_eventHandler);
			r1.Interfaces.Add(new Interface(
				new Address("192.168.1.1", "/24"), //to s1 LAN
				r1));
			r1.Interfaces.Add(new Interface(
				new Address("10.1.1.1", "/30"), //to r2 10.1.1.2
				r1));
			r1.Interfaces.Add(new Interface(
				new Address("10.1.1.5", "/30"), //to r3 10.1.1.6
				r1));

			PC pc11 = new PC(_eventHandler);
			PC pc12 = new PC(_eventHandler);
			PC pc13 = new PC(_eventHandler);
			pc11.Interfaces.Add(new Interface(
				new Address("192.168.1.2", "/24"),
				pc11));
			pc12.Interfaces.Add(new Interface(
				new Address("192.168.1.3", "/24"),
				pc12));
			pc13.Interfaces.Add(new Interface(
				new Address("192.168.1.4", "/24"),
				pc13));

			Switch s1 = new Switch(_eventHandler, 4);
			s1[0].ConnectTo(pc11[0]);
			s1[1].ConnectTo(pc12[0]);
			s1[2].ConnectTo(pc13[0]);
			s1[3].ConnectTo(r1[0]);


			//Network 2
			Router r2 = new Router(_eventHandler);
			r2.Interfaces.Add(new Interface(
				new Address("220.0.0.1", "/24"), //to s2 LAN
				r2));
			r2.Interfaces.Add(new Interface(
				new Address("10.1.1.2", "/30"), //to r1 10.1.1.1
				r2));
			r2.Interfaces.Add(new Interface(
				new Address("140.1.0.1", "/16"), //to r3 140.1.0.2
				r2));

			PC pc21 = new PC(_eventHandler);
			PC pc22 = new PC(_eventHandler);
			PC pc23 = new PC(_eventHandler);
			pc21.Interfaces.Add(new Interface(
				new Address("220.0.0.2", "/24"),
				pc21));
			pc22.Interfaces.Add(new Interface(
				new Address("220.0.0.3", "/24"),
				pc22));
			pc23.Interfaces.Add(new Interface(
				new Address("220.0.0.4", "/24"),
				pc23));

			Switch s2 = new Switch(_eventHandler, 4);
			s2[0].ConnectTo(pc21[0]);
			s2[1].ConnectTo(pc22[0]);
			s2[2].ConnectTo(pc23[0]);
			s2[3].ConnectTo(r2[0]);


			//Network 3
			Router r3 = new Router(_eventHandler);
			r3.Interfaces.Add(new Interface(
				new Address("172.168.1.1", "/26"), //to s3 LAN
				r3));
			r3.Interfaces.Add(new Interface(
				new Address("10.1.1.6", "/30"), //to r1 10.1.1.5
				r3));
			r3.Interfaces.Add(new Interface(
				new Address("140.1.0.2", "/16"), //to r2 140.1.0.1
				r3));

			PC pc31 = new PC(_eventHandler);
			PC pc32 = new PC(_eventHandler);
			PC pc33 = new PC(_eventHandler);
			pc31.Interfaces.Add(new Interface(
				new Address("172.168.1.2", "/26"),
				pc31));
			pc32.Interfaces.Add(new Interface(
				new Address("172.168.1.3", "/26"),
				pc32));
			pc33.Interfaces.Add(new Interface(
				new Address("172.168.1.4", "/26"),
				pc33));

			Switch s3 = new Switch(_eventHandler, 4);
			s3[0].ConnectTo(pc31[0]);
			s3[1].ConnectTo(pc32[0]);
			s3[2].ConnectTo(pc33[0]);
			s3[3].ConnectTo(r3[0]);



			//Connect all the routers
			r1[1].ConnectTo(r2[1]);
			r1[2].ConnectTo(r3[1]);
			r2[2].ConnectTo(r3[2]);

			//Start everything up
			s1.Start();
			s2.Start();
			s3.Start();
			r1.Start();
			r2.Start();
			r3.Start();
			pc11.Start();
			pc12.Start();
			pc13.Start();
			pc21.Start();
			pc22.Start();
			pc23.Start();
			pc31.Start();
			pc32.Start();
			pc33.Start();

			Thread.Sleep(1000);



			//Sending test

			//This should be sendable without route, r2 is directly connected
			r1.SendTo(new Address("10.1.1.2", "/30"),
				new Packet("Hello R2 from R1, no routes!",
					new Address("10.1.1.2", "/30"),
					new Address("10.1.1.1", "/30")));

			Console.ReadLine();

			//This should be sendable without route, same as above but from PC
			pc11.SendOutOf(0,
				new Packet("Hello R2 from PC11, no routes!",
					new Address("10.1.1.2", "/30"),
					new Address("192.168.1.2", "/24")));

			Console.ReadLine();

			//This should be sendable without route, pc12 is in same LAN
			pc11.SendOutOf(0,
				new Packet("Hello PC12 from PC11, through switch.",
					new Address("192.168.1.3", "/24"),
					new Address("192.168.1.2", "/24")));

			Console.ReadLine();

			//This should fail, r1 won't know how to get to LAN3
			pc13.SendOutOf(0,
				new Packet("Hi PC33, no routes (won't work)!",
					new Address("172.168.1.4", "255.255.255.192"),
					pc13[0].Addr));

			Console.ReadLine();


			//Create routes (note that routes go to networks, not interfaces)
			//Routes to get to other router->router connections... redundantly (survives broken links)
			r1.AddRoute(new Route(
				new Address("140.1.0.0", "/16"), //to r2<->r3
				new Address("10.1.1.2", "/30"), //through r2
				1));

			r1.AddRoute(new Route(
				new Address("140.1.0.0", "/16"), //to r2<->r3
				new Address("10.1.1.6", "/30"), //through r3
				1));

			r2.AddRoute(new Route(
				new Address("10.1.1.4", "/30"), //to r1<->r3
				new Address("10.1.1.1", "/30"), //through r1
				1));

			r2.AddRoute(new Route(
				new Address("10.1.1.4", "/30"), //to r1<->r3
				new Address("140.1.0.2", "/16"), //through r3
				1));

			r3.AddRoute(new Route(
				new Address("10.1.1.0", "/30"), //to r1<->r2
				new Address("10.1.1.5", "/30"), //through r1
				1));

			r3.AddRoute(new Route(
				new Address("10.1.1.0", "/30"), //to r1<->r2
				new Address("140.1.0.1", "/16"), //through r2
				1));

			//To other router's LANs
			//To s2 LAN
			r1.AddRoute(new Route(
				new Address("220.0.0.0", "/24"), //to s2 LAN
				new Address("10.1.1.2", "/30"), //through r2
				1));

			r1.AddRoute(new Route(
				new Address("220.0.0.0", "/24"), //to s2 LAN
				new Address("10.1.1.6", "/30"), //through r3
				2));

			r3.AddRoute(new Route(
				new Address("220.0.0.0", "/24"), //to s2 LAN
				new Address("10.1.1.5", "/30"), //through r2
				1));

			r3.AddRoute(new Route(
				new Address("220.0.0.0", "/24"), //to s2 LAN
				new Address("140.1.0.1", "/16"), //through r1
				2));

			//To s1 LAN
			r2.AddRoute(new Route(
				new Address("192.168.1.0", "/24"), //to s1 LAN
				new Address("10.1.1.1", "/30"), //through r1
				1));

			r2.AddRoute(new Route(
				new Address("192.168.1.0", "/24"), //to s1 LAN
				new Address("140.1.0.2", "/16"), //through r3
				2));

			r3.AddRoute(new Route(
				new Address("192.168.1.0", "/24"), //to s1 LAN
				new Address("10.1.1.5", "/30"), //through r1
				1));

			r3.AddRoute(new Route(
				new Address("192.168.1.0", "/24"), //to s1 LAN
				new Address("140.1.0.1", "/16"), //through r2
				2));

			//To s3 LAN
			r1.AddRoute(new Route(
				new Address("172.168.1.0", "/26"), //to s3 LAN
				new Address("10.1.1.6", "/30"), //through r3
				1));

			r1.AddRoute(new Route(
				new Address("172.168.1.0", "/26"), //to s3 LAN
				new Address("10.1.1.2", "/30"), //through r2
				2));

			r2.AddRoute(new Route(
				new Address("172.168.1.0", "/26"), //to s3 LAN
				new Address("140.1.0.2", "/16"), //through r3
				1));

			r2.AddRoute(new Route(
				new Address("172.168.1.0", "/26"), //to s3 LAN
				new Address("10.1.1.1", "/30"), //through r1
				2));

			Console.WriteLine("\n-------------\nRoutes Created!\n--------------\n");
			Console.ReadLine();


			//This should now work, as routes have been added
			pc13.SendOutOf(0,
				new Packet("Hi PC33 (with routes)!",
					new Address("172.168.1.4", "255.255.255.192"),
					pc13[0].Addr));

			Console.ReadLine();

			//Switches shouldn't need to broadcast this time as they know pc13 and r2
			pc33.SendOutOf(0,
				new Packet("Hi PC13 (no broadcast)!",
					new Address("192.168.1.4", "255.255.255.0"),
					pc33[0].Addr));

			Console.ReadLine();

			//Destroy the best route, and then do previous test again to see if it finds a path
			r3.DeleteRoute(new Route(
				new Address("192.168.1.0", "/24"), //to s1 LAN
				new Address("10.1.1.5", "/30"), //through r1
				1));

			pc33.SendOutOf(0,
				new Packet("Hi PC13 (reroute)!",
					new Address("192.168.1.4", "255.255.255.0"),
					pc33[0].Addr));


			Console.ReadLine();
			//Shut everything down
			Console.WriteLine("Shutting down...");
			pc11.Shutdown();
			pc12.Shutdown();
			pc13.Shutdown();
			pc21.Shutdown();
			pc22.Shutdown();
			pc23.Shutdown();
			pc31.Shutdown();
			pc32.Shutdown();
			pc33.Shutdown();
			s1.Shutdown();
			s2.Shutdown();
			s3.Shutdown();
			r1.Shutdown();
			r2.Shutdown();
			r3.Shutdown();
		}
	}
}
