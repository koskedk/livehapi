﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using LiveHAPI.Core.Model.People;
using LiveHAPI.Shared.Interfaces.Model;
using LiveHAPI.Shared.Model;

namespace LiveHAPI.Core.Model.Lookup
{
    public class IdentifierType : Entity<string>, IIdentifierType
    {
        [Key]
        [MaxLength(50)]
        public override string Id { get; set; }
        [MaxLength(100)]
        public string Name { get; set; }

        public ICollection<ClientIdentifier> ClientIdentifiers { get; set; }

        public IdentifierType()
        {
        }

        public override string ToString()
        {
            return $"{Name}";
        }
    }
}