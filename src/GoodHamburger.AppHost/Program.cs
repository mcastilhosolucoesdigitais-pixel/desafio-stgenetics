var builder = DistributedApplication.CreateBuilder(args);

var api = builder.AddProject<Projects.GoodHamburger_Api>("api")
                 .WithEndpoint("http", e => e.Port = 5001);

builder.AddProject<Projects.GoodHamburger_Web>("web")
       .WithEndpoint("http", e => e.Port = 5002)
       .WithReference(api)
       .WithEnvironment("ApiBaseUrl", api.GetEndpoint("http"));

builder.Build().Run();
