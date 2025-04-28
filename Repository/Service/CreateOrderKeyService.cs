using DataLayer;
using Domain;

namespace Repository.Service
{
    /// <summary>
    /// برای ایجاد  یک ایدی منحصر به فرد برای فرستادن به سمت بانک
    /// </summary>
    public class CreateOrderKeyService : GenericRepository<CreateOrderKey>
    {
        public CreateOrderKeyService(ahmadiDbContext context) : base(context)
        {
        }

        public long GetOrderId()
        {
            var model = new CreateOrderKey();
            Insert(model);
            context.SaveChanges();
            return model.Id;
        }
    }
}
