param (
    $Version
)
. $PSScriptRoot\build.ps1

helm upgrade --install --namespace kubedemo --create-namespace kubedemo-web $PSScriptRoot\helm-output\kubedemo-web-$Version.tgz
helm upgrade --install --namespace kubedemo --create-namespace kubedemo-worker $PSScriptRoot\helm-output\kubedemo-worker-$Version.tgz

helm upgrade --install --namespace kubedemo rabbitmq bitnami/rabbitmq

kubectl get pods -n kubedemo
