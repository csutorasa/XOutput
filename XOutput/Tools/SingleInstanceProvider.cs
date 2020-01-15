using System;
using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Threading;
using XOutput.Logging;
using XOutput.Core.DependencyInjection;
using XOutput.Core.Threading;

namespace XOutput.Tools
{
    public class SingleInstanceProvider : IDisposable
    {
        private static readonly ILogger logger = LoggerFactory.GetLogger(typeof(SingleInstanceProvider));

        private const string MutexName = "XOutputRunningAlreadyMutex";
        private const string PipeName = "XOutputRunningAlreadyNamedPipe";
        private const string ShowCommand = "Show";
        private const string OkResponse = "OK";
        private const string ErrorResponse = "ERROR";

        public event Action ShowEvent;

        private ThreadContext notifyThreadContext;
        private readonly Mutex mutex = new Mutex(false, MutexName);

        [ResolverMethod]
        public SingleInstanceProvider()
        {

        }

        public bool TryGetLock()
        {
            return mutex.WaitOne(0, false);
        }

        public void ReleaseLock()
        {
            mutex.ReleaseMutex();
        }

        public void Close()
        {
            StopNamedPipe();
            mutex.Close();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            Close();
        }

        public void StartNamedPipe()
        {
            notifyThreadContext = ThreadCreator.Create("XOutputRunningAlreadyNamedPipe reader", ReadPipe).Start();
        }

        public void StopNamedPipe()
        {
            if (notifyThreadContext != null)
            {
                notifyThreadContext.Cancel().Wait();
            }
        }

        public bool Notify()
        {
            using (var client = new NamedPipeClientStream(PipeName))
            {
                client.Connect();
                StreamString ss = new StreamString(client);
                ss.WriteString(ShowCommand);
                return ss.ReadString() == OkResponse;
            }
        }

        private void ReadPipe(CancellationToken token)
        {
            using (var notifyServerStream = new NamedPipeServerStream(PipeName, PipeDirection.InOut, 1, PipeTransmissionMode.Message, PipeOptions.Asynchronous))
            {
                token.Register(() => {
                    notifyServerStream?.SafePipeHandle.Close();
                    notifyServerStream?.Close();
                });
                while (!token.IsCancellationRequested)
                {
                    notifyServerStream.WaitForConnection();
                    StreamString ss = new StreamString(notifyServerStream);
                    string command = ss.ReadString();
                    ss.WriteString(ProcessCommand(command));
                    notifyServerStream.Disconnect();
                }
            }
        }

        private string ProcessCommand(string request)
        {
            if(request == ShowCommand)
            {
                ShowEvent?.Invoke();
                return OkResponse;
            }
            return ErrorResponse;
        }
    }

    class StreamString
    {
        private Stream ioStream;
        private UnicodeEncoding streamEncoding = new UnicodeEncoding();

        public StreamString(Stream ioStream)
        {
            this.ioStream = ioStream;
        }

        public string ReadString()
        {
            int len;
            len = ioStream.ReadByte() * 256;
            len += ioStream.ReadByte();
            byte[] inBuffer = new byte[len];
            ioStream.Read(inBuffer, 0, len);

            return streamEncoding.GetString(inBuffer);
        }

        public int WriteString(string outString)
        {
            byte[] outBuffer = streamEncoding.GetBytes(outString);
            int len = outBuffer.Length;
            if (len > ushort.MaxValue)
            {
                len = ushort.MaxValue;
            }
            ioStream.WriteByte((byte)(len / 256));
            ioStream.WriteByte((byte)(len & 255));
            ioStream.Write(outBuffer, 0, len);
            ioStream.Flush();

            return outBuffer.Length + 2;
        }
    }
}
