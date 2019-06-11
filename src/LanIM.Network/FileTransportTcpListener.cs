using Com.LanIM.Common.Logger;
using Com.LanIM.Common.Security;
using Com.LanIM.Network.Packets;
using Com.LanIM.Network.PacketsResolver;
using LanIM.Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Com.LanIM.Network
{
    public class FileTransportTcpListener
    {
        private const int DEFAULT_RECEIVE_BUFFER_SIZE = 1024 * 1024;//1M

        private TcpListener _tcpListener = null;
        private TaskFactory _taskFactory = new TaskFactory();
        private ConcurrentDictionary<long, TransportFile> _transportFileDic = new ConcurrentDictionary<long, TransportFile>();

        public int SendBufferSize { get; set; }
        public int ReceiveBufferSize { get; set; }
        public SecurityKeys SecurityKeys { get; set; }

        public FileTransportTcpListener()
        {
            this.ReceiveBufferSize = DEFAULT_RECEIVE_BUFFER_SIZE;
        }

        public bool Listen(IPAddress local, int port)
        {
            try
            {
                this._tcpListener = new TcpListener(local, port);
                this._tcpListener.AllowNatTraversal(true);
                this._tcpListener.Start();

                this._tcpListener.BeginAcceptTcpClient(AsyncAcceptClientHandler, null);
                return true;
            }
            catch (Exception e)
            {
                LoggerFactory.Instance().Error("FileTransTcpListener Start Error", e);
            }
            return false;
        }

        public void Close()
        {
            if (this._tcpListener != null)
            {
                this._tcpListener.Stop();
            }
        }

        private void AsyncAcceptClientHandler(IAsyncResult ar)
        {
            try
            {
                TcpClient client = _tcpListener.EndAcceptTcpClient(ar);

                //启动线程任务进行发送文件处理
                _taskFactory.StartNew(new Action<object>(SendFileHandler), client);

                //继续下一个监听
                this._tcpListener.BeginAcceptTcpClient(AsyncAcceptClientHandler, null);
            }
            catch (Exception e)
            {
                LoggerFactory.Instance().Error("TcpClientEx Connect Error", e);
            }
        }

        private void SendFileHandler(object clientobj)
        {
            TcpClient client = clientobj as TcpClient;
            NetworkStream ns = client.GetStream();

            if (!ns.CanRead)
            {
                //想定外
                ns.Close();
                client.Close();
                return;
            }

            //取得传送的文件ID
            byte[] buff = new byte[this.ReceiveBufferSize];
            int len = ns.Read(buff, 0, buff.Length);
            IPacketResolver resolver = PacketResolverFactory.CreateResolver(buff, 0, buff.Length, this.SecurityKeys.Private);
            TcpPacket packet = resolver.Resolve() as TcpPacket;
            if (packet == null)
            {
                ns.Close();
                client.Close();
                return;
            }
            TcpPacketRequestFileTransportExtend extend = packet.Extend as TcpPacketRequestFileTransportExtend;
            if (extend == null)
            {
                ns.Close();
                client.Close();
                return;
            }

            TransportFile file = null;
            if (!_transportFileDic.TryRemove(extend.FileID, out file))
            {
                ns.Close();
                client.Close();
                return;
            }

            LanFile lanFile = file.File;
            if (!File.Exists(lanFile.Path))
            {
                ns.Close();
                client.Close();
                return;
            }

            FileStream fs = LanFile.OpenReadFileStream(lanFile.Path);
            if (fs == null)
            {
                ns.Close();
                client.Close();
                return;
            }

            try
            {
                //发送文件
                while ((len = fs.Read(buff, 0, buff.Length)) != 0)
                {
                    ns.Write(buff, 0, len);
                }
            }
            catch (Exception e)
            {
                LoggerFactory.Instance().Error("网络错误", e);
            }
            finally
            {
                fs.Close();

                ns.Flush();
                ns.Close();
                client.Close();
            }
        }

        public void AddTransportFile(TransportFile file)
        {
            _transportFileDic.TryAdd(file.ID, file);
        }
    }
}
