using System;
using System.IO;
using System.Net;

namespace Fantasy.Core.Network
{
    public abstract class ANetworkChannel
    {
        public uint Id { get; private set; }
        public Scene Scene { get; protected set; }
        public long NetworkId { get; private set; }
        public bool IsDisposed { get; protected set; }
        public EndPoint RemoteEndPoint { get; protected set; }
        public APacketParser PacketParser { get; protected set; }

        public abstract event Action OnDispose;
        public abstract event Action<APackInfo> OnReceiveMemoryStream;

        protected ANetworkChannel(Scene scene, uint id, long networkId)
        {
            Id = id;
            Scene = scene;
            NetworkId = networkId;
        }

        public virtual void Dispose()
        {
            NetworkThread.Instance.RemoveChannel(NetworkId, Id);
            
            Id = 0;
            Scene = null;
            NetworkId = 0;
            IsDisposed = true;
            RemoteEndPoint = null;

            if (PacketParser != null)
            {
                PacketParser.Dispose();
                PacketParser = null;
            }
        }
    }
}