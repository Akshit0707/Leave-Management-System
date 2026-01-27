# Leave Management System

A full-stack leave management system for organizations, built with **ASP.NET Core 8** (backend API), **Angular 21** (frontend), and **PostgreSQL** (database).

---

## 🚀 Live Demo

- **Frontend (Netlify):** "https://leavems.netlify.app"
- **Backend (Railway):** " https://leave-management-system.up.railway.app " 
  

---

## 🏗️ Project Structure

```
LeaveManagementSystem.sln
leave-management-ui/         # Angular 21 frontend
LeaveManagement.API/         # ASP.NET Core 8 backend API
```

---

## 📦 Tech Stack

- **Frontend:** Angular 21, RxJS, Angular SSR, CSS
- **Backend:** ASP.NET Core 8, Entity Framework Core, JWT Auth, Swagger
- **Database:** PostgreSQL (managed by Railway)
- **Deployment:** Railway (API & DB), Netlify (frontend)
- **Containerization:** Docker (for API)
- **Testing:** Vitest (frontend), xUnit (backend, if added)
- **Other:** CORS, Role-based access, RESTful API

---


## 📝 Features

- User registration & login (Manager/Employee roles)
- JWT authentication and authorization
- Manager and Employee dashboards
- Leave request creation, approval, and tracking
- Manager can view and approve/reject pending leaves
- Employee can view their leave history and status
- Secure password hashing (BCrypt)
- Automated database migrations on deploy
- CORS and environment-based configuration

### New & Improved Features (2026)

- **Modernized UI/UX:**
  - Consistent, professional design with improved button and navigation styling
  - Responsive layout for all devices
- **Enhanced Password Reset Flow:**
  - Multi-step process: request, approval, reset, done
  - Real-time status updates (pending, approved, rejected)
  - Admin rejection/approval comments shown to user
  - Clear feedback and actionable messages for all states
- **Navigation Improvements:**
  - Modern, visually prominent navbar with left-aligned title and right-aligned back button
  - "Back to Home" and "Back to Login" buttons styled for clarity and accessibility
- **Robust Status Handling:**
  - UI always reflects the latest request status (pending, approved, rejected, completed)
  - Admin comments and rejection reasons are clearly displayed
- **General Polish:**
  - Improved form layouts and field consistency
  - Angular Material integration for a modern look
  - Accessibility and usability enhancements

---

## 🛠️ Local Development

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download)
- [Node.js & npm](https://nodejs.org/)
- [Angular CLI](https://angular.io/cli)
- [PostgreSQL](https://www.postgresql.org/) (for local DB)

### Backend (API)

```sh
cd LeaveManagement.API
# Set your local DATABASE_URL or use appsettings.Development.json
# Run migrations (if needed):
dotnet ef database update
# Start API
DOTNET_ENVIRONMENT=Development dotnet run
```
- API runs on `http://localhost:8080` by default.

### Frontend (Angular)

```sh
cd leave-management-ui
npm install
ng serve
```
- App runs on `http://localhost:4200` by default.

---

## ⚙️ Environment Variables

### Backend

- `DATABASE_URL` (PostgreSQL connection string, e.g. for Railway or local)
- `Jwt:Key`, `Jwt:Issuer`, `Jwt:Audience` (JWT settings, see `appsettings.Development.json`)

### Frontend

- `src/environments/environment.ts` → `apiUrl` (set to your backend URL)

---

## 🗄️ Database & Migrations

- Uses Entity Framework Core migrations.
- On Railway, migrations are applied automatically on startup.
- For local dev, run:  
  ```sh
  dotnet ef database update
  ```

---

## 🐳 Docker

The backend API can be built and run in a container:

```sh
cd LeaveManagement.API
docker build -t leave-management-api .
docker run -p 8080:8080 --env DATABASE_URL=... leave-management-api
```

---

## 🧪 Testing
### Frontend

```sh
cd leave-management-ui
npm test
```

### Backend


```sh
cd LeaveManagement.API.Tests
dotnet test
```

### API Endpoint Test Coverage

- **100% API endpoint coverage:** Every controller action in the backend API is exercised by at least one automated test (see `*.ControllerTests.cs` files).
- **How to verify:**
  - Run `dotnet test` to see all tests pass (e.g., `46 succeeded, 0 failed`).
  - Generate a coverage report (see below) to view line/branch coverage.
  - Open the HTML report at `coverage-report/index.html` for a visual summary.
- **Line coverage:** ~10% (see coverage report for details)
- **How this is achieved:**
  - Each API controller action has a corresponding test method.
  - Tests cover both success and error/edge cases for all endpoints.

#### Generate Coverage Report

```sh
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura /p:CoverletOutput=coverage.xml
reportgenerator -reports:./**/coverage.cobertura.xml -targetdir:coverage-report
# Open coverage-report/index.html in your browser
```

---

## 🛡️ Security

- Passwords are hashed with BCrypt.
- JWT-based authentication.
- Role-based authorization (Manager/Employee).
- CORS policy restricts allowed origins.

---

## 🌐 Deployment

- **Backend:** Push to GitHub → Railway auto-deploys and runs migrations.
- **Frontend:** Push to GitHub → Netlify auto-deploys.

---

## 📄 License

MIT

---

## 🙏 Credits

- Built with .NET 8, Angular 21, PostgreSQL, Railway, and Netlify.
