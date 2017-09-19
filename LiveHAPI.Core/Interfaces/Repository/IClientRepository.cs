﻿using System;
using LiveHAPI.Core.Model.People;

namespace LiveHAPI.Core.Interfaces.Repository
{
    public interface IClientRepository : IRepository<Client,Guid>
    {
        Client GetClient(Guid id);
    }
}