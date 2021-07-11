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

        # Create main Api project
        New-Project -SolutionLocation $SolutionLocation -SolutionName $SolutionName -ProjectName Api -ProjectType webapi

        # Create main Services project
        New-Project -SolutionLocation $SolutionLocation -SolutionName $SolutionName -ProjectName Services -ProjectType classlib

        # Create main Api project
        New-Project -SolutionLocation $SolutionLocation -SolutionName $SolutionName -ProjectName Domain -ProjectType classlib

        # Create main Api project
        New-Project -SolutionLocation $SolutionLocation -SolutionName $SolutionName -ProjectName Data -ProjectType classlib

        # Add Api project to the solution
        Write-Verbose "Adding project $SolutionName.Api to the main solution $SolutionName ..."
        dotnet sln "$SolutionLocation\$SolutionName.sln" add "$SolutionLocation\$SolutionName.Api\$SolutionName.Api.csproj"
        Write-Verbose "Project $SolutionName.Api successfully added to solution $SolutionName."

        # New-Folder $SolutionLocation "$SolutionName.Api"
    }
    end
    {
    }
}