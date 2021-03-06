﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using LiveHAPI.Core.Interfaces.Repository;
using LiveHAPI.Core.Interfaces.Services;
using LiveHAPI.Core.Model.Encounters;
using LiveHAPI.Shared.ValueObject;

namespace LiveHAPI.Core.Service
{
    public class PSmartStoreService : IPSmartStoreService
    {
        private readonly IPSmartStoreRepository _pSmartStoreRepository;
        public PSmartStoreService(IPSmartStoreRepository pSmartStoreRepository)
        {
            _pSmartStoreRepository = pSmartStoreRepository;
        }

        public void Sync(List<PSmartStoreInfo> infos)
        {
            var stores = PSmartStore.Create(infos);
            _pSmartStoreRepository.InsertOrUpdate(stores);
            _pSmartStoreRepository.Save();
        }

        public void Sync(PSmartStoreInfo info)
        {
            var pSmartStore = PSmartStore.Create(info);
            _pSmartStoreRepository.InsertOrUpdate(pSmartStore);
            _pSmartStoreRepository.Save();
        }
    }
}