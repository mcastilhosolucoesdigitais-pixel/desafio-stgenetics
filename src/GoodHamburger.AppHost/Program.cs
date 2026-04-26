var builder = DistributedApplication.CreateBuilder(args);

var api = builder.AddProject<Projects.GoodHamburger_Api>("api");

builder.AddProject<Projects.GoodHamburger_Web>("web")
       .WithReference(api)
       .WithEnvironment("ApiBaseUrl", api.GetEndpoint("http"));

builder.Build().Run();
