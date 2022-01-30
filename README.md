# Azure Function with injected Table Storage dependency
This repo contains a .NET 6 C# solution which illustrates how to use dependency injection (instead of bindings) to interact with Azure Table Storage in an Azure Function project using Visual Studio 2022.

After wasting some time trying to get table bindings working on an Azure Function by following the official documentation, I decided to adopt a more classic ASP.NET web application structure for an Azure Function, with a Startup class and dependency injection. 

This solution illustrates how you can use dependency injection to integrate data services that access several tables in storage. This sample function retrieves *Message* entities from a table called *Message*, which is linked to a table called *Company*. The messages that are returned are only messages linked to active companies.

## Project structure
The following folders are used to separate the several areas of the solution:
- [Abstractions](AzFunctionTSDemo/AzFunctionTSDemo/Abstractions): contains the interfaces for the data services:
	- [ITableStorageService](AzFunctionTSDemo/AzFunctionTSDemo/Abstractions/ITableStorageService.cs) is a generic interface which defines the basic data access operations that have been implemented: *RetrieveEntityAsync* and *RetrieveCollectionAsync*
	- [ICompanyDataService](AzFunctionTSDemo/AzFunctionTSDemo/Abstractions/ICompanyDataService.cs) is the interface for the data service which retrieves data from the *Company* table. This extends the *ITableStorageService* interface using the concrete type *Company*.
	- [IMessageDataService](AzFunctionTSDemo/AzFunctionTSDemo/Abstractions/IMessageDataService.cs) is the interface for the data service which retrieves data from the *Message* table. This extends the *ITableStorageService* interface using the concrete type *Message*.
- [DTOs](AzFunctionTSDemo/AzFunctionTSDemo/DTOs): contains the DTOs for passing data in and out of the function:
	- [GetMessagesRequestDto](AzFunctionTSDemo/AzFunctionTSDemo/DTOs/GetMessagesRequestDto.cs) is the DTO for the data comining into the function. It defines the message search criteria.
	- [MessageDto](AzFunctionTSDemo/AzFunctionTSDemo/DTOs/MessageDto.cs) is the DTO for the data going out of the function. It defines the message. A separate DTO was created for this purpose instead of using the *Message* entity to keep contracts separated from data.
- [Entities](AzFunctionTSDemo/AzFunctionTSDemo/Entities): contains the entities that mirror the data in table storage (properties named after the table column names):
	- [Company](AzFunctionTSDemo/AzFunctionTSDemo/Entities/Company.cs) is the Company entity.
	- [Message](AzFunctionTSDemo/AzFunctionTSDemo/Entities/Company.cs) is the Message entity.
- [Services](AzFunctionTSDemo/AzFunctionTSDemo/Services): contains the implementation of the data services. There is one data service per table:
	- [TableStorageDataService](AzFunctionTSDemo/AzFunctionTSDemo/Services/TableStorageDataService.cs) is a generic abstract base class for the data services which defines the implementation of the operations declared in the *ITableStorageService* interface. The concrete data services inherit from this class so that the implementation of the data access is contained only on the base class.
	- [CompanyDataService](AzFunctionTSDemo/AzFunctionTSDemo/Services/CompanyDataService.cs) is a data service class for accessing data from the *Company* table. It inherits from the *TableStorageDataService* class and all it does is define the filter criteria for getting the data.
	- [MessageDataService](AzFunctionTSDemo/AzFunctionTSDemo/Services/MessageDataService.cs) is a data service class for accessing data from the *Message* table. It inherits from the *TableStorageDataService* class and all it does is define the filter criteria for getting the data.
- [GetMessagesFunction](AzFunctionTSDemo/AzFunctionTSDemo/GetMessagesFunction.cs): is the Azure Function class. It is decorated with OpenApi attributes so that it can be used with SwaggerUI. *ICompanyDataService* and *IMessageDataService* are injected via the constructor and used to get the data.
- [local.settings.rename.json](AzFunctionTSDemo/AzFunctionTSDemo/local.settings.rename.json): is a placeholder for the *local.settings.json* file which can be used to run the function locally. There are more details about this on the Setup section.



## Setup
Follow these steps to set up the Azure table storage for this example using Azure CLI.

### Prerequisites
1. [Latest Azure CLI](https://docs.microsoft.com/en-us/cli/azure/).
2. You must have access to an Azure subscription that allows you to create Storage Accounts.


### Log in to your Azure tenant
`az login --tenant {your tenant id}`

### Create an Azure Storage account (skip this if you want to use an existing account)
`az storage account create --name {pick a valid acocunt name} --resource-group {choose an existing resource group}`

### Create tables in the storage account
`az storage table create --name Company --account-name {the storage account name}`

`az storage table create --name Message --account-name {the storage account name}`

### Insert sample data

#### Sample companies
`az storage entity insert --account-name {the storage account name} --table-name Company --entity PartitionKey=a RowKey=1 Name="Test company 1" Description="bla bla bla" Active=true Active@odata.type=Edm.Boolean`

`az storage entity insert --account-name {the storage account name} --table-name Company --entity PartitionKey=a RowKey=2 Name="Test company 2" Description="bla bla bla" Active=true Active@odata.type=Edm.Boolean`

`az storage entity insert --account-name {the storage account name} --table-name Company --entity PartitionKey=a RowKey=3 Name="Test company 3" Description="bla bla bla" Active=false Active@odata.type=Edm.Boolean`


#### Sample messages
`az storage entity insert --account-name {the storage account name} --table-name Message --entity PartitionKey=a RowKey=1 CompanyId="1" Content="this is the message content 1" Processed=false Processed@odata.type=Edm.Boolean`

`az storage entity insert --account-name {the storage account name} --table-name Message --entity PartitionKey=a RowKey=2 CompanyId="1" Content="this is the message content 2" Processed=true Processed@odata.type=Edm.Boolean`

`az storage entity insert --account-name {the storage account name} --table-name Message --entity PartitionKey=a RowKey=3 CompanyId="1" Content="this is the message content 3" Processed=true Processed@odata.type=Edm.Boolean`

`az storage entity insert --account-name {the storage account name} --table-name Message --entity PartitionKey=a RowKey=4 CompanyId="2" Content="this is the message content 4" Processed=false Processed@odata.type=Edm.Boolean`

`az storage entity insert --account-name {the storage account name} --table-name Message --entity PartitionKey=a RowKey=5 CompanyId="2" Content="this is the message content 5" Processed=false Processed@odata.type=Edm.Boolean`

`az storage entity insert --account-name {the storage account name} --table-name Message --entity PartitionKey=a RowKey=6 CompanyId="2" Content="this is the message content 6" Processed=true Processed@odata.type=Edm.Boolean`

`az storage entity insert --account-name {the storage account name} --table-name Message --entity PartitionKey=a RowKey=7 CompanyId="3" Content="this is the message content 7" Processed=true Processed@odata.type=Edm.Boolean`

`az storage entity insert --account-name {the storage account name} --table-name Message --entity PartitionKey=a RowKey=8 CompanyId="3" Content="this is the message content 8" Processed=false Processed@odata.type=Edm.Boolean`

### Deploy the Azure function
Choose your favourite way to deploy the AzFunctionTSDemo function. One of the easiest ways is to do it from Visual Studio once you've configured your Azure Portal account in the Visual Studio Accounts'.

### Run locally
Make a copy of the local.settings.rename.json file and rename it to local.settings.json. Go to the deployed Azure Function on the Azure Portal and copy the value of the AzureWebJobsStorage configuration setting. Paste it in the corresponding setting in the local.settings.json file. Run the solution. A terminal window will come up with the solution endpoints. If you go to http://localhost:7071/api/swagger/ui you'll be able to call the Azure Function from the Swagger UI.
