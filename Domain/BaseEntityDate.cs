using System;
using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class BaseEntityDate : System.Object
    {

        #region Ctor
        public BaseEntityDate()
        {
            Id = Guid.NewGuid();
            InsertDate = DateTime.Now.Date;
        }
        public BaseEntityDate(bool isActive, int displaySort)
        {
            Id = Guid.NewGuid();
            InsertDate = DateTime.Now.Date;
        }
        #endregion

        #region Properties
        [Key]
        [Required]
        [System.ComponentModel.DataAnnotations.Schema.DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.None)]
        public System.Guid Id { get; set; }

        [Required]
        public DateTime InsertDate { get; set; }

        public DateTime? UpdateDate { get; set; }



        #endregion
    }

}
