helm uninstall kubedemo-web --namespace kubedemo
helm uninstall kubedemo-worker --namespace kubedemo
kubectl delete namespace kubedemo
Remove-Item .\helm-output -Force -Recurse
