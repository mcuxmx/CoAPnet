using CoAPnet.Protocol;

namespace CoAPnet.Client
{
    public sealed class CoapResponse
    {
        public CoapResponseStatusCode StatusCode
        {
            get; set;
        }

        public CoapResponseOptions Options
        {
            get; set;
        }

        public byte[] Payload
        {
            get; set;
        }

        public CoapMessage Message{ get; set; }
    }
}

