==================

## Setup:

==================

1. chmod +x <>.sh -> And test that working without sudo or extra efford.
2. @Change: \*.sh paths

==================

## Creating:

==================

1. dotnet new worker -n Portfolio.Core.CleanAndBackup.Worker
2. Setup the logger and copy paste the packages that i used to \*csproj if i have problem identifying them.
3. Write the Worker code
4. dotnet build
5. "dotnet publish -c Release -r <runtime-identifier> --self-contained"
   <runtime-identifier> : For Windows: win-x64
   For Linux: linux-x64
   For macOS: osx-x64
   In my case: "dotnet publish -c Release -r linux-x64 --self-contained"
6. cd bin/Release/net8.0/<runtime-identifier>/publish -> In my case: "cd bin/Release/net8.0/linux-x64/publish"
7. "./Portfolio.Core.CleanAndBackup.Worker" -> When want to stop it just ctrl+C
   Or if i want to run it detached (create a systemd file or:")

- nohup ./Portfolio.Core.CleanAndBackup.Worker > output.Portfolio.Core.CleanAndBackup.Worker.log 2>&1 &
- ps aux | grep Portfolio.Core.CleanAndBackup.Worker | grep -v grep
- sudo kill -9 <pid>

==================

## Debug:

==================

1. Create the .vscode/launfh.json:

---

{
"version": "0.2.0",
"configurations": [
{
"name": ".NET Core Launch (console)",
"type": "coreclr",
"request": "launch",
"preLaunchTask": "build",
"program": "${workspaceFolder}/bin/Debug/net6.0/MyApp.dll",
        "args": [],
        "cwd": "${workspaceFolder}",
"stopAtEntry": false,
"serverReadyAction": {
"action": "openExternally",
"pattern": "\\bNow listening on:\\s+http://\\S+:([0-9]+)",
},
"env": {
"ASPNETCORE_ENVIRONMENT": "Development"
},
"console": "internalConsole",
"internalConsoleOptions": "openOnSessionStart"
}
]
}

(If failed to find bin -> get as message go to Debug Vscode section and on the Run and Debug change the path -> There is supportive image: "Debug.png" select the c# and then select one of the choices that will be displayed)
