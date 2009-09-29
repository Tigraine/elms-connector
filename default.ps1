properties {
    $castleTrunk = '..\open-source\castle-trunk\'
    $compilecastle = $FALSE
    $config = 'debug'
    $showtestresult = $FALSE
    $base_dir = resolve-path .
    $lib_dir = "$base_dir\lib\"
    $build_dir = "$base_dir\build\" 
    $release_dir = "$base_dir\release\"
}

task default -depends Release

task Clean {
    remove-item -force -recurse $build_dir -ErrorAction SilentlyContinue 
    remove-item -force -recurse $release_dir #-ErrorAction SilentlyContinue 
}

task Init -depends Clean {
    new-item $build_dir -itemType directory
    new-item $release_dir -itemType directory
}

task Build -depends Init {
    msbuild src\ElmsConnector\ElmsConnector.csproj /p:OutDir=$build_dir /p:Configuration=$config
}

task Test -depends Build {
    msbuild src\ElmsConnector.Tests\ElmsConnector.Tests.csproj /p:OutDir=$build_dir /p:Configuration=$config
    
    $old = pwd
    cd $build_dir
    & $lib_dir\xunit\xunit.console.exe $build_dir\ElmsConnector.Tests.dll /html $build_dir\TestResult.htm
    if ($lastExitCode -ne 0) {
        throw "Error: Failed to execute tests"
        if ($showtestresult)
        {
            start $build_dir\TestResult.htm
        }
    }
    cd $old
}

task Merge -depends Build {
    $old = pwd
    cd $build_dir
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
    if ($lastExitCode -ne 0) {
        throw "Error: Failed to merge assemblies"
    }
    cd $old
}

task Release -depends Test, Merge {
    & $lib_dir\7zip\7za.exe a $release_dir\ElmsConnector-$version.zip `
    $build_dir\ElmsConnector.dll `
    $build_dir\ElmsConnector.pdb `
    license.txt `
    acknowledgements.txt `
    $build_dir\Testresult.htm
    
    Write-host "-----------------------------"
    Write-Host "ElmsConnector was successfully compiled and packaged."
    Write-Host "The release bits can be found in ""$release_dir"""
    Write-Host "Thank you for using ElmsConnector!"
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