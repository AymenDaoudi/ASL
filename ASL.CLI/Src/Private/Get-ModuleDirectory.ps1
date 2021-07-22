function Get-ModuleDirectory {
    [CmdletBinding()]
    param (
        [Parameter(Mandatory=$false)]
        [ValidateNotNullOrEmpty()]
        [string] $ModuleName = "ASL"
    )

    begin
    {
    }
    process
    {
        $parentFolder = (get-item $PSScriptRoot).Parent
        $parentFolderName = $parentFolder.Name

        while ($parentFolderName -cne $ModuleName) {
            $parentFolder = $parentFolder.Parent
            $parentFolderName = $parentFolder.Name
        }

        return $parentFolder
    }
    end
    {
    }
}