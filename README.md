# demo-aspire

A deliberately small monorepo for demonstrating **.NET Aspire** locally and on Azure:

- React + Vite frontend
- ASP.NET Core minimal API
- Azure SQL Database, emulated locally by an Aspire-managed SQL Server container
- Azure Container Apps deployment target

## Run locally

Prerequisites: [.NET 10 SDK](https://dotnet.microsoft.com/download), Node.js, and Docker Desktop/Engine with its daemon running.

```bash
git clone https://github.com/starigazdam/demo-aspire.git
cd demo-aspire
npm ci --prefix frontend
dotnet run --project DemoAspire.AppHost
```

Open the Aspire dashboard URL printed by the AppHost, then open the `frontend` resource. Add an item: the browser calls the API through Vite's `/api` proxy and the API stores it in SQL Server.

## Deploy dev or tst

The checked-in command maps `dev` to resource group `demo-aspire-dev` and `tst` to `demo-aspire-tst`; Aspire keeps deployment state separately for each `--environment`.

```bash
az login
export AZURE_SUBSCRIPTION_ID="<subscription-id>"
export AZURE_LOCATION="westeurope"
scripts/deploy.sh dev
scripts/deploy.sh tst
```

Preview Aspire's deployment pipeline without provisioning anything:

```bash
scripts/deploy.sh dev --plan
```

`aspire deploy` provisions or reuses the selected resource group and can create Azure SQL, Container Apps, Container Registry, managed identity, and Log Analytics resources. Confirm the Azure SQL Free Offer and the portal's cost estimate before the first deploy.

## Checks

```bash
dotnet build DemoAspire.sln
npm run build --prefix frontend
scripts/deploy.sh dev --plan
```
