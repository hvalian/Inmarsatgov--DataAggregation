C:

rmdir C:\Deployment\ig-enms-dataaggregation /s/q

md C:\Deployment\ig-enms-dataaggregation\App
md C:\Deployment\ig-enms-dataaggregation\Service 
md C:\Deployment\ig-enms-dataaggregation\Portal 
md C:\Deployment\ig-enms-dataaggregation\BuildPortal
md C:\Deployment\ig-enms-dataaggregation\Deploy

md C:\Deployment\ig-enms-dataaggregation\BatchFiles
md C:\Deployment\ig-enms-dataaggregation\ConfigurationFiles
md C:\Deployment\ig-enms-dataaggregation\Scripts

DOTNET clean C:\WorkSpace\ig-enms-dataaggregation\enms_aggregation.sln /property:Configuration=Release
DOTNET build C:\WorkSpace\ig-enms-dataaggregation\enms_aggregation.sln /t:Clean;Rebuild /property:Configuration=Release
DOTNET publish -c Release -r win-x64 --output "C:\Deployment\ig-enms-dataaggregation\BuildPortal" "C:\WorkSpace\ig-enms-dataaggregation\Aggregation.WebPortal\Aggregation.WebPortal.csproj"

xcopy C:\WorkSpace\ig-enms-dataaggregation\Aggregation.App\bin\Release\*.* C:\Deployment\ig-enms-dataaggregation\App
xcopy C:\WorkSpace\ig-enms-dataaggregation\Aggregation.WindowsService\bin\Release\*.* C:\Deployment\ig-enms-dataaggregation\Service
xcopy C:\WorkSpace\ig-enms-dataaggregation\Aggregation.Deployment\bin\Release\net6.0-windows\*.* C:\Deployment\ig-enms-dataaggregation\Deploy /s /y

erase C:\Deployment\ig-enms-dataaggregation\App\AggregationApp.exe.config
erase C:\Deployment\ig-enms-dataaggregation\App\AggregationDataAccess.dll.config

erase C:\Deployment\ig-enms-dataaggregation\Service\AggregationService.exe.config
erase C:\Deployment\ig-enms-dataaggregation\Service\AggregationDataAccess.dll.config

erase C:\Deployment\ig-enms-dataaggregation\Portal\appsettings.json

xcopy C:\WorkSpace\ig-enms-dataaggregation\ConfigurationFiles\*.*  C:\Deployment\ig-enms-dataaggregation\ConfigurationFiles /s /y
xcopy C:\WorkSpace\ig-enms-dataaggregation\Scripts\*.*  C:\Deployment\ig-enms-dataaggregation\Scripts /s /y
xcopy C:\WorkSpace\ig-enms-dataaggregation\BatchFiles\InstallService.bat  C:\Deployment\ig-enms-dataaggregation\BatchFiles /s /y
xcopy C:\WorkSpace\ig-enms-dataaggregation\BatchFiles\UnInstallService.bat  C:\Deployment\ig-enms-dataaggregation\BatchFiles /s /y

copy C:\Deployment\ig-enms-dataaggregation\BuildPortal\AggregationDataModels.pdb C:\Deployment\ig-enms-dataaggregation\Portal
copy C:\Deployment\ig-enms-dataaggregation\BuildPortal\web.config C:\Deployment\ig-enms-dataaggregation\Portal
copy C:\Deployment\ig-enms-dataaggregation\BuildPortal\Aggregation.WebPortal.deps.json C:\Deployment\ig-enms-dataaggregation\Portal
copy C:\Deployment\ig-enms-dataaggregation\BuildPortal\Aggregation.WebPortal.runtimeconfig.json C:\Deployment\ig-enms-dataaggregation\Portal
copy C:\Deployment\ig-enms-dataaggregation\BuildPortal\Aggregation.WebPortal.dll C:\Deployment\ig-enms-dataaggregation\Portal
copy C:\Deployment\ig-enms-dataaggregation\BuildPortal\Aggregation.WebPortal.exe C:\Deployment\ig-enms-dataaggregation\Portal
copy C:\Deployment\ig-enms-dataaggregation\BuildPortal\Aggregation.WebPortal.pdb C:\Deployment\ig-enms-dataaggregation\Portal
copy C:\Deployment\ig-enms-dataaggregation\BuildPortal\Aggregation.Services.dll C:\Deployment\ig-enms-dataaggregation\Portal
copy C:\Deployment\ig-enms-dataaggregation\BuildPortal\Aggregation.Services.pdb C:\Deployment\ig-enms-dataaggregation\Portal
copy C:\Deployment\ig-enms-dataaggregation\BuildPortal\AggregationDataModels.dll C:\Deployment\ig-enms-dataaggregation\Portal

rem powershell Compress-Archive C:\Deployment\ig-enms-dataaggregation -Force C:\Deployment\ig-enms-dataaggregation.zip


