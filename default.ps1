properties {
    $castleTrunk = '..\open-source\castle-trunk\'
    $compilecastle = $FALSE
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