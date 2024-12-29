using Microsoft.AspNetCore.Identity;

namespace WhatsApp_Clone.Tables
{
    public class User:IdentityUser
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string About { get; set; }

        public string ImageURL { get; set; }
        public ICollection<React>? Reacts { get; set; }  
        public ICollection<GroupUser>? UserGroups { get; set; }  


    }
}
