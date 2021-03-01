helm uninstall kubedemo-web --namespace kubedemo
helm uninstall kubedemo-worker --namespace kubedemo
Remove-Item .\helm-output -Force -Recurse
