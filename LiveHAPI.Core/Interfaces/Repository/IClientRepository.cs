﻿using System;
using System.Collections.Generic;
using LiveHAPI.Core.Model.Exchange;
using LiveHAPI.Core.Model.People;
using LiveHAPI.Shared.Enum;

namespace LiveHAPI.Core.Interfaces.Repository
{
    public interface IClientRepository : IRepository<Client,Guid>
    {
        Client GetClient(Guid id, bool withIds = true);
        Client GetClientStates(Guid id);
        IEnumerable<PersonMatch> GetById(Guid id);
        IEnumerable<PersonMatch> GetRelationsById(Guid id);
        IEnumerable<PersonMatch> Search(string searchItem);
        void UpdateIds(List<ClientIdentifier> identifiers);
        void UpdateTempRelations(List<ClientRelationship> identifiers);
        void UpdateClientState(Guid clientId, List<ClientState> clientStates);
        void UpdateRelationships();
        void UpdateSyncStatus(IEnumerable<ClientStage> clientStages);
    }
}