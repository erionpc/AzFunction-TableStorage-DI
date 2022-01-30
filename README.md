# Azure Function with Table Storage dependency
This repo contains a .NET 6 C# solution which illustrates how to use dependency injection (instead of bindings) to interact with Azure Table Storage in an Azure Function project using Visual Studio 2022.

After wasting some time trying to get table bindings working on an Azure Function by following the official documentation, I decided to adopt a more classic ASP.NET web application structure for an Azure Function, with a Startup class and dependency injection. 

This solution illustrates how you can use dependency injection to integrate data services that access several tables in storage. This sample function retrieves *Message* entities from a table called *Message*, which is linked to a table called *Company*. The messages that are returned are only messages linked to active companies.

## Setup
Follow these steps to set up the Azure table storage for this example using Azure CLI.

### Prerequisites
1. [Latest Azure CLI](https://docs.microsoft.com/en-us/cli/azure/).
2. You must have access to an Azure subscription that allows you to create Storage Accounts.


#### Log in to your Azure tenant
`az login --tenant {your tenant id}`

#### Create an Azure Storage account (skip this if you want to use an existing account)
`az storage account create --name {pick a valid acocunt name} --resource-group {choose an existing resource group}`

#### Create tables in the storage account
<code>
az storage table create --name Company --account-name {the storage account name}

az storage table create --name Message --account-name {the storage account name}
</code>

#### Insert sample data
<code>
az storage entity insert --account-name {the storage account name} --table-name Company --entity PartitionKey=a RowKey=1 Name="Test company 1" Description="bla bla bla" Active=true Active@odata.type=Edm.Boolean

az storage entity insert --account-name {the storage account name} --table-name Company --entity PartitionKey=a RowKey=2 Name="Test company 2" Description="bla bla bla" Active=true Active@odata.type=Edm.Boolean

az storage entity insert --account-name {the storage account name} --table-name Company --entity PartitionKey=a RowKey=3 Name="Test company 3" Description="bla bla bla" Active=false Active@odata.type=Edm.Boolean
</code>
<code>
az storage entity insert --account-name {the storage account name} --table-name Message --entity PartitionKey=a RowKey=1 CompanyId="1" Content="this is the message content 1" Processed=false Processed@odata.type=Edm.Boolean

az storage entity insert --account-name {the storage account name} --table-name Message --entity PartitionKey=a RowKey=2 CompanyId="1" Content="this is the message content 2" Processed=true Processed@odata.type=Edm.Boolean

az storage entity insert --account-name {the storage account name} --table-name Message --entity PartitionKey=a RowKey=3 CompanyId="1" Content="this is the message content 3" Processed=true Processed@odata.type=Edm.Boolean

az storage entity insert --account-name {the storage account name} --table-name Message --entity PartitionKey=a RowKey=4 CompanyId="2" Content="this is the message content 4" Processed=false Processed@odata.type=Edm.Boolean

az storage entity insert --account-name {the storage account name} --table-name Message --entity PartitionKey=a RowKey=5 CompanyId="2" Content="this is the message content 5" Processed=false Processed@odata.type=Edm.Boolean

az storage entity insert --account-name {the storage account name} --table-name Message --entity PartitionKey=a RowKey=6 CompanyId="2" Content="this is the message content 6" Processed=true Processed@odata.type=Edm.Boolean

az storage entity insert --account-name {the storage account name} --table-name Message --entity PartitionKey=a RowKey=7 CompanyId="3" Content="this is the message content 7" Processed=true Processed@odata.type=Edm.Boolean

az storage entity insert --account-name {the storage account name} --table-name Message --entity PartitionKey=a RowKey=8 CompanyId="3" Content="this is the message content 8" Processed=false Processed@odata.type=Edm.Boolean
</code>

#### Deploy the Azure function
Choose your favourite way to deploy the AzFunctionTSDemo function. One of the easiest ways is to do it from Visual Studio once you've configured your Azure Portal account in the Visual Studio Accounts'.

#### Run locally
Make a copy of the local.settings.rename.json file and rename it to local.settings.json. Go to the deployed Azure Function on the Azure Portal and copy the value of the AzureWebJobsStorage configuration setting. Paste it in the corresponding setting in the local.settings.json file. Run the solution. A terminal window will come up with the solution endpoints. If you go to http://localhost:7071/api/swagger/ui you'll be able to call the Azure Function from the Swagger UI.
