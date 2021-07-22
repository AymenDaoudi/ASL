function Get-SolutionRootDirectory {
    [CmdletBinding()]
    param (
        $currentLocation = (Get-Location)
    )

    begin
    {
    }
    process
    {
        if (!$currentLocation) {
            $currentLocation = Get-Location
        }

        $ContainsSolutionFile = Get-ChildItem "$currentLocation\*" -Include *.sln | Measure-Object

        if ($ContainsSolutionFile.Count -ne 0) {
            return $currentLocation
        }

        $HasParent = (get-item $currentLocation).Parent

        if (!$HasParent) {
            return $null
        }

        $Parent = ((get-item $currentLocation).Parent).FullName
        return Get-SolutionRootDirectory $Parent

    }
    end
    {
    }
}