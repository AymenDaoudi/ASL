function Add-DISettings {
    [CmdletBinding()]
    param (
        [Parameter(Mandatory=$true)]
        [ValidateNotNullOrEmpty()]
        [string] $StartupFilePath
    )

    begin
    {
    }
    process
    {
        $ServiceProvider = Get-CodegeneratorServiceProvider
        $Type = [Type]"ASL.CodeGenerator.StartupClass.IStartupClassService"
        $StartupClassService = Resolve-DIService $ServiceProvider $Type

        $task = $StartupClassService.AddRegisterRepositoriesAndRegisterServicesAsync($StartupFilePath)

        # This is to allow the task to finish before asking the result,
        # this will allow stopping the pipeline with CTRL + C to work properly.
        while (-not $task.AsyncWaitHandle.WaitOne(200)) { }

        $null = $task.GetAwaiter().GetResult()
    }
    end
    {
    }
}