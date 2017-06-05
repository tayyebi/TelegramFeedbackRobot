namespace TheBot.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Messages
    {
        [StringLength(50)]
        public string Id { get; set; }

        [Required]
        [StringLength(50)]
        public string ChatId { get; set; }

        public string Value { get; set; }

        public byte[] Content { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime DateTime { get; set; }
    }
}
