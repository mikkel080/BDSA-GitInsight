run-docker: 
	docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=<YourStrong@Passw0rd>" -p 1433:1433 -d mcr.microsoft.com/mssql/server:2022-latest

run-api: 
	dotnet run --project GitInsight.Api

run-blazor:
	dotnet run --project GitInsight.Blazor

