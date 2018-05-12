﻿using System.Collections.Generic;
using System.Threading.Tasks;
using LiveHAPI.Sync.Core.Exchange;

namespace LiveHAPI.Sync.Core.Interface.Writers
{
    public interface IClientWriter<T>
    {
        string Message { get; }
        List<SendError> Errors { get; }
        Task<IEnumerable<SynchronizeClientsResponse>> Write();
    }
}