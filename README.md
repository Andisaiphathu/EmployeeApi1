**# Employee Management System API**

**A secure, Scalable backend API built with ASP.NET CORE 8, implementing authentication, role-based access and production ready DevOps practices.**

<img width="1536" height="1024" alt="image" src="https://github.com/user-attachments/assets/d3674e53-ea7b-4c0b-bc1a-c99f03e25dcd" />


# **1. Tech Stack**

# **2. Key Feauters**

- User Registration & Login
- JWT Authentication
- Refresh Tokens
- Password Reset (secure token-based)
- Role Management (Super Admin/ Admin/ User}
- Email Notications (Login Alerts, Reset   Password)
- Token Expiry & Cleanup Background service
- Secure Secret Management (.env, Kubernetes secrets)
- Token hashing
- Background cleanup
- Secrets management
- Kubernetes
  
# **3. Project Architecture**
<img width="1885" height="472" alt="image" src="https://github.com/user-attachments/assets/29fbc4ea-d754-46e1-9e06-7b99635a1f0b" />

# **4. BPMN Workflows (PlantUML)**
**User Register**
<img width="251" height="415" alt="image" src="https://github.com/user-attachments/assets/8451440d-3990-46ac-a794-d3e3624af5a2" />

**User Login**
<img width="490" height="587" alt="image" src="https://github.com/user-attachments/assets/b32af841-b194-47c0-bc8f-c39771cc1aaf" />

**Super Admin Flow**
<img width="575" height="560" alt="image" src="https://github.com/user-attachments/assets/a1b96447-cebc-4e7a-b638-95a703082155" />

**Admin Flow**
<img width="565" height="534" alt="image" src="https://github.com/user-attachments/assets/7cc4a096-7937-4e66-83d1-5ed0af3f88c6" />

**Business Logic Diagram**
<img width="1536" height="1024" alt="image" src="https://github.com/user-attachments/assets/540a1862-94a8-4b21-8d77-70239f2e8717" />

**Sequence Diagram**
<img width="478" height="307" alt="image" src="https://github.com/user-attachments/assets/beea603e-134f-4957-8f79-f3b304c9bd9b" />

**Use Case Diagram**
<img width="709" height="179" alt="image" src="https://github.com/user-attachments/assets/268d4bc6-f05c-4feb-b5f1-84e64da5849d" />

**5. Quick Start**

**Quick Start**

**Prerequisites**
- .NET 8 SDK
- Docker
- SQL Server
- Git

** ▶ Run with Docker Compose**

docker-compose up --build

**6. CI/CD Pipeline**
- Docker + CI/CD
- Build Docker Image
- Push to DockerHub
- Ready for Kubernetes Deployment

**7. App runs on:**
http://localhost:5000

**▶ Run Locally**
- dotnet restore
- dotnet build
- dotnet run


**8. API Endpoints**
<img width="1920" height="1080" alt="Screenshot (469)" src="https://github.com/user-attachments/assets/b9bc94b3-a262-4f77-afcc-1df1bfe66ae3" />

<img width="1806" height="826" alt="Screenshot 2026-05-04 201835" src="https://github.com/user-attachments/assets/b2aa861d-024e-4cc8-b074-d25cd30bcddf" />

<img width="1781" height="870" alt="Screenshot 2026-05-04 202020" src="https://github.com/user-attachments/assets/65848d30-c9b8-49e1-acdb-9dd4eebd19c4" />

<img width="535" height="816" alt="Screenshot 2026-05-02 122625" src="https://github.com/user-attachments/assets/0f4facf8-f0d4-47b0-a7c6-93ff1999eecd" />

### Swagger UI
http://localhost:5000/swagger

**9. PostMan APIs**

<img width="1196" height="914" alt="Screenshot 2026-05-02 150141" src="https://github.com/user-attachments/assets/4ab8d53c-481b-473a-8603-216f46701c52" />

<img width="1198" height="901" alt="Screenshot 2026-05-02 145547" src="https://github.com/user-attachments/assets/3fcc8423-5853-4453-b387-f64a68daa0d0" />

**10. Default Admin Account**

**Default Admin**

Email: superadmin@example.com  
Password: (seeded in DB – change after first login)

**11. Running Tests**

**Running Tests**

dotnet test



**11. Environment Variables**

```md
## 🔑 Environment Variables

| Variable | Default | Description |
|---------|--------|------------|
| ConnectionStrings__DefaultConnection | - | Database connection |
| JwtConfig__Key | - | JWT secret key |
| JwtConfig__Issuer | localhost | Token issuer |
| JwtConfig__Audience | localhost | Token audience |
| EmailSettings__Host | smtp | SMTP server |
| EmailSettings__Email | - | Email address |
| EmailSettings__Password | - | Email password |
| EmailSettings__Port | 587 | SMTP port |

**12. Kubernetes**
### Apply deployment

☸ Kubernetes Deployment

kubectl apply -f deployment.yaml

**Apply service**

kubectl apply -f service.yaml

**Check pods**

kubectl get pods




