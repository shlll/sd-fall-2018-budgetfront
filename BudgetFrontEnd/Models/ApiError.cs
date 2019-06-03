using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BudgetFrontEnd.Models
{
    public class ApiError
    {
        public string Message { get; set; }
        public Dictionary<string, string[]> ModelState { get; set; }
    }
}