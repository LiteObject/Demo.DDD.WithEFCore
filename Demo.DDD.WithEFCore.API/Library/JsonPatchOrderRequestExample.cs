namespace Demo.DDD.WithEFCore.API.Library
{
    public class JsonPatchOrderRequestExample
    {
        public Operation[] GetExamples()
        {
            return new[]
            {
                new Operation
                {
                    Op = "replace",
                    Path = "/name",
                    Value = ""
                },
                new Operation
                {
                    Op = "replace",
                    Path = "/email",
                    Value = "test@email.com"
                }
            };
        }
    }
}
