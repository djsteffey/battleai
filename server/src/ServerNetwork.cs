using common.network;
using Lidgren.Network;

namespace server
{
    // delegates
    public delegate bool NetworkAuthenticationProvider(string username, string password);

    // server network
    internal class ServerNetwork : Network
    {
        // variables
        private NetworkAuthenticationProvider m_networkAuthenticationProvider;

        // methods
        public ServerNetwork(NetworkAuthenticationProvider networkAuthenticationProvider, NetworkMessageHandler networkMessageHandler) : base(networkMessageHandler)
        {
            this.m_networkAuthenticationProvider = networkAuthenticationProvider;
        }

        protected override void networkHandleConnectionApproval(NetIncomingMessage nim)
        {
            // get username and password
            string username = nim.ReadString();
            string password = nim.ReadString();

            // ask the authenticator
            bool success = this.m_networkAuthenticationProvider(username, password);

            // accept or deny
            if (success)
            {
                nim.SenderConnection.Approve();
            }
            else
            {
                nim.SenderConnection.Deny("invalid credentials");
            }
        }

        protected override NetPeer createNetPeer()
        {
            NetPeerConfiguration config = new NetPeerConfiguration("battleai");
            config.Port = 12345;
            config.EnableMessageType(NetIncomingMessageType.ConnectionApproval);
            NetServer netServer = new NetServer(config);
            return netServer;
        }
    }
}
