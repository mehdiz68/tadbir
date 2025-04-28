using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Data;
using Domain;
using Domain.ViewModel;

namespace Domain.ViewModels
{
    public class ProductItemRanks
    {
        #region Ctor
        public ProductItemRanks()
        {

        }
        #endregion

        #region Properties
        
        public int Id { get; set; }

        public string Name { get; set; }

        public string LatinName { get; set; }

        public string Title { get; set; }
        public string PageAddress { get; set; }

        public string Descr { get; set; }


        public string BrandName { get; set; }
        public  Brand Brand { get; set; }

       
        public string Code { get; set; }


        public int ProductStateId{ get; set; }
        public string ProductStateTitle { get; set; }
        public  ProductState ProductState { get; set; }

        public  ProductIcon ProductIcon { get; set; }

        public string MainImageFileName { get; set; }
        public ProductImage MainImage { get; set; }
        public IEnumerable<string> OtherImageFileName { get; set; }
        public IEnumerable<ProductImage> OtherImages{ get; set; }
        //public IEnumerable<ProductCategoryDto> ProductCategories { get; set; }
        public IEnumerable<ProductCategory> ProductCategories { get; set; }

        public IEnumerable<string> Colors { get; set; }

        public double avgRankValue { get; set; }
        public int countRankValue { get; set; }

        public long Price { get; set; }
        public long finalPrice { get; set; }
        public long offValue{ get; set; }
        public long offFinalValue { get; set; }

        public double tax { get; set; }

        public bool hasoff { get; set; }

        public short offtype { get; set; }
        public int Favorates { get; set; }
        public int LetmeKnows { get; set; }
        public int? TaxId { get; set; }

        public IEnumerable<ProductRank> ProductRanks { get; set; }
        #endregion
    }
}
