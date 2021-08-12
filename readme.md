# Enable Telepresence Debug
```shell
$ telepresence --swap-deployment person-service --expose 8080 --run dotnet person-service/bin/Debug/net5.0/person-service.dll
$ telepresence intercept person-service --port 8080 -- dotnet person-service/bin/Debug/net5.0/person-service.dll --env-file ~/person-service-debug.env
$ telepresence unistall --agent person-service
$ telepresence quit
```

# Enable Telepresence Intercept
```shell
$ telepresence connect
$ telepresence list
$ telepresence intercept person-service --port 8080:8080 --env-file ~/person-service-intercept.env
```
