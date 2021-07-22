function New-ASLService {
    [CmdletBinding()]
    param (
        [Parameter(Mandatory=$true)]
        [ValidateNotNullOrEmpty()]
        [string] $ServiceName,
        [Parameter(Mandatory=$false)]
        [Switch] $WithInterface = $false
    )

    begin
    {
        $ModulePath = Get-ModuleDirectory
        Use-Depedencies -location "$ModulePath\Dependencies"
    }
    process
    {
        $ServiceProvider = Get-CodegeneratorServiceProvider
        $Type = [type]"ASL.CodeGenerator.IServicesService"

        $SolutionDirectory = Get-SolutionRootDirectory

        if (!$SolutionDirectory) {
            Write-Host "Couldn't find solution or services project"
            Break
        }

        $SolutionName = (Get-Item $SolutionDirectory).Name

        $ServiceFolderName = $ServiceName + "s"

        $ServiceFolder = New-FolderIfNotExist -Location "$SolutionDirectory\$SolutionName.Services\" -Name $ServiceFolderName

        $ServicesService = Resolve-DIService $ServiceProvider $Type

        try {
            if ($WithInterface) {
                $ServicesService.CreateService($ServiceFolder, "$SolutionName.Services.$ServiceFolderName", $ServiceName, $ServiceFolder, "$SolutionName.Services.$ServiceFolderName")
            }
            else {
                $ServicesService.CreateService($ServiceFolder, "$SolutionName.Services.$ServiceFolderName", $ServiceName)
            }
        }
        catch {
            Write-Host "$($_.Exception.Message)"
        }

        New-FolderIfNotExist -Location $ServiceFolder -Name "Models" | Out-Null
        New-FolderIfNotExist -Location $ServiceFolder -Name "Mappings" | Out-Null
        New-FolderIfNotExist -Location $ServiceFolder -Name "Assertions" | Out-Null
    }
    end
    {
    }
}