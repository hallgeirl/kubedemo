. $PSScriptRoot\build.ps1

helm upgrade --install --namespace kubedemo --create-namespace kubedemo-web $PSScriptRoot\helm-output\kubedemo-web-1.0.tgz
helm upgrade --install --namespace kubedemo --create-namespace kubedemo-worker $PSScriptRoot\helm-output\kubedemo-worker-1.0.tgz

helm upgrade --install --namespace kubedemo rabbitmq bitnami/rabbitmq

kubectl get pods -n kubedemo