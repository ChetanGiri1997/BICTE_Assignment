# 💼 Employee Management System

An elegant and modern **ASP.NET MVC** web application for managing employees and their qualifications — built using **Visual Programming in C#**, and deployed on the cloud using **DigitalOcean**, **Cloudflare DNS**, and **cloudflared tunnel** for secure access.

---

## 🌐 Live Preview

🌍 **URL**: _[https://app.chetangiri.com.np] 
🔐 Tunnel: Powered by **Cloudflare Tunnel** (secured and encrypted access)

---

## 🚀 Tech Stack & Features

| Layer         | Tech Used |
|---------------|-----------|
| 👨‍💻 Language      | `C#`, `.NET 6+` |
| 🎨 Frontend     | Razor Views, Bootstrap 5 |
| ⚙️ Backend      | ASP.NET MVC, Entity Framework Core |
| 💾 Database     | SQL Server  |
| ☁️ Deployment   | DigitalOcean Droplet |
| 🌩️ Tunneling    | Cloudflared Tunnel (Cloudflare Zero Trust) |
| 🌐 DNS          | Cloudflare (DNS + CDN) |
| 🔐 Authentication | Cookie-based login system |

---

## 🏧 Folder Structure

```plaintext
EmployeeManagementSystem/
├── Controllers/            # MVC Controllers (Account, Employee)
├── Data/                   # DbContext & Seeder
├── Models/                 # C# Models (Employee, Qualification)
├── Views/                  # Razor Views (Login, CRUD, Layout)
├── wwwroot/                # Static files (CSS, JS, Bootstrap)
├── appsettings.json        # App configs
├── Program.cs              # App startup logic
└── EmployeeManagementSystem.csproj
```

---


## ⚙️ Setup Instructions

### 🧑‍💻 Local Development

1. Clone the repo:
   ```bash
   git clone https://github.com/ChetanGiri1997/BICTE_Assignment.git
   cd EmployeeManagementSystem
   ```

2. Restore dependencies & run:
   ```bash
   dotnet restore
   dotnet ef database update
   dotnet run
   ```

3. Visit: [http://localhost:5000]

---

## ☁️ Deployment on DigitalOcean

- Deployed on a **Linux Droplet**
- ASP.NET app hosted using **Kestrel** + **Nginx reverse proxy**
- Auto-start configured with **Systemd**

### 🌩️ Tunneling via Cloudflare

- Installed `cloudflared` on droplet
- Configured secure tunnel with:

```bash
cloudflared tunnel create employee-management
cloudflared tunnel route dns employee-management employee.yourdomain.com
```

- Managed securely via **Cloudflare Zero Trust Dashboard**

---

## ✨ Features

- ✅ User Authentication (Login system)
- 🧑 Add, Edit, Delete Employees
- 🎓 Manage Employee Qualifications
- 🔍 Clean UI using Bootstrap
- 🌐 Secure and live with HTTPS using Cloudflare

---


## 📚 Learnings & Tools

- MVC Architecture with Razor Views
- Entity Framework for database interaction
- Cloud deployment and tunneling
- Real-world Visual Programming practices

---

## 👨‍🏫 Academic Relevance

> 📘 **Course**: Visual Programming (BICTE 6th Semester)  
> 🧪 **Focus**: MVC Architecture, Full-stack ASP.NET Development, Cloud Deployment  
> 🎓 **Use Case**: Demonstrates practical knowledge of web dev using C# in real-world deployment environments

---

## 📩 Contact

**👨‍💻 Developer**: Chetan Giri  
📧 Email: info@chetangiri.com.np 
🌐 Portfolio: [https://chetangiri.com.np]

---

## 🏑 Final Note

This project demonstrates **real-world web development** using **Microsoft's modern tech stack**, fully integrated with cloud services and production-ready best practices. It’s a perfect showcase for both **academic evaluation** and **professional deployment**.

⭐ Star this repo if you liked the project!

