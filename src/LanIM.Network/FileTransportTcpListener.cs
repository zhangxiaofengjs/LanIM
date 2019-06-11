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
using static Com.LanIM.Network.FileTransportErrorEventArgs;

namespace Com.LanIM.Network
{
    public class FileTransportTcpListener
    {
        private const int DEFAULT_RECEIVE_BUFFER_SIZE = 1024 * 1024;//1M
        private const int DEFAULT_PROGRESS_CHANGE_INTERVAL = 5000000;//500ms

        private TcpListener _tcpListener = null;
        private TaskFactory _taskFactory = new TaskFactory();
        private ConcurrentDictionary<long, TransportFile> _transportFileDic = new ConcurrentDictionary<long, TransportFile>();
        private int _progressChangeInterval = DEFAULT_PROGRESS_CHANGE_INTERVAL;
        //进度更新的间隔，毫秒
        public int ProgressChangeInterval
        {
            get { return _progressChangeInterval / 10000; }
            set { _progressChangeInterval = value * 10000; }
        }
        public int SendBufferSize { get; set; }
        public int ReceiveBufferSize { get; set; }
        public SecurityKeys SecurityKeys { get; set; }
        private SynchronizationContext _context;

        public event FileTransportEventHandler ProgressChanged;
        public event FileTransportEventHandler Completed;
        public event FileTransportErrorEventHandler Error;

        public FileTransportTcpListener(SynchronizationContext context)
        {
            this.ReceiveBufferSize = DEFAULT_RECEIVE_BUFFER_SIZE;
            this._context = context;
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
                OnError(null, ErrorReason.NotExistTransportId);
                return;
            }

            //取得传送的文件ID
            byte[] buff = new byte[this.ReceiveBufferSize];
            int len = ns.Read(buff, 0, buff.Length);
            IPacketResolver resolver = PacketResolverFactory.CreateResolver(buff, 0, len, this.SecurityKeys.Private);
            TcpPacket packet = resolver.Resolve() as TcpPacket;
            if (packet == null)
            {
                ns.Close();
                client.Close();
                OnError(null, ErrorReason.NotExistTransportId);
                return;
            }
            TcpPacketRequestFileTransportExtend extend = packet.Extend as TcpPacketRequestFileTransportExtend;
            if (extend == null)
            {
                ns.Close();
                client.Close();
                OnError(null, ErrorReason.NotExistTransportId);
                return;
            }

            TransportFile file = null;
            if (!_transportFileDic.TryRemove(extend.FileID, out file))
            {
                ns.Close();
                client.Close();
                OnError(null, ErrorReason.NotExistTransportId);
                return;
            }

            LanFile lanFile = file.File;
            if (!File.Exists(lanFile.Path))
            {
                ns.Close();
                client.Close();
                OnError(file, ErrorReason.FileOpenError);
                return;
            }

            FileStream fs = LanFile.OpenReadFileStream(lanFile.Path);
            if (fs == null)
            {
                ns.Close();
                client.Close();
                OnError(file, ErrorReason.FileOpenError);
                return;
            }

            try
            {
                //发送文件
                file.StartTransport();
                long lastProgressTicks = file.NowTransportTicks;

                while ((len = fs.Read(buff, 0, buff.Length)) != 0)
                {
                    ns.Write(buff, 0, len);

                    file.Transported(len);

                    if (file.TransportedLength == file.File.Length ||
                            (DateTime.Now.Ticks - lastProgressTicks) > this._progressChangeInterval) //避免进度太频繁，500ms一次
                    {
                        OnProgressChanged(file);
                        lastProgressTicks = file.NowTransportTicks;
                    }
                }
            }
            catch (Exception e)
            {
                OnError(file, ErrorReason.NetworkError);
                LoggerFactory.Instance().Error("网络错误", e);
            }
            finally
            {
                fs.Close();

                ns.Flush();
                ns.Close();
                client.Close();

                OnCompleted(file);
            }
        }

        protected virtual void OnProgressChanged(TransportFile file)
        {
            FileTransportEventArgs args = new FileTransportEventArgs(file);
            if (_context == null)
            {
                ProgressChangedSendOrPostCallBack(args);
            }
            else
            {
                //在指定线程上调用事件委托
                _context.Post(ProgressChangedSendOrPostCallBack, args);
            }
        }

        private void ProgressChangedSendOrPostCallBack(object state)
        {
            if (ProgressChanged != null)
            {
                ProgressChanged(this, state as FileTransportEventArgs);
            }
        }

        protected virtual void OnCompleted(TransportFile file)
        {
            FileTransportEventArgs args = new FileTransportEventArgs(file);
            if (_context == null)
            {
                CompletedSendOrPostCallBack(args);
            }
            else
            {
                //在指定线程上调用事件委托
                _context.Post(CompletedSendOrPostCallBack, args);
            }
        }

        private void CompletedSendOrPostCallBack(object state)
        {
            if (Completed != null)
            {
                Completed(this, state as FileTransportEventArgs);
            }
        }

        protected virtual void OnError(TransportFile file, ErrorReason reason)
        {
            FileTransportErrorEventArgs args = new FileTransportErrorEventArgs(file, reason);
            if (_context == null)
            {
                ErrorSendOrPostCallBack(args);
            }
            else
            {
                //在指定线程上调用事件委托
                _context.Post(ErrorSendOrPostCallBack, args);
            }
        }

        private void ErrorSendOrPostCallBack(object state)
        {
            if (Error != null)
            {
                Error(this, state as FileTransportErrorEventArgs);
            }
        }

        public void AddTransportFile(TransportFile file)
        {
            _transportFileDic.TryAdd(file.ID, file);
        }
    }
}
