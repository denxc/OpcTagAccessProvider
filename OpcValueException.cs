namespace OpcTagAccessProvider
{
    using System;

    public class OpcValueException : Exception
    {
        public OpcValueException(string aMessage) : base(aMessage)
        {
        }
    }
}
