<p align="center">
  <img src="wwwroot/AspGoatLogo-Github.png" alt="AspGoat Logo" height="400" width="1000"/>
</p>

<h1 align="center">AspGoat</h1>

<p align="center">
  <i>An intentionally vulnerable ASP.NET Core web application for learning and practicing web application security.</i>
</p>

<p align="center">
  <a href="https://github.com/Soham7-dev/AspGoat/blob/main/LICENSE">
    <img src="https://img.shields.io/github/license/Soham7-dev/AspGoat?style=flat-square&color=blue" alt="license"/>
  </a>
  <a href="https://github.com/Soham7-dev/AspGoat/stargazers">
    <img src="https://img.shields.io/github/stars/Soham7-dev/AspGoat?style=flat-square&color=yellow" alt="stars"/>
  </a>
  <a href="https://github.com/Soham7-dev/AspGoat/network/members">
    <img src="https://img.shields.io/github/forks/Soham7-dev/AspGoat?style=flat-square&color=green" alt="forks"/>
  </a>
  <a href="https://github.com/Soham7-dev/AspGoat/actions">
    <img src="https://img.shields.io/github/actions/workflow/status/Soham7-dev/AspGoat/dotnet.yml?style=flat-square" alt="ci-status"/>
  </a>
  <a href="https://hub.docker.com/r/YOUR_DOCKERHUB_USERNAME/aspgoat">
    <img src="https://img.shields.io/docker/pulls/YOUR_DOCKERHUB_USERNAME/aspgoat?style=flat-square&logo=docker" alt="docker pulls"/>
  </a>
</p>

---

## 📖 About AspGoat

**AspGoat** is an intentionally vulnerable **ASP.NET Core** application designed to help security engineers and developers analyze, exploit, and mitigate common web application vulnerabilities.  
It includes labs covering the **OWASP Top 10** and additional scenarios, offering a practical way to learn secure coding.

⚠️ **Disclaimer**: This project is for **educational purposes only**. Do **not** deploy it in production environments.  

---

## ✨ Features

- Intentionally vulnerable ASP.NET Core MVC application  
- Hands-on security labs covering:
  - Cross-Site Scripting (**XSS**)
  - Cross-Site Request Forgery (**CSRF**)
  - SQL Injection (**SQLi**)
  - XML External Entity (**XXE**)
  - Local File Inclusion (**LFI**)
  - Remote Code Execution (**RCE**)
  - Unrestricted File Upload
  - Information Disclosure
  - Broken Authentication
  - Server-Side Request Forgery (**SSRF**)
  - Insecure Direct Object Reference (**IDOR**)
  - Insecure Deserialization
  - Command Injection
- Secure vs. insecure code snippets for comparison  
- Docker-ready for quick setup  

---

## 🛠️ Installation

### Option 1: Using Docker (recommended)

Clone the repository:

```bash
git clone https://github.com/Soham7-dev/AspGoat.git
cd AspGoat
````

Build the image:

```bash
docker build -t aspgoat .
```

Run the container:

```bash
docker run --rm -p 8000:8000 aspgoat
```

Access the application at:

```
http://localhost:8000
```

---

### Option 2: Using .NET SDK

1. Install **.NET SDK 8.0 (LTS)**:
   👉 [Download here](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)

   *(The SDK includes the runtime, so no additional installation is needed.)*

2. Clone the repository:

   ```bash
   git clone https://github.com/Soham7-dev/AspGoat.git
   cd AspGoat
   ```

3. Restore dependencies:

   ```bash
   dotnet restore
   ```

4. Run the application:

   ```bash
   dotnet run
   ```

5. Access the application at:

   ```
   http://localhost:5073
   ```

---

## 👥 Contributors

<a href="https://github.com/Soham7-dev/AspGoat/graphs/contributors">
  <img src="https://contrib.rocks/image?repo=Soham7-dev/AspGoat" />
</a>
