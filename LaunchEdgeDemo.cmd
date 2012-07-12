pushd %~dp0
%systemroot%\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe
popd
pushd %~dp0EdgeDemo
..\packages\Ghost.0.3.4\bin\Ghost.exe -v
popd