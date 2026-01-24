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

- (If you have xUnit tests, run with `dotnet test`)

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
