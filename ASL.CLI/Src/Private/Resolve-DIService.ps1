function Resolve-DIService {
    [CmdletBinding()]
    param (
        [Parameter(Mandatory=$true)]
        [ValidateNotNullOrEmpty()]
        [Microsoft.Extensions.DependencyInjection.ServiceProvider] $ServiceProvider,
        [Parameter(Mandatory=$true)]
        [ValidateNotNullOrEmpty()]
        [type] $Type
    )

    begin
    {
    }
    process
    {
        $Service = [Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions].GetMethod("GetService").MakeGenericMethod($Type).Invoke([Microsoft.Extensions.DependencyInjection.ServiceCollectionContainerBuilderExtensions], $ServiceProvider)
        return  $Service
    }
    end
    {
    }
}