import React, { useState, useEffect } from "react";
import * as signalR from "@microsoft/signalr";
import SimulationAnimation from "./SimulationAnimation";
import { useParams } from "react-router-dom";

const SimulationViewer = () => {
  //const [simulationId, setSimulationId] = useState("waiting for id");
  const [simulationData, setSimulationData] = useState("");
  const [connection, setConnection] = useState(null);
  const [connect, setConect] = useState(false);
  const [token, setToken] = useState("");

  const { simulationId } = useParams();
  //console.log("simulationId:", simulationId);

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

  const fetchStartSimulation = async () => {
    try {
      const response = await fetch("/api/BackendSimulation/play", {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify({ simulationId }),
      });

      if (!response.ok) {
        throw new Error(`Hiba: ${response.status}`);
      }

      console.log("Szimuláció elindítva!");
    } catch (error) {
      console.error("Szimuláció indítás sikertelen:", error);
    }
  };

  const joinSimulation = () => {
    if (connection && simulationId) {
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
      <div>
        <button onClick={joinSimulation} style={{ padding: "5px 10px" }}>
          Join Simulation
        </button>
      </div>
      <div>
        <button onClick={fetchStartSimulation} style={{ padding: "5px 10px" }}>
          Start Simulation
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
