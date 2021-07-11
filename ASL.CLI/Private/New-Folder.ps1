function New-Folder {
    [CmdletBinding()] #<<-- This turns a regular function into an advanced function
    param (
        [ValidateNotNullOrEmpty()]
        [string] $Location = (Get-Location),
        [Parameter(Mandatory, ValueFromPipeline)]
        [string] $Name
    )

    begin
    {
        $LocationExists = Test-Path -Path $Location -PathType Container

        if (-Not $LocationExists) {
            Write-Error("No such directory : $Location") -ErrorAction Stop
        }
    }
    process
    {
        Write-Verbose "Creating $Name folder ..."
        $CreatedFolder = New-Item -Path $Location -ItemType Directory -Name $Name

        return $CreatedFolder
    }
    end
    {
        Write-Verbose("Folder created : $CreatedFolder")
    }
}