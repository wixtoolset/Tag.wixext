@setlocal
@pushd %~dp0

nuget restore

msbuild -p:Configuration=Release -t:Restore

msbuild -p:Configuration=Release src\test\WixToolsetTest.Tag\WixToolsetTest.Tag.csproj

msbuild -p:Configuration=Release -t:Pack src\wixext\WixToolset.Tag.wixext.csproj

@popd
@endlocal