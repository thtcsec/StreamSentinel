# SentinelStream - Secure Incident Response War Room

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
1. Clone the repo: `git clone https://github.com/thtcsec/SentinelStream.git`
2. Copy `.env.example` to `.env` and fill in your Agora credentials.
3. Build the solution in Visual Studio 2022.
4. Run the Python agent on the target server to begin log ingestion.

## 🔒 Security Disclaimer
This tool is for educational and professional incident response purposes only. Ensure you have proper authorization before monitoring any system.
