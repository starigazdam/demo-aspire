# demo-aspire

A deliberately small monorepo for demonstrating **.NET Aspire** locally and on Azure:

- React + Vite frontend
- .NET isolated Azure Functions HTTP API
- Azure Cosmos DB for NoSQL, emulated locally by Aspire
- Linux Azure Function App deployment target

For Azure, Aspire builds the Vite assets into the Function App image. The Function App serves the SPA at `/web/` and the API at `/api/todos`.

## Run locally

Prerequisites: [.NET 10 SDK](https://dotnet.microsoft.com/download), Node.js, and Docker Desktop/Engine with its daemon running.

```bash
git clone https://github.com/starigazdam/demo-aspire.git
cd demo-aspire
npm ci --prefix frontend
dotnet run --project DemoAspire.AppHost
```

Open the Aspire dashboard URL printed by the AppHost, then open the `frontend` resource. Add an item: the browser calls the API through Vite's `/api` proxy and the API stores it in Cosmos DB.

## Deploy dev or tst

The checked-in command maps `dev` to resource group `demo-aspire-dev` and `tst` to `demo-aspire-tst`; Aspire keeps deployment state separately for each `--environment`.

```bash
az login
export AZURE_SUBSCRIPTION_ID="<subscription-id>"
export AZURE_LOCATION="westeurope"
scripts/deploy.sh dev
scripts/deploy.sh tst

# Destroy all resources for one environment after a disposable run.
scripts/destroy.sh dev
```

Preview Aspire's deployment pipeline without provisioning anything:

```bash
scripts/deploy.sh dev --plan
```

`aspire deploy` provisions or reuses the selected resource group and can create Azure Cosmos DB, a Linux Function App on an App Service Premium V3 plan, Container Registry, managed identity, storage, and Application Insights resources. This is not a free hosting plan; confirm the portal's cost estimate before deploying.

## Checks

```bash
dotnet build DemoAspire.sln
npm run build --prefix frontend
scripts/deploy.sh dev --plan
```
