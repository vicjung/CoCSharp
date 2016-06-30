﻿using CoCSharp.Csv;
using CoCSharp.Data.Models;
using CoCSharp.Logic;
using CoCSharp.Network;
using CoCSharp.Network.Messages.Commands;
using CoCSharp.Server.Core;

namespace CoCSharp.Server.Handlers.Commands
{
    public static partial class CommandHandlers
    {
        public static void HandleUpgradeBuildableCommand(CoCServer server, CoCRemoteClient client, Command command)
        {
            var ubCommand = (UpgradeBuildableCommand)command;
            var token = new VillageObjectToken(server, client);
            var buildable = client.Avatar.Home.GetVillageObject<Buildable>(ubCommand.BuildableGameID);
            if (buildable.IsConstructing)
            {
                FancyConsole.WriteLine(BuildableAlreadyInConstructionFormat, client.Avatar.Token, ubCommand.BuildableGameID);
                return;
            }

            var data = (CsvData)null;
            if (buildable is Building)
            {
                data = server.AssetManager.SearchCsv<BuildingData>(buildable.GetDataID(), buildable.Level + 1);
            }
            else if (buildable is Trap)
            {
                data = server.AssetManager.SearchCsv<TrapData>(buildable.GetDataID(), buildable.Level + 1);
            }

            buildable.ConstructionFinished += ConstructionFinished;

            buildable.UserToken = token;
            buildable.Data = data;
            buildable.BeginConstruction();

            FancyConsole.WriteLine(StartedConstructionFormat, client.Avatar.Token, buildable.X, buildable.Y, buildable.Level);
        }
    }
}
