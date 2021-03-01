param (
    $Version = "1.0"
)
. $PSScriptRoot\build.ps1 -Version $Version

helm upgrade --install --namespace kubedemo --create-namespace kubedemo-web `
    --set appConfig.rabbitmqHost=rabbitmq.kubedemo.svc.cluster.local --set appConfig.rabbitmqUser=user --set appConfig.rabbitmqPassword=IhruhuhDF43 `
    $PSScriptRoot\helm-output\kubedemo-web-$Version.tgz

helm upgrade --install --namespace kubedemo --create-namespace kubedemo-worker `
    --set appConfig.rabbitmqHost=rabbitmq.kubedemo.svc.cluster.local --set appConfig.rabbitmqUser=user --set appConfig.rabbitmqPassword=IhruhuhDF43 `
    $PSScriptRoot\helm-output\kubedemo-worker-$Version.tgz



kubectl get pods -n kubedemo
