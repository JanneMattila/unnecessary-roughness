# Variables
$containerAppsEnvironment = "cae-ur"
$workspaceName = "log-ur"

$resourceGroup = "rg-ur"
$location = "northeurope"

# Login to Azure
az login

# List subscriptions
az account list -o table

# *Explicitly* select your working context
az account set --subscription SubName

# Show current context
az account show -o table

# Prepare extensions and provider
az extension remove -n containerapp
az extension add --yes --upgrade --source "https://workerappscliextension.blob.core.windows.net/azure-cli-extension/containerapp-0.2.2-py2.py3-none-any.whl"
az provider register --namespace Microsoft.Web

# Double check the registration
az provider show -n Microsoft.Web -o table

# Create new resource group
az group create --name $resourceGroup --location $location -o table

# Create Log Analytics workspace
$workspaceCustomerId = (az monitor log-analytics workspace create --resource-group $resourceGroup --workspace-name $workspaceName --query customerId -o tsv)
$workspaceKey = (az monitor log-analytics workspace get-shared-keys --resource-group $resourceGroup --workspace-name $workspaceName --query primarySharedKey -o tsv)

# Create Container Apps environment
az containerapp env create `
  --name $containerAppsEnvironment `
  --resource-group $resourceGroup `
  --logs-workspace-id $workspaceCustomerId `
  --logs-workspace-key $workspaceKey `
  --location $location

#############
# Create App
#############
$fqdn = (az containerapp create `
    --name ur `
    --revisions-mode single `
    --resource-group $resourceGroup `
    --environment $containerAppsEnvironment `
    --image "jannemattila/unnecessary-roughness:latest" `
    --cpu "0.25" `
    --memory "0.5Gi" `
    --ingress "external" `
    --target-port 80 `
    --min-replicas 0 `
    --max-replicas 1 `
    --query latestRevisionFqdn -o tsv)

# If you want to fetch existing container app details
$fqdn = (az containerapp show --name ur --resource-group $resourceGroup --query latestRevisionFqdn -o tsv)

"https://$fqdn/"

# Query logs
az monitor log-analytics query `
  --workspace $workspaceCustomerId `
  --analytics-query "ContainerAppConsoleLogs_CL | where ContainerAppName_s == 'ur'" `
  --out table

# Update image only
az containerapp update --name ur --resource-group $resourceGroup --image "jannemattila/unnecessary-roughness:latest"

# Update with new revision and make it active right away
$fqdn = (az containerapp update `
    --name ur `
    --revisions-mode single `
    --resource-group $resourceGroup `
    --image "jannemattila/unnecessary-roughness:latest" `
    --cpu "0.25" `
    --memory "0.5Gi" `
    --ingress "external" `
    --target-port 80 `
    --min-replicas 0 `
    --max-replicas 1 `
    --query latestRevisionFqdn -o tsv)

"https://$fqdn/"

# Wipe out the resources
az group delete --name $resourceGroup -y
