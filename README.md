# MediScope 🏥

**A Comprehensive Healthcare Management System**

**Authors:** Omrahn Faqiri, Maryam Elhamidi

---

## 📋 Table of Contents
- [MediScope 🏥](#mediscope-)
  - [📋 Table of Contents](#-table-of-contents)
  - [🔍 Overview](#-overview)
  - [Features](#features)
    - [Administrators](#administrators)
    - [Doctors](#doctors)
    - [Patients](#patients)
    - [Core System Features](#core-system-features)
  - [🛠 Technology Stack](#-technology-stack)
    - [Backend](#backend)
    - [Frontend](#frontend)
    - [Architecture Patterns](#architecture-patterns)
  - [Prerequisites](#prerequisites)
  - [Installation](#installation)
    - [1. Clone the Repository](#1-clone-the-repository)
    - [2. Restore Dependencies](#2-restore-dependencies)
    - [3. Build the Project](#3-build-the-project)
  - [Configuration](#configuration)
    - [Database Connection](#database-connection)
  - [Running the Application](#running-the-application)
    - [Using Terminal](#using-terminal)
      - [HTTPS Mode (Port 5001)](#https-mode-port-5001)
  - [Default Login Credentials](#default-login-credentials)
  - [Project Structure](#project-structure)
  - [Database](#database)
    - [Database Schema](#database-schema)
  - [Architecture](#architecture)
    - [Layered Architecture](#layered-architecture)
    - [Dependency Injection](#dependency-injection)
  - [Key Components](#key-components)
    - [Controllers](#controllers)
    - [Services](#services)
    - [Repositories](#repositories)

---

## 🔍 Overview

MediScope is a modern, full-featured healthcare management system built with ASP.NET Core MVC and Entity Framework Core. The application provides comprehensive tools for managing medical appointments, patient records, doctor schedules, departments, and resources in a healthcare facility.

The system implements role-based access control with three distinct user types (Admin, Doctor, and Patient), each with tailored functionality and views to streamline healthcare operations.

---

## Features

### Administrators
- **Dashboard Analytics**: View real-time metrics and system statistics
- **User Management**: Manage doctors, patients, and administrative accounts
- **Department Management**: Create and organize medical departments
- **Resource Allocation**: Manage hospital resources and equipment
- **System Logs**: Monitor system activities and audit trails
- **Feedback Management**: Review and respond to patient feedback
- **Analytics API**: Access detailed analytics data via REST endpoints

### Doctors
- **Appointment Management**: View and manage scheduled appointments
- **Patient Records**: Access and upload medical reports
- **Test Results**: Record and review patient test results
- **Department Association**: Linked to specific medical departments
- **Schedule Management**: Track daily appointments and availability

### Patients
- **Book Appointments**: Schedule appointments with available doctors
- **View Medical History**: Access and download personal test results and records
- **Appointment Tracking**: Monitor upcoming and past appointments
- **Feedback System**: Provide feedback on services and care

### Core System Features
- **Secure Authentication**: ASP\.NET Core Identity with role-based authorization
- **Session Management**: Secure user sessions with configurable timeouts
- **Responsive Design**: Mobile friendly interface
- **Data Validation**: Robust nput validation service
- **Logging System**: Track all critical system operations
- **Database Migrations**: Schema updates via EF Core

---

## 🛠 Technology Stack

### Backend
- **Framework**: ASP.NET Core 8.0 (MVC)
- **Language**: C# with .NET 8.0
- **ORM**: Entity Framework Core 8.0.21
- **Database**: SQLite
- **Authentication**: ASP.NET Core Identity

### Frontend
- **Template Engine**: Razor Views
- **Styling**: CSS (wwwroot/css)
- **JavaScript**: Standard JavaScript (wwwroot/js)

### Architecture Patterns
- **MVC Pattern**: Model-View-Controller architecture
- **Repository Pattern**: Data access abstraction
- **Dependency Injection**: Built-in ASP.NET Core DI
- **Service Layer**: Business logic separation

---

## Prerequisites
- **.NET 8.0 SDK** or later
  - Download from: https://dotnet.microsoft.com/download
  - Verify installation: `dotnet --version`

---

## Installation

### 1. Clone the Repository

```bash
git clone https://github.com/imrahnf/MediScope
cd MediScope
```

### 2. Restore Dependencies

```bash
dotnet restore
```

### 3. Build the Project

```bash
dotnet build
```

---

## Configuration

### Database Connection

The application uses SQLite by default. The connection string is configured in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=MediScope.db"
  }
}
```
If it does not yet exist, it will create it **automatically**.

---

## Running the Application
This application can run in any .NET IDE
### Using Terminal

#### HTTPS Mode (Port 5001)
```bash
dotnet run --launch-profile https
```

- Access the Application via **HTTPS**: https://localhost:5001

> **Note**: The application will automatically:
> - Apply pending database migrations
> - Create the SQLite database if it doesn't exist
> - Initialize the database schema

---

## Default Login Credentials

Use these credentials to access the system after first run:

| Username   | Password    | Role    | Description                    |
|------------|-------------|---------|--------------------------------|
| `admin`    | `Admin@123!`| Admin   | Full system administrator      |
| `doctor1`  | `Doc@123!`  | Doctor  | Dr. Sarah Johnson (Family Medicine) |
| `doctor2`  | `Doc@123!`  | Doctor  | Dr. Mark Patel (Cardiology)    |
| `patient1` | `Pat@123!`  | Patient | John Doe                       |
| `patient2` | `Pat@123!`  | Patient | Emily Chen                     |

---

## Project Structure

```
MediScope/
├── Controllers/                  # MVC Controllers
│   ├── AccountController.cs      
│   ├── AdminController.cs        
│   ├── AdminAnalyticsApiController.cs
│   ├── AppointmentController.cs  
│   ├── DepartmentController.cs   
│   ├── DoctorController.cs       
│   ├── HomeController.cs         
│   ├── PatientController.cs      
│   └── ResourceController.cs     
│
├── Models/                   # Data Models
│   ├── Administrator.cs          
│   ├── AnalyticsRecord.cs        
│   ├── Appointment.cs            
│   ├── Department.cs             
│   ├── Doctor.cs                 
│   ├── Feedback.cs               
│   ├── Log.cs                    
│   ├── MediScopeContext.cs       
│   ├── Patient.cs                
│   ├── Resource.cs               
│   ├── TestResult.cs             
│   └── ViewModels/               
│
├── Views/                    # Razor Views
│   ├── Account/                  
│   ├── Admin/                    
│   ├── Appointment/              
│   ├── Department/               
│   ├── Doctor/                   
│   ├── Home/                     
│   ├── Patient/                  
│   ├── Resource/                 
│   ├── Shared/                   
│   ├── _ViewImports.cshtml       
│   └── _ViewStart.cshtml         
│
├── Repositories/             # Data Access Layer
│   ├── IRepository.cs            
│   ├── Repository.cs             
│   ├── AdminRepository.cs        
│   ├── AppointmentRepository.cs  
│   ├── DepartmentRepository.cs   
│   ├── DoctorRepository.cs       
│   ├── PatientRepository.cs      
│   └── ResourceRepository.cs     
│
├── Services/                 # Business Logic Layer
│   ├── AnalyticsService.cs       
│   ├── AppointmentService.cs     
│   ├── AuthenticationService.cs  
│   ├── FeedbackService.cs        
│   ├── LoggingService.cs         
│   └── ValidationService.cs      
│
├── Identity/                 # Identity Configuration
│   └── ApplicationUser.cs        
│
├── Data/                     # Database Utilities
│   └── SeedData.cs               
│
├── Properties/
│   └── launchSettings.json   
│
├── Program.cs                # Application entry point
├── MediScope.csproj          # Project configuration
├── MediScope.sln             # Solution file
├── appsettings.json          # Application configuration
├── appsettings.Development.json  # Dev environment settings
└── README.md                 # This file
```

---

## Database

### Database Schema

The application uses the following main entities:

- **ApplicationUser** (Identity): Base user accounts
- **Administrator**: Admin profile data
- **Doctor**: Doctor profiles with specialties and departments
- **Patient**: Patient demographic information
- **Appointment**: Scheduled appointments between doctors and patients
- **TestResult**: Medical test results linked to patients
- **Department**: Medical departments (e.g., Cardiology, Family Medicine)
- **Resource**: Hospital resources and equipment
- **Feedback**: Patient feedback and ratings
- **Log**: System activity logs
- **AnalyticsRecord**: Analytics and metrics data

> **Note**: Migrations are automatically applied on application startup via `context.Database.Migrate()` in `Program.cs`.

---

## Architecture

### Layered Architecture

```
┌─────────────────────────────────────┐
│     Presentation Layer (MVC)        │
│  Controllers → Views → ViewModels   │
└──────────────┬──────────────────────┘
               │
┌──────────────▼──────────────────────┐
│        Service Layer                │
│  Business Logic & Validation        │
└──────────────┬──────────────────────┘
               │
┌──────────────▼──────────────────────┐
│     Repository Layer                │
│  Data Access & Persistence          │
└──────────────┬──────────────────────┘
               │
┌──────────────▼──────────────────────┐
│     Data Layer (EF Core)            │
│  MediScopeContext → SQLite DB       │
└─────────────────────────────────────┘
```

### Dependency Injection

All major components are registered in `Program.cs`:

- **Repositories**: Scoped lifetime for data access
- **Services**: Scoped lifetime for business logic
- **DbContext**: Scoped lifetime for database operations
- **Identity**: Configured with custom user model

---

## Key Components

### Controllers
- **Role-based routing** via `[Authorize(Roles = "...")]`
- **Action filters** for validation and logging
- **Async/await** patterns for database operations

### Services
- **AnalyticsService**: Computes system metrics and reports
- **AppointmentService**: Manages appointment lifecycle
- **ValidationService**: Centralized input validation
- **LoggingService**: Tracks system events and user actions

### Repositories
- **Generic Repository Pattern**: Reusable CRUD operations
- **Specialized Repositories**: Domain-specific queries