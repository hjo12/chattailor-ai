# ChatTailorAI.WinUI

This solution is the WinUI3 project for ChatTailor AI.

## Getting Started

Add a reference to the ChatTailorAI library in your project

## Troubleshooting

If errors occur during startup with store engagement, try installing first through nuget then retry. If it still fails, try here: https://marketplace.visualstudio.com/items?itemName=AdMediator.MicrosoftStoreServicesSDK

### Issues Post UWP -> WinUI3 Migration

- `Release` mode fails when `PublishTrimmed` is set to True, failing on Newtonsoft.json and EFCore exceptions
  - Most likely due to using less-strict types like 'dynamic' that haven't been refactored after the migration to WinUI3
	- See if the below additions in .csproj help when published trim is set to true and fails:
	  ```xml
	  <PropertyGroup>
  		<PublishTrimmed>true</PublishTrimmed>
		<TrimmerShowWarnings>true</TrimmerShowWarnings>
		<TrimmerVerbosity>detailed</TrimmerVerbosity>
	  </PropertyGroup>
	  ```
