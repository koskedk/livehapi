﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Dapper.Contrib.Extensions;
using LiveHAPI.Core.Interfaces.Repository;
using LiveHAPI.Core.Model;
using LiveHAPI.Core.Model.Lookup;
using LiveHAPI.Core.Model.Network;
using LiveHAPI.Core.Model.People;
using Microsoft.EntityFrameworkCore;
using Z.Dapper.Plus;

namespace LiveHAPI.Infrastructure.Repository
{
    public class UserRepository : BaseRepository<User, Guid>, IUserRepository
    {
        public UserRepository(LiveHAPIContext context) : base(context)
        {
        }

        public int GetUserId(Guid id)
        {
            var user= Context.Users.AsNoTracking().SingleOrDefault(x => x.Id==id);
            if (null != user)
            {
                int.TryParse(user.SourceRef,out var userId);
                return userId;
            }
            return 1;
        }

        public User GetByUsername(string username)
        {
            return Context.Users.FirstOrDefault(x => x.UserName.ToLower().Trim() == username.ToLower().Trim());
        }

        public User GetByUser(string username)
        {
            User user;
            using (var con = GetDbConnection())
            {
                user= con.GetAll<User>()
                    .FirstOrDefault(x => x.UserName.ToLower().Trim() == username.ToLower().Trim());
            }

            return user;
        }

        public void Sync(User user)
        {
            var existingUser = GetByUsername(user.UserName);
            if (null != existingUser)
            {
                existingUser.UpdateTo(user);
                var personName= Context.PersonNames.FirstOrDefault(x => x.PersonId == existingUser.PersonId);
                if (null != personName)
                {
                    personName.UpdateTo(user.Source, user.SourceSys);
                    Context.PersonNames.Update(personName);
                }

                var person = Context.Persons.FirstOrDefault(x => x.Id == existingUser.PersonId);

                if (null != person)
                {
                    if (person.ProfileNeedsUpdate())
                    {
                        person.Gender = person.HasGender() ? person.Gender : "M";
                        person.BirthDate = person.HasDOB() ? person.BirthDate : new DateTime(1983, 7, 4);
                        person.BirthDateEstimated = person.HasDOBEstimate() ? person.BirthDateEstimated : false;
                        Context.Persons.Update(person);
                    }
                }

                Update(existingUser);
            }
            else
            {
                //set PracticeId
                var practiceId = GetPracticeId();

                //Person
                var person = new Person();
                person.BirthDate=new DateTime(1983,7,4);
                person.Gender = "M";
                person.BirthDateEstimated = false;
                var personName = new PersonName();
                personName.FirstName = user.Source;
                personName.LastName = user.SourceSys;
                person.AddName(personName);

                //Provider
                var provider = new Provider();
                provider.ProviderTypeId = "HW";
                provider.PracticeId = practiceId;
                person.AddProvider(provider);

                Context.Persons.Add(person);
                Context.SaveChanges();

                user.PersonId = person.Id;
                user.PracticeId = practiceId;

         
                Insert(user);
            }
        }

        public void Sync(IEnumerable<User> users)
        {
            var updateList = new List<User>();
            var personNameUpdateList = new List<PersonName>();
            var personUpdateList = new List<Person>();
            var insertList = new List<User>();
            var personInsertList = new List<Person>();
            foreach (var user in users)
            {
                var existingUser = GetByUser(user.UserName);
                if (null != existingUser)
                {
                    existingUser.UpdateTo(user);
                    updateList.Add(existingUser);

                    var personName = GetDbConnection().GetAll<PersonName>().FirstOrDefault(x => x.PersonId == existingUser.PersonId);
                    if (null != personName)
                    {
                        personName.UpdateTo(user.Source, user.SourceSys);
                        personNameUpdateList.Add(personName);
                    }

                    var person = GetDbConnection().GetAll<Person>().FirstOrDefault(x => x.Id == existingUser.PersonId);

                    if (null != person)
                    {
                        if (person.ProfileNeedsUpdate())
                        {
                            person.Gender = person.HasGender() ? person.Gender : "M";
                            person.BirthDate = person.HasDOB() ? person.BirthDate : new DateTime(1983, 7, 4);
                            person.BirthDateEstimated = person.HasDOBEstimate() ? person.BirthDateEstimated : false;
                            personUpdateList.Add(person);
                        }
                    }
                }
                else
                {
                    //  PracticeId
                    var practiceId = GetFacilityId();

                    //  Person
                    var person = new Person();
                    person.BirthDate = new DateTime(1983, 7, 4);
                    person.Gender = "M";
                    person.BirthDateEstimated = false;

                    //  PersonNames
                    var personName = new PersonName();
                    personName.FirstName = user.Source;
                    personName.LastName = user.SourceSys;
                    person.AddName(personName);

                    //  Providers
                    var provider = new Provider();
                    provider.ProviderTypeId = "HW";
                    provider.PracticeId = practiceId;
                    person.AddProvider(provider);

                    personInsertList.Add(person);

                    //  Users
                    user.PersonId = person.Id;
                    user.PracticeId = practiceId;

                    insertList.Add(user);
                }
            }

            using (var con = GetDbConnection())
            {

                con.BulkUpdate(updateList);
                con.BulkUpdate(personNameUpdateList);
                con.BulkUpdate(personUpdateList);
                con.BulkInsert(personInsertList)
                    .AlsoBulkInsert(x => x.Names, x => x.Providers);
                con.BulkInsert(insertList);
            }
        }

        private Guid? GetPracticeId()
        {
            var prac = Context.Practices.FirstOrDefault(x => x.IsDefault && x.PracticeTypeId == "Facility");
            return prac?.Id;
        }
        private Guid? GetFacilityId()
        {
            Practice prac;
            using (var con = GetDbConnection())
            {
                 prac = con.GetAll<Practice>()
                    .FirstOrDefault(x => x.IsDefault && x.PracticeTypeId == "Facility");
            }

            return prac?.Id;
        }
    }
}