using insurance_service.Message;

namespace insurance_service.Model
{
    public class InsuranceDetails
    {
        public InsuranceDetails(Insurance insurance, Person person)
        {
            Insurance = insurance;
            Person = person;
        }

        public Insurance Insurance { get; }

        public Person Person { get; }
    }
}