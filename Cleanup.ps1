Get-ChildItem -Path . -Recurse -Directory -Filter "bin" -Force | ForEach-Object {
    Write-Host "Deleting folder: $($_.FullName)" -ForegroundColor Yellow
    Remove-Item -Path $_.FullName -Recurse -Force -ErrorAction SilentlyContinue
}

Get-ChildItem -Path . -Recurse -Directory -Filter "obj" -Force | ForEach-Object {
    Write-Host "Deleting folder: $($_.FullName)" -ForegroundColor Yellow
    Remove-Item -Path $_.FullName -Recurse -Force -ErrorAction SilentlyContinue
}

Write-Host "All bin and obj folders have been deleted." -ForegroundColor Green