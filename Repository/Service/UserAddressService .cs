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
    public class UserAddressService : GenericRepository<UserAddress>
    {
        public UserAddressService(ahmadiDbContext context) : base(context)
        {

        }
        /// <summary>
        /// متد بررسی موجود بودن آدرس در لیست ادرس های کاربر
        /// </summary>
        /// <param name="addressId">کد آدرس</param>
        /// <param name="userid">کد کاربر</param>
        /// <returns></returns>

        public int? checkUserUserAddress(int addressId,string userid)
        {
            return  Get(x => x.Id, x => x.Id == addressId && x.UserId == userid,null,"",0,0,true).FirstOrDefault();

        }
        /// <summary>
        /// حذف آدرس کاربر
        /// </summary>
        /// <param name="id">ردیف آدرس کاربر</param>
        /// <returns></returns>
        public async Task removeUserAddress(int id)
        {
            Delete(dbSet.Find(id));
            await context.SaveChangesAsync();
        }
        
        /// <summary>
        /// افزودن آدرس به آدرس های کاربر
        /// </summary>
        /// <param name="useraddress"></param>
        /// <returns></returns>
        public async Task addUserAddress(UserAddress useraddress)
        {
            Insert(useraddress);
            await context.SaveChangesAsync();
        }
       

        /// <summary>
        /// ویرایش آدرس کاربر
        /// </summary>
        /// <param name="useraddress"></param>
        /// <returns></returns>
        public async Task EditUserAddress(UserAddress useraddress)
        {
            Update(useraddress);
            await context.SaveChangesAsync();
        }


    }
}
