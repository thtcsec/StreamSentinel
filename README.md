# SentinelStream (StreamSentinel)

[![Agora Logo](agora.png)](https://www.agora.io/)

Desktop **war room** shell for security operations: a WPF dashboard that shows a **live log pane**, **participant list**, and **chat**, while a small **Python agent** can tail real log files or emit timed JSON events over WebSockets.

**Repository:** [github.com/thtcsec/SentinelStream](https://github.com/thtcsec/SentinelStream)

## What this is for (today)

This repo is a **structured prototype**, not a finished SOC product.

- **Useful now:** plug in a log source (file tail or mock interval), point the WPF client at the agent with `LOG_SERVER_URL`, and you get a **single place to watch lines** during drills, demos, or light monitoring—similar in spirit to tailing logs in a shared terminal, but in a dedicated UI.
- **Not implemented yet:** real Agora RTC (voice/video), true E2EE on media, shared remote terminal, ASP.NET token service, and automated forensic hashing of recordings. The C# layer includes **stubs and helpers** (`AgoraWarRoomClient`, `AesGcmEncryptor`, `ForensicHasher`) for a future integration path; the UI copy reflects **actual** behavior where we changed it.

**Rule of thumb:** only deploy against systems you are **authorized** to monitor.

## Architecture

| Piece | Role |
|--------|------|
| **WPF app** (`src/SentinelStream.App`) | Login → War Room UI; optional **log substring filter** (full buffer kept for export); connects to the agent when configured. |
| **LogStreamClient** (`SentinelStream.Services`) | `ClientWebSocket` → JSON or plain text lines → `LogEntry`. |
| **AgoraWarRoomClient** (`SentinelStream.Core`) | **Simulation** of channel join/leave/chat (no Agora NuGet wired yet). |
| **Python agent** (`agent/log_exporter.py`) | WebSocket `/ws/logs`, optional **HTTP POST `/ingest/log`**, optional **Syslog UDP**; all env-driven. |

## Configuration (no hardcoded URLs in code paths)

### Repo root `.env` (WPF)

Copy `.env.example` to `.env` and run `.\setup_env.ps1` to rotate crypto-related keys.

| Variable | Meaning |
|----------|---------|
| `LOG_SERVER_URL` | e.g. `ws://127.0.0.1:8000/ws/logs` — empty = no agent connection. |
| `DEMO_LOG_FEED` | `true` / `false` — extra **demo** lines generated inside the app (independent of the agent). |
| `AGORA_APP_ID` | Reserved for future Agora integration. |
| `ENCRYPTION_KEY` | Reserved for future media encryption when Agora is integrated. |
| `FORENSIC_SALT` | Salt for `ForensicHasher` (session log export). |
| `SESSION_EXPORT_ON_LEAVE` | `true` / `false` — when `true` and salt is set, leaving the war room writes a `.log` + forensic report (SHA-256). |
| `SESSION_EXPORT_DIRECTORY` | Optional folder for those files (default: system temp). |

The app resolves `.env` from the working directory or next to the executable.

### Agent `agent/.env` (optional)

Copy `agent/.env.example` to `agent/.env`. All sources are optional; combine as needed.

| Variable | Meaning |
|----------|---------|
| `LOG_TAIL_PATH` | One UTF-8 text file to **follow** (like `tail -f`). Ignored when `LOG_TAIL_PATHS` is set. |
| `LOG_TAIL_PATHS` | Comma-separated list of files to tail (max 32); overrides `LOG_TAIL_PATH` when non-empty. |
| `AGENT_MOCK_INTERVAL_SEC` | Seconds between **synthetic** JSON log events (`0` = off). |
| `AGENT_MOCK_MESSAGE` | Mock template; `{iso}` → UTC timestamp. |
| `AGENT_MOCK_SOURCE` | `source` field for mock lines. |
| `INGEST_TOKEN` | If set, `POST /ingest/log` must send header `X-Sentinel-Token: <token>`. |
| `SYSLOG_UDP_PORT` | UDP port to listen on (e.g. `5514`); messages are broadcast to every open dashboard. |
| `SYSLOG_QUEUE_MAX` | Max queued datagrams before drop (default `1000`). |

### Push logs without tailing files

With the agent running and the WPF client connected (`LOG_SERVER_URL` set), any script can push a JSON line:

```powershell
# PowerShell (optional token: -Headers @{ "X-Sentinel-Token" = "your-secret" })
$body = @{ message = "Backup job finished"; severity = "info"; source = "backup-script" } | ConvertTo-Json
Invoke-RestMethod -Uri "http://127.0.0.1:8000/ingest/log" -Method Post -Body $body -ContentType "application/json"
```

```bash
curl -sS -X POST http://127.0.0.1:8000/ingest/log \
  -H "Content-Type: application/json" \
  -d '{"message":"Cron alert","severity":"warning","source":"cron"}'
```

Batch: `POST /ingest/log/batch` with a JSON array (max 500 items). Same optional `X-Sentinel-Token`.

**Syslog:** set `SYSLOG_UDP_PORT`, then point a forwarder at `udp://<agent-host>:<port>`. Lines appear in the war room as `source` `syslog:<ip>:<port>`.

## Quick start

### 1. Keys (optional for log-only use)

```powershell
.\setup_env.ps1
```

Edit `.env`: set `LOG_SERVER_URL` if the agent runs locally:

```env
LOG_SERVER_URL=ws://127.0.0.1:8000/ws/logs
DEMO_LOG_FEED=false
```

Use `DEMO_LOG_FEED=true` if you want fake SOC-style lines **without** running the agent.

### 2. Monitoring agent

```powershell
cd agent
python -m venv .venv
.\.venv\Scripts\activate
pip install -r requirements.txt
copy .env.example .env
# Edit .env: set LOG_TAIL_PATH and/or AGENT_MOCK_INTERVAL_SEC
python -m uvicorn log_exporter:app --host 0.0.0.0 --port 8000
```

Health check: `GET http://127.0.0.1:8000/health` shows tail paths, mock interval, syslog port, whether ingest token is required, and how many WebSocket clients are connected.

### 3. WPF client

```powershell
dotnet build
dotnet run --project src/SentinelStream.App
```

Enter operator name and war room id. The footer shows **log agent connection status** and a short **prototype** note for RTC.

## Roadmap (high level)

1. More **sources** (e.g. Windows Event Log) behind env flags; tighten ingest auth if exposed beyond localhost.  
2. Integrate **Agora RTC** + token flow; then align encryption UI with real behavior.  
3. Optional **ASP.NET** backend for sessions and tokens.  
4. **Forensics:** extend beyond on-leave log export (e.g. recordings when RTC exists).  
5. **Shared terminal** only with a clear security and audit model.

## Security disclaimer

For **authorized** incident response, education, and testing only. Misuse against systems you do not own or lack permission to monitor may be illegal.

---

*Branding: project names use `SentinelStream.*`; the Agora image links to [Agora.io](https://www.agora.io/) as the intended RTC vendor for a future integration.*
