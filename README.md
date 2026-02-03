# 🚀 AI-Powered Onboarding Workflow Backend

A scalable backend system that automates employee onboarding using structured workflows, task orchestration, progress tracking, and AI-generated onboarding plans.

This project replaces manual onboarding processes (spreadsheets, emails, ad-hoc checklists) with a centralized, workflow-driven platform that standardizes onboarding across teams and projects.

---

## 📌 Problem

Employee onboarding in many organizations is:

- Manual and inconsistent
- Managed through spreadsheets or emails
- Hard to track progress
- Time-consuming for mentors
- Difficult to standardize across teams

There is no intelligent system to automate onboarding or personalize task plans.

---

## ✅ Solution

This backend provides:

- Role-based onboarding templates
- Automatic task assignment
- User-specific task tracking
- Real-time progress calculation
- Project & team management
- AI-generated onboarding plans using Gemini

The system transforms onboarding into a structured, automated, and measurable workflow.

---

## 🧠 Core Features

### 🔐 Authentication & Access Control
- User login
- Role-based access control (Admin / Mentor / User)
- Permissions & sections

### 📁 Projects & Teams
- Create and manage projects
- Assign mentors and members
- Track tech stacks & modules
- Project overview endpoints

### ⚙️ Onboarding Workflow Engine
- Create onboarding templates
- Define template tasks
- Assign templates to users
- Auto-generate user tasks
- Track completion status
- Automatic progress (%) calculation

### 📊 Progress & Analytics
- Task-level completion tracking
- Real-time onboarding progress
- Dashboard statistics for admins

### 🤖 AI Integration (Gemini)
- Generate onboarding tasks automatically
- Role & tech stack based task suggestions
- Prompt engineering
- Response sanitization & parsing

---

## 🏗 Architecture Overview

Client (Future Frontend)
        ↓
ASP.NET Core Web API
        ↓
Service Layer (Business Logic)
        ↓
EF Core ORM
        ↓
SQL Server Database
        ↓
Gemini AI API (Task Generation)

---

## 🛠 Tech Stack

### Backend
- .NET Core Web API
- Entity Framework Core
- SQL Server
- RESTful APIs
- Dependency Injection
- Service Layer Architecture

### AI
- Google Gemini API

### Config & Security
- Environment variables / User Secrets
- Secure API key handling

---

## 📂 Key Modules

- Auth & Users
- Roles & Permissions
- Projects & Members
- Onboarding Templates
- Template Tasks
- User Tasks
- Progress Tracking
- AI Task Generator

---

## 🚀 Example Workflow

1. Admin creates onboarding template  
2. Adds tasks manually OR generates tasks using AI  
3. Assigns template to a user  
4. Tasks auto-created for that user  
5. User completes tasks  
6. System calculates real-time progress  

---

## ▶️ Running Locally

### 1. Clone repo
git clone <repo-url>

### 2. Setup database
Update connection string in appsettings.Development.json

### 3. Add Gemini API key
Using user secrets:

dotnet user-secrets set "Gemini:ApiKey" "YOUR_KEY"

### 4. Run
dotnet run

API runs at:
http://localhost:5000

---

## 📈 Future Improvements

- Frontend dashboard (React)
- JWT authentication
- Deployment to cloud
- Notifications
- Reporting & analytics
- AI mentor chat assistant

---

## 💡 What This Project Demonstrates

- Backend architecture design
- Workflow engine implementation
- REST API development
- Role-based systems
- AI service integration
- Scalable SaaS-style thinking

---

## 👤 Author

Aryan Mehta  
Backend & AI-focused Software Engineer
