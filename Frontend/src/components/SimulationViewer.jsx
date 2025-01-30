import React, { useState, useEffect } from "react";
import * as signalR from "@microsoft/signalr";
import SimulationAnimation from "./SimulationAnimation";

const SimulationViewer = () => {
  const [simulationId, setSimulationId] = useState("waiting for id");
  const [simulationData, setSimulationData] = useState("");
  const [connection, setConnection] = useState(null);
  const [connect, setConect] = useState(false);

  const startConnection = () => {
    const newConnection = new signalR.HubConnectionBuilder()
      .withUrl("http://localhost:5213/simulationHub")
      .withAutomaticReconnect()
      .configureLogging(signalR.LogLevel.Information)
      .build();

    newConnection
      .start()
      .then(() => {
        console.log("Kapcsolódás sikeres!");
        setConnection(newConnection);
      })
      .catch((err) => console.error("Kapcsolódási hiba: ", err));

    return () => {
      if (newConnection) {
        newConnection.stop();
      }
    };
  };

  const joinSimulation = () => {
    if (connection) {
      connection
        .invoke("JoinSimulation", simulationId)
        .then(() => console.log(`Csatlakoztál a ${simulationId} csoporthoz.`))
        .catch((err) => console.error("Hiba a csatlakozás során: ", err));

      connection.on("ReceiveGameState", (state) => {
        setSimulationData(JSON.stringify(state, null, 2));
      });
    } else {
      console.error("A kapcsolat még nincs készen!");
    }
  };

  return (
    <div style={{ padding: "20px", fontFamily: "Arial, sans-serif" }}>
      <h1>Simulation Viewer</h1>
      <button onClick={startConnection} style={{ padding: "5px 10px" }}>
        Connect simulation
      </button>

      <div style={{ marginBottom: "10px" }}>
        <label htmlFor="simulationId">Simulation ID: </label>
        <input
          type="text"
          id="simulationId"
          value={simulationId}
          onChange={(e) => setSimulationId(e.target.value)}
          style={{ marginRight: "10px" }}
        />
        <button onClick={joinSimulation} style={{ padding: "5px 10px" }}>
          Join Simulation
        </button>
      </div>
      <pre
        style={{
          padding: "10px",
          borderRadius: "5px",
          overflowX: "auto",
        }}
      >
        {(simulationData && SimulationAnimation(simulationData)) ||
          "Nincs még játékállapot..."}
      </pre>
    </div>
  );
};

export default SimulationViewer;
