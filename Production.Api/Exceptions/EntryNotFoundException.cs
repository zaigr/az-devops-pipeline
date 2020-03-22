using System;

namespace Production.Api.Exceptions
{
    public class EntryNotFoundException : Exception
    {
        public EntryNotFoundException(object value)
        {
            Value = value;
        }

        public object Value { get; }
    }
}
