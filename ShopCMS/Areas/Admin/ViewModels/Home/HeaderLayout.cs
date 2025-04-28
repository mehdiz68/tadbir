using System.Linq;
using Domain;
using System.Collections.Generic;

namespace ahmadi.Areas.Admin.ViewModel.AdminPanel
{
    public class HeaderLayout
    {
        public HeaderLayout()
        {
          
        }

        public ApplicationUser au { get; set; }
        public string RoleName { get; set; }
        public IEnumerable<Comment> Comments { get; set; }
        public IEnumerable<ContactUs> ContactUs { get; set; }
        public IEnumerable<FormRequest> FormRequests { get; set; }
        public IEnumerable<Ticket> Tickets { get; set; }
        public IEnumerable<ProductQuestion> ProductQuestions { get; set; }
        public IEnumerable<ProductComment> ProductComments { get; set; }
        //public IQueryable<Order> Orders { get; set; }
    }
}
