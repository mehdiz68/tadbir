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
    public class ProductCommentService : GenericRepository<ProductComment>
    {
        private readonly ProductService _productService;
        public ProductCommentService(ahmadiDbContext context, ProductService productService) : base(context)
        {
            this._productService = productService;
        }

        public IEnumerable<ProductItemRanks> GetNoUserCommenProductsRanks(Guid id, string userid,out int count)
        {
            var products = context.OrderRows.Include("ProductComments").Include("Order.OrderWallets.Wallet").Where(x => !x.Product.ProductComments.Any(s => s.UserId == userid) && x.OrderId == id && x.Order.UserId == userid && x.Order.OrderWallets.Any(s => s.Wallet.State == true) && x.Order.IsActive).AsNoTracking().Select(x => x.ProductId).Distinct();
            count = products.Count();
            return _productService.ProductItemListRanks(x => products.Contains(x.Id), x => x.OrderByDescending(s => s.Id), 0, 1);
        }
        public IEnumerable<ProductItem> GetNoUserCommenProducts(string userid)
        {
            var products = context.OrderRows.Include("ProductComments").Include("Order.OrderWallets.Wallet").Where(x => !x.Product.ProductComments.Any(s => s.UserId == userid) && x.Order.UserId == userid && x.Order.OrderWallets.Any(s => s.Wallet.State == true) && x.Order.IsActive).AsNoTracking().Select(x => x.ProductId).Distinct();
            return _productService.ProductItemList(x => products.Contains(x.Id), x => x.OrderByDescending(s => s.Id));
        }

        /// <summary>
        /// نمایش لیست نظرات کاربر
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public IQueryable<ProductCommentViewModel> GetUserComments(string userid)
        {

            return GetByReturnQueryable(x => new ProductCommentViewModel()
            {
                Id = x.Id,
                ProductItem = _productService.ProductItem(s => s.Id == x.ProductId),
                IsActive = x.IsActive,
                IsBuy = x.IsBuy,
                Text = x.Text,
                Title = x.Title,
                Useful = x.Useful,
                AvgPoint = x.ProductRankSelectValues.Any() ? (x.ProductRankSelectValues.Sum(s => s.Value) / x.ProductRankSelectValues.Count()) : 0,
                InsertDate = x.InsertDate,
                Satisfaction = x.Satisfaction
            }, x => x.UserId == userid, null, "ProductRankSelectValues"); ;
        }
        /// <summary>
        /// نمایش لیست محصولات در صفحه محصول تب نظرات
        /// </summary>
        /// <param name="productid"></param>
        /// <returns></returns>
        public IQueryable<ProductCommentList> GetProductComments(int productid)
        {
            return GetByReturnQueryable(x => new ProductCommentList
            {
                Id = x.Id,
                FullName = x.User.FirstName,
                InsertDate = CoreLib.Infrastructure.DateTime.DateTimeConverter.ChangeMiladiToLongShamsi(x.InsertDate),
                InsertTime = x.InsertDate.ToShortTimeString(),
                Text = x.Text,
                Title = x.Title,
                UserAvatar = x.User.Avatar,
                UserAvatarattachment = x.User.Avatar.HasValue ? x.User.Avatarattachment.FileName : null,
                UserGender = x.User.Gender,
                IsBuy = x.IsBuy,
                Useful = x.Useful,
                ProductCommentSatisfaction = x.Satisfaction.HasValue ? x.Satisfaction : null,
                ProductRankSelectValues = x.ProductRankSelectValues,
                attachments = x.attachments,
                ProductAdvantages = x.ProductCommentAdvantages.Any() ? x.ProductCommentAdvantages.Select(s => s.Title) : null,
                ProductCommentDisAdvantages = x.ProductCommentDisAdvantages.Any() ? x.ProductCommentDisAdvantages.Select(s => s.Title) : null,
                UnUseful = x.Unuseful


            }, x => x.ProductId == productid && x.IsActive, null, "User,User.Avatarattachment,ProductRankSelectValues,attachments,ProductCommentAdvantages,ProductCommentDisAdvantages");
        }

        public bool SetUsefulcomment(int commentid)
        {
            var pc = Get(x => x, x => x.Id == commentid).FirstOrDefault();
            if (pc != null)
            {
                pc.Useful++;
                Update(pc);
                return true;
            }
            else
                return false;
        }
        public bool SetUnusefulcomment(int commentid)
        {
            var pc = Get(x => x, x => x.Id == commentid).FirstOrDefault();
            if (pc != null)
            {
                pc.Unuseful++;
                Update(pc);
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// متد بررسی موجود بودن نظر برای کاربر 
        /// </summary>
        /// <param name="productid">کد محصول</param>
        /// <param name="userid">کد کاربر</param>
        /// <returns></returns>

        public int? checkUserProductComment(int productcommentid, string userid)
        {
            return Get(x => x.Id, x => x.Id == productcommentid && x.UserId == userid, null, "", 0, 0, true).FirstOrDefault();

        }
        /// <summary>
        /// حذف  کاربر
        /// </summary>
        /// <param name="id">ردیف نظر </param>
        /// <returns></returns>
        public async Task removeProductComment(int id)
        {
            Delete(dbSet.Find(id));
            await context.SaveChangesAsync();
        }


    }
}
