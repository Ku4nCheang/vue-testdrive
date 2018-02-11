namespace netcore.Core.ErrorDescribers
{
    public class BaseDescriber 
    {
        public BaseDescriber() 
        {
        }

        public virtual Error InvalidModelError() 
        {
            return new Error() { Code = nameof(InvalidModelError), Description = $"Invalid model" };
        }

        public virtual Error DefaultError() 
        {
            return new Error() { Code = nameof(DefaultError), Description = $"An error occurred" };
        }
    }
}