# Sonar.AutoSwitch

Automatically switch Sonar haming configurations when a game is in focus.

<img width="688" alt="image" src="https://user-images.githubusercontent.com/27368554/204064870-da45ebaa-e577-4998-aaa1-4c18386149e1.png">


## How to use

1. Download the app from the release section.
2. Extract the ZIP and launch `Sonar.AutoSwitch.exe`
3. Set the `Per app config` per game / apps
4. Set the `Default config` to apply to all **other** games/apps

### Built With
* [dotnet-7.0](https://dotnet.microsoft.com/download/dotnet-core/7.0)
* [avalonia](https://github.com/AvaloniaUI/Avalonia/discussions/7886)

## Build and run

1. Install .NET 7
2. Go into repo folder
3. Run `dotnet run` in command line

## Publish command

Currently releases uses single file publish - 
```
dotnet publish -c release -r win10-x64 --self-contained=true /p:PublishSingleFile=true
```
