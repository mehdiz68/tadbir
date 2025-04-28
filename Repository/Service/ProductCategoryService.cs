using DataLayer;
using Domain;
using Domain.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Service
{
    public class ProductCategoryService : GenericRepository<ProductCategory>
    {
        public ProductCategoryService(ahmadiDbContext context) : base(context)
        {
        }


        public string GerProductBreadcrumb(int catid)
        {
            string breadcrumb = "";
            List<ProductCategory> breadcrumbList = new List<ProductCategory>();
            var prcats = Get(x => x, x => x.Id == catid, null).First();
            if (!prcats.ParrentId.HasValue)
                breadcrumb += string.Format("<li itemprop='itemListElement' itemscope='' itemtype='http://schema.org/ListItem' class='breadcrumb-item'><a itemprop='item' href='/TFC/{0}/{1}'><span itemprop='name'>{2}</span></a><meta itemprop='position' content='1' /></li>", prcats.Id, CoreLib.Infrastructure.CommonFunctions.NormalizeAddress(prcats.PageAddress), prcats.Name);
            else
            {
                var parent = prcats.ParentCat;
                while (parent != null)
                {
                    breadcrumbList.Add(new ProductCategory() { Id = parent.Id, Name = parent.Name, PageAddress = parent.PageAddress });
                    parent = parent.ParentCat;
                }
                int i = 1;
                foreach (var item in breadcrumbList.OrderBy(x => x.Id))
                {
                    string prefix = "TFS";
                    if (!item.ParrentId.HasValue)
                        prefix = "TFC";

                    breadcrumb += string.Format("<li itemprop='itemListElement' itemscope='' itemtype='http://schema.org/ListItem' class='breadcrumb-item'><a itemprop='item' href='/" + prefix + "/{0}/{1}'><span itemprop='name'>{2}</span></a><meta itemprop='position' content='{3}' /></li>", item.Id, CoreLib.Infrastructure.CommonFunctions.NormalizeAddress(item.PageAddress), item.Name, i);
                    i++;
                }
                breadcrumb += string.Format("<li itemprop='itemListElement' itemscope='' itemtype='http://schema.org/ListItem' class='breadcrumb-item'><a itemprop='item' href='/TFC/{0}/{1}'><span itemprop='name'>{2}</span></a><meta itemprop='position' content='{3}' /></li>", prcats.Id, CoreLib.Infrastructure.CommonFunctions.NormalizeAddress(prcats.PageAddress), prcats.Name, i);
            }
            return breadcrumb;
        }

        public string GerProductSearchBreadcrumb(int catid)
        {
            string breadcrumb = "";
            List<ProductCategory> breadcrumbList = new List<ProductCategory>();
            var prcats = Get(x => x, x => x.Id == catid, null).First();
            if (!prcats.ParrentId.HasValue)
                breadcrumb += string.Format("<li itemprop='itemListElement' itemscope='' itemtype='http://schema.org/ListItem' class='breadcrumb-item'><a itemprop='item' href='/TFC/{0}/{1}'><span itemprop='name'>{2}</span></a><meta itemprop='position' content='1' /></li>", prcats.Id, CoreLib.Infrastructure.CommonFunctions.NormalizeAddress(prcats.PageAddress2), prcats.Name);
            else
            {
                var parent = prcats.ParentCat;
                while (parent != null)
                {
                    breadcrumbList.Add(new ProductCategory() { Id = parent.Id, Name = parent.Name, PageAddress = parent.PageAddress, PageAddress2 = parent.PageAddress2, ParrentId = parent.ParrentId });
                    parent = parent.ParentCat;
                }
                int i = 1;
                foreach (var item in breadcrumbList.OrderBy(x => x.Id))
                {
                    string prefix = "TFS", pgadress = item.PageAddress;
                    if (!item.ParrentId.HasValue)
                    {
                        prefix = "TFC";
                        pgadress = item.PageAddress2;
                    }
                    breadcrumb += string.Format("<li itemprop='itemListElement' itemscope='' itemtype='http://schema.org/ListItem' class='breadcrumb-item'><a itemprop='item' href='/" + prefix + "/{0}/{1}'><span itemprop='name'>{2}</span></a><meta itemprop='position' content='{3}' /></li>", item.Id, CoreLib.Infrastructure.CommonFunctions.NormalizeAddress(pgadress), item.Name, i);
                    i++;
                }
                breadcrumb += string.Format("<li itemprop='itemListElement' itemscope='' itemtype='http://schema.org/ListItem' class='breadcrumb-item'><a itemprop='item' href='/TFS/{0}/{1}'><span itemprop='name'>{2}</span></a><meta itemprop='position' content='{3}' /></li>", prcats.Id, CoreLib.Infrastructure.CommonFunctions.NormalizeAddress(prcats.PageAddress2), prcats.Name, i);
            }
            return breadcrumb;
        }
        public List<int> GetChilderenId(int pcId)
        {
            var result = dbSet.AsNoTracking().Where(c => c.Id == pcId ||
            c.ParentCat.Id == pcId ||
            c.ParentCat.ParentCat.Id == pcId ||
            c.ParentCat.ParentCat.ParentCat.Id == pcId ||
            c.ParentCat.ParentCat.ParentCat.ParentCat.Id == pcId ||
            c.ParentCat.ParentCat.ParentCat.ParentCat.ParentCat.Id == pcId ||
            c.ParentCat.ParentCat.ParentCat.ParentCat.ParentCat.ParentCat.Id == pcId ||
            c.ParentCat.ParentCat.ParentCat.ParentCat.ParentCat.ParentCat.ParentCat.Id == pcId ||
            c.ParentCat.ParentCat.ParentCat.ParentCat.ParentCat.ParentCat.ParentCat.ParentCat.Id == pcId ||
            c.ParentCat.ParentCat.ParentCat.ParentCat.ParentCat.ParentCat.ParentCat.ParentCat.ParentCat.Id == pcId ||
            c.ParentCat.ParentCat.ParentCat.ParentCat.ParentCat.ParentCat.ParentCat.ParentCat.ParentCat.ParentCat.Id == pcId
            ).Select(c => c.Id).ToList();
            return result;
        }

        public ProductCategory GetCategoryViewModel(int id)
        {
            var model = dbSet.AsNoTracking().Where(c => c.Id == id).FirstOrDefault();
            return model;
        }


        public List<ProductCategoryDto> GetCategoryDtosByQueryInput(IQueryable<ProductCategory> productCategories)
        {

            var dbre = productCategories.Select(c => new
            {
                Id = c.Id,
                Title = c.Title,
                PageAddress = c.PageAddress,
                Sort = c.Sort,
                ParrentId = c.ParrentId,
                ChildCategory = c.ChildCategory.Select(s => new
                {
                    Id = s.Id,
                    Title = s.Title,
                    PageAddress = s.PageAddress,
                    Sort = s.Sort,
                    ParrentId = s.ParrentId,
                    ChildCategory = c.ChildCategory.Select(sd => new
                    {
                        Id = sd.Id,
                        Title = sd.Title,
                        PageAddress = sd.PageAddress,
                        Sort = sd.Sort,
                        ParrentId = sd.ParrentId,
                    }).ToList()
                })
            }).OrderBy(s => s.Sort).ToList();

            List<ProductCategoryDto> mResult = new List<ProductCategoryDto>();
            dbre.ForEach(c =>
            {
                mResult.Add(new ProductCategoryDto
                {
                    Id = c.Id,
                    Title = c.Title,
                    PageAddress = c.PageAddress,
                    ParrentId = c.ParrentId,
                    Sort = c.Sort,
                    ChildCategory = c.ChildCategory == null ? new List<ProductCategoryDto>() : c.ChildCategory.Select(s => new ProductCategoryDto
                    {
                        Id = s.Id,
                        Title = s.Title,
                        PageAddress = s.PageAddress,
                        ParrentId = s.ParrentId,
                        Sort = s.Sort,
                        ChildCategory = s.ChildCategory == null ? new List<ProductCategoryDto>() : s.ChildCategory.Select(sd => new ProductCategoryDto
                        {
                            Id = sd.Id,
                            Title = sd.Title,
                            PageAddress = sd.PageAddress,
                            ParrentId = sd.ParrentId,
                            Sort = sd.Sort,
                        }).ToList()
                    }).ToList()


                });
            });

            return mResult;
        }
    }
}
