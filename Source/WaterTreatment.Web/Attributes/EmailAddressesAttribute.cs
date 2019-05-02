using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WaterTreatment.Web.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
    public class EmailAddressesAttribute : DataTypeAttribute
    {
        private static readonly EmailAddressAttribute EMAIL_ADDRESS_ATTRIBUTE = new EmailAddressAttribute();
        private static readonly char[] DELIMITERS = { ';', ' ', ',', '|' };

        public EmailAddressesAttribute() : base(DataType.EmailAddress) { }

        /// <summary>
        /// Checks if the value is valid
        /// </summary>
        /// <param name="value">The raw text containing email addresses</param>
        /// <returns>Boolean value signifying if only valid email addresses are contained</returns>
        public override bool IsValid(object value)
        {
            var rawAddresses = Convert.ToString(value);

            if (string.IsNullOrWhiteSpace(rawAddresses))
            {
                return true;
            }

            var emailsAddresses = GetAddresses(rawAddresses);
            return emailsAddresses.All(t => EMAIL_ADDRESS_ATTRIBUTE.IsValid(t));
        }

        public static IEnumerable<String> GetAddresses(String addressChunk)
        {
            return addressChunk.Split(DELIMITERS, StringSplitOptions.RemoveEmptyEntries).Where(x => EMAIL_ADDRESS_ATTRIBUTE.IsValid(x));
        }

    }
}