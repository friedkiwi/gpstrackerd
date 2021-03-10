if "%2" == "QA" (exit)

if exist "C:\SonarScan.net\SonarScanner.MSBuild.exe" (
   cd %1
   C:\SonarScan.net\SonarScanner.MSBuild.exe begin /k:gpstrackerd
   "C:\Program Files (x86)\Microsoft Visual Studio\2019\Enterprise\MSBuild\Current\Bin\msbuild.exe" /t:rebuild /p:Platform="Any CPU" /p:Configuration=QA
   C:\SonarScan.net\SonarScanner.MSBuild.exe end
)