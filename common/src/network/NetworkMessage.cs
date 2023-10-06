using System.ComponentModel;

namespace common.network
{
    public class NetworkMessage
    {
        public enum EMessageType
        {
            STATUS_CHANGED, ERROR, LOG_MESSAGE, DATA
        }

        // variables
        private long m_sourceId;
        private EMessageType m_messageType;

        // proeprties
        public long SourceId
        {
            get { return this.m_sourceId; }
        }
        public EMessageType MessageType
        {
            get { return m_messageType; }
        }

        // methods
        public NetworkMessage(long sourceId, EMessageType messageType)
        {
            this.m_sourceId = sourceId;
            this.m_messageType = messageType;
        }
    }

    public class NetworkMessageStatusChanged : NetworkMessage
    {
        // enum
        public enum EStatus
        {
            CONNECTED, DISCONNECTED
        }

        // variables
        private EStatus m_status;

        // properties
        public EStatus Status
        {
            get { return this.m_status; }
        }

        // methods
        public NetworkMessageStatusChanged(long sourceId, EStatus status) : base(sourceId, EMessageType.STATUS_CHANGED)
        {
            this.m_status = status;
        }
    }

    public class NetworkMessageError : NetworkMessage
    {
        // variables
        private string m_errorMessage;

        // properties
        public string ErrorMessage
        {
            get { return this.m_errorMessage; }
        }

        // methods
        public NetworkMessageError(long sourceId, string errorMessage) : base(sourceId, EMessageType.ERROR)
        {
            this.m_errorMessage = errorMessage;
        }
    }

    public class NetworkMessageLogMessage : NetworkMessage
    {
        // enum
        public enum ELogLevel
        {
            ERROR, WARNING, DEBUG, VERBOSE_DEBUG
        }

        // variables
        private ELogLevel m_logLevel;
        private string m_msg;

        // properties
        public ELogLevel LogLevel
        {
            get { return this.m_logLevel; }
        }
        public string Message
        {
            get { return this.m_msg; }
        }

        // methods
        public NetworkMessageLogMessage(long sourceId, ELogLevel logLevel, string msg) : base(sourceId, EMessageType.LOG_MESSAGE)
        {
            this.m_logLevel = logLevel;
            this.m_msg = msg;
        }
    }

    public class NetworkMessageData : NetworkMessage
    {
        // variables
        private byte[] m_bytes;

        // properties
        public byte[] Bytes
        {
            get { return this.m_bytes; }
        }

        // methods
        public NetworkMessageData(long sourceId, byte[] bytes) : base(sourceId, EMessageType.DATA)
        {
            this.m_bytes = bytes;
        }
    }
}
