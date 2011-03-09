param($installPath, $toolsPath, $package, $project)

Write-Host "Starting install"

$solutionRoot = Join-Path $dte.Solution.FileName '..\'
$solutionBin = Join-Path $dte.Solution.FileName '..\Bin\'

#Write-Host $solutionRoot

if ((Test-Path $solutionBin) -ne $true){
    Write-Host "Creating Bin Folder"
    New-Item $solutionBin -type directory
}

#Write-Host "Install Path"
#Write-Host ($installPath | Get-Member)
#Write-Host $installPath

#Write-Host "Tools"
#Write-Host $toolsPath

#Write-Host "Package"
#Write-Host ($package | Get-Member)

#Write-Host "Package Files"
#Write-Host $package.GetFiles()
#Write-Host ($package.GetFiles() | Get-Member)

Write-Host "Installing Dlls:"

$package.GetFiles() |  ForEach  {    
  if ($_.Path.StartsWith("Bin")){
    $fileName = Join-Path $solutionRoot $_.Path
    
    Write-Host $_.Path
    #Write-Host $fileName
    
    $writer = new-object System.IO.FileStream $fileName, "Create"
    
    [byte[]]$buffer = new-object byte[] 4096
    [int]$total = [int]$count = 0
    
    $stream = $_.GetStream()
    
    do   
    {
        $count = $stream.Read($buffer, 0, $buffer.Length)
        $writer.Write($buffer, 0, $count)
    } while ($count -gt 0)
    
    $writer.Close()
    $stream.Close()
  }
}