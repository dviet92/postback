using sshsv.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace sshsv
{
    public class sshresponse
    {
        public bool status { get; set; }
        public List<string> listSSH { get; set; }
    }
    public class IPAddressRange
    {
        public static List<ssh> listsshes = new List<ssh>();
        readonly AddressFamily addressFamily;
        readonly byte[] lowerBytes;
        readonly byte[] upperBytes;
        public string country { get; set; }

        public IPAddressRange(IPAddress lower, IPAddress upper, string ctry)
        {
            // Assert that lower.AddressFamily == upper.AddressFamily

            this.addressFamily = lower.AddressFamily;
            this.lowerBytes = lower.GetAddressBytes();
            this.upperBytes = upper.GetAddressBytes();
            country = ctry;
        }
        public bool Bigger(IPAddress address)
        {
            if (address.AddressFamily != addressFamily)
            {
                return false;
            }
            byte[] addressBytes = address.GetAddressBytes();
            if ((int)(addressBytes[0]*0x1000000+ addressBytes[1]*0x10000+ addressBytes[2]*0x100+ addressBytes[3]) <= (int)(upperBytes[0] * 0x1000000 + upperBytes[1] * 0x10000 + upperBytes[2] * 0x100 + upperBytes[3])) return true;
            return false;
        }
        public bool IsInRange(IPAddress address)
        {
            if (address.AddressFamily != addressFamily)
            {
                return false;
            }

            byte[] addressBytes = address.GetAddressBytes();

            bool lowerBoundary = true, upperBoundary = true;

            for (int i = 0; i < this.lowerBytes.Length &&
                (lowerBoundary || upperBoundary); i++)
            {
                if ((lowerBoundary && addressBytes[i] < lowerBytes[i]) ||
                    (upperBoundary && addressBytes[i] > upperBytes[i]))
                {
                    return false;
                }

                lowerBoundary &= (addressBytes[i] == lowerBytes[i]);
                upperBoundary &= (addressBytes[i] == upperBytes[i]);
            }

            return true;
        }
    }
}
