﻿using LiveHAPI.Core.Model.Lookup;
using LiveHAPI.Core.Model.Network;
using LiveHAPI.Shared.ValueObject;

namespace LiveHAPI.Core.Interfaces.Services
{
    public interface IActivationService
    {
        Practice GetCentral();
        Practice GetLocal();
        MasterFacility Verify(int code);
        Practice EnrollPractice(string code);
        string GetActivationCode(string code, DeviceInfo info, DeviceLocationInfo locationInfo=null);
    }
}