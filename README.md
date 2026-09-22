# demo-aspire

A deliberately small monorepo for demonstrating **.NET Aspire** locally:

- React + Vite frontend
- ASP.NET Core minimal API
- PostgreSQL, run as an Aspire-managed container

## Run locally

Prerequisites: [.NET 10 SDK](https://dotnet.microsoft.com/download), Node.js, and Docker Desktop/Engine with its daemon running.

```bash
git clone https://github.com/starigazdam/demo-aspire.git
cd demo-aspire
npm ci --prefix frontend
dotnet run --project DemoAspire.AppHost
```

Open the Aspire dashboard URL printed by the AppHost, then open the `frontend` resource. Add an item: the browser calls the API through Vite's `/api` proxy and the API stores it in PostgreSQL.

## Azure demo boundary

The AppHost has a real static-site publish owner for the frontend, but this repository deliberately does **not** provision Azure resources or credentials. Authenticate and inspect your subscription before choosing a target:

```bash
az login
az account show --output table
```

A practical low-cost demo split is Azure Static Web Apps for the Vite output plus Azure Container Apps for the API. PostgreSQL Flexible Server is not a permanently free service; use its trial/free offer only if it is currently available in the selected subscription, or choose a short-lived demo database and delete it afterwards. Check current Azure pricing and the portal's cost estimate before provisioning.

## Checks

```bash
dotnet build DemoAspire.sln
npm run build --prefix frontend
```
