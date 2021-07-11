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
            Write-Error "Incompatible project name and project type" -ErrorAction Stop | Out-Null
        }

        # Create Api project
        Write-Verbose "Creating web api project $SolutionName.$ProjectName ..."
        New-Item -ItemType Directory -Path $SolutionLocation -Name "$SolutionName.$ProjectName" | Out-Null
        dotnet new $ProjectType --language "C#" --name "$SolutionName.$ProjectName" --output "$SolutionLocation\$SolutionName.$ProjectName" | Out-Null
        Write-Verbose "Project $SolutionName.$ProjectName created successfully."
    }
    end
    {
    }
}