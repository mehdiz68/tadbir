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
    public class YektanetProduct
    {

        #region Properties


        public string sku { get; set; }
        public string title { get; set; }
        public string image { get; set; }
        public List<string> category { get; set; }
        public long price { get; set; }
        public long discount { get; set; }
        public string currency { get; set; }
        public string brand { get; set; }
        public double averageVote { get; set; }
        public int totalVotes { get; set; }
        public bool isAvailable { get; set; }
        public int expiration { get; set; }
        #endregion
    }
}
