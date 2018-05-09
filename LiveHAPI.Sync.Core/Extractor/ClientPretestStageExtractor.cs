﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LiveHAPI.Core.Interfaces.Repository;
using LiveHAPI.Core.Model.Exchange;
using LiveHAPI.Sync.Core.Interface.Extractors;

namespace LiveHAPI.Sync.Core.Extractor
{
    public class ClientPretestStageExtractor : IClientPretestStageExtractor
    {
        private readonly IClientEncounterRepository _clientEncounterRepository;
        private readonly IClientStageRepository _clientStageRepository;

        private readonly IClientPretestStageRepository _clientPretestStageRepository;
        private readonly ISubscriberSystemRepository _subscriberSystemRepository;

        public ClientPretestStageExtractor(IClientStageRepository clientStageRepository, IClientPretestStageRepository clientPretestStageRepository,
            ISubscriberSystemRepository subscriberSystemRepository, IClientEncounterRepository clientEncounterRepository)
        {
            _clientStageRepository = clientStageRepository;
            _clientPretestStageRepository = clientPretestStageRepository;
            _subscriberSystemRepository = subscriberSystemRepository;
            _clientEncounterRepository = clientEncounterRepository;
        }

        public async Task<IEnumerable<ClientPretestStage>> Extract()
        {
            var subscriber = await _subscriberSystemRepository.GetDefaultAsync();

            if (null == subscriber)
                throw new Exception("Default EMR NOT SET");
            var pretestStages = new List<ClientPretestStage>();

            var clientIds = _clientStageRepository.GetAll().Select(x => x.ClientId).ToList();

            foreach (var clientId in clientIds)
            {
                //Pretests   

                var finalResults = _clientEncounterRepository.GetFinalTesting(clientId).ToList();
                if (finalResults.Any())
                {

                }
            }


//            var persons = _personRepository.GetAllClients();
//            foreach (var person in persons)
//            {
//                clients.Add(ClientPretestStage.Create(person, subscriber));
//            }




            _clientPretestStageRepository.BulkInsert(pretestStages);

            return _clientPretestStageRepository.GetAll();
        }
    }
}