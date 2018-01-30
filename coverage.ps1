.\packages\OpenCover.4.6.519\tools\OpenCover.Console.exe -register:user `
	-output:opencover.xml `
	-filter:"+[ViewSize.Tests]*" `
	-target:.\packages\NUnit.ConsoleRunner.3.8.0\tools\nunit3-console.exe `
	-targetargs:"--where:cat!=Performance --workers=1 .\ViewSize.Tests\bin\Debug\ViewSize.Tests.dll"

.\packages\ReportGenerator.3.1.2\tools\ReportGenerator.exe -reports:opencover.xml -targetdir:coverage
