﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LiveHAPI.Core.Interfaces.Repository;
using LiveHAPI.Core.Model.Exchange;
using LiveHAPI.Shared.Custom;
using LiveHAPI.Sync.Core.Interface.Extractors;

namespace LiveHAPI.Sync.Core.Extractor
{
    public class ClientFamilyTracingStageExtractor : IClientFamilyTracingStageExtractor
    {
        private readonly IContactsEncounterRepository _clientEncounterRepository;
       private readonly ISubscriberSystemRepository _subscriberSystemRepository;

        public ClientFamilyTracingStageExtractor(IContactsEncounterRepository clientEncounterRepository, ISubscriberSystemRepository subscriberSystemRepository)
        {
            _clientEncounterRepository = clientEncounterRepository;
            _subscriberSystemRepository = subscriberSystemRepository;
        }

        public async Task<IEnumerable<ClientFamilyTracingStage>> Extract(Guid? htsClientId = null)
        {
            var subscriber = await _subscriberSystemRepository.GetDefaultAsync();

            if (null == subscriber)
                throw new Exception("Default EMR NOT SET");
            var clients = new List<ClientFamilyTracingStage>();

            var encounters = _clientEncounterRepository.GetFamilyTracing(htsClientId);
            foreach (var encounter in encounters)
            {
                clients.AddRange(ClientFamilyTracingStage.Create(encounter, subscriber));
            }

            return clients.Where(x => !x.Id.IsNullOrEmpty());
        }

        public Task<int> ExtractAndStage()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            _clientEncounterRepository?.Dispose();
            _subscriberSystemRepository?.Dispose();
        }
    }
}