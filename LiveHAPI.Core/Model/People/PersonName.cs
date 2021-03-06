﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using LiveHAPI.Shared.Custom;
using LiveHAPI.Shared.Interfaces.Model;
using LiveHAPI.Shared.Model;
using LiveHAPI.Shared.ValueObject;

namespace LiveHAPI.Core.Model.People
{
    public class PersonName:Entity<Guid>, IPersonName, ISourceIdentity
    {
        [MaxLength(100)]
        public  string FirstName { get; set; }
        [MaxLength(100)]
        public  string MiddleName { get; set; }
        [MaxLength(100)]
        public  string LastName { get; set; }
        [MaxLength(100)]
        public string MothersName { get; set; }
        [MaxLength(100)]
        public string NickName { get; set; }

        [MaxLength(50)]
        public string Source { get; set; }
        [MaxLength(50)]
        public string SourceRef { get; set; }
        [MaxLength(50)]
        public string SourceSys { get; set; }
        public bool Preferred { get; set; }
        public Guid PersonId { get; set; }
        public string FullName
        {
            get { return $"{FirstName} {MiddleName} {LastName}"; }
        }

        public PersonName()
        {
            Id = LiveGuid.NewGuid();
        }
        private PersonName(string firstName, string middleName, string lastName, string mothersName, string nickName) : this()
        {
            FirstName = firstName;
            MiddleName = middleName;
            LastName = lastName;
            MothersName = mothersName;
            NickName = nickName;
        }
        private PersonName(string firstName, string middleName, string lastName, string mothersName, string source, string sourceRef, string sourceSys, string nickName) :this(firstName,middleName,lastName,mothersName, nickName)
        {
            Source = source;
            SourceRef = sourceRef;
            SourceSys = sourceSys;
        }
        public static PersonName Create(PersonNameInfo personNameInfo)
        {
            return new PersonName(personNameInfo.FirstName, personNameInfo.MiddleName, personNameInfo.LastName, personNameInfo.MothersName, personNameInfo.SourceIdentity.Source,
                personNameInfo.SourceIdentity.SourceRef, personNameInfo.SourceIdentity.SourceSys,personNameInfo.NickName);
        }

        public static List<PersonName> Create(PersonInfo personInfo)
        {
            var list = new List<PersonName>
            {
                new PersonName(personInfo.FirstName, personInfo.MiddleName, personInfo.LastName, personInfo.MothersName, personInfo.NickName)
            };
            return list;
        }

        public void ChangeTo(PersonName name)
        {
            FirstName = name.FirstName;
            MiddleName = name.MiddleName;
            LastName = name.LastName;
            MothersName = name.MothersName;
        }

        public override string ToString()
        {
            return $"{FullName}";
        }

        public void UpdateTo(string existingUserSource, string existingUserSourceSys)
        {
            FirstName = existingUserSource;
            LastName = existingUserSourceSys;
        }
    }
}