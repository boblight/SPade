using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using SPade.Models.DAL;

namespace SPade.ViewModels.Admin
{
    public class AddModuleViewModel
    {
        [Required(ErrorMessage = "Please do not leave the Module Code blank!")]
        [StringLength(50)]
        [Display(Name = "Module Code")]
        public string ModuleCode { get; set; }

        [Required(ErrorMessage = "Please do not leave the Module Name blank!")]
        [StringLength(100)]
        [Display(Name = "Module Name")]
        public string ModuleName { get; set; }

        public List<ProgLanguage> Languages { get; set; }

        public int ProgLangId { get; set; }

    }
}