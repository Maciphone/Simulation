import React, { useState, useEffect } from "react";
import * as signalR from "@microsoft/signalr";
import SimulationAnimation from "./SimulationAnimation";

const SimulationViewer = () => {
  const [simulationId, setSimulationId] = useState("waiting for id");
  const [simulationData, setSimulationData] = useState("");
  const [connection, setConnection] = useState(null);
  const [connect, setConect] = useState(false);
  const [token, setToken] = useState("");

  useEffect(() => {
    const getToken = async () => {
      try {
        const response = await fetch("/api/auth/guest", {
          // Ha proxy van beállítva, elég csak "/api"
          method: "POST",
          // headers: {
          //   "Content-Type": "application/json",
          // },
          credentials: "include",
        });

        if (!response.ok) {
          throw new Error(`Hiba: ${response.status}`);
        }

        // const data = await response.json();
        // setToken(data.token);
        // console.log("Vendég token:", data.token);
        console.log("Vendég token sikeresen lekérve! cookieba mentve");
      } catch (error) {
        console.error("Token lekérés sikertelen:", error);
      }
    };

    getToken();
  }, []);

  const startConnection = () => {
    const newConnection = new signalR.HubConnectionBuilder()
      .withUrl("/simulationHub", {
        //accessTokenFactory: () => token, // Itt adjuk át a JWT tokent!
        withCredentials: true,
      })
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

      connection.on("JoinedSimulation", (simulationId) => {
        console.log(`Sikeresen csatlakoztál a ${simulationId} csoporthoz!`);
      });

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
        {simulationData || "Nincs még játékállapot..."}
      </pre>
    </div>
  );
};

export default SimulationViewer;
