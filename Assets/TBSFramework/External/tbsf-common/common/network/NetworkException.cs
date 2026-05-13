using System;

namespace TurnBasedStrategyFramework.Common.Network
{
    public class NetworkException : Exception
    {
        public NetworkException(string message) : base(message)
        {
        }
    }
}