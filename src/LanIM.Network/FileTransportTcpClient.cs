using Com.LanIM.Common;
using Com.LanIM.Common.Logger;
using Com.LanIM.Common.Security;
using Com.LanIM.Network.Packets;
using Com.LanIM.Network.PacketEncoder;
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
                LoggerFactory.Debug("start receive file:id={0}", file.ID);
                string tmpPath = file.SavePath + "." + Path.GetRandomFileName().Replace(".", "") + ".lamim";
                FileStream fs = null;
                try
                {
                    fs = new FileStream(tmpPath, FileMode.Create);
                }
                catch (Exception e)
                {
                    OnError(Errors.FileOpenError, "打开文件失败:"+ tmpPath, file, e);
                    return;
                }
                LoggerFactory.Debug("opened file:" + tmpPath);

                LoggerFactory.Debug("try connect remote:remote={0}, port={1}", file.Remote, file.Port);
                TcpClient tcpClient = null;
                NetworkStream ns = null;
                try
                {
                    tcpClient = new TcpClient(AddressFamily.InterNetwork)
                    {
                        ReceiveBufferSize = this.ReceiveBufferSize
                    };

                    tcpClient.Connect(file.Remote, file.Port);

                    ns = tcpClient.GetStream();
                }
                catch (Exception e)
                {
                    OnError(Errors.NetworkError, "连接失败", file, e);
                    return;
                }
                LoggerFactory.Debug("conneted remote:remote={0}, port={1}", file.Remote, file.Port);

                LoggerFactory.Debug("encode packet, request file:id={0}", file.ID);
                TcpPacket packet = null;
                EncodeResult result = null;
                try
                {
                    //发送要接受的文件ID
                    TcpPacketRequestFileTransportExtend extend = new TcpPacketRequestFileTransportExtend
                    {
                        EncryptKey = file.PublicKey,
                        FileID = file.ID
                    };

                    packet = new TcpPacket
                    {
                        Command = TcpPacket.CMD_REQUEST_FILE_TRANSPORT,
                        Extend = extend
                    };

                    IPacketEncoder encoder = PacketEncoderFactory.CreateEncoder(packet);
                    result = encoder.Encode(null);
                }
                catch (Exception e)
                {
                    OnError(Errors.EncodeError, "请求文件ID包加密失败。" + packet.ToString(), file, e);
                    return;
                }

                LoggerFactory.Debug("send packet, request file:id={0}", file.ID);
                try
                {
                    ns.Write(result.Fragments[0], 0, result.Fragments[0].Length);
                }
                catch(Exception e)
                {
                    OnError(Errors.NetworkError, "请求文件ID包发送失败", file, e);
                    return;
                }

                LoggerFactory.Debug("receive file start:id={0}", file.ID);
                try
                {
                    int len = 0;
                    file.StartTransport();
                    long lastProgressTicks = file.NowTransportTicks;

                    byte[] buff = new byte[this.ReceiveBufferSize];
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
                    LoggerFactory.Debug("receive file end:id={0}", file.ID);
                }
                catch (Exception e)
                {
                    OnError(Errors.NetworkError, "文件接收失败", file, e);
                }
                finally
                {
                    LoggerFactory.Debug("close connect and save file:id={0}", file.ID);
                    //关闭连接
                    ns.Close();
                    tcpClient.Close();

                    fs.Flush(true);
                    fs.Close();

                    if (LanFile.Rename(tmpPath, file.SavePath))
                    {
                        //发送接收完毕
                        OnCompleted(file);
                    }
                    else
                    { 
                        OnError(Errors.FileWriteError, "文件写入失败", file, null);
                    }
                }
                LoggerFactory.Debug("end receive file:id={0}", file.ID);
            });
        }

        protected virtual void OnProgressChanged(TransportFile file)
        {
            FileTransportEventArgs args = new FileTransportEventArgs(file);

            LoggerFactory.Debug("receive progress changed:id={0}, progress={1}", file.ID, file.Progress);
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
            ProgressChanged?.Invoke(this, state as FileTransportEventArgs);
        }

        protected virtual void OnCompleted(TransportFile file)
        {
            FileTransportEventArgs args = new FileTransportEventArgs(file);
            LoggerFactory.Debug("receive completed:id={0}", file.ID);
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
            Completed?.Invoke(this, state as FileTransportEventArgs);
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
            Error?.Invoke(this, state as FileTransportErrorEventArgs);
        }
    }
}
