.\packages\OpenCover.4.6.519\tools\OpenCover.Console.exe -register:user -filter:"+[ViewSize.Tests]*" -target:.\packages\NUnit.ConsoleRunner.3.7.0\tools\nunit3-console.exe -targetargs:"--where:cat!=Performance --workers=1 .\ViewSize.Tests\bin\Debug\ViewSize.Tests.dll"
.\packages\ReportGenerator.3.0.2\tools\ReportGenerator.exe -reports:results.xml -targetdir:coveragereport
