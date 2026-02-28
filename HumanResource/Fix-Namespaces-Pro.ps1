param(
    [string]$SolutionPath = "."
)

Write-Host "🔍 Searching for all .csproj files..."

$projects = Get-ChildItem -Path $SolutionPath -Filter *.csproj -Recurse

if (-not $projects) {
    Write-Host "❌ No .csproj files found."
    exit
}

foreach ($csproj in $projects) {

    Write-Host ""
    Write-Host "=============================="
    Write-Host "📦 Project: $($csproj.Name)"
    Write-Host "=============================="

    [xml]$projXml = Get-Content $csproj.FullName
    $rootNamespaceNode = $projXml.Project.PropertyGroup.RootNamespace

    if ($rootNamespaceNode) {
        $rootNamespace = $rootNamespaceNode
    }
    else {
        $rootNamespace = [System.IO.Path]::GetFileNameWithoutExtension($csproj.Name)
    }

    Write-Host "📌 RootNamespace: $rootNamespace"

    $projectRoot = Split-Path $csproj.FullName

    Get-ChildItem -Path $projectRoot -Recurse -Filter *.cs | ForEach-Object {

        $filePath = $_.FullName
        $relativePath = $filePath.Replace($projectRoot + "\", "")

        if ($relativePath -match "bin\\|obj\\") {
            return
        }

        $directoryPath = Split-Path $relativePath

        if ($directoryPath -eq "") {
            $expectedNamespace = $rootNamespace
        }
        else {
            $namespacePart = $directoryPath -replace "\\", "."
            $expectedNamespace = "$rootNamespace.$namespacePart"
        }

        $content = Get-Content $filePath
        $currentNamespaceLine = $content | Where-Object { $_ -match "^namespace\s" }

        if ($currentNamespaceLine) {

            $currentNamespace = $currentNamespaceLine -replace "namespace\s", "" -replace ";", ""

            if ($currentNamespace.Trim() -ne $expectedNamespace.Trim()) {

                Write-Host "🔄 Updating: $relativePath"
                Write-Host "   Old: $currentNamespace"
                Write-Host "   New: $expectedNamespace"

                $updatedContent = $content | ForEach-Object {
                    if ($_ -match "^namespace\s") {
                        "namespace $expectedNamespace"
                    }
                    else {
                        $_
                    }
                }

                Set-Content $filePath $updatedContent
            }
        }
    }
}
