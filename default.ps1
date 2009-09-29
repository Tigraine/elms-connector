properties {
    $castleTrunk = '..\open-source\castle-trunk\'
    $compilecastle = $FALSE
    $config = 'debug'
    $showtestresult = $FALSE
    $base_dir = resolve-path .
    $lib_dir = "$base_dir\lib\"
}

task Clean {
    if (Test-Path .\build)
    {
        Remove-Item .\build -recurse -force
        Write-Host "Successfully removed build folder"
    }
    else
    {
        Write-Host "No build folder to delete"
    }
}

task Init {
    if (!(Test-Path .\build))
    {
        mkdir .\build
    }
}

task Build -depends Init {
    msbuild src\ElmsConnector\ElmsConnector.csproj /p:Configuration=$config
}

task Build-Tests -depends Build {
    msbuild src\ElmsConnector.Tests\ElmsConnector.Tests.csproj
    .\lib\xunit\xunit.console.exe .\build\$config\ElmsConnector.Tests.dll /html .\build\$config\TestResult.htm
    if ($showtestresult)
    {
        start .\build\$config\TestResult.htm
    }
}

task Merge -depends Build-Tests {
    $old = pwd
    cd .\build\$config\
    Remove-Item ElmsConnector-partial.dll -ErrorAction SilentlyContinue
    Rename-Item ElmsConnector.dll ElmsConnector-partial.dll
    write-host "Executing ILMerge"
    & $lib_dir\ilmerge\ILMerge.exe ElmsConnector-partial.dll `
        Castle.Core.dll `
        Castle.DynamicProxy2.dll `
        Castle.Facilities.Logging.dll `
        Castle.MicroKernel.dll `
        Castle.Windsor.dll `
        /out:ElmsConnector.dll `
        /t:library
    cd $old
}

task Update-Castle {
    if (Test-Path $castleTrunk)
    {
        Write-Host "Castle-trunk found at $castleTrunk"
        $buildDirExists = (Test-Path "$castleTrunk\build")
        if (!$buildDirExists -or $compilecastle)
        {
            Write-Host "compilation not yet available.."
        }
        Write-Host "Copying assemblies..."
        Copy-Assemblies "Castle.Core.*"
        Copy-Assemblies "Castle.DynamicProxy2.*"
        Copy-Assemblies "Castle.MicroKernel.*"
        Copy-Assemblies "Castle.Windsor.*"
    }
    else
    {
        Write-Host "Could not find $castleTrunk"
    }
}

Function global:Copy-Assemblies($filename)
{
    Write-Host "copying $filename"
    Copy-Item $castleTrunk\build\net-3.5\release\$fileName -destination lib\castle\
}