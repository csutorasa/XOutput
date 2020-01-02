using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XOutput.Logging;

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

        private Thread notifyThread;
        private readonly Mutex mutex = new Mutex(false, MutexName);
        private CancellationTokenSource source;

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
            Close();
        }

        public void StartNamedPipe()
        {
            notifyThread = ThreadHelper.CreateAndStart(new ThreadStartParameters
            {
                Name = "XOutputRunningAlreadyNamedPipe reader",
                IsBackground = true,
                Task = async () => await ReadPipe(),
            });
        }

        public void StopNamedPipe()
        {
            if (notifyThread != null)
            {
                source?.Cancel();
                ThreadHelper.StopAndWait(notifyThread);
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

        private async Task ReadPipe()
        {
            source = new CancellationTokenSource();
            while (!source.IsCancellationRequested)
            {
                using (var notifyWait = new NamedPipeServerStream(PipeName, PipeDirection.InOut, 1))
                {
                    await Task.Run(() => notifyWait.WaitForConnection(), source.Token);
                    try
                    {
                        StreamString ss = new StreamString(notifyWait);
                        string command = ss.ReadString();
                        ss.WriteString(ProcessCommand(command));
                    }
                    catch (IOException e)
                    {
                        logger.Error(e);
                    }
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
        private UnicodeEncoding streamEncoding;

        public StreamString(Stream ioStream)
        {
            this.ioStream = ioStream;
            streamEncoding = new UnicodeEncoding();
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
            if (len > UInt16.MaxValue)
            {
                len = (int)UInt16.MaxValue;
            }
            ioStream.WriteByte((byte)(len / 256));
            ioStream.WriteByte((byte)(len & 255));
            ioStream.Write(outBuffer, 0, len);
            ioStream.Flush();

            return outBuffer.Length + 2;
        }
    }
}
