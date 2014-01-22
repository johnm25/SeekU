﻿using System;
using SampleDomain.Commands;
using SampleDomain.Domain;
using SeekU;
using SeekU.Azure.Eventing;
using SeekU.Commanding;
using SeekU.Eventing;
using SeekU.StructureMap;
using StructureMap;

namespace AzureStorageSample
{
    class Program
    {
        static void Main(string[] args)
        {
            // Use MongoDB for event and snapshot storage
            var host = new SeekUHostConfiguration<SeekUDemoDependencyResolver>();
            host.ForSnapshotStore().Use<AzureBlobSnapshotStore>(store =>
            {
                store.ConnectionString = "DefaultEndpointsProtocol=https;AccountName=[Your account name];AccountKey[Your account key]";
            });

            var bus = host.GetCommandBus();

            // I'm not a proponent of Guids for primary keys.  This method returns
            // a sequential Guid to make database sorting behave like integers.
            // http://www.informit.com/articles/article.asp?p=25862
            var id = SequentialGuid.NewId();

            // Create the account
            bus.Send(new CreateNewAccountCommand(id, 95));

            // Use the account to create a history of events including a snapshot
            bus.Send(new DebitAccountCommand(id, 5));
            bus.Send(new CreditAccountCommand(id, 12));
            bus.Send(new DebitAccountCommand(id, 35));

            Console.Read();
        }
    }

    public class SeekUDemoDependencyResolver : SeekUStructureMapResolver
    {
        public SeekUDemoDependencyResolver()
        {
            Container.Configure(x => x.Scan(scan =>
            {
                scan.TheCallingAssembly();
                scan.AssemblyContainingType<BankAccount>();
                scan.WithDefaultConventions();
                scan.ConnectImplementationsToTypesClosing(typeof(IHandleCommands<>));
                scan.ConnectImplementationsToTypesClosing(typeof(IHandleDomainEvents<>));
            }));

            Container = ObjectFactory.Container;
        }
    }
}
