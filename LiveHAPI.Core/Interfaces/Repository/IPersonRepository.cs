﻿using System;
using System.Collections.Generic;
using LiveHAPI.Core.Model.People;
using LiveHAPI.Core.Model.Subscriber;

namespace LiveHAPI.Core.Interfaces.Repository
{
    public interface IPersonRepository : IRepository<Person,Guid>
    {
        Model.People.Person GetProvider(Guid id);
        Model.People.Person GetDemographics(Guid id);
        IEnumerable<Person> GetStaff();
        IEnumerable<PersonMatch> Search(string searchItem);
        IEnumerable<PersonMatch> GetByCohort(SubscriberCohort cohort);
    }
}