@setlocal
@pushd %~dp0

nuget restore || exit /b

msbuild -p:Configuration=Release -t:Restore || exit /b

msbuild -p:Configuration=Release src\test\WixToolsetTest.Tag\WixToolsetTest.Tag.csproj || exit /b
dotnet test -c Release --no-build src\test\WixToolsetTest.Tag || exit /b

msbuild -p:Configuration=Release -t:Pack src\wixext\WixToolset.Tag.wixext.csproj || exit /b

@popd
@endlocal