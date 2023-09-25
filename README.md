# Sonar.AutoSwitch

Automatically switch Sonar gaming configurations when a game is in focus.

<img width="688" alt="image" src="https://user-images.githubusercontent.com/27368554/204064870-da45ebaa-e577-4998-aaa1-4c18386149e1.png">

## Contribution to game database

Auto switch will switch based on game database in `game_database.json`.  
Feel free to create pull requests to add more games, the schema of the JSON is -
```json
{
  "SonarProfileName": "Game Name", // Game Name as shown in UI (required)
  "ExeName": "FileName", // Filter based on Exe name without the .exe (optional)
  "Title": "WindowTitle" // Filter based on window title (optional)
}
```
`ExeName` or `Title` must be filled for it to work.


## How to use

1. Download the app from the release section.
2. Extract the ZIP and launch `Sonar.AutoSwitch.exe`
3. Set the `Per app config` per game / apps
4. Set the `Default config` to apply to all **other** games/apps
5. To set game, set the `Executable name` of the app. For some games like Valorant it won't work, if that's the case then use `Title` instead and leave the `Executable name` empty.

### Built With
* [dotnet-7.0](https://dotnet.microsoft.com/download/dotnet-core/7.0)
* [avalonia](https://github.com/AvaloniaUI/Avalonia/)

## Build and run

1. Install .NET 7
2. Go into repo folder
3. Run `dotnet run` in command line

## Publish command

Currently releases uses single file publish - 
```
dotnet publish -c release -r win10-x64 --self-contained=true /p:PublishSingleFile=true
```
