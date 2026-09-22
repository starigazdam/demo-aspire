# demo-aspire

A deliberately small monorepo for demonstrating **.NET Aspire** locally:

- React + Vite frontend
- ASP.NET Core minimal API
- Azure SQL Database, emulated locally by an Aspire-managed SQL Server container

## Run locally

Prerequisites: [.NET 10 SDK](https://dotnet.microsoft.com/download), Node.js, and Docker Desktop/Engine with its daemon running.

```bash
git clone https://github.com/starigazdam/demo-aspire.git
cd demo-aspire
npm ci --prefix frontend
dotnet run --project DemoAspire.AppHost
```

Open the Aspire dashboard URL printed by the AppHost, then open the `frontend` resource. Add an item: the browser calls the API through Vite's `/api` proxy and the API stores it in SQL Server.

## Azure demo boundary

The AppHost has a real static-site publish owner for the frontend, but this repository deliberately does **not** provision Azure resources or credentials. Authenticate and inspect your subscription before choosing a target:

```bash
az login
az account show --output table
```

A practical low-cost demo split is Azure Static Web Apps for the Vite output plus Azure Container Apps for the API. The AppHost declares Azure SQL and uses its local SQL Server container emulator during development; Aspire's Azure SQL integration selects the Azure SQL Free Offer when deployed. Confirm the offer is available for the selected subscription and check the portal's cost estimate before provisioning.

## Checks

```bash
dotnet build DemoAspire.sln
npm run build --prefix frontend
```
