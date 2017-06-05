namespace TheBot.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Errors
    {
        public Guid Id { get; set; }

        public DateTime DateTime { get; set; }

        [Required]
        public string Message { get; set; }
    }
}
