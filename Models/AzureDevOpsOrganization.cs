using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazor.MSAL.Models
{
    public class AzureDevOpsOrganization
    {
        /// <summary>
        /// Organization Name
        /// </summary>
        public string AccountName { get; set; }

        /// <summary>
        /// Organization Id
        /// </summary>
        public string AccountId { get; set; }
    }
}
