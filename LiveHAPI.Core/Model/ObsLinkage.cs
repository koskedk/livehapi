﻿using System;
using LiveHAPI.Shared.Custom;
using LiveHAPI.Shared.Model;

namespace LiveHAPI.Core.Model
{
    public class ObsLinkage : Entity<Guid>
    {
        public string ReferredTo { get; set; }
        public DateTime? DatePromised { get; set; }
        public string FacilityHandedTo { get; set; }
        public string HandedTo { get; set; }
        public string WorkerCarde { get; set; }
        public DateTime? DateEnrolled { get; set; }
        public string EnrollmentId { get; set; }
        public string Remarks { get; set; }
        public Guid EncounterId { get; set; }

        public ObsLinkage()
        {
            Id = LiveGuid.NewGuid();
        }

        public ObsLinkage(string referredTo, DateTime? datePromised, string facilityHandedTo, string handedTo, string workerCarde, DateTime? dateEnrolled, string enrollmentId, string remarks, Guid encounterId):this()
        {
            FacilityHandedTo = facilityHandedTo;
            ReferredTo = referredTo;
            DatePromised = datePromised;
            HandedTo = handedTo;
            WorkerCarde = workerCarde;
            DateEnrolled = dateEnrolled;
            EnrollmentId = enrollmentId;
            Remarks = remarks;
            EncounterId = encounterId;
        }

        public static ObsLinkage Create(string referredTo, DateTime? datePromised, string facilityHandedTo, string handedTo, string workerCarde, DateTime? dateEnrolled, string enrollmentId, string remarks, Guid encounterId)
        {
            var obs = new ObsLinkage(referredTo, datePromised, handedTo, facilityHandedTo, workerCarde, dateEnrolled,
                enrollmentId, remarks, encounterId);
            return obs;
        }
        public static ObsLinkage Create(Guid id, string referredTo, DateTime? datePromised, string facilityHandedTo, string handedTo, string workerCarde, DateTime? dateEnrolled, string enrollmentId, string remarks, Guid encounterId)
        {
            var obs = new ObsLinkage(referredTo, datePromised, handedTo, facilityHandedTo, workerCarde, dateEnrolled, enrollmentId, remarks, encounterId);
            obs.Id = id;
            return obs;
        }
        public static ObsLinkage CreateNew(string referredTo, DateTime? datePromised, Guid encounterId)
        {
            var obs = new ObsLinkage();
            obs.ReferredTo = referredTo;
            obs.DatePromised = datePromised;
            obs.EncounterId = encounterId;
            return obs;
        }

        public static ObsLinkage CreateNew(string facilityHandedTo, string handedTo, string workerCarde, DateTime? dateEnrolled, string enrollmentId, string remarks, Guid encounterId)
        {
            var obs = new ObsLinkage();
            obs.FacilityHandedTo = facilityHandedTo;
            obs.HandedTo = handedTo;
            obs.WorkerCarde = workerCarde;
            obs.DateEnrolled = dateEnrolled;
            obs.EnrollmentId = enrollmentId;
            obs.Remarks = remarks;
            return obs;
        }
    }
}