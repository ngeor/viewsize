set -e
msbuild ViewSizeMac.sln
mono ./packages/NUnit.ConsoleRunner.3.8.0/tools/nunit3-console.exe \
	--where:cat!=Performance --workers=1 \
    ./ViewSize.Tests/bin/Debug/ViewSize.Tests.dll
