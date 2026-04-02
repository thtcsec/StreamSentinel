# SentinelStream - Secure Incident Response War Room

![Agora Logo](agora.png)

**SentinelStream** is a real-time collaboration platform designed specifically for Cyber Security teams (SOC/IR) to handle incident response. Built with C# and Agora SDK.

## 🛡️ Key Features
* **Real-time Video/Audio**: Low-latency communication via Agora RTC.
* **Log Streaming Integration**: Live system logs and metrics streamed directly into the session using Agora Data Stream.
* **E2EE (End-to-End Encryption)**: All media streams are encrypted with **AES-256** before being sent.
* **Immutable Forensics**: Recordings are automatically hashed (SHA-256) upon session termination to ensure data integrity for legal investigations.
* **Shared Terminal**: Real-time collaborative terminal for executing emergency commands.

## 🛠️ Tech Stack
* **Client**: C# / WPF (.NET 8)
* **Backend**: ASP.NET Core
* **SDK**: Agora RTC & Real-Time Messaging (RTM)
* **Monitoring Agent**: Python (FastAPI / WebSockets)
* **Encryption**: System.Security.Cryptography (AES-GCM)

## 🚀 Getting Started

### 1. Configuration ⚙️
1. Clone the repo: `git clone https://github.com/thtcsec/SentinelStream.git`
2. Run the setup script to generate secure keys:
   ```powershell
   .\setup_env.ps1
   ```
3. Open `.env` and fill in your `AGORA_APP_ID` and `AGORA_APP_CERTIFICATE`.

### 2. Run the Monitoring Agent (Target Server) 🐍
1. Navigate to the `agent` folder.
2. Activate the virtual environment and run the agent:
   ```powershell
   cd agent
   .\.venv\Scripts\activate
   python -m uvicorn log_exporter:app --host 0.0.0.0 --port 8000
   ```

### 3. Run the SOC Dashboard (WPF Client) 🛡️
1. Build the solution in Visual Studio 2022 or via CLI:
   ```powershell
   dotnet build
   ```
2. Run the WPF application:
   ```powershell
   dotnet run --project src/SentinelStream.App
   ```

## 🔍 How It Works?
1. **The SOC Team** logs into the WPF app. They enter a "War Room" (Session ID).
2. **The App** establishes an encrypted Video/Audio link via **Agora RTC** (using AES-256 E2EE).
3. **The Target Server** (running the Python Agent) starts streaming real-time system logs and security alerts directly to the dashboard via WebSockets.
4. **Forensics**: Every session is recorded, and its hash (SHA-256) is stored to ensure the evidence remains immutable.

## 🔒 Security Disclaimer
This tool is for educational and professional incident response purposes only. Ensure you have proper authorization before monitoring any system.
