# Serverless Document Control Portal (.NET 10)

An enterprise-ready decoupled system featuring an **ASP.NET Core MVC Frontend Dashboard** linked seamlessly with an **HTTP-Triggered .NET 10 Isolated Azure Function Backend** to orchestrate cloud object file pipelines.

## 🏗️ Architectural Flow
1. **Frontend Tier:** Users drop binary assets onto the MVC dashboard web control grid.
2. **Microservice Tier:** The MVC tier safely packages and routes the stream to an HTTP Azure Function endpoint via `IHttpClientFactory`.
3. **Storage Tier:** The function deduplicates names using timestamps and streams raw blocks directly into Azure Blob Storage.

## 🛠️ Tech Stack & Patterns
* **Framework:** .NET 10.0 (Isolated Worker Model)
* **Cloud Storage:** Azure Blob Storage (SDK v12)
* **Local Architecture Emulator:** Azurite via VS Code 
* **Design Pattern:** Hybrid Metadata Architecture (Separating heavy file objects from light tracking database lookups)

## 🚀 How to Run Locally
1. Open **VS Code**, go to settings, enable **`Azurite: Skip Api Version Check`**, and click **Azurite Blob** to turn on port `10000`.
2. Open `ServerlessDocPortal.sln` inside Visual Studio 2022.
3. Configure the solution to **Multiple Startup Projects** to run both the MVC Client and Function app together.
4. Press `F5` to execute the end-to-end CRUD loop.
