﻿using System;

namespace LiveHAPI.Shared.Interfaces.Model
{
    public interface IAddress
    {
        string Landmark { get; set; }
        int? CountyId { get; set; }
        decimal? Lat { get; set; }
        decimal? Lng { get; set; }
     
    }
}