namespace Account.Api.Models
{
    public class CommandResponse<T> : CommandResponse
    {
        public T Data { get; set; }

        public CommandResponse()
        {
            Data = default(T);
        }
    }
    public class CommandResponse
    {
        public bool Success { get; set; } = true;

        public CommandMessage Message { get; set; }
    }
}
