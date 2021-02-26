if (-not (Test-Path $PSScriptRoot\helm-output))
{
    Write-Host "helm-output not found. Building helm packages..."
    . $PSScriptRoot\build.ps1
}


helm upgrade --install --namespace kubedemo --create-namespace kubedemo-web $PSScriptRoot\helm-output\kubedemo-web-1.0.tgz
helm upgrade --install --namespace kubedemo --create-namespace kubedemo-worker $PSScriptRoot\helm-output\kubedemo-worker-1.0.tgz

kubectl get pods -n kubedemo