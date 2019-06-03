using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BudgetFrontEnd.Models
{
    public class LoginInfo
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }
    }
}