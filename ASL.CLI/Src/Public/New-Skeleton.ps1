using module ..\Exceptions\InvalidProjectParametersException.psm1

function New-Skeleton {
    [CmdletBinding()] #<<-- This turns a regular function into an advanced function
    param (
        [ValidateNotNullOrEmpty()]
        [string] $RootFolder = (Get-Location),
        [Parameter(Mandatory)]
        [string] $SolutionName
    )

    begin
    {
        Use-Depedencies -location "$PSScriptRoot\Dependencies"
    }
    process
    {
        # Create empty solution
        $SolutionLocation = New-Solution $RootFolder $SolutionName

        try {
            # Create main Api project
            New-Project -SolutionLocation $SolutionLocation -SolutionName $SolutionName -ProjectName Api -ProjectType webapi
            Remove-Item "$SolutionLocation\$SolutionName.Api\Controllers\WeatherForecastController.cs"
            Remove-Item "$SolutionLocation\$SolutionName.Api\WeatherForecast.cs"
            New-Folder -Location "$SolutionLocation\$SolutionName.Api" -Name "Dtos" | Out-Null

            # Create Services project
            New-Project -SolutionLocation $SolutionLocation -SolutionName $SolutionName -ProjectName Services -ProjectType classlib
            Remove-Item "$SolutionLocation\$SolutionName.Services\Class1.cs"

            # Create Domain project
            New-Project -SolutionLocation $SolutionLocation -SolutionName $SolutionName -ProjectName Domain -ProjectType classlib
            Remove-Item "$SolutionLocation\$SolutionName.Domain\Class1.cs"

            # Create Data project
            New-Project -SolutionLocation $SolutionLocation -SolutionName $SolutionName -ProjectName Data -ProjectType classlib
            Remove-Item "$SolutionLocation\$SolutionName.Data\Class1.cs"
        }
        catch [System.IO.IOException]{
            Write-Host "$($_.Exception.Message)"
            Write-Verbose "API creation failed, reverting and deleting created elements."
            Remove-Item "$SolutionLocation" -Recurse
            Break
        }
        catch [InvalidProjectParametersException]{
            Write-Host "$($_.Exception.Message)"
            Write-Verbose "API creation failed, reverting and deleting created elements."
            Remove-Item "$SolutionLocation" -Recurse
            Break
        }
        catch{
            throw "$($_.Exception.Message)"
            Write-Verbose "API creation failed, reverting and deleting created elements."
            Remove-Item "$SolutionLocation" -Recurse
            Break
        }

        New-ServiceCollectionExtensionsClass -location "$SolutionLocation\$SolutionName.Api\IServiceCollectionExtensions.cs"

        # Reference Service project in WebApi project
        Write-Verbose "Referencing Services project in Api project..."
        dotnet add "$SolutionLocation\$SolutionName.Api\$SolutionName.Api.csproj" reference "$SolutionLocation\$SolutionName.Services\$SolutionName.Services.csproj"
        Write-Verbose "Services project referenced in Api project successfully."

        # Reference Service project in WebApi project
        Write-Verbose "Referencing Domain project in Data project..."
        dotnet add "$SolutionLocation\$SolutionName.Data\$SolutionName.Data.csproj" reference "$SolutionLocation\$SolutionName.Domain\$SolutionName.Domain.csproj"
        Write-Verbose "Domain project referenced in Data project successfully."

        # Reference Service project in WebApi project
        Write-Verbose "Referencing Data project in Services project..."
        dotnet add "$SolutionLocation\$SolutionName.Services\$SolutionName.Services.csproj" reference "$SolutionLocation\$SolutionName.Data\$SolutionName.Data.csproj"
        Write-Verbose "Data project referenced in Services project successfully."

        # Add Api project to the solution
        Write-Verbose "Adding project $SolutionName.Api to the main solution $SolutionName ..."
        dotnet sln "$SolutionLocation\$SolutionName.sln" add "$SolutionLocation\$SolutionName.Api\$SolutionName.Api.csproj"
        Write-Verbose "Project $SolutionName.Api successfully added to solution $SolutionName."

        # Add Services project to the solution
        Write-Verbose "Adding project $SolutionName.Services to the main solution $SolutionName ..."
        dotnet sln "$SolutionLocation\$SolutionName.sln" add "$SolutionLocation\$SolutionName.Services\$SolutionName.Services.csproj"
        Write-Verbose "Project $SolutionName.Services successfully added to solution $SolutionName."

        # Add Domain project to the solution
        Write-Verbose "Adding project $SolutionName.Domain to the main solution $SolutionName ..."
        dotnet sln "$SolutionLocation\$SolutionName.sln" add "$SolutionLocation\$SolutionName.Domain\$SolutionName.Domain.csproj"
        Write-Verbose "Project $SolutionName.Domain successfully added to solution $SolutionName."

         # Add Data project to the solution
         Write-Verbose "Adding project $SolutionName.Data to the main solution $SolutionName ..."
         dotnet sln "$SolutionLocation\$SolutionName.sln" add "$SolutionLocation\$SolutionName.Data\$SolutionName.Data.csproj"
         Write-Verbose "Project $SolutionName.Data successfully added to solution $SolutionName."
    }
    end
    {
    }
}