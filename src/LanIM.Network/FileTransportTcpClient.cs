using Com.LanIM.Common.Logger;
using Com.LanIM.Common.Security;
using Com.LanIM.Network.Packets;
using Com.LanIM.Network.PacketsEncoder;
using LanIM.Common;
using System;
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
    public class FileTransportTcpClient
    {
        private const int DEFAULT_RECEIVE_BUFFER_SIZE = 1024 * 1024;//1M
        private const int DEFAULT_PROGRESS_CHANGE_INTERVAL = 5000000;//500ms

        public int ReceiveBufferSize { get; set; }
        public SecurityKeys SecurityKeys { get; set; }

        private int _progressChangeInterval = DEFAULT_PROGRESS_CHANGE_INTERVAL;
        //进度更新的间隔，毫秒
        public int ProgressChangeInterval
        {
            get { return _progressChangeInterval / 10000; }
            set { _progressChangeInterval = value * 10000; }
        }

        private SynchronizationContext _context;

        public event FileTransportEventHandler ProgressChanged;
        public event FileTransportEventHandler Completed;
        public event FileTransportErrorEventHandler Error;

        public FileTransportTcpClient(SynchronizationContext context)
        {
            this._context = context;
            this.ReceiveBufferSize = DEFAULT_RECEIVE_BUFFER_SIZE;
        }

        public void Receive(TransportFile file)
        {
            TaskFactory taskFactory = new TaskFactory();
            taskFactory.StartNew(() =>
            {
                string tmpPath = file.SavePath + "." + Path.GetRandomFileName().Replace(".", "") + ".lamim";
                FileStream fs = LanFile.OpenCreateFileStream(tmpPath);
                if (fs == null)
                {
                    OnError(file, ErrorReason.FileOpenError);
                    return;
                }

                try
                {
                    TcpClient tcpClient = new TcpClient(AddressFamily.InterNetwork);
                    tcpClient.ReceiveBufferSize = this.ReceiveBufferSize;

                    tcpClient.Connect(file.Remote, file.Port);

                    NetworkStream ns = tcpClient.GetStream();

                    //发送要接受的文件ID
                    TcpPacketRequestFileTransportExtend extend = new TcpPacketRequestFileTransportExtend();
                    extend.EncryptKey = file.PublicKey;
                    extend.FileID = file.ID;

                    TcpPacket packet = new TcpPacket();
                    packet.Command = TcpPacket.CMD_REQUEST_FILE_TRANSPORT;
                    packet.Extend = extend;

                    IPacketEncoder encoder = PacketEncoderFactory.CreateEncoder(packet);
                    byte[] buff = encoder.Encode();
                    ns.Write(buff, 0, buff.Length);

                    int len = 0;
                    file.StartTransport();
                    long lastProgressTicks = file.NowTransportTicks;

                    buff = new byte[this.ReceiveBufferSize];
                    while ((len = ns.Read(buff, 0, buff.Length)) != 0)
                    {
                        fs.Write(buff, 0, len);

                        file.Transported(len);

                        if (file.TransportedLength == file.File.Length ||
                            (DateTime.Now.Ticks - lastProgressTicks) > this._progressChangeInterval) //避免进度太频繁，500ms一次
                        {
                            OnProgressChanged(file);
                            lastProgressTicks = file.NowTransportTicks;
                        }
                    }

                    //关闭连接
                    ns.Close();
                    tcpClient.Close();
                }
                catch (Exception e)
                {
                    OnError(file, ErrorReason.NetworkError);
                    LoggerFactory.Instance().Error("网络错误:", e);
                }
                finally
                {
                    fs.Flush(true);
                    fs.Close();

                    if (LanFile.Rename(tmpPath, file.SavePath))
                    {
                        //发送接收完毕
                        OnCompleted(file);
                    }
                    else
                    { 
                        OnError(file, ErrorReason.FileWriteError);
                    }
                }
            });
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
    }
}
