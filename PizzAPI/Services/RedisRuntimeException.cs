namespace PizzAPI.Services
{
    public class RedisRuntimeException : Exception
    {
        public RedisRuntimeException() { }

        public RedisRuntimeException(string message)
            : base(message) { }

        public RedisRuntimeException(string message, Exception inner)
            : base(message, inner) { }
    }
}
