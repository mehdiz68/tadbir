using System;
using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class EventLog : Object
    {
        #region Ctor
        public EventLog()
        {

        }
        public EventLog(Int16 logType,string controllerName,string actionName,bool requestType,int statusCode,string description,DateTime logDateTime,string userId)
        {
            this.LogType = logType;
            this.ControllerName = controllerName;
            this.ActionName = actionName;
            this.RequestType = requestType;
            this.StatusCode = statusCode;
            this.Description = description;
            this.LogDateTime = logDateTime;
            this.UserId = userId;
        }
        #endregion

        #region Configuration
        public class Configuration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<EventLog>
        {
            public Configuration()
            {
              
                HasRequired(Current => Current.User).WithMany(Current => Current.EventLogs).HasForeignKey(Current => Current.UserId);
            }
        }

        #endregion

        #region Properties

        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        [Display(Name = "نوع لاگ")]
        public Int16 LogType { get; set; }

        [Required]
        [Display(Name = "نام ماژول")]
        [MaxLength(50, ErrorMessage = "حداکثر طول کارکتر ، 50")]
        public string ControllerName { get; set; }

        [Required]
        [Display(Name = "نام عمل")]
        [MaxLength(50, ErrorMessage = "حداکثر طول کارکتر ، 50")]
        public string ActionName { get; set; }

        [Required]
        [Display(Name = "نوع درخواست")]
        public bool RequestType { get; set; }

        [Required]
        [Display(Name = "کد وضعیت http")]
        public int StatusCode { get; set; }

        [Display(Name = "توضیحات")]
        public string Description { get; set; }

        [Required]
        [Display(Name = "زمان ثبت")]
        public DateTime LogDateTime { get; set; }


        public string IP { get; set; }

        public string UserId { get; set; }
        public  ApplicationUser User { get; set; }

        #endregion
    }
}
