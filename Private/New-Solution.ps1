function New-Solution {
    [CmdletBinding()]
    param (
        [ValidateNotNullOrEmpty()]
        [string] $RootFolder = (Get-Location),
        [Parameter(Mandatory)]
        [string] $SolutionName
    )

    begin{

    }
    process{

        # Create Solution folder
        $SolutionLocation = New-Folder -Location $RootFolder -Name $SolutionName
        Write-Verbose "Created folder $SolutionLocation ..."

        # Create Solution
        Write-Verbose "Creating empty solution $SolutionName ..."
        dotnet new sln --name $SolutionName --output $SolutionLocation | Out-Null
        Write-Verbose "Empty solution $SolutionName created successfully."

        return $SolutionLocation
    }
    end{

    }
}