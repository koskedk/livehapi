﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LiveHAPI.Core.Interfaces.Repository;
using LiveHAPI.Core.Model.Exchange;
using LiveHAPI.Sync.Core.Interface.Extractors;

namespace LiveHAPI.Sync.Core.Extractor
{
    public class ClientPartnerTracingStageExtractor : IClientPartnerTracingStageExtractor
    {
        private readonly IContactsEncounterRepository _clientEncounterRepository;
       private readonly ISubscriberSystemRepository _subscriberSystemRepository;
        
        public ClientPartnerTracingStageExtractor(IContactsEncounterRepository clientEncounterRepository, ISubscriberSystemRepository subscriberSystemRepository)
        {
            _clientEncounterRepository = clientEncounterRepository;
            _subscriberSystemRepository = subscriberSystemRepository;
        }

        public async Task<IEnumerable<ClientPartnerTracingStage>> Extract()
        {
            var subscriber = await _subscriberSystemRepository.GetDefaultAsync();

            if (null == subscriber)
                throw new Exception("Default EMR NOT SET");
            var clients = new List<ClientPartnerTracingStage>();

            var encounters = _clientEncounterRepository.GetPartnerTracing();
            foreach (var encounter in encounters)
            {
                clients.AddRange(ClientPartnerTracingStage.Create(encounter, subscriber));
            }

            return clients;
        }

        public Task ExtractAndStage()
        {
            throw new NotImplementedException();
        }
    }
}