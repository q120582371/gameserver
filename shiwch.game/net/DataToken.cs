using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Text;

namespace shiwch.game.net
{
    class DataToken
    {
        public byte[] byteArrayForMessage;
        public byte[] byteArrayForPrefix = new byte[4];
        public int messageBytesDone;
        public int prefixBytesDone;
        public int messageLength;
        public int bufferOffset;
        public int bufferSkip;

        public int DataOffset
        {
            get { return bufferOffset + bufferSkip; }
        }
        public int RemainByte
        {
            get { return messageLength - messageBytesDone; }
        }
        public bool IsMessageReady
        {
            get { return messageBytesDone == messageLength; }
        }

        public void Reset(bool skip)
        {
            byteArrayForMessage = null;
            byteArrayForPrefix[0] = 0;
            byteArrayForPrefix[1] = 0;
            byteArrayForPrefix[2] = 0;
            byteArrayForPrefix[3] = 0;
            messageBytesDone = 0;
            prefixBytesDone = 0;
            messageLength = 0;
            if (skip)
                bufferSkip = 0;
        }
    }
}
