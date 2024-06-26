﻿using CRM.Core.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CRM.Core.Reports
{
    public class SalesRepresentativeReport:SalesReport
    {
        //public int Revenue {  get; set; }
        public IEnumerable<ItemCount> DoneDeals { get; set; }=new List<ItemCount>();
        public IEnumerable<ItemCount> Sources { get; set; }= new List<ItemCount>();
        [JsonIgnore(Condition =JsonIgnoreCondition.WhenWritingNull)]
        public BestDeal BestDeal { get; set; }
        [JsonIgnore]
        public bool IsSuccess { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string[] Errors { get; set; }

    }
    public class ItemCount
    {
        public string Name { get; set; }
        public int Count { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public double Revenue { get; set; }
    }
    public class BestDeal
    {
        public string CustomerFirstName { get; set; }
        public string CustomerLastName { get; set; }
        public string InterestName { get; set; }
        public double DealPrice { get; set; }
    }
}
