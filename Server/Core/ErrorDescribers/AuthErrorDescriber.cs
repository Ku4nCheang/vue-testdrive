namespace netcore.Core.ErrorDescribers
{
    public class AuthErrorDescriber: BaseDescriber 
    {
        public AuthErrorDescriber() 
        {
        }

        public virtual Error AccountNotFound() 
        {
            return new Error() { Code = nameof(AccountNotFound), Description = $"Account not found." };
        }

        public virtual Error AccountAlreadyDeactivated() 
        {
            return new Error() { Code = nameof(AccountAlreadyDeactivated), Description = $"Account was already deactivated." };
        }

        public virtual Error IncorrectPassword() 
        {
            return new Error() { Code = nameof(IncorrectPassword), Description = $"Incorrect password." };
        }
    }
}