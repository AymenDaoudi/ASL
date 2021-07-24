function Register-Repository {
    [CmdletBinding()]
    param (
        [Parameter(Mandatory)]
        [ValidateNotNullOrEmpty()]
        [ValidateSet('Scoped','Transient','Singleton')]
        [string] $DILifeTime,
        [Parameter(Mandatory)]
        [ValidateNotNullOrEmpty()]
        [string] $ConcreteRepositoryName,
        [Parameter(Mandatory = $false)]
        [string] $AbstractRepositoryName,
        [Parameter(Mandatory = $true)]
        [ValidateNotNullOrEmpty()]
        [String[]]$Usings
    )

    begin
    {
        $ModulePath = Get-ModuleDirectory
        Use-Depedencies -location "$ModulePath\Dependencies"
    }
    process
    {
        $SolutionDirectory = Get-SolutionRootDirectory
        $ServiceCollectionExtensionsFilePath = "$SolutionDirectory\$SolutionName.Api\IServiceCollectionExtensions.cs"

        $Type = [type]"ASL.CodeGenerator.ServiceCollectionExtensions.IServiceCollectionExtensionsService"
        $ServiceCollectionExtensionsService = Resolve-DIService $ServiceProvider $Type

        switch ($DILifeTime) {
            'Scoped' { $DILifeTime = [ASL.CodeGenerator.DILifetime]::Scoped }
            'Transient' { $DILifeTime = [ASL.CodeGenerator.DILifetime]::Transient }
            'Singleton' { $DILifeTime = [ASL.CodeGenerator.DILifetime]::Singleton }
            Default { $DILifeTime = [ASL.CodeGenerator.DILifetime]::Scoped }
        }

        if ($AbstractRepositoryName) {
            $task = $ServiceCollectionExtensionsService.RegisterNewRepositoryAsync($ServiceCollectionExtensionsFilePath, $DILifeTime, $ConcreteRepositoryName, $AbstractRepositoryName, $Usings)
        }
        else {
            #Implement other cases later
        }

        # This is to allow the task to finish before asking the result,
        # this will allow stopping the pipeline with CTRL + C to work properly.
        while (-not $task.AsyncWaitHandle.WaitOne(200)) { }

        $null = $task.GetAwaiter().GetResult()
    }
    end
    {
    }
}