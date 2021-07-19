function New-ServiceCollectionExtensionsClass {
    [CmdletBinding()] #<<-- This turns a regular function into an advanced function
    param (
        [Parameter(Mandatory=$true)]
        [ValidateNotNullOrEmpty()]
        [string] $location = (Get-Location)
    )

    begin
    {
    }
    process
    {
        $ServiceProvider = Get-CodegeneratorServiceProvider
        $Type = [type]"ASL.CodeGenerator.IServiceCollectionExtensionsService"
        $ServiceCollectionExtensionsService = Resolve-DIService $ServiceProvider $Type

        $usings = @("System", "Microsoft.Extensions.DependencyInjection")
        $ServiceCollectionExtensionsService.CreateFile($location, "Miscellaneous", "System", "Microsoft.Extensions.DependencyInjection")
    }
    end
    {
    }
}