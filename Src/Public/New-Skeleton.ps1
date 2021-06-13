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
    }
    process
    {
        # Create Solution folder
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
            Remove-Item "$SolutionLocation"
            Exit
        }
        catch [InvalidProjectParametersException]{
            Write-Host "$($_.Exception.Message)"
            Remove-Item "$SolutionLocation"
            Exit
        }
        catch{
            throw "$($_.Exception.Message)"
            Remove-Item "$SolutionLocation"
            Exit
        }

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