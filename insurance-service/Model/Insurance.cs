using System;

namespace insurance_service.Model
{
    public class Insurance
    {
        public int Id { get; set; }
        
        public int PersonId { get; set; }
        
        public int Amount { get; set; }
        
        public DateTime Expiry { get; set; }
        
        public InsuranceType Type { get; set; }
    }
}