using Lidgren.Network;

namespace common.network
{
    internal class ClientNetwork : Network
    {
        // variables


        // methods
        public ClientNetwork(NetworkMessageHandler networkMessageHandler) : base(networkMessageHandler)
        {

        }

        public void connectAsync()
        {

        }

        protected override NetPeer createNetPeer()
        {
            NetPeerConfiguration config = new NetPeerConfiguration("battleai");
            NetClient netClient = new NetClient(config);
            return netClient;
        }
    }
}
