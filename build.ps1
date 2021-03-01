param
(
    [switch]$BuildDockerImage
)

if ($BuildDockerImage)
{
    Remove-Item -Recurse $PSScriptRoot\src\WebApp\obj
    Remove-Item -Recurse $PSScriptRoot\src\WorkerApp\obj
    
    docker build $PSScriptRoot\src\WebApp -t hallgeirl/kubedemo-web:1.0
    docker build $PSScriptRoot\src\WorkerApp -t hallgeirl/kubedemo-worker:1.0
    
    docker push hallgeirl/kubedemo-web:1.0
    docker push hallgeirl/kubedemo-worker:1.0
}

helm package helm/kubedemo-web --version 1.0 -d helm-output
helm package helm/kubedemo-worker --version 1.0 -d helm-output
