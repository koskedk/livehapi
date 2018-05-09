﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using LiveHAPI.Core.Model.People;
using LiveHAPI.Shared.Custom;
using LiveHAPI.Shared.Model;

namespace LiveHAPI.Core.Model.Subscriber
{
    public class SubscriberSystem : Entity<Guid>
    {
        public string Name { get; set; }
        public bool IsDefault { get; set; }
        public ICollection<SubscriberConfig> Configs { get; set; }
        public ICollection<SubscriberSqlAction> Actions { get; set; }
        public ICollection<SubscriberMessage> Messages { get; set; }
        public ICollection<SubscriberMap> Maps { get; set; }
        public ICollection<SubscriberTranslation> Translations { get; set; }
        public ICollection<SubscriberCohort> Cohorts { get; set; }

        [NotMapped] public List<User> Users { get; set; }

        public SubscriberSystem()
        {
            Id = LiveGuid.NewGuid();
        }

        public string GetTranslation(object code,string subref, string def)
        {
            var translation =
                Translations.FirstOrDefault(x => x.SubRef.IsSameAs(subref) &&
                                                 x.Code.IsSameAs(code.ToString()));

            if (null != translation)
                return translation.SubCode;

            return def;
        }

        public string GetTranslation(object code, string subref, string hapiRef, string def)
        {
            var translation =
                Translations.FirstOrDefault(x => x.Ref.IsSameAs(hapiRef) &&
                                                 x.SubRef.IsSameAs(subref) &&
                                                 x.Code.IsSameAs(code.ToString()));

            if (null != translation)
                return translation.SubCode;

            return def;
        }
    }
}
