# if (Test-Path (Join-Path -Path (Split-Path -Path $PSScriptRoot -Parent) -ChildPath "\Nested\FileManagement\Exceptions\InvalidProjectParametersException.psm1")) {
#     #this is to get the class definitions shared between modules
#     $ModuleRoot = Split-Path -Path $PSScriptRoot -Parent
#     $InvalidProjectParametersException = Join-Path -Path $ModuleRoot -ChildPath ""\Nested\FileManagement\Exceptions\InvalidProjectParametersException.psm1"";
#     Write-Host "using module $InvalidProjectParametersException"
#     $useBlock = [ScriptBlock]::Create("using module $InvalidProjectParametersException")
#     . $useBlock
# }

using module ..\Exceptions\InvalidProjectParametersException.psm1

function New-Project {
    [CmdletBinding()] #<<-- This turns a regular function into an advanced function
    param (
        [Parameter(Mandatory)]
        [string] $SolutionLocation,
        [Parameter(Mandatory)]
        [string] $SolutionName,
        [Parameter(Mandatory)]
        [ValidateNotNullOrEmpty()]
        [ValidateSet('Api','Services','Domain','Data')]
        [string] $ProjectName,
        [Parameter(Mandatory)]
        [ValidateNotNullOrEmpty()]
        [ValidateSet('webapi','classlib')]
        [string] $ProjectType
    )

    begin
    {
    }
    process
    {
        if ((($ProjectName -ieq 'Api') -And ($ProjectType -ine 'webapi')) -Or (($ProjectType -ieq 'webapi') -And ($ProjectName -ine 'Api'))) {
            throw [InvalidProjectParametersException] "Incompatible project name ""$ProjectName"" and project type ""$ProjectType"""
        }

        # Create project
        Write-Verbose "Creating project $SolutionName.$ProjectName ..."
        try {
            # Create project folder
            New-Folder -Location $SolutionLocation -Name "$SolutionName.$ProjectName" | Out-Null
        }
        catch [System.IO.IOException]{
            Write-Host "$($_.Exception.Message)"; Exit
        }
        catch{
            throw "$($_.Exception.Message)"
        }
        Write-Verbose "Created folder $SolutionName.$ProjectName."
        dotnet new $ProjectType --language "C#" --name "$SolutionName.$ProjectName" --output "$SolutionLocation\$SolutionName.$ProjectName" | Out-Null
        Write-Verbose "Project $SolutionName.$ProjectName created successfully."
    }
    end
    {
    }
}