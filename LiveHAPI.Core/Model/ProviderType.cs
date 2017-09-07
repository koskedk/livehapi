﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using LiveHAPI.Shared.Model;

namespace LiveHAPI.Core.Model
{
    public class ProviderType:Entity<string>
    {
        [Key]
        [MaxLength(50)]
        public override string Id { get; set; }
        [MaxLength(100)]
        public string Name { get; set; }
        public ICollection<Provider> Providers { get; set; }

        public ProviderType()
        {
        }
    }
}