var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.MeraStore_Services_Cross_Cutting_Api>("merastore-services-cross-cutting-api");

builder.Build().Run();
