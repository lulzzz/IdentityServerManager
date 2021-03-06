﻿using IdentityServer4.EntityFramework.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServerManager.UI.Models
{
    public class ClientClaimsViewModel
    {
        public int Id { get; set; }
        public string NextUrl { get; set; }
        public List<ClientClaim> Claims { get; set; }
    }
}
