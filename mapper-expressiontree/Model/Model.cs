using System;
using System.Collections.Generic;

namespace MapperUsingExpressionTree.Model
{
    /// <summary>
    /// Dummy Class property.
    /// </summary>
    public class BenefitsBase
    {
        public string ApprovedBy { get; set; }

        public string UpdateDateTime { get; set; }
    }

    public class EmployeeBenefits<T> : BenefitsBase where T : new()
    {
        public int Id { get; set; }
        public NameEntity Name { get; set; }
        public string Email { get; set; }

        public PhoneEntity PhoneNumber { get; set; }

        public DateTime RejectedDateTime { get; set; }

        public List<T> StockAwards { get; set; }

        public List<Benefits> Benefits { get; set; }

        public Dictionary<string, string> PromotionMapping { get; set; }
    }

    public class StockAwardsData
    {
        public string Name { get; set; }
        public string Issuer { get; set; }
        public decimal Duration { get; set; }
        public DateTime YieldToMaturity { get; set; }

        public List<string> StockAwardsValue { get; set; }
    }

    public class Benefits
    {
        public NameEntity Name { get; set; }
        public Enum BenefitType { get; set; }
        public decimal Duration { get; set; }
    }

    public class NameEntity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PetName { get; set; }
    }

    public class PhoneEntity
    {
        public string HomeNumber { get; set; }
        public string OfficeNumber { get; set; }

        public List<PhoneLocation> PhoneLocationVal { get; set; }
    }

    public class PhoneLocation
    {
        public string Location { get; set; }
        public string Coordinates { get; set; }

        public List<string> Val { get; set; }
    }
}
