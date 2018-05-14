﻿using System;
using System.Linq;
using LiveHAPI.Core.Interfaces.Repository;
using LiveHAPI.Infrastructure;
using LiveHAPI.Infrastructure.Repository;
using LiveHAPI.Shared.Enum;
using LiveHAPI.Sync.Core.Extractor;
using LiveHAPI.Sync.Core.Interface.Loaders;
using LiveHAPI.Sync.Core.Interface.Writers;
using LiveHAPI.Sync.Core.Loader;
using LiveHAPI.Sync.Core.Reader;
using LiveHAPI.Sync.Core.Writer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;

namespace LiveHAPI.Sync.Core.Tests.Writer
{
    [TestFixture]
    public class IndexClientMessageWriterTests
    {
        private readonly string _baseUrl = "http://192.168.1.78:3333";

        private LiveHAPIContext _context;
        private IPracticeRepository _practiceRepository;
        private IClientStageRepository _clientStageRepository;
        private IClientPretestStageRepository _clientPretestStageRepository;

        private IClientEncounterRepository _clientEncounterRepository;
        private ISubscriberSystemRepository _subscriberSystemRepository;

        private IIndexClientMessageLoader _clientMessageLoader;
        private IIndexClientMessageWriter _clientMessageWriter;
        private ClientStageExtractor _clientStageExtractor;
        private ClientPretestStageExtractor _clientPretestStageExtractor;


        [SetUp]
        public void SetUp()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
            var connectionString = config["connectionStrings:livehAPIConnection"];
            var options = new DbContextOptionsBuilder<LiveHAPIContext>()
                .UseSqlServer(connectionString)
                .Options;

            _context = new LiveHAPIContext(options);


            _clientPretestStageRepository = new ClientPretestStageRepository(_context);
            _clientEncounterRepository = new ClientEncounterRepository(_context);
            _subscriberSystemRepository = new SubscriberSystemRepository(_context);
            _practiceRepository = new PracticeRepository(_context);
            _clientStageRepository = new ClientStageRepository(_context);

            _clientStageExtractor = new ClientStageExtractor(new PersonRepository(_context), _clientStageRepository, _subscriberSystemRepository);
            _clientPretestStageExtractor = new ClientPretestStageExtractor(_clientStageRepository, _clientPretestStageRepository, _subscriberSystemRepository, _clientEncounterRepository, new ClientRepository(_context));


            _clientMessageLoader =
                new IndexClientMessageLoader(_practiceRepository, _clientStageRepository, _clientPretestStageRepository,
                    new ClientTestingStageExtractor(_clientEncounterRepository, _subscriberSystemRepository),
                    new ClientFinalTestStageExtractor(_clientEncounterRepository, _subscriberSystemRepository),
                    new ClientReferralStageExtractor(_clientEncounterRepository, _subscriberSystemRepository),
                    new ClientTracingStageExtractor(_clientEncounterRepository, _subscriberSystemRepository),
                    new ClientLinkageStageExtractor(_clientEncounterRepository, _subscriberSystemRepository)

                );

            _clientMessageWriter =
                new IndexClientMessageWriter(new RestClient(_baseUrl), _clientMessageLoader);
        }

        [Test]
        public void should_Write_Clients()
        {
            var clients = _clientStageExtractor.ExtractAndStage().Result;
            var pretests = _clientPretestStageExtractor.ExtractAndStage().Result;

            var clientsResponses = _clientMessageWriter.Write().Result.ToList();
            Assert.False(string.IsNullOrWhiteSpace(_clientMessageWriter.Message));

            if (_clientMessageWriter.Errors.Any())
                foreach (var e in _clientMessageWriter.Errors)
                {
                    Console.WriteLine(e.Message);

                    Console.WriteLine(new string('*', 40));
                }

            Console.WriteLine(_clientMessageWriter.Message);
            Assert.True(clientsResponses.Any());
            foreach (var response in clientsResponses)
            {
                Console.WriteLine(response);
            }
        }
        
        [TestCase(LoadAction.RegistrationOnly)]
        [TestCase(LoadAction.Pretest)]
        [TestCase(LoadAction.Pretest,LoadAction.Testing)]
        [TestCase(LoadAction.Pretest,LoadAction.Testing,LoadAction.Referral)]
        [TestCase(LoadAction.Pretest,LoadAction.Testing,LoadAction.Referral,LoadAction.Linkage)]
        [TestCase(LoadAction.Linkage)]
        [TestCase(LoadAction.Tracing)]
        public void should_Write_Client_By_Actions(params LoadAction[] actions)
        {
            var clients = _clientStageExtractor.ExtractAndStage().Result;
            var pretests = _clientPretestStageExtractor.ExtractAndStage().Result;

            var clientsResponses = _clientMessageWriter.Write(LoadAction.RegistrationOnly).Result.ToList();
            Assert.False(string.IsNullOrWhiteSpace(_clientMessageWriter.Message));

            if (_clientMessageWriter.Errors.Any())
                foreach (var e in _clientMessageWriter.Errors)
                {
                    Console.WriteLine(e.Message);

                    Console.WriteLine(new string('*', 40));
                }

            Console.WriteLine(_clientMessageWriter.Message);
            Assert.True(clientsResponses.Any());
            foreach (var response in clientsResponses)
            {
                Console.WriteLine(response);
            }
        }
    }
}