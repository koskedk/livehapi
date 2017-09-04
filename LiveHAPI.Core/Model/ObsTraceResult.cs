﻿using System;
using LiveHAPI.Shared.Custom;
using LiveHAPI.Shared.Model;

namespace LiveHAPI.Core.Model
{
    public class ObsTraceResult : Entity<Guid>
    {
        public DateTime Date { get; set; }
        
        public Guid Mode { get; set; }
        
        public Guid Outcome { get; set; }
        public Guid EncounterId { get; set; }

        public ObsTraceResult()
        {
            Id = LiveGuid.NewGuid();
        }

        public ObsTraceResult(DateTime date, Guid mode, Guid outcome, Guid encounterId)
        {
            Date = date;
            Mode = mode;
            Outcome = outcome;
            EncounterId = encounterId;
        }

        public static ObsTraceResult Create(Guid id, DateTime date, Guid mode, Guid outcome, Guid encounterId)
        {
            var obs=new ObsTraceResult(date,mode,outcome,encounterId);
            obs.Id = id;
            return obs;
        }
        public static ObsTraceResult Create(DateTime date, Guid mode, Guid outcome, Guid encounterId)
        {
            return new ObsTraceResult(date, mode, outcome, encounterId);
        }
        public static ObsTraceResult CreateNew(DateTime date, Guid mode, Guid outcome, Guid encounterId)
        {
            return new ObsTraceResult(date, mode, outcome, encounterId);
        }
        public static ObsTraceResult CreateNew(Guid encounterId)
        {
            var obs= new ObsTraceResult();
            obs.Date=DateTime.Today;
            obs.EncounterId = encounterId;
            return obs;
        }


    }
}