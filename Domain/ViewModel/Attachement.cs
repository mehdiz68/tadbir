using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ViewModel
{
    public class Attachement
    {
        public Attachement()
        {

        }

        [Key]
        [Required]
        [System.ComponentModel.DataAnnotations.Schema.DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.None)]
        public System.Guid Id { get; set; }

        [Required(ErrorMessage = "اجباری")]
        [Display(Name = "نام فایل")]
        public string Title { get; set; }

        [Display(Name = "فایل فایل")]
        public string FileName { get; set; }



        public bool HasMultiSize { get; set; }

        public bool HasWatermark { get; set; }

        public int Capacity { get; set; }

        public int UseCount { get; set; }
        public DateTime InsertDate { get; set; }
        public bool IsActive { get; set; }
        public Int16? LanguageId { get; set; }

        public int FileTypeId { get; set; }

        public int? FolderId { get; set; }
    }
}
