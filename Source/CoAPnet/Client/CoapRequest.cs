using CoAPnet.Protocol;
using System;

namespace CoAPnet.Client
{
    public class CoapRequest
    {
        public CoapRequestMethod Method { get; set; } = CoapRequestMethod.Get;

        public CoapRequestOptions Options { get; set; } = new CoapRequestOptions();

        public ArraySegment<byte> Payload { get; set; }

        public CoapMessageToken Token { get; set; } = null;

        public CoapBlockSizeType BlockSize { get; set; } = CoapBlockSizeType.BLOCK_SIZE_128;

        public CoapMessageType Type { get; set; } = CoapMessageType.Confirmable;

        public uint Interval { get; set; } = 10;

        public uint RetransmissionCount { get; set; } = 3;
    }
}

