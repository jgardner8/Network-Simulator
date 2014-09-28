namespace NetworkSimulator.Model
{
	struct Address
	{
		private readonly uint _ip, _mask;

		/// <summary>
		/// Initialise IP address and subnet mask with plain binary unsigned integers
		/// </summary>
		public Address(uint ipAddress, uint subnetMask)
		{
			_ip = ipAddress;
			_mask = subnetMask;
		}

		/// <summary>
		/// Initialise IP address and subnet mask in dotted decimal or slash notation
		/// </summary>
		/// <param name="ipAddress">Dotted decimal notation (aaa.bbb.ccc.ddd)</param>
		/// <param name="subnetMask">Dotted decimal (aaa.bbb.ccc.ddd) or slash notation (/xy)</param>
		public Address(string ipAddress, string subnetMask)
		{
			_ip = DottedDecimalToUInt(ipAddress);
			_mask = subnetMask.Length <= 3 ? SlashNotationToUInt(subnetMask) : DottedDecimalToUInt(subnetMask);
		}

		public uint IP { get { return _ip; } }
		public uint Mask { get { return _mask; } }
		public string FormattedIP { get { return UIntToDottedDecimal(_ip); } }
		public string FormattedMask { get { return UIntToSlashNotation(_mask); } }
		public string FormattedIPMask { get { return FormattedIP + FormattedMask; } }

		#region Conversion Functions
		/// <summary>
		/// Converts an IP address or subnet mask in aaa.bbb.ccc.ddd form to a uint (native binary form)
		/// Returns 0 on fail... doesn't support 0.0.0.0
		/// </summary>
		/// <param name="toConvert">IP address in form aaa.bbb.ccc.ddd</param>
		public static uint DottedDecimalToUInt(string toConvert)
		{
			string[] tokenised = toConvert.Split('.');
			if (tokenised.Length != 4)
				return 0;
			
			byte[] octets = new byte[4];
			for (int i = 0; i < 4; i++)
				if (!byte.TryParse(tokenised[i], out octets[i]))
					return 0;

			return (uint)(octets[0] << 24)
				 + (uint)(octets[1] << 16)
				 + (uint)(octets[2] << 8)
				 +		 (octets[3]);
		}

		/// <summary>
		/// Converts a subnet mask in slash notation to a uint (native binary form)
		/// Returns 0 on fail
		/// </summary>
		/// <param name="toConvert">Subnet mask in slash notation</param>
		public static uint SlashNotationToUInt(string toConvert)
		{
			if (toConvert.Length != 2 && toConvert.Length != 3)
				return 0;
			
			int numOnes; // Transforms "/27" into 27
			if (!int.TryParse(toConvert.Remove(0,1), out numOnes))
				return 0;

			uint result = 1;
			for (int i = 0; i < numOnes; i++)
				result = (result << 1) + 1;

			for (int i = 0; i < 32 - numOnes; i++)
				result <<= 1;

			return result;
		}
		
		/// <summary>
		/// Turns 32bit uint (native binary form) into dotted decimal notation (aaa.bbb.ccc.ddd)
		/// </summary>
		public static string UIntToDottedDecimal(uint toConvert)
		{
			byte[] octets = new byte[4];
			octets[0] = (byte)(toConvert>> 24);
			octets[1] = (byte)((toConvert << 8) >> 24);
			octets[2] = (byte)((toConvert << 16) >> 24);
			octets[3] = (byte)((toConvert << 24) >> 24);
			return string.Format("{0}.{1}.{2}.{3}", octets[0], octets[1], octets[2], octets[3]);
		}

		/// <summary>
		/// Turns 32bit uint (native binary form) into slash notation (/xy)
		/// </summary>
		/// <param name="toConvert"></param>
		/// <returns></returns>
		public static string UIntToSlashNotation(uint toConvert)
		{
			if (toConvert == 0)
				return "/32";

			int i = 0;
			while ((toConvert & 1) == 0)
			{
				toConvert >>= 1;
				i++;
			}
			return "/" + (32 - i);
		}
		#endregion

		#region Operator Overloading
		public static bool operator ==(Address a, Address b)
		{
			return a.IP == b.IP && a.Mask == b.Mask;
		}

		public static bool operator !=(Address a, Address b)
		{
			return !(a == b);
		}

		public override bool Equals(object o)
		{
			if (o is Address)
				return this == (Address)o;
			return false;
		}

		public override int GetHashCode() { return 0; } //unimplemented
		#endregion
	}
}
