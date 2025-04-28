using DataLayer;
using Domain;
using Domain.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Repository.Service
{
    public class ProductQuestionService : GenericRepository<ProductQuestion>
    {
        private readonly ProductService _productService;
        public ProductQuestionService(ahmadiDbContext context, ProductService productService) : base(context)
        {
            this._productService = productService;
        }



        /// <summary>
        /// نمایش لیست پرسش و پاسخ ها در صفحه محصول تب نظرات
        /// </summary>
        /// <param name="productid"></param>
        /// <returns></returns>
        public IQueryable<ProductFAQList> GetProductFAQ(int productid)
        {
            return GetByReturnQueryable(x => new ProductFAQList
            {
                Id = x.Id,
                FullName = !String.IsNullOrEmpty(x.FakeUserFullName) ? x.FakeUserFullName : x.User.FirstName,
                InsertDate = CoreLib.Infrastructure.DateTime.DateTimeConverter.ChangeMiladiToLongShamsi(x.InsertDate),
                InsertTime = x.InsertDate.ToShortTimeString(),
                Text = x.Message,
                UserAvatar = x.User.Avatar,
                UserGender = x.User.Gender,
                like = x.Like,
                unlike = x.UnLike,
                ChildComment = x.ChildComment,
                UserId = x.UserId,
                parrentid = x.ParrentId


            }, x => x.ProductId == productid && x.IsActive, x => x.OrderByDescending(s => s.Id), "ChildComment.attachments,User");
        }


        public IEnumerable<ProductFAQGoogleList> GetProductGoogleFAQ(int productid)
        {
            return Get(x => new ProductFAQGoogleList
            {
                @type = "Question",
                name = x.Message,
                acceptedAnswer = new acceptedAnswer { type = "Answer", text = x.ChildComment.Any() ? x.ChildComment.First().Message : "----" }


            }, x => x.ParrentId == null && x.ProductId == productid && x.IsActive && x.ChildComment.Any(), x => x.OrderByDescending(s => s.Id), "ChildComment,User", 0, 5);
        }
     

        public bool SetUsefulQuestion(int commentid)
        {
            var pc = Get(x => x, x => x.Id == commentid).FirstOrDefault();
            if (pc != null)
            {
                pc.Like++;
                Update(pc);
                return true;
            }
            else
                return false;
        }
        public bool SetUnusefulQuestion(int commentid)
        {
            var pc = Get(x => x, x => x.Id == commentid).FirstOrDefault();
            if (pc != null)
            {
                pc.UnLike++;
                Update(pc);
                return true;
            }
            else
                return false;
        }




    }
}
