using Lidgren.Network;
using System.Threading;

namespace common.network
{
    public abstract class Network
    {
        // delegate
        public delegate void NetworkMessageHandler(NetworkMessage networkMessage);

        // variables
        private bool m_running;
        protected NetPeer m_netPeer;
        private Thread m_networkThread;
        private NetworkMessageHandler m_networkMessageHandler;

        // methods
        public Network(NetworkMessageHandler networkMessageHandler)
        {
            this.m_running = false;
            this.m_netPeer = null;
            this.m_networkThread = null;
            this.m_networkMessageHandler = networkMessageHandler;
        }

        public bool start()
        {
            // check running
            if (this.m_running)
            {
                return false;
            }
            this.m_running = true;

            // create the netpeer
            this.m_netPeer = this.createNetPeer();
            this.m_netPeer.Start();

            // run the async thread
            this.m_networkThread = new Thread(this.networkThreadMethod);
            this.m_networkThread.Start();

            // done
            return true;
        }

        public bool stop()
        {
            // check running
            if (this.m_running == false)
            {
                return false;
            }
            this.m_running = false;

            // stop the net peer
            if (this.m_netPeer != null)
            {
                this.m_netPeer.Shutdown("shutting down");
            }
            this.m_netPeer = null;

            // get the thread back
            if (this.m_networkThread != null)
            {
                this.m_networkThread.Join(3000);
            }
            this.m_networkThread = null;

            // done
            return true;
        }

        private void networkThreadMethod()
        {
            // loop while running
            while (this.m_running)
            {
                // wait briefly for a nim
                NetIncomingMessage nim = this.m_netPeer.WaitMessage(250);

                // check
                if (nim != null)
                {
                    // convert to a network message
                    switch (nim.MessageType)
                    {
                        case NetIncomingMessageType.ConnectionApproval: { this.networkHandleConnectionApproval(nim); break; }
                        case NetIncomingMessageType.ConnectionLatencyUpdated: { this.networkHandleConnectionLatencyUpdated(nim); break; }
                        case NetIncomingMessageType.Data: { this.networkHandleData(nim); break; }
                        case NetIncomingMessageType.DebugMessage: { this.networkHandleDebugMessage(nim); break; }
                        case NetIncomingMessageType.DiscoveryRequest: { this.networkHandleDiscoveryRequest(nim); break; }
                        case NetIncomingMessageType.DiscoveryResponse: { this.networkHandleDiscoveryResponse(nim); break; }
                        case NetIncomingMessageType.Error: { this.networkHandleError(nim); break; }
                        case NetIncomingMessageType.ErrorMessage: { this.networkHandleErrorMessage(nim); break; }
                        case NetIncomingMessageType.NatIntroductionSuccess: { this.networkHandleNatIntroductionSuccess(nim); break; }
                        case NetIncomingMessageType.Receipt: { this.networkHandleReceipt(nim); break; }
                        case NetIncomingMessageType.StatusChanged: { this.networkHandleStatusChanged(nim); break; }
                        case NetIncomingMessageType.UnconnectedData: { this.networkHandleUnconnectedData(nim); break; }
                        case NetIncomingMessageType.VerboseDebugMessage: { this.networkHandleVerboseDebugMessage(nim); break; }
                        case NetIncomingMessageType.WarningMessage: { this.networkHandleWarningMessage(nim); break; }
                        default: { } break;
                    }
                }
            }
        }

        protected virtual void networkHandleConnectionApproval(NetIncomingMessage nim)
        {
            // nothing
            // should be overridden in the server to auth the client
        }

        private void networkHandleConnectionLatencyUpdated(NetIncomingMessage nim)
        {
            // nothing
        }

        private void networkHandleData(NetIncomingMessage nim)
        {
            int numBytes = nim.ReadInt32();
            byte[] bytes = nim.ReadBytes(numBytes);
            NetworkMessage networkMessage = new NetworkMessageData(nim.SenderConnection.RemoteUniqueIdentifier, bytes);
            this.m_networkMessageHandler(networkMessage);
        }

        private void networkHandleDebugMessage(NetIncomingMessage nim)
        {
            string msg = nim.ReadString();
            NetworkMessageLogMessage networkMessage = new NetworkMessageLogMessage(nim.SenderConnection.RemoteUniqueIdentifier, NetworkMessageLogMessage.ELogLevel.DEBUG, msg);
            this.m_networkMessageHandler(networkMessage);
        }

        private void networkHandleDiscoveryRequest(NetIncomingMessage nim)
        {
            // nothing
        }

        private void networkHandleDiscoveryResponse(NetIncomingMessage nim)
        {
            // nothing
        }

        private void networkHandleError(NetIncomingMessage nim)
        {
            string errorMessage = nim.ReadString();
            NetworkMessageError networkMessage = new NetworkMessageError(nim.SenderConnection.RemoteUniqueIdentifier, errorMessage);
            this.m_networkMessageHandler(networkMessage);
        }

        private void networkHandleErrorMessage(NetIncomingMessage nim)
        {
            string msg = nim.ReadString();
            NetworkMessageLogMessage networkMessage = new NetworkMessageLogMessage(nim.SenderConnection.RemoteUniqueIdentifier, NetworkMessageLogMessage.ELogLevel.ERROR, msg);
            this.m_networkMessageHandler(networkMessage);
        }

        private void networkHandleNatIntroductionSuccess(NetIncomingMessage nim)
        {
            // nothing
        }

        private void networkHandleReceipt(NetIncomingMessage nim)
        {
            // nothing
        }

        private void networkHandleStatusChanged(NetIncomingMessage nim)
        {
            switch (nim.SenderConnection.Status)
            {
                case NetConnectionStatus.Connected:
                    {
                        NetworkMessageStatusChanged networkMessage = new NetworkMessageStatusChanged(nim.SenderConnection.RemoteUniqueIdentifier, NetworkMessageStatusChanged.EStatus.CONNECTED);
                        this.m_networkMessageHandler(networkMessage);
                    } break;
                case NetConnectionStatus.Disconnected:
                    {
                        NetworkMessageStatusChanged networkMessage = new NetworkMessageStatusChanged(nim.SenderConnection.RemoteUniqueIdentifier, NetworkMessageStatusChanged.EStatus.DISCONNECTED);
                        this.m_networkMessageHandler(networkMessage);
                    } break;
                case NetConnectionStatus.Disconnecting: break;
                case NetConnectionStatus.InitiatedConnect: break;
                case NetConnectionStatus.None: break;
                case NetConnectionStatus.ReceivedInitiation: break;
                case NetConnectionStatus.RespondedAwaitingApproval: break;
                case NetConnectionStatus.RespondedConnect: break;
            }
        }

        private void networkHandleUnconnectedData(NetIncomingMessage nim)
        {
            // nothing
        }

        private void networkHandleVerboseDebugMessage(NetIncomingMessage nim)
        {
            string msg = nim.ReadString();
            NetworkMessageLogMessage networkMessage = new NetworkMessageLogMessage(nim.SenderConnection.RemoteUniqueIdentifier, NetworkMessageLogMessage.ELogLevel.VERBOSE_DEBUG, msg);
            this.m_networkMessageHandler(networkMessage);
        }

        private void networkHandleWarningMessage(NetIncomingMessage nim)
        {
            string msg = nim.ReadString();
            NetworkMessageLogMessage networkMessage = new NetworkMessageLogMessage(nim.SenderConnection.RemoteUniqueIdentifier, NetworkMessageLogMessage.ELogLevel.WARNING, msg);
            this.m_networkMessageHandler(networkMessage);
        }

        protected abstract NetPeer createNetPeer();
    }
}
