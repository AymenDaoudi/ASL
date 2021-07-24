function New-ASLRepository {
    [CmdletBinding()]
    param (
        [Parameter(Mandatory=$true)]
        [ValidateNotNullOrEmpty()]
        [string] $RepositoryName,
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

        $servicesService = $Factory.Invoke([ASL.CodeGenerator.Services.ServiceType]::Repository)

        $SolutionDirectory = Get-SolutionRootDirectory

        if (!$SolutionDirectory) {
            Write-Host "Couldn't find solution or services project"
            Break
        }

        $SolutionName = (Get-Item $SolutionDirectory).Name

        $RepositoryFolderName = $RepositoryName + "s"

        $DomainFolder = New-FolderIfNotExist -Location "$SolutionDirectory\$SolutionName.Domain\Repositories" -Name $RepositoryFolderName
        $DataFolder = New-FolderIfNotExist -Location "$SolutionDirectory\$SolutionName.Data\Repositories" -Name $RepositoryFolderName

        $RepositoryClassName = "$($RepositoryName)Repository"

        $existingRepository = Get-ChildItem "$DataFolder\$RepositoryClassName*" -Include *.cs | Measure-Object

        if ($existingRepository.Count -ne 0) {
            Write-Host "Service already exists."
                break;
        }

        if ($WithInterface) {

            $RepositoryInterfaceName = "I$($RepositoryName)Repository"

            $usings = @("System", "$SolutionName.Domain.Repositories.$RepositoryFolderName")

            $ServicesService.CreateInterface($RepositoryInterfaceName, $DomainFolder, "$SolutionName.Domain.Repositories.$RepositoryFolderName", "System")
            $ServicesService.Create($RepositoryClassName, $DataFolder, "$SolutionName.Data.Repositories.$RepositoryFolderName", $RepositoryInterfaceName, $usings)

            $usings = @("$SolutionName.Data.Repositories.$RepositoryFolderName", "$SolutionName.Domain.Repositories.$RepositoryFolderName")

            Register-Repository $DILifeTime $RepositoryInterfaceName $RepositoryClassName $usings
        }
        else {
            $ServicesService.Create($RepositoryClassName, $DataFolder, "$SolutionName.Domain.Repositories.$RepositoryFolderName", "System")

            Register-Repository $DILifeTime $RepositoryClassName "$SolutionName.Domain.Repositories.$RepositoryFolderName"
        }
    }
    end
    {
    }
}