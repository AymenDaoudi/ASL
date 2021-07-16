function Install-Dependencies {
    [CmdletBinding()] #<<-- This turns a regular function into an advanced function
    param (
        [Parameter(Mandatory=$true)]
        [ValidateNotNullOrEmpty()]
        [string] $InstallationFolder = (Get-Location)
    )

    begin
    {
    }
    process
    {
        try {
            Write-Host "Installing Nuget package provider if not installed.";
            $NugetPackageProvider = Get-PackageProvider -Name NuGet | Install-PackageProvider -Force
        }
        catch [Exception]{
            Write-Host "$($_.Exception.Message)";
        }

        $NugetPackageSource = Get-PackageSource -ProviderName NuGet | where {$_.Location -eq "https://www.nuget.org/api/v2"}

        if (-Not $NugetPackageSource) {

            Write-Host "Registering Nuget.org package source.";
            $NugetPackageSource = Register-PackageSource -Name MyNuGet -Location https://www.nuget.org/api/v2 -ProviderName NuGet
        }
        else {
            Write-Host "Package source: https://www.nuget.org/api/v2, already registered.";
        }

        $DepdendenciesFolder = New-Item -ItemType Directory -Path $InstallationFolder -Name Dependencies

        Write-Host "Downloading dependencies ...";
        Find-Package -Name ApiSecriptingLibrary.CodeGenerator -Source $NugetPackageSource.Name | Install-Package -Scope CurrentUser -Destination $DepdendenciesFolder -Force

        $DotnetStandardFolders = Get-ChildItem -Recurse $DepdendenciesFolder | where { $_.PSIsContainer } | where {$_.Name -contains "netstandard2.1"}

        Write-Host "Installing dependencies ...";
        Foreach($folder in $DotnetStandardFolders)
        {
            Try
            {
                $dlls = Get-ChildItem -Recurse $folder | where {! $_.PSIsContainer } | where {$_ -like "*.dll"}

                Foreach($dll in $dlls)
                {
                    Move-Item -Path $dll -Destination $DepdendenciesFolder
                }

            }
            Catch
            {
                Write-Error -Message "Failed to Installing dependency: $_"
            }
        }

        Get-ChildItem $DepdendenciesFolder -Directory | Remove-Item -Recurse -Force
    }
    end
    {
    }
}