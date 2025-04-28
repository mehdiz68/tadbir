using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Domain;

namespace ahmadi.ViewModels
{
    public class ShowSideBarProfileViewModel
    {
        public System.Guid? Avatar { get; set; }
        public Domain.attachment Avatarattachment { get; set; }
        public ApplicationUser AppUser { get; set; }
        public int MessageCout { get; set; }
    }
}