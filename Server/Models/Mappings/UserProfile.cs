using AutoMapper;
using netcore.Models;
using netcore.Models.ViewModels;
using netcore.Models.ViewModels.SharedViewModels;

namespace netcore.Models.Mappings
{
    public class UserProfile: Profile
    {
        public UserProfile()
        {
            CreateMap<User, UserViewModel>();
        }
    }
}