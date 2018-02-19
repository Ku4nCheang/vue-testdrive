namespace netcore.Core.ErrorDescribers
{
    public class UserErrorDescriber: BaseDescriber 
    {
        public UserErrorDescriber() 
        {
        }

        public Error UnauthorizedUserAccess() 
        {
            return new Error() { Code = nameof(UnauthorizedUserAccess), Description = $"Unauthorizred user access." };
        }

        public Error UserNotFound() 
        {
            return new Error() { Code = nameof(UserNotFound), Description = $"User not found." };
        }

        public Error UserAlreadyDeactivated() 
        {
            return new Error() { Code = nameof(UserAlreadyDeactivated), Description = $"User was already deactivated." };
        }
    }
}