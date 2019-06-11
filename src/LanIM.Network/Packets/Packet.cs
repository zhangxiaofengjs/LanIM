﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Com.LanIM.Network.Packets
{
    public abstract class Packet
    {
        //版本
        public const short VERSION = 0x10;
        public const short PACKTE_TYPE_UDP = 0x01;
        public const short PACKTE_TYPE_TCP = 0x02;
        //编码
        public static Encoding ENCODING = Encoding.UTF8;

        public short Version { get; set; }
        //包种类, 0:udp 1:tcp
        public abstract short Type { get; }
        public object Extend { get; set; }

        public Packet()
        {
            this.Version = VERSION;
        }

        public override string ToString()
        {
            string str = string.Format("{{ver={0}}}",
                    Version
                    );
            return str;
        }
    }
}
