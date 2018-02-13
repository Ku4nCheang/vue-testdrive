namespace netcore.Core.ErrorDescribers
{
    public class AuthErrorDescriber: BaseDescriber 
    {
        public AuthErrorDescriber() 
        {
        }

        public virtual Error UserNotFound() 
        {
            return new Error() { Code = nameof(UserNotFound), Description = $"User not found." };
        }

        public virtual Error IncorrectPassword() 
        {
            return new Error() { Code = nameof(IncorrectPassword), Description = $"Incorrect password." };
        }
    }
}