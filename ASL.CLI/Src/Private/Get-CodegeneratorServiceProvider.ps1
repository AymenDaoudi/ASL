function Get-CodegeneratorServiceProvider {
    param ()

    begin
    {
    }
    process
    {
        $Services = [ASL.CodeGenerator.Startup]::Services
        $ServiceProvider = [Microsoft.Extensions.DependencyInjection.ServiceCollectionContainerBuilderExtensions]::BuildServiceProvider($Services)

        return $ServiceProvider
    }
    end
    {
    }
}