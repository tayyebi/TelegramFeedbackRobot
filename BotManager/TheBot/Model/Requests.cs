namespace TheBot.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Requests
    {
        public Guid Id { get; set; }

        [StringLength(15)]
        public string NationalCode { get; set; }

        [StringLength(100)]
        public string Fullname { get; set; }

        [StringLength(4000)]
        public string Title { get; set; }

        public int? Type { get; set; }

        public string Body { get; set; }

        public int Status { get; set; }

        [Required]
        [StringLength(50)]
        public string ChatId { get; set; }

        public DateTime SubmitDate { get; set; }

        [StringLength(10)]
        public string Age { get; set; }

        [StringLength(50)]
        public string Phone { get; set; }

        [StringLength(4000)]
        public string Question1 { get; set; }

        [StringLength(30)]
        public string Gender { get; set; }
    }
}
