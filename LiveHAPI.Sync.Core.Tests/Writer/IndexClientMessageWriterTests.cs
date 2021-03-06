﻿using System;
using System.Linq;
using LiveHAPI.Core.Interfaces.Repository;
using LiveHAPI.Infrastructure;
using LiveHAPI.Infrastructure.Repository;
using LiveHAPI.Shared.Custom;
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
        //  private readonly string _baseUrl = "http://localhost:3333";
        private readonly string _baseUrl = "http://localhost/iqcareapi";
        private readonly bool goLive = true;
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
            
            string connectionString=string.Empty;

            if (goLive)
                connectionString = config["connectionStrings:livehAPIConnection"];
            else
                connectionString = config["connectionStrings:hAPIConnection"].Replace("#dir#",
                    TestContext.CurrentContext.TestDirectory.HasToEndWith(@"\"));
                
            
            var options = new DbContextOptionsBuilder<LiveHAPIContext>()
                .UseSqlServer(connectionString)
                .Options;

            _context = new LiveHAPIContext(options);


            _clientPretestStageRepository = new ClientPretestStageRepository(_context);
            _clientEncounterRepository = new ClientEncounterRepository(_context);
            _subscriberSystemRepository = new SubscriberSystemRepository(_context);
            _practiceRepository = new PracticeRepository(_context);
            _clientStageRepository = new ClientStageRepository(_context);

            _clientStageExtractor = new ClientStageExtractor(new PersonRepository(_context), _clientStageRepository, _subscriberSystemRepository,new ClientRepository(_context), new PracticeRepository(_context));
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
                new IndexClientMessageWriter(new RestClient(_baseUrl), _clientMessageLoader,_clientStageRepository);
        }

        [Test]
        [Category("live")]
        public void should_Write_Clients()
        {
            var clientsResponses = _clientMessageWriter.Write().Result.ToList();

            foreach (var message in _clientMessageWriter.Messages)
            {

                Assert.False(string.IsNullOrWhiteSpace(message));

            }

            var stagedIndexClients = _clientStageRepository.GetIndexClients();
            Assert.False(stagedIndexClients.Any());
            if (_clientMessageWriter.Errors.Any())
                foreach (var e in _clientMessageWriter.Errors)
                {
                    Console.WriteLine(e.Message);

                    Console.WriteLine(new string('*', 40));
                }

            foreach (var message in _clientMessageWriter.Messages)
            {
                Console.WriteLine(message);
                Console.WriteLine(new string('|', 40));
            }

            Assert.True(clientsResponses.Any());
            foreach (var response in clientsResponses)
            {
                Console.WriteLine(response);
            }
        }

        [Test]
        public void should_Load_Write_Clients()
        {
            var clients = _clientStageExtractor.ExtractAndStage().Result;
            var pretests = _clientPretestStageExtractor.ExtractAndStage().Result;

            var clientsResponses = _clientMessageWriter.Write().Result.ToList();
            
            foreach (var message in _clientMessageWriter.Messages)
            {
                    
                Assert.False(string.IsNullOrWhiteSpace(message));
               
            }

            var stagedIndexClients = _clientStageRepository.GetIndexClients();
            Assert.False(stagedIndexClients.Any());

          


            if (_clientMessageWriter.Errors.Any())
                foreach (var e in _clientMessageWriter.Errors)
                {
                    Console.WriteLine(e.Message);

                    Console.WriteLine(new string('*', 40));
                }

            foreach (var message in _clientMessageWriter.Messages)
            {
                Console.WriteLine(message);
                Console.WriteLine(new string('|', 40));
            }

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
//            var clients = _clientStageExtractor.ExtractAndStage().Result;
//            var pretests = _clientPretestStageExtractor.ExtractAndStage().Result;

            var clientsResponses = _clientMessageWriter.Write(actions).Result.ToList();
            foreach (var message in _clientMessageWriter.Messages)
                Assert.False(string.IsNullOrWhiteSpace(message));

            if (_clientMessageWriter.Errors.Any())
                foreach (var e in _clientMessageWriter.Errors)
                {
                    Console.WriteLine(e.Message);

                    Console.WriteLine(new string('*', 40));
                }

            foreach (var message in _clientMessageWriter.Messages)
                Console.WriteLine(message);
            Assert.True(clientsResponses.Any());
            foreach (var response in clientsResponses)
            {
                Console.WriteLine(response);
            }
        }
    }
}