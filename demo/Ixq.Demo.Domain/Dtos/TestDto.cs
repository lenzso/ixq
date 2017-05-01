﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ixq.Core.Dto;
using Ixq.Data.DataAnnotations;
using Ixq.Demo.Entities;
using Ixq.UI.ComponentModel.DataAnnotations;

namespace Ixq.Demo.Domain.Dtos
{
    [Page(DefaultSortname = nameof(Id), TitleName = "Test",IsDescending = true)]
    public class TestDto : DtoBaseInt32<Test>
    {
        [Key]
        [Required]
        [Hide(IsHiddenOnCreate = true, IsHiddenOnDetail = true, IsHiddenOnEdit = true, IsHiddenOnView = false)]
        [Display(Name = "唯一标识")]
        [ColModel(Sortable =true)]
        public override int Id { get; set; }

        [Display(Name = "创建时间", Order = 1)]
        [ColModel(Sortable = true)]
        [PropertyAuthorization(Roles = new[] { "Admin" })]
        public DateTime CreateDate { get; set; }

        [Display(Name = "Name")]
        [StringLength(200)]
        [ColModel(Sortable =true)]
        public string Name { get; set; }

        [StringLength(200)]
        [Display(Name = "Name1")]
        [ColModel(Sortable =true)]
        public string Name1 { get; set; }

        [StringLength(200)]
        [Display(Name = "Name2")]
        [ColModel(Sortable =true)]
        public string Name2 { get; set; }

        [StringLength(200)]
        [Display(Name = "Name3")]
        [ColModel(Sortable =true)]
        public string Name3 { get; set; }

        [StringLength(200)]
        [Display(Name = "Name4")]
        [ColModel(Sortable =true)]
        public string Name4 { get; set; }
    }
}