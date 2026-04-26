var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.GoodHamburger_Api>("api");

builder.Build().Run();
