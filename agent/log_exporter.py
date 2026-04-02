import os
import asyncio
import logging
from fastapi import FastAPI, WebSocket
from dotenv import load_dotenv

# Load environment variables
load_dotenv()

app = FastAPI(title="SentinelStream Log Exporter")

# Configure logging
logging.basicConfig(level=logging.INFO, format='%(asctime)s - %(name)s - %(levelname)s - %(message)s')
logger = logging.getLogger(__name__)

@app.get("/")
def read_root():
    return {"status": "running", "component": "SentinelStream.Agent"}

@app.websocket("/ws/logs")
async def websocket_endpoint(websocket: WebSocket):
    await websocket.accept()
    logger.info("New WebSocket connection established.")
    try:
        while True:
            # Demonstration: Reading a mock log or system event
            # In a real scenario, this would tail a log file or listen for system events
            mock_log = "2026-04-02 21:40:43 INF [System] Security alert: Unrecognized login attempt from 192.168.1.105"
            await websocket.send_text(mock_log)
            await asyncio.sleep(5)
    except Exception as e:
        logger.error(f"WebSocket error: {e}")
    finally:
        await websocket.close()

if __name__ == "__main__":
    import uvicorn
    # Start the agent
    # UVICORN_HOST = os.getenv("AGENT_HOST", "0.0.0.0")
    # UVICORN_PORT = int(os.getenv("AGENT_PORT", 8080))
    # uvicorn.run(app, host=UVICORN_HOST, port=UVICORN_PORT)
    print("Log Exporter Agent Initialized. Use uvicorn to run this script.")
