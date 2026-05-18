# Global Logistics Management System (GLMS)

###### Enterprise Application Development POE Part 2

###### Student: Lené Prinsloo

###### Student Number: ST10496124

###### YouTube Demonstration Link:
Add your video link here before submission.

---

# Project Overview

GLMS is an ASP.NET Core MVC enterprise prototype developed for TechMove Logistics.

The system manages:
- Clients
- Contracts
- Service Requests
- PDF Agreement Uploads
- Workflow Validation
- Currency Conversion
- Automated Testing

The project demonstrates enterprise software development principles using ASP.NET Core MVC, Entity Framework Core, SQL Server, GitHub Actions CI/CD, and multiple enterprise design patterns.

---

# Technologies

- ASP.NET Core MVC
- Entity Framework Core
- SQL Server LocalDB
- Bootstrap
- xUnit
- GitHub Actions
- HttpClient API Integration

---

# Features

- Client CRUD management
- Contract CRUD management
- Service Request CRUD management
- PDF agreement upload for contracts
- Contract workflow validation
- Blocks service requests for:
  - Draft contracts
  - Expired contracts
  - On Hold contracts
- USD to ZAR currency conversion using an external API
- Unit testing for:
  - Currency calculation
  - File validation
  - Workflow validation
- GitHub Actions CI/CD pipeline

---

# Design Patterns Implemented

## MVC Pattern
Separates:
- Models
- Views
- Controllers

Improves maintainability and scalability.

---

## Strategy Pattern
Used for currency conversion.

### Classes
- `ICurrencyConverter`
- `UsdToZarConverter`

Allows future currency strategies without modifying controller logic.

---

## Observer Pattern
Used for service request status notifications.

### Classes
- `IServiceRequestObserver`
- `ServiceRequestLogger`

Logs service request workflow updates.

---

## Factory Pattern
Used for centralized service request object creation.

### Classes
- `IServiceRequestFactory`
- `ServiceRequestFactory`

Improves scalability and object creation management.

---

## Dependency Injection
ASP.NET Core dependency injection is used throughout the application for:
- Services
- Validators
- Observers
- Factories

---

# Database Migration Scripts

## Initial Database Setup

```bash
Add-Migration InitialCreate
Update-Database
```

## Additional Migrations

```bash
Add-Migration AddContractValidation
Add-Migration AddCurrencyConversion
Update-Database
```

---

# How to Run the Application

## 1. Clone Repository

![Home Page](screenshots/github-main.png)

```bash
git clone https://github.com/LeBeatrix/GLMS.git
```

---

## 2. Open Solution

Open:

```text
GLMS.slnx
```

in Visual Studio.

---

## 3. Update Database

```bash
dotnet ef database update --project GLMS.Web
```

---

## 4. Run Application

```bash
dotnet run --project GLMS.Web
```

---

# How to Run Tests

```bash
dotnet test GLMS.slnx
```

---

# Example Test Data

## Clients

### Client 1

```text
Name: Cape Global Freight
Contact: support@capeglobal.co.za
Region: South Africa
```

### Client 2

```text
Name: EuroTrans Logistics
Contact: operations@eurotrans.eu
Region: Europe
```

### Client 3

```text
Name: Atlantic Cargo Solutions
Contact: contact@atlanticcargo.com
Region: North America
```

---

# Validation Features

## Contract Validation
- End date must be after start date
- Signed PDF agreement required
- Only PDF files accepted

---

## Service Request Validation
Service requests cannot be created for:
- Expired contracts
- Draft contracts
- On Hold contracts

---

# Screenshots

## Home Page

![Home Page](screenshots/home-page.png)

---

## Clients Management

![Clients](screenshots/clients-index.png)

---

## Contract Management

![Contracts](screenshots/contracts-index.png)

---

## PDF Upload Validation

![PDF Upload](screenshots/pdf-upload.png)

---

## Currency Conversion

![Currency Conversion](screenshots/currency-conversion.png)

---

## Workflow Validation

![Workflow Validation](screenshots/workflow-validation.png)

---

## Unit Testing

![Unit Tests](screenshots/unit-tests.png)

---

## GitHub Actions Pipeline

![GitHub Actions](screenshots/github-actions.png)

---

# Unit Testing

The application uses xUnit testing.

Tests include:
- Currency conversion tests
- File validation tests
- Workflow validation tests

---

# CI/CD Pipeline

GitHub Actions automatically:
- Restores dependencies
- Builds the solution
- Runs unit tests

This ensures continuous integration and automated quality assurance.

---
