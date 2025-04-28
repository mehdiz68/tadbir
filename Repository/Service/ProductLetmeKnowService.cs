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
    public class ProductLetmeKnowService : GenericRepository<ProductLetmeknow>
    {
        private readonly ProductService _productService;
        public ProductLetmeKnowService(ahmadiDbContext context, ProductService productService) : base(context)
        {
            this._productService = productService;
        }

        /// <summary>
        /// نمایش لیست به من اطلاع بده ی کاربر
        /// </summary>
        /// <param name="userid">کد کاربر</param>
        /// <returns></returns>
        public IQueryable<ProductletmeKnowViewModel> GetList(string userid)
        {

            return GetByReturnQueryable(x => new ProductletmeKnowViewModel()
            {
                Id = x.Id,
                ProductItem = _productService.ProductItem(s => s.Id == x.ProductId),
                AmazingOffer = x.AmazingOffer,
                Available = x.Available,
                InsertDate = x.InsertDate,
                NotificationType = x.NotificationType
            }, x => x.UserId == userid && (x.Notofied == false || x.NotofiedEmail == false || x.NotofiedSms == false));
        }

        /// <summary>
        /// متد بررسی موجود بودن محصول در لیست به من اطلاع بده
        /// </summary>
        /// <param name="productid">کد محصول</param>
        /// <param name="userid">کد کاربر</param>
        /// <returns></returns>

        public int? checkUserProductletmeKnow(int productid, string userid)
        {
            return Get(x => x.Id, x => x.ProductId == productid && x.UserId == userid, null, "", 0, 0, true).FirstOrDefault();

        }
        /// <summary>
        /// حذف به من اطلاع بده ی کاربر
        /// </summary>
        /// <param name="id">ردیف به من اطلاع بده</param>
        /// <returns></returns>
        public async Task removeLetmeKnow(int id)
        {
            Delete(dbSet.Find(id));
            await context.SaveChangesAsync();
        }
        /// <summary>
        /// افزودن محصوله به لیست به من اطلاع بده ی کاربر
        /// </summary>
        /// <param name="productId">کد محصول</param>
        /// <param name="userid">کد کاربر</param>
        /// <returns></returns>
        public async Task addLetmeKnow(int productId, string userid, bool amazingoffer, bool available, short notificationType)
        {
            var plmn = Get(x => x, x => x.ProductId == productId && x.UserId == userid).FirstOrDefault();
            if (plmn == null)
            {
                Insert(new ProductLetmeknow()
                {
                    InsertDate = DateTime.Now,
                    ProductId = productId,
                    UserId = userid,
                    AmazingOffer = amazingoffer,
                    Available = available,
                    NotificationType = notificationType
                });
            }
            else
            {
                plmn.InsertDate = DateTime.Now;
                plmn.AmazingOffer = amazingoffer;
                plmn.Available = available;
                plmn.NotificationType = notificationType;
                Update(plmn);
            }



            await context.SaveChangesAsync();
        }

        public async Task<bool> CheckLetmeKnowsOfProduct(int productId)
        {
            return await context.ProductLetmeknows.AnyAsync(x => x.ProductId == productId);
        }
        public async Task<List<Domain.ProductLetmeknow>> GetLetmeKnowsOfProduct(int productId)
        {
            return await context.ProductLetmeknows.Where(x => x.ProductId == productId).ToListAsync();
        }
        public async Task removeNotifications(int productId)
        {
           
        }
    }
}
