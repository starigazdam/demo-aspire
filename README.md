# demo-aspire

A deliberately small monorepo for demonstrating **.NET Aspire** locally and on Azure:

- React + Vite frontend
- .NET isolated Azure Functions HTTP API
- Azure Cosmos DB for NoSQL, emulated locally by Aspire
- Azure Container Apps Consumption deployment target

For Azure, Aspire builds the Vite assets into the Functions container image. The Container App serves the SPA at `/web/` and the API at `/api/todos`.

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

The checked-in command maps `dev` to resource group `demo-aspire-dev` and `tst` to `demo-aspire-tst`; Aspire keeps deployment state separately for each `--environment`. You need the Azure CLI logged in to the target subscription and Docker with Buildx and a running daemon; Aspire builds and pushes the Functions container image.

```bash
az login
export AZURE_SUBSCRIPTION_ID="<subscription-id>"
export AZURE_LOCATION="uksouth" # choose a region that accepts new Cosmos and Container Apps resources

# Optional: inspect the pipeline without provisioning.
scripts/deploy.sh dev --plan

# Deploy. The command prints the public Container Apps URL.
scripts/deploy.sh dev
```

## Clean up

This permanently deletes all resources in the selected environment. First inspect the target without deleting it, then rerun with the explicit confirmation flag:

```bash
scripts/destroy.sh dev       # prints the target and stops
scripts/destroy.sh dev --yes # destroys the printed target
```

`aspire deploy` provisions or reuses the selected resource group and can create Azure Cosmos DB, a consumption-based Azure Container Apps environment, Container Registry, managed identities, storage, and Log Analytics resources. The hosted Aspire dashboard is disabled; confirm the portal's cost estimate before deploying.

## Checks

```bash
dotnet build DemoAspire.sln
npm run build --prefix frontend
scripts/deploy.sh dev --plan
```
