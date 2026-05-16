# Global Logistics Management System (GLMS)


###### Enterprise Application Development POE Part 2  

###### Student: Lené Prinsloo  

###### Student Number: ST1049612

###### Youtube link: 

# 

### Project Overview

GLMS is an ASP.NET Core MVC enterprise prototype for TechMove Logistics. The system manages clients, contracts, service requests, PDF signed agreements, workflow validation, currency conversion, and unit testing.

# 

### Technologies

* ASP.NET Core MVC
* Entity Framework Core
* SQL Server LocalDB
* Bootstrap
* xUnit
* GitHub Actions

# 

### Features

* Client CRUD management
* Contract CRUD management
* Service Request CRUD management
* PDF agreement upload for contracts
* Contract workflow validation
* Blocks service requests for Draft, Expired, and On Hold contracts
* USD to ZAR currency conversion using an external API
* Unit tests for currency calculation, file validation, and workflow validation
* GitHub Actions pipeline for build and test automation

# 

### Design Patterns Implemented

\- MVC Pattern

\- Strategy Pattern for currency conversion

\- Observer Pattern for service request status updates

\- Factory Pattern for service request creation

\- Dependency Injection

# 

### How to Run

1. Open the solution in Visual Studio.
2. Update the database:

```bash

dotnet ef database update --project GLMS.Web

```

3\. Run the application:

```bash

dotnet run --project GLMS.Web

```



### How to Run Tests

```bash

dotnet test GLMS.slnx

```





