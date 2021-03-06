﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using LiveHAPI.Shared.Custom;
using LiveHAPI.Shared.Interfaces.Model;
using LiveHAPI.Shared.Model;

namespace LiveHAPI.Core.Model.Lookup
{
    public class Item : Entity<Guid>, IItem
    {
        [MaxLength(50)]
        public string Code { get; set; }
        [MaxLength(50)]
        public string Display { get; set; }

        public ICollection<CategoryItem> Items { get; set; } = new List<CategoryItem>();

        public Item()
        {
            Id = LiveGuid.NewGuid();
        }
    }
}