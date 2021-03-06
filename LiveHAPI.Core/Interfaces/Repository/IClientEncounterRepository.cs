﻿using System;
using System.Collections.Generic;
using LiveHAPI.Core.Model.Encounters;
using LiveHAPI.Core.Model.Exchange;
using LiveHAPI.Core.Model.People;
using LiveHAPI.Core.Model.Subscriber;

namespace LiveHAPI.Core.Interfaces.Repository
{
    public interface IClientEncounterRepository : IRepository<Encounter, Guid>
    {
        IEnumerable<Encounter> GetReferralLinkage(Guid? clientId=null);
        IEnumerable<Encounter> GetTracing(Guid? clientId = null);
        IEnumerable<Encounter> GetTesting(Guid? clientId = null);
        IEnumerable<Encounter> GetFinalTesting(Guid? clientId = null);
        IEnumerable<Encounter> GetPretest(Guid? clientId = null);
        Guid? GetPretestEncounterId(Guid clientId,DateTime encounterDate);
    }
}