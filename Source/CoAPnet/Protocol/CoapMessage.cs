using CoAPnet.Protocol.Options;
using System;
using System.Collections.Generic;

namespace CoAPnet.Protocol
{
    public sealed class CoapMessage
    {
        public CoapMessageType Type
        {
            get; set;
        }

        public byte[] Token
        {
            get; set;
        }

        public CoapMessageCode Code
        {
            get; set;
        }

        public ushort Id
        {
            get; set;
        }

        public List<CoapMessageOption> Options
        {
            get; set;
        }

        public ArraySegment<byte> Payload
        {
            get; set;
        }


        public uint Interval { get; set; } = 10;
        public CoapBlockSizeType BlockSizeType { get; set; } = CoapBlockSizeType.BLOCK_SIZE_128;
        public uint BlockSize
        {
            get
            {
                uint[] blocks = { 16, 32, 64, 128, 256, 512, 1024 };

                return blocks[(int)BlockSizeType];
            }
        }


        public uint BlockIndex
        {
            get; set;
        } = 0;

        public uint BlockNumber
        {
            get
            {
                return Convert.ToUInt32((Payload.Count + BlockSize - 1) / BlockSize);
            }
        }

        public ArraySegment<byte> BlockPayload
        {
            get
            {
                ArraySegment<byte> blockPayload;
                if (Payload == null || Payload.Count <= 0)
                {
                    blockPayload = new ArraySegment<byte>();
                    return blockPayload;
                }
                
                int blockSize;

                if(BlockIndex > BlockNumber){
                    blockPayload = new ArraySegment<byte>();
                    return blockPayload;
                }
                int BlockOffset = Convert.ToInt32(BlockSize * BlockIndex);
                if (BlockOffset + BlockSize <= Payload.Count)
                {
                    blockSize = (int)BlockSize;
                }
                else
                {
                    blockSize = Payload.Count - BlockOffset;
                }

                blockPayload = new ArraySegment<byte>(Payload.Array, BlockOffset, blockSize);

                return blockPayload;
            }
        }

        public bool MoreBlock
        {
            get
            {
                return (BlockIndex+1) < BlockNumber;
            }
        }
    }


    public enum CoapBlockSizeType : int
    {
        BLOCK_SIZE_16 = 0,
        BLOCK_SIZE_32,
        BLOCK_SIZE_64,
        BLOCK_SIZE_128, 
        BLOCK_SIZE_256,
        BLOCK_SIZE_512,
        BLOCK_SIZE_1024,
    }
}