﻿using System;
using System.Collections.Generic;
using System.Linq;
using LiveHAPI.Core.Model.People;
using LiveHAPI.Core.Model.Subscriber;
using LiveHAPI.Shared.Custom;
using LiveHAPI.Shared.Model;
using LiveHAPI.Sync.Core.Enum;

namespace LiveHAPI.Core.Model.Exchange
{
    public class ClientStage:Entity<Guid>
    {
        public string Serial { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        /// <summary>
        /// "ESTIMATED/EXACT"
        /// </summary>
        public string DateOfBirthPrecision { get; set; }
        public int Sex { get; set; }
        public int KeyPop { get; set; }
        public string Landmark { get; set; }
        public string Phone { get; set; }
        public int MaritalStatus { get; set; }
        public DateTime RegistrationDate { get; set; }

        public Guid ClientId { get; set; }
        public SyncStatus SyncStatus { get; set; }
        public DateTime StatusDate { get; set; }
        public string SyncStatusInfo { get; set; }
      

        public ClientStage()
        {
            SyncStatus = SyncStatus.Staged;
            StatusDate=DateTime.Now;
        }

        public static ClientStage Create(Person person, SubscriberSystem subscriber)
        {
            var clientStage=new ClientStage();

            if (null != person.PersonClient)
            {
                clientStage.Id = person.PersonClient.Id;
                clientStage.ClientId = person.PersonClient.Id;
            }
            else
            {
                clientStage.Id = LiveGuid.NewGuid();
            }


            if (null != person.PersonName)
            {
                clientStage.FirstName = person.PersonName.FirstName;
                clientStage.MiddleName = person.PersonName.MiddleName;
                clientStage.LastName = person.PersonName.LastName;
            }
            clientStage.DateOfBirth = person.HasDOB() ? person.BirthDate.Value : new DateTime(1900, 1, 1);
            clientStage.DateOfBirthPrecision = person.HasDOBEstimate()
                ? (person.BirthDateEstimated.Value ? "EXACT" : "ESTIMATED")
                : "ESTIMATED";
            clientStage.Sex = subscriber.GetTranslation(person.Gender, "Gender", "0").SafeConvert<int>();

            if (null!=person.PersonClient)
            {
                var client = person.PersonClient;

                if (null != client.HtsEnrollment)
                {
                    clientStage.Serial = client.HtsEnrollment.Identifier;
                    clientStage.RegistrationDate = client.HtsEnrollment.RegistrationDate;
                }
                clientStage.KeyPop=subscriber.GetTranslation(client.KeyPop, "HTSKeyPopulation", "0").SafeConvert<int>();
                clientStage.MaritalStatus = subscriber.GetTranslation(client.MaritalStatus, "HTSMaritalStatus", "0").SafeConvert<int>();
            }
           
            return clientStage;
        }

        public override string ToString()
        {
            return $"{FirstName} {LastName},{Sex} {Serial} {MaritalStatus} [{ClientId} {Id}]";
        }
    }
}