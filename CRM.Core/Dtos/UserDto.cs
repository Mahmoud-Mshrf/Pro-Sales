﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CRM.Core.Dtos
{
    public class UserDto
    {
        public string Id { get; set; }
        //public string Name { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public IList<string> Roles { get; set; }
        public string Email { get; set; }
        [JsonIgnore(Condition =JsonIgnoreCondition.WhenWritingDefault)]
        public int customers { get; set; }
    }
}
