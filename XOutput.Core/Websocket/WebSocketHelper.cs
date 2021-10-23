using System;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace XOutput.Websocket
{
    public class WebSocketHelper
    {
        public WebSocketHelper()
        {

        }

        public async Task<string> ReadStringAsync(System.Net.WebSockets.WebSocket websocket, Encoding encoding, CancellationToken cancellationToken)
        {
            var data = await ReadBytesAsync(websocket, cancellationToken);
            if (data == null)
            {
                return null;
            }
            return encoding.GetString(data);
        }

        public async Task<byte[]> ReadBytesAsync(System.Net.WebSockets.WebSocket websocket, CancellationToken cancellationToken)
        {
            WebSocketReceiveResult result;
            var buffer = new ArraySegment<byte>(new byte[8192]);
            int length = 0;
            do
            {
                result = await websocket.ReceiveAsync(buffer, cancellationToken);
                if (result.CloseStatus != null)
                {
                    return null;
                }
                length += result.Count;
            }
            while (!result.EndOfMessage);

            return buffer.Array.Take(length).ToArray();
        }

        public Task SendStringAsync(System.Net.WebSockets.WebSocket websocket, string message, Encoding encoding, CancellationToken cancellationToken)
        {
            return SendBytesAsync(websocket, encoding.GetBytes(message), cancellationToken);
        }

        public async Task SendBytesAsync(System.Net.WebSockets.WebSocket websocket, byte[] data, CancellationToken cancellationToken)
        {
            if (websocket.CloseStatus == null)
            {
                ArraySegment<byte> buffer = new ArraySegment<byte>(data);
                await websocket.SendAsync(buffer, WebSocketMessageType.Text, true, cancellationToken);
            }
        }
    }
}
