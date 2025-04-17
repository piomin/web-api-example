using System.ComponentModel.DataAnnotations.Schema;
using System;

namespace insurance_service.Model
{
    [Table("insurances")]
    public class Insurance
    {
        [System.ComponentModel.DataAnnotations.Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("person_id")]
        public int PersonId { get; set; }

        [Column("amount")]
        public int Amount { get; set; }

        [Column("expiry")]
        public DateTime Expiry { get; set; }

        [Column("insurance_type")]
        public InsuranceType Type { get; set; }
    }
}