using Com.LanIM.Common.Security;
using Com.LanIM.Network;
using Com.LanIM.Network.Packets;
using LanIM.Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Com.LanIM.Network
{
    public class TransportFile
    {
        public long ID { get; set; }
        public IPAddress Remote{ get; set; }
        public int Port { get; set; }
        public byte[] PublicKey { get; set; }
        public LanFile File { get; set; }
        public string SavePath { get; set; }
        private long _transportedLength = 0;
        public long TransportedLength
        {
            get
            {
                return _transportedLength;
            }
            set
            {
                _transportedLength = value;
                _nowTransportTicks = DateTime.Now.Ticks;
            }
        }
        //每秒的传输字节数
        public long TransportedSpeed
        {
            get
            {
                return (long)(_transportedLength * 10000000.0 / (NowTransportTicks - _startTransportTicks) );
            }
        }

        public int Progress
        {
            get
            {
                double p = (double)TransportedLength * 100 / (double)File.Length;
                return (int)p;
            }
        }

        private long _nowTransportTicks;
        internal long NowTransportTicks { get => _nowTransportTicks; }

        private long _startTransportTicks;

        public TransportFile(long id, IPAddress remote, int port, byte[] publicKey, LanFile file)
        {
            this.ID = id;
            this.Remote = remote;
            this.Port = port;
            this.PublicKey = publicKey;
            this.File = file;
        }

        public override string ToString()
        {
            return string.Format("id={0}, remote={1}, port={2}, file={3}, savepath={4}",
                this.ID, this.Remote, this.Port, this.File, this.SavePath);
        }

        public void StartTransport()
        {
            _startTransportTicks = _nowTransportTicks = DateTime.Now.Ticks;
        }

        internal void Transported(int len)
        {
        }
    }
}
