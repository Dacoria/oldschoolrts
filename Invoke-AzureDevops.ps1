#https://docs.microsoft.com/en-us/azure/app-service/tutorial-custom-container?pivots=container-linux
cls

Import-Module "$PSScriptRoot\variables\Variables.psm1" -force
$variables = Get-Variables

function Build-Docker($imageName, $relativeDockerFolder) {
	write-host "----------------------------"
	write-host "Build-Docker"
	write-host "----------------------------"
	docker build --tag $imageName "$PSScriptRoot"
}

function Run-DockerLocally($localhostPort, $dockerInternallyExposedPort, $imageName) {
	write-host "----------------------------"
	write-host "Run-DockerLocally"
	write-host "----------------------------"
	docker run -d -p "$localhostPort`:$dockerInternallyExposedPort" $imageName
	Start-Process "http://localhost:$localhostPort"
}

function New-ResourceGroup($resourceGroup, $location) {
	write-host "----------------------------"
	write-host "New-ResourceGroup"
	write-host "----------------------------"
	az group create --name $resourceGroup --location $location
}

function New-Acr($acrName, $resourceGroup) {
	write-host "----------------------------"
	write-host "New-Acr"
	write-host "----------------------------"
	az acr create --name $acrName --resource-group $resourceGroup --sku Basic --admin-enabled true
}

function Get-AcrCredentials($resourceGroup, $acrName) {
	write-host "----------------------------"
	write-host "Get-AcrCredentials"
	write-host "----------------------------"
	return az acr credential show --resource-group $resourceGroup --name $acrName --query "{username:username, password:passwords[0].value}" | ConvertFrom-JSON
}

function Login-Docker($acrName, $acrCredentials) {
	write-host "----------------------------"
	write-host "Login-Docker"
	write-host "----------------------------"
	$loggedIn = $False
	$retryCount = 0
	
	while ($loggedIn -ne $True) {
		if ($retryCount -ge 10) {
			throw "Could not log in after 10 retries, I'm throwing a tantrum"
		}
		
		docker login "$acrName.azurecr.io" --username $acrCredentials.username --password $acrCredentials.password
		
		if ($LastExitCode -ne 0) {
			Write-Warning "Could not log in. Retry attempt: $retryCount"
			$retryCount++
		} else {
			$loggedIn = $true
		}
	}
}

function Tag-DockerBuild($acrName, $imageName, $imagetag) {
	write-host "----------------------------"
	write-host "Tag-DockerBuild"
	write-host "----------------------------"
	docker tag $imageName "$acrName.azurecr.io/$imageName`:$imagetag"
}

function Push-Docker($acrName, $imageName, $imagetag) {
	write-host "----------------------------"
	write-host "Push-Docker"
	write-host "----------------------------"
	docker push "$acrName.azurecr.io/$imageName`:$imagetag"
}

function New-LinuxAppServicePlan($appServicePlanName, $resourceGroup) {
	write-host "----------------------------"
	write-host "New-LinuxAppServicePlan"
	write-host "----------------------------"
	az appservice plan create --name $appServicePlanName --resource-group $resourceGroup --is-linux
}

function New-WebApp($resourceGroup, $appServicePlanName, $webAppName, $acrName, $imageName, $imagetag) {
	write-host "----------------------------"
	write-host "New-WebApp"
	write-host "----------------------------"
	az webapp create --resource-group $resourceGroup --plan $appServicePlanName --name $webAppName --deployment-container-image-name "$acrName.azurecr.io/$imageName`:$imagetag"
}

function Set-WebAppConfigAndPort($resourceGroup, $webAppName, $dockerInternallyExposedPort) {
	write-host "----------------------------"
	write-host "Set-WebAppConfigAndPort"
	write-host "----------------------------"
	az webapp config appsettings set --resource-group $resourceGroup --name $webAppName --settings WEBSITES_PORT=$dockerInternallyExposedPort
}

function Get-ManagedIdentityPrincipalId($resourceGroup, $webAppName) {
	write-host "----------------------------"
	write-host "Get-ManagedIdentityPrincipalId"
	write-host "----------------------------"
	return az webapp identity assign --resource-group $resourceGroup --name $webAppName --query principalId --output tsv
}

function Assign-AcrPullRights($managedIdentityPrincipalId, $resourceGroup, $subscriptionId, $acrName) {
	write-host "----------------------------"
	write-host "Assign-AcrPullRights"
	write-host "----------------------------"
	az role assignment create --assignee $managedIdentityPrincipalId --scope "/subscriptions/$subscriptionId/resourceGroups/$resourceGroup/providers/Microsoft.ContainerRegistry/registries/$acrName" --role "AcrPull"
}

function Deploy-App($webAppName, $resourceGroup, $acrName, $imageName, $imagetag) {
	write-host "----------------------------"
	write-host "Deploy-App"
	write-host "----------------------------"
	az webapp config container set --name $webAppName --resource-group $resourceGroup --docker-custom-image-name "$acrName.azurecr.io/$imageName`:$imagetag" --docker-registry-server-url "https://$acrName.azurecr.io"
}




Build-Docker $variables.imageName $variables.relativePathToDockerFolderContainingDockerFile
#for debugging purposes
#Run-DockerLocally $variables.localhostPort $variables.dockerInternallyExposedPort $variables.imageName
New-ResourceGroup $variables.resourceGroup $variables.location
New-Acr $variables.acrName $variables.resourceGroup
$acrCredentials = Get-AcrCredentials $variables.resourceGroup $variables.acrName
$acrCredentials
Login-Docker $variables.acrName $acrCredentials

Tag-DockerBuild $variables.acrName $variables.imageName $variables.imagetag
Push-Docker $variables.acrName $variables.imageName $variables.imagetag

New-LinuxAppServicePlan $variables.appServicePlanName $variables.resourceGroup
New-WebApp $variables.resourceGroup $variables.appServicePlanName $variables.webAppName $variables.acrName $variables.imageName $variables.imagetag
Set-WebAppConfigAndPort $variables.resourceGroup $variables.webAppName $variables.dockerInternallyExposedPort
$managedIdentityPrincipalId = Get-ManagedIdentityPrincipalId $variables.resourceGroup $variables.webAppName

##-----------------
$subscriptionId = az account show --query id --output tsv
##-----------------

#deze stap is stom: heeft PIM nodig en PIM is stom
Assign-AcrPullRights $managedIdentityPrincipalId $variables.resourceGroup $subscriptionId $variables.acrName

Deploy-App $variables.webAppName $variables.resourceGroup $variables.acrName $variables.imageName $variables.imagetag