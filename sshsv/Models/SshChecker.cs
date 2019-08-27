using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using System.IO;
using System.Threading;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Globalization;

using Routrek.Crypto;
using Routrek.SSHC;
using Routrek.SSHCV1;
using Routrek.SSHCV2;
using Routrek.Toolkit;
using Routrek.PKI;

namespace sshserver3
{
    public class SshChecker
    {
        public int LineIndex { set; get; }

        private SSHConnection _conn;
        private Socket _socket;
        private Reader _reader;
        private SSHConnectionParameter _f;
        public string row;
        public int TimeoutSeconds;
        public int DefaultPort;
        private string Host;
        private string User;
        private string Pass;
        private string Country;
        public delegate void PassControl(int LineIndex, bool IsFresh, string Ssh, string ErrorMessage);
        public PassControl passControl;
        private bool SSHConnectTimeout = false;

        public void CheckFresh()
        {
            //if (ThreadManager.IsRunning == false) return;

            try
            {
                string[] cols = row.Split('|');
                Host = cols[0].Trim();
                User = cols[1].Trim();
                Pass = cols[2].Trim();
                if (cols.Count() == 3) Country = "UNKNOWN";
                else Country = cols[3].Trim();
                _f = new SSHConnectionParameter();
                _f.UserName = User;
                _f.Password = Pass;
                _f.Protocol = SSHProtocol.SSH2;
                _f.AuthenticationType = AuthenticationType.Password;
                _f.WindowSize = 0x1000;

                _reader = new Reader();
                _reader.TimeoutSeconds = TimeoutSeconds;
                _reader.passControl = new Reader.PassControl(passControl);
                _reader._country = Country;
                _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                //s.Blocking = false;
                _socket.SendTimeout = TimeoutSeconds * 1000;
                _socket.ReceiveTimeout = TimeoutSeconds * 1000;
                _socket.Connect(new IPEndPoint(IPAddress.Parse(Host), DefaultPort));

                new Thread(new ThreadStart(SetSSHConnectionTimeout)).Start();
                _conn = SSHConnection.Connect(_f, _reader, _socket);
                if (SSHConnectTimeout) throw new Exception("SSH Connect Timeout");
                _reader._conn = _conn;

                SSHChannel ch = _conn.ForwardPort(_reader, "ipinfo.io", 80, "localhost", 80);
                _reader._pf = ch;

                int seconds = TimeoutSeconds;
                while (_reader._ready == false && seconds > 0)
                {
                    seconds--; Thread.Sleep(1000);
                }
                if (_reader._ready == false && seconds <= 0)
                {
                    throw new Exception("Reader._ready timeout");
                }

                _reader.LineIndex = LineIndex;
                _reader.Host = Host;
                _reader.User = User;
                _reader.Pass = Pass;

                new Thread(new ThreadStart(_reader.SetHTTPRequestTimeout)).Start();
                _reader._pf.Transmit(Encoding.ASCII.GetBytes("GET /json HTTP/1.1\r\nHost:ipinfo.io\r\n\r\n")); //http://ipinfo.io/json
            }
            catch (Exception ex)
            {
                passControl(LineIndex, false, "", ex.Message);
            }
        }
        private void SetSSHConnectionTimeout()
        {
            int seconds = TimeoutSeconds;
            while (seconds > 0)
            {
                seconds--; Thread.Sleep(1000);
                if (_conn != null) break;
            }

            if (seconds <= 0 && _conn == null)
            {
                SSHConnectTimeout = true;

                try
                {
                    _socket.Disconnect(false);
                }
                catch { }
                try
                {
                    _socket.Close();
                }
                catch { }
                try
                {
                    _f = new SSHConnectionParameter();
                }
                catch { }
                try
                {
                    _reader = new Reader();
                    _reader.TimeoutSeconds = TimeoutSeconds;
                    _reader.passControl = new Reader.PassControl(passControl);
                    _reader._country = Country;
                }
                catch { }
            }
        }
    }

    public class Reader : ISSHConnectionEventReceiver, ISSHChannelEventReceiver
    {
        public SSHConnection _conn;
        public SSHChannel _pf;
        public bool _ready;
        public delegate void PassControl(int LineIndex, bool IsFresh, string Ssh, string ErrorMessage);
        public PassControl passControl;
        public int TimeoutSeconds;
        //PhuongTM add more properties
        public int LineIndex;

        public string Host;
        public string User;
        public string Pass;
        public string _country;
        private DateTime StartRequest;
        private DateTime EndRequest;

        public void SetHTTPRequestTimeout()
        {
            StartRequest = DateTime.Now;

            int seconds = TimeoutSeconds;
            while (seconds > 0)
            {
                seconds--; Thread.Sleep(1000);
                if (EndRequest != null) break;
            }

            if (seconds <= 0 && EndRequest == null)
            {
                //Close anything
                try
                {
                    _conn.CancelForwardedPort("localhost", 80);
                }
                catch { }
                try
                {
                    _pf.Close();
                }
                catch { }
                try
                {
                    _conn.Disconnect("");
                }
                catch { }
                try
                {
                    _conn.Close();
                }
                catch { }

                passControl(LineIndex, false, "", "HTTP Request Timeout");
            }
        }
        public void OnData(byte[] data, int offset, int length)
        {
            //System.Console.Write(Encoding.ASCII.GetString(data, offset, length));

            EndRequest = DateTime.Now;

            //Ping Miliseconds
            TimeSpan ts = EndRequest - StartRequest;

            string response = Encoding.ASCII.GetString(data, offset, length);

            bool IsFresh = response.Length > 0 ? true : false;

            //Close anything
            try
            {
                _conn.CancelForwardedPort("localhost", 80);
            }
            catch { }
            try
            {
                _pf.Close();
            }
            catch { }
            try
            {
                _conn.Disconnect("");
            }
            catch { }
            try
            {
                _conn.Close();
            }
            catch { }

            //"country": "VN",
            string Country = "";
            Match mat = Regex.Match(response, "\"country\"\\:\\s?\"(?<country>[^\"]+)\"", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            if (mat.Success) Country = _country;

            //"region": "California",
            string State = "";
            mat = Regex.Match(response, "\"region\"\\:\\s?\"(?<region>[^\"]+)\"", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            if (mat.Success) State = mat.Groups["region"].Value;

            //"city": null,
            string City = "";
            mat = Regex.Match(response, "\"city\"\\:\\s?\"(?<city>[^\"]+)\"", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            if (mat.Success) City = mat.Groups["city"].Value;

            passControl(LineIndex, IsFresh, string.Format("{0}|{1}|{2}|{3}|{4}|{5}|{6}|", Host, User, Pass, Country, State, City, (int)ts.TotalMilliseconds), IsFresh ? "" : "HTTP Response is empty");
        }
        public void OnDebugMessage(bool always_display, byte[] data)
        {
            Debug.WriteLine("DEBUG: " + Encoding.ASCII.GetString(data));
        }
        public void OnIgnoreMessage(byte[] data)
        {
            Debug.WriteLine("Ignore: " + Encoding.ASCII.GetString(data));
        }
        public void OnAuthenticationPrompt(string[] msg)
        {
            Debug.WriteLine("Auth Prompt " + msg[0]);
        }

        public void OnError(Exception error, string msg)
        {
            Debug.WriteLine("ERROR: " + msg);
        }
        public void OnChannelClosed()
        {
            Debug.WriteLine("Channel closed");
            _conn.Disconnect("");
            //_conn.AsyncReceive(this);
        }
        public void OnChannelEOF()
        {
            _pf.Close();
            Debug.WriteLine("Channel EOF");
        }
        public void OnExtendedData(int type, byte[] data)
        {
            Debug.WriteLine("EXTENDED DATA");
        }
        public void OnConnectionClosed()
        {
            Debug.WriteLine("Connection closed");
        }
        public void OnUnknownMessage(byte type, byte[] data)
        {
            Debug.WriteLine("Unknown Message " + type);
        }
        public void OnChannelReady()
        {
            _ready = true;
        }
        public void OnChannelError(Exception error, string msg)
        {
            Debug.WriteLine("Channel ERROR: " + msg);
        }
        public void OnMiscPacket(byte type, byte[] data, int offset, int length)
        {
        }

        public PortForwardingCheckResult CheckPortForwardingRequest(string host, int port, string originator_host, int originator_port)
        {
            PortForwardingCheckResult r = new PortForwardingCheckResult();
            r.allowed = true;
            r.channel = this;
            return r;
        }
        public void EstablishPortforwarding(ISSHChannelEventReceiver rec, SSHChannel channel)
        {
            _pf = channel;
        }

    }
}
