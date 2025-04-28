using DataLayer;
using Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Repository.Service
{
    public class ProductFavorateService : GenericRepository<ProductFavorate>
    {
        public ProductFavorateService(ahmadiDbContext context) : base(context)
        {

        }
        /// <summary>
        /// متد بررسی موجود بودن محصول در لیست مورد علاقه کاربر
        /// </summary>
        /// <param name="productid">کد محصول</param>
        /// <param name="userid">کد کاربر</param>
        /// <returns></returns>

        public int? checkUserProductFavorate(int productid,string userid)
        {
            return  Get(x => x.Id, x => x.ProductId == productid && x.UserId == userid,null,"",0,0,true).FirstOrDefault();

        }
        /// <summary>
        /// حذف مورد علاقه کاربر
        /// </summary>
        /// <param name="id">ردیف مورد علاقه کاربر</param>
        /// <returns></returns>
        public async Task removeFavorate(int id)
        {
            Delete(dbSet.Find(id));
            await context.SaveChangesAsync();
        }
        /// <summary>
        /// افزودن محصوله به لیست موردعلاقه کاربر
        /// </summary>
        /// <param name="productId">کد محصول</param>
        /// <param name="userid">کد کاربر</param>
        /// <returns></returns>
        public async Task addFavorate(int productId,string userid)
        {
            var pf = Get(x => x, x => x.ProductId == productId && x.UserId == userid).FirstOrDefault();
            if (pf == null)
            {
                Insert(new ProductFavorate()
                {
                    Description = "",
                    FolderName = "محصولات",
                    InsertDate = DateTime.Now,
                    ProductId = productId,
                    UserId = userid
                });
            }
            else
            {
                Delete(pf);
            }

            await context.SaveChangesAsync();
        }

    }
}
