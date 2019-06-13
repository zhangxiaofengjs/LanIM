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
                LoggerFactory.Debug("try listen.local={0}, port={1}", local, port);

                this._tcpListener = new TcpListener(local, port);
                this._tcpListener.AllowNatTraversal(true);
                this._tcpListener.Start();

                LoggerFactory.Debug("listening...");
                this._tcpListener.BeginAcceptTcpClient(AsyncAcceptClientHandler, null);
                return true;
            }
            catch (Exception e)
            {
                OnError(Errors.NetworkError, "监听失败。", null, e);
            }
            return false;
        }

        public void Close()
        {
            LoggerFactory.Debug("try close listener");
            try
            {
                if (this._tcpListener != null)
                {
                    this._tcpListener.Stop();
                }
                LoggerFactory.Debug("listener closed");
            }
            catch (Exception e)
            {
                OnError(Errors.NetworkError, "停止监听失败。", null, e);
            }
        }

        private void AsyncAcceptClientHandler(IAsyncResult ar)
        {
            try
            {
                LoggerFactory.Debug("try accept client");
                TcpClient client = _tcpListener.EndAcceptTcpClient(ar);

                LoggerFactory.Debug("accepted client and start send file task");
                //启动线程任务进行发送文件处理
                _taskFactory.StartNew(new Action<object>(SendFileHandler), client);

                //继续下一个监听
                LoggerFactory.Debug("try accept next client");
                this._tcpListener.BeginAcceptTcpClient(AsyncAcceptClientHandler, null);
            }
            catch (Exception e)
            {
                OnError(Errors.NetworkError, "接收客户端连接失败。", null, e);
            }
        }

        private void SendFileHandler(object clientobj)
        {
            LoggerFactory.Debug("begin send file");

            TcpClient client = null;
            NetworkStream ns = null;
            try
            {
                client = clientobj as TcpClient;
                ns = client.GetStream();

                if (!ns.CanRead)
                {
                    //想定外
                    ns.Close();
                    client.Close();
                    OnError(Errors.NotExistTransportId, "未收到传送ID", null, null);
                    return;
                }
            }
            catch (Exception e)
            {
                OnError(Errors.NetworkError, "尝试收取传送ID失败", null, e);
                return;
            }

            LoggerFactory.Debug("get file id");
            //取得传送的文件ID
            byte[] buff = null;
            int len = 0;
            try
            {
                buff = new byte[this.ReceiveBufferSize];
                len = ns.Read(buff, 0, buff.Length);
            }
            catch (Exception e)
            {
                ns.Close();
                client.Close();
                OnError(Errors.NetworkError, "尝试收取传送ID失败", null, e);
                return;
            }

            TcpPacket packet = null;
            try
            {
                IPacketResolver resolver = PacketResolverFactory.CreateResolver(buff, 0, len, this.SecurityKeys.Private);
                packet = resolver.Resolve() as TcpPacket;
            }
            catch (Exception e)
            {
                ns.Close();
                client.Close();
                OnError(Errors.ResolveError, "解密文件ID失败", null, e);
                return;
            }

            LoggerFactory.Debug("get file");
            TransportFile file = null;
            TcpPacketRequestFileTransportExtend extend = packet.Extend as TcpPacketRequestFileTransportExtend;
            if (!_transportFileDic.TryRemove(extend.FileID, out file))
            {
                ns.Close();
                client.Close();
                OnError(Errors.NotExistTransportId, "不存在的文件ID=" + extend.FileID, null, null);
                return;
            }

            LanFile lanFile = file.File;
            if (!File.Exists(lanFile.Path))
            {
                ns.Close();
                client.Close();
                OnError(Errors.FileOpenError, "不存在的文件:" + lanFile.Path, file, null);
                return;
            }

            LoggerFactory.Debug("open file");
            FileStream fs = null;
            try
            {
                fs = new FileStream(lanFile.Path, FileMode.Open);
            }
            catch (Exception e)
            {
                ns.Close();
                client.Close();
                OnError(Errors.FileOpenError, "文件打开失败", file, e);
                return;
            }

            LoggerFactory.Debug("sending file");
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
                OnError(Errors.NetworkError, "文件传输网络错误", file, e);
            }
            finally
            {
                fs.Close();

                ns.Flush();
                ns.Close();
                client.Close();

                OnCompleted(file);
            }
            LoggerFactory.Debug("end send file");
        }

        protected virtual void OnProgressChanged(TransportFile file)
        {
            LoggerFactory.Debug("send file progress change: file={0}, progress={1}", file, file.Progress);
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
            LoggerFactory.Debug("send file completed");
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

        protected virtual void OnError(Errors error, string message, TransportFile file, Exception e)
        {
            LoggerFactory.Error("{0}, file={1}, exception={2}", message, file, e);
            FileTransportErrorEventArgs args = new FileTransportErrorEventArgs(error, message, file, e);
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
            LoggerFactory.Debug("add ready send file={0}", file);
            _transportFileDic.TryAdd(file.ID, file);
        }
    }
}
