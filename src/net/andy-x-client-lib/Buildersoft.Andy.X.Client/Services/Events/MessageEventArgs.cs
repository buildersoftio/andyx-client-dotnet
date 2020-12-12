using System;
using System.Collections.Generic;
using System.Text;

namespace Buildersoft.Andy.X.Client.Services.Events
{
    public class MessageEventArgs
    {
        //
        // Summary:
        //     Gets the message data as a System.String.
        public object Data { get; set; }
        //
        // Summary:
        //     Gets a value indicating whether the message type is binary.
        public bool IsBinary { get; }
        //
        // Summary:
        //     Gets a value indicating whether the message type is ping.
        public bool IsPing { get; }
        //
        // Summary:
        //     Gets a value indicating whether the message type is text.
        public bool IsText { get; }
        //
        // Summary:
        //     Gets the message data as an array of System.Byte.
        public byte[] RawData { get; }

        public MessageEventArgs(object data)
        {
            Data = data;
        }
    }

}
