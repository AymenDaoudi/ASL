function New-ASLService {
    [CmdletBinding()]
    param (
        [Parameter(Mandatory=$true)]
        [ValidateNotNullOrEmpty()]
        [string] $ServiceName,
        [Parameter(Mandatory=$false)]
        [Switch] $WithInterface = $false,
        [Parameter(Mandatory)]
        [ValidateNotNullOrEmpty()]
        [ValidateSet('Scoped','Transient','Singleton')]
        [string] $DILifeTime
    )

    begin
    {
        $ModulePath = Get-ModuleDirectory
        Use-Depedencies -location "$ModulePath\Dependencies"
    }
    process
    {
        $ServiceProvider = Get-CodegeneratorServiceProvider
        $FactoryType = [Type]"System.Func[ASL.CodeGenerator.Services.ServiceType, ASL.CodeGenerator.Services.IServicesService]"

        [System.Func[ASL.CodeGenerator.Services.ServiceType, ASL.CodeGenerator.Services.IServicesService]]$Factory = Resolve-DIService $ServiceProvider $FactoryType

        $servicesService = $Factory.Invoke([ASL.CodeGenerator.Services.ServiceType]::Service)

        $SolutionDirectory = Get-SolutionRootDirectory

        if (!$SolutionDirectory) {
            Write-Host "Couldn't find solution or services project"
            Break
        }

        $SolutionName = (Get-Item $SolutionDirectory).Name

        $ServiceFolderName = $ServiceName + "s"

        $ServiceFolder = New-FolderIfNotExist -Location "$SolutionDirectory\$SolutionName.Services\" -Name $ServiceFolderName

        $ServiceClassName = "$($ServiceName)Service"

        $existingService = Get-ChildItem "$ServiceFolder\$ServiceClassName*" -Include *.cs | Measure-Object

        if ($existingService.Count -ne 0) {
            Write-Host "Service already exists."
            break;
        }

        if ($WithInterface) {

            $ServiceInterfaceName = "I$($ServiceName)Service"

            $ServicesService.CreateInterface($ServiceInterfaceName, $ServiceFolder, "$SolutionName.Services.$ServiceFolderName", "System")
            $ServicesService.Create($ServiceClassName, $ServiceFolder, "$SolutionName.Services.$ServiceFolderName", $ServiceInterfaceName, "System")

            Register-Service $DILifeTime $ServiceInterfaceName $ServiceClassName -Usings "$SolutionName.Services.$ServiceFolderName"
        }
        else {
            $ServicesService.Create($ServiceClassName, $ServiceFolder, "$SolutionName.Services.$ServiceFolderName", "System")

            Register-Service $DILifeTime $ServiceClassName -Usings "$SolutionName.Services.$ServiceFolderName"
        }

        New-FolderIfNotExist -Location $ServiceFolder -Name "Models" | Out-Null
        New-FolderIfNotExist -Location $ServiceFolder -Name "Mappings" | Out-Null
        New-FolderIfNotExist -Location $ServiceFolder -Name "Assertions" | Out-Null
    }
    end
    {
    }
}