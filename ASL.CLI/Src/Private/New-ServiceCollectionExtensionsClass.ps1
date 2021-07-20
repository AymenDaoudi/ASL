﻿function New-ServiceCollectionExtensionsClass {
    [CmdletBinding()]
    param (
        [Parameter(Mandatory=$true)]
        [ValidateNotNullOrEmpty()]
        [string] $location = (Get-Location),
        [Parameter(Mandatory=$true)]
        [ValidateNotNullOrEmpty()]
        [string] $namespace = (Get-Location)
    )

    begin
    {
    }
    process
    {
        $ServiceProvider = Get-CodegeneratorServiceProvider
        $Type = [type]"ASL.CodeGenerator.IServiceCollectionExtensionsService"
        $ServiceCollectionExtensionsService = Resolve-DIService $ServiceProvider $Type

        $ServiceCollectionExtensionsService.CreateFile($location, $namespace, "Microsoft.Extensions.DependencyInjection")
    }
    end
    {
    }
}