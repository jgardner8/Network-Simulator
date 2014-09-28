using System;
using System.Windows.Forms;

namespace NetworkSimulator.View
{
	public partial class MainForm : Form
	{
		public MainForm()
		{
			InitializeComponent();
		}

		private void RunTests()
		{
			//Tests.DirectPcTest();
			//Tests.HubTest();
			//Tests.SwitchTest();
			//Tests.BasicRouterTest();
			Tests.ComplexNetworkTest();
		}

		public void OnLoad(object sender, EventArgs e)
		{
			RunTests();
		}
	}
}
