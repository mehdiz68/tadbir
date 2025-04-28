using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Data;
using Domain;

namespace Domain.ViewModels
{
    public class ShippingSetting
    {
        #region Ctor
        public ShippingSetting()
        {

        }
        #endregion

        #region Properties
        public int Id { get; set; }
        public int SendWayBoxID { get; set; }
        public int sendwayId { get; set; }
        public string sendwayName { get; set; }
        public int cityId { get; set; }
        public string cityName { get; set; }
        public int provienceId{ get; set; }
        public string provienceName { get; set; }
        public int boxId { get; set; }
        public string boxName { get; set; }
        public long Cost { get; set; }
        public bool isActive { get; set; }
        public int limitation { get; set; }

        #endregion
    }
}
