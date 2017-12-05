using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using MapperUsingExpressionTree.Mapper;
using MapperUsingExpressionTree.Model;
using Newtonsoft.Json;

namespace MapperUsingExpressionTree
{
    /// <summary>
    /// Mapper Initiator test.
    /// </summary>
    public class Program
    {
        public static void Main(params string[] args)
        {
            Console.WriteLine("Starting Mapper Test....");
            Stopwatch sw = new Stopwatch();
            sw.Start();
            Console.WriteLine("Invoking StopWatch ....");
            Console.WriteLine("Defining Input Dictionary....");
            //Input Dictionary for mapping.
            var fakeData = new Dictionary<string, object>
            {
                { "NAME", "Shiladitya" },
                { "CoordinatesVal", "124567.567" },
                { "BenefitsVal", 181.30 },
                { "MobileNumber", "5575778979" },
                { "EffectiveDate", "11/25/2013" }, // This is Datetime value
                { "EmailAddress", "shiladitya.srivastava@gmail.com" },
                { "TestType", "ExpressionTree Usage" },
                { "StockIssuer", "Fidelity" },
                { "OwnerAuthority", "Fidelity Services" },
                { "LastUpdatedDate", DateTime.UtcNow }
            };

            Console.WriteLine("Instantiating mapper to start mapping....");
            Console.WriteLine("........");
            //Mapper initialized to create class instance.
            var mappedPropertyBag = new Mapper<EmployeeBenefits<StockAwardsData>>()
                .UsingEntity(() => new EmployeeBenefits<StockAwardsData>())
                .IgnoreUnMapped()
                .Map(x => x.Name.FirstName, "NAME", p => (string)p)
                .Map(x => x.Email, "EmailAddress", p => (string)p)
                .Map(x => x.RejectedDateTime, "EffectiveDate", p => DateTime.ParseExact((string)p, "MM/dd/yyyy", CultureInfo.InvariantCulture))
                .Map(x => x.PhoneNumber.PhoneLocationVal[0].Coordinates, "CoordinatesVal", p => (string)p)
                .Map(x => x.PhoneNumber.HomeNumber, "MobileNumber", p => (string)p)
                .Map(x => x.StockAwards[0].Issuer, "StockIssuer", p => (string)p)
                .Map(x => x.ApprovedBy, "OwnerAuthority", p => (string)p)
                .Map(x => x.UpdateDateTime, "LastUpdatedDate", p => Convert.ToString(p))
                .Map(x => x.PhoneNumber.PhoneLocationVal[0].Val, "TestType", p => new List<string>() { p.ToString() })
                .Map(x => x.PromotionMapping, "BenefitsVal", p => new Dictionary<string, string>() { { "Temperature", p.ToString() } })
                .MapSource(fakeData);

            Console.WriteLine("Mapping completed....");
            var serializedProperties = JsonConvert.SerializeObject(mappedPropertyBag);
            Console.WriteLine(serializedProperties);
            Console.WriteLine("Elapsed Time(in milliseconds) : " + sw.Elapsed.Milliseconds);
            Console.WriteLine("End....");
            Console.ReadLine();
        }
    }
}
