function New-FolderIfNotExist {
    [CmdletBinding()]
    param (
        [ValidateNotNullOrEmpty()]
        [string] $Location = (Get-Location),
        [Parameter(Mandatory, ValueFromPipeline)]
        [string] $Name
    )

    begin
    {
    }
    process
    {
        if (!(test-path "$Location\$Name")) {
            $NewDirectory = New-Item -ItemType Directory -Name $Name -Path $Location
        }
        else {
            $NewDirectory = "$Location\$Name"
        }

        return $NewDirectory
    }
    end
    {
    }
}