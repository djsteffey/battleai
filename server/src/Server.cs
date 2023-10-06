using common.network;
using server.log;

namespace server
{
    internal class Server
    {
        // variables
        private bool m_running;
        private ILogger m_logger;
        private ServerNetwork m_network;

        // methods
        public Server()
        {
            this.m_running = false;
            this.m_logger = null;
            this.m_network = null;
        }

        public void start()
        {
            // check running
            if (this.m_running)
            {
                return;
            }
            this.m_running = true;

            // logging
            this.m_logger = new ConsoleLogger();

            // network
            this.startNetwork();

            // loop
            this.m_logger.log("Server", "running main loop");
            while (this.m_running)
            {
                Thread.Sleep(250);
            }
        }

        public void stop()
        {
            // check running
            if (this.m_running == false)
            {
                return;
            }
            this.m_running = false;

            this.m_logger.log("Server", "stop");

            // network
            this.stopNetwork();
        }

        private bool startNetwork()
        {
            this.m_logger.log("Server", "startNetwork");
            this.m_network = new ServerNetwork(this.networkAuthenticateUsernamePassword, this.networkHandleNetworkMessage);
            return this.m_network.start();
        }

        private bool stopNetwork()
        {
            this.m_logger.log("Server", "stopNetwork");
            if (this.m_network != null)
            {
                return this.m_network.stop();
            }
            return true;
        }

        // NetworkAuthenticationProvider
        private bool networkAuthenticateUsernamePassword(string username, string password)
        {
            this.m_logger.log("Server", "networkAuthenticateUsernamePassword => " + username + " / " + password);

            // only allow bob
            if ((username == "bob") && (password == "pswd"))
            {
                return true;
            }
            return false;
        }

        // NetworkMessageHandler
        private void networkHandleNetworkMessage(NetworkMessage networkMessage)
        {
            this.m_logger.log("Server", "networkHandleNetworkMessage => " + networkMessage.SourceId + ": " + networkMessage.MessageType);

            switch (networkMessage.MessageType)
            {
                case NetworkMessage.EMessageType.DATA: { this.networkHandleNetworkMessageData((NetworkMessageData)networkMessage); } break;
                case NetworkMessage.EMessageType.ERROR: { this.networkHandleNetworkMessageData((NetworkMessageData)networkMessage); } break;
                case NetworkMessage.EMessageType.LOG_MESSAGE: { this.networkHandleNetworkMessageData((NetworkMessageData)networkMessage); } break;
                case NetworkMessage.EMessageType.STATUS_CHANGED: { this.networkHandleNetworkMessageData((NetworkMessageData)networkMessage); } break;
            }
        }

        private void networkHandleNetworkMessageData(NetworkMessageData networkMessage)
        {

        }

        private void networkHandleNetworkMessageError(NetworkMessageError networkMessage)
        {
            this.m_logger.log("Server", "networkHandleNetworkMessageError => " + networkMessage.ErrorMessage);
        }

        private void networkHandleNetworkMessageLogMessage(NetworkMessageLogMessage networkMessage)
        {
            this.m_logger.log("Server", "networkHandleNetworkMessageLogMessage => " + networkMessage.LogLevel + ": " + networkMessage.Message);
        }

        private void networkHandleNetworkMessageStatusChanged(NetworkMessageStatusChanged networkMessage)
        {
            this.m_logger.log("Server", "networkHandleNetworkMessageStatusChanged => " + networkMessage.SourceId + "=" + networkMessage.Status);
        }
    }
}
