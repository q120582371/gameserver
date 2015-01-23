using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace shiwch.game.net
{
    class MessageHandler
    {
        public int HandleMessage(SocketAsyncEventArgs saea, DataToken dataToken, int remainingBytesToProcess)
        {
            if (dataToken.messageBytesDone == 0)
            {
                dataToken.byteArrayForMessage = new byte[dataToken.messageLength];
            }

            int nonCopiedBytes = 0;
            if (remainingBytesToProcess + dataToken.messageBytesDone >= dataToken.messageLength)
            {
                var copyedBytes = dataToken.RemainByte;
                nonCopiedBytes = remainingBytesToProcess - copyedBytes;
                Buffer.BlockCopy(saea.Buffer, dataToken.DataOffset, dataToken.byteArrayForMessage, dataToken.messageBytesDone, copyedBytes);
                dataToken.messageBytesDone = dataToken.messageLength;
                dataToken.bufferSkip += copyedBytes;
            }
            else
            {
                Buffer.BlockCopy(saea.Buffer, dataToken.DataOffset, dataToken.byteArrayForMessage, dataToken.messageBytesDone, remainingBytesToProcess);
                dataToken.messageBytesDone += remainingBytesToProcess;
            }

            return nonCopiedBytes;
        }
    }
}
