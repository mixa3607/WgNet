using System;
using System.Net;

namespace ArkProjects.Wireguard.Mesh.Misc
{
    //https://github.com/aspnet/BasicMiddleware/blob/master/src/Microsoft.AspNetCore.HttpOverrides/IPNetwork.cs
    // ReSharper disable once InconsistentNaming
    public class IPNetwork
    {
        public IPNetwork(string network)
        {
            if (network == null)
                throw new ArgumentNullException(nameof(network));
            var parts = network.Split('/');
            var prefixLength = 32;
            var prefix = IPAddress.Parse(parts[0]);
            
            if (parts.Length >= 2) 
                prefixLength = int.Parse(parts[1]);

            Prefix = prefix;
            PrefixLength = prefixLength;
            PrefixBytes = Prefix.GetAddressBytes();
            Mask = CreateMask();
        }

        public IPNetwork(IPAddress prefix, int prefixLength = 32)
        {
            Prefix = prefix;
            PrefixLength = prefixLength;
            PrefixBytes = Prefix.GetAddressBytes();
            Mask = CreateMask();
        }

        public IPAddress Prefix { get; }

        private byte[] PrefixBytes { get; }

        /// <summary>
        /// The CIDR notation of the subnet mask 
        /// </summary>
        public int PrefixLength { get; }

        private byte[] Mask { get; }

        public bool Contains(IPAddress address)
        {
            if (Prefix.AddressFamily != address.AddressFamily)
            {
                return false;
            }

            var addressBytes = address.GetAddressBytes();
            for (int i = 0; i < PrefixBytes.Length && Mask[i] != 0; i++)
            {
                if (PrefixBytes[i] != (addressBytes[i] & Mask[i]))
                {
                    return false;
                }
            }

            return true;
        }

        private byte[] CreateMask()
        {
            var mask = new byte[PrefixBytes.Length];
            int remainingBits = PrefixLength;
            int i = 0;
            while (remainingBits >= 8)
            {
                mask[i] = 0xFF;
                i++;
                remainingBits -= 8;
            }
            if (remainingBits > 0)
            {
                mask[i] = (byte)(0xFF << (8 - remainingBits));
            }

            return mask;
        }

        public override string ToString()
        {
            return Prefix + "/" + PrefixLength;
        }
    }
}