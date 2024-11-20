using MinimalApi;

IHostBuilder CreateHostBuilder(string[] args) {
    return Host.CreateDefaultBuilder(args)
        .ConfigureWebHostDefaults(webBuilder => 
        {
            webBuilder.UseStartup<StartUp>();
        });
}

CreateHostBuilder(args).Build().Run();