function Use-Depedencies {
    [CmdletBinding()]
    param (
        [Parameter(Mandatory=$true)]
        [ValidateNotNullOrEmpty()]
        [string] $location
    )

    begin
    {
    }
    process
    {
        $ModuleLocation = (Get-ModuleDirectory "ASL").FullName
        $DepedenciesFolder = "$ModuleLocation\Dependencies"

        $ASL_CodeGenerator = Resolve-Path "$DepedenciesFolder\ASL.CodeGenerator.dll"
        $DI = Resolve-Path "$DepedenciesFolder\Microsoft.Extensions.DependencyInjection.dll"
        $DIAbstractions = Resolve-Path "$DepedenciesFolder\Microsoft.Extensions.DependencyInjection.Abstractions.dll"
        $CSCGAbstract = Resolve-Path "$DepedenciesFolder\CSCG.Abstract.dll"
        $CSCGRoslyn = Resolve-Path "$DepedenciesFolder\CSCG.Roslyn.dll"
        $CodeAnalaysisCSharp = Resolve-Path "$DepedenciesFolder\Microsoft.CodeAnalysis.CSharp.dll"
        $CodeAnalaysis = Resolve-Path "$DepedenciesFolder\Microsoft.CodeAnalysis.dll"

        Add-Type -Path $ASL_CodeGenerator
        Add-Type -Path $DI
        Add-Type -Path $DIAbstractions
        Add-Type -Path $CSCGAbstract
        Add-Type -Path $CSCGRoslyn
        Add-Type -Path $CodeAnalaysisCSharp
        Add-Type -Path $CodeAnalaysis
    }
    end
    {
    }
}