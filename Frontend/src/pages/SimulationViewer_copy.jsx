import React, { useState, useEffect, useRef } from "react";
import * as signalR from "@microsoft/signalr";
import { useParams } from "react-router-dom";
import * as PIXI from "pixi.js";
import { Application, Graphics } from "pixi.js";

const SimulationViewer_copy = () => {
  const { simulationId } = useParams();
  const [connection, setConnection] = useState(null);
  const pixiContainerRef = useRef(null);
  const pixiAppRef = useRef(null);
  const pixiFlag = useRef(false);

  useEffect(() => {
    const getToken = async () => {
      try {
        const response = await fetch("/api/auth/guest", {
          method: "POST",
          credentials: "include",
        });

        if (!response.ok) {
          throw new Error(`Hiba: ${response.status}`);
        }

        console.log("Vendég token sikeresen lekérve! cookieba mentve");
      } catch (error) {
        console.error("Token lekérés sikertelen:", error);
      }
    };

    getToken();
  }, []);

  useEffect(() => {
    async function init() {
      if (pixiFlag.current) return; // otherwise react appends it twice, in strict mode
      pixiFlag.current = true;
      const app = new Application();
      pixiAppRef.current = app;
      await app.init({ background: "#AA0000", width: 1000, height: 1000 });
      pixiContainerRef.current.appendChild(app.canvas);
    }
    init();
  }, []);

  const startConnection = () => {
    const newConnection = new signalR.HubConnectionBuilder()
      .withUrl("/simulationHub", {
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
        const gameState = JSON.parse(state);
        updatePixiScene(gameState);
        // console.log("Új játékállapot érkezett:", gameState);
      });
    } else {
      console.error("A kapcsolat még nincs készen!");
    }
  };

  const updatePixiScene = (gameState) => {
    const pixiGraphics = new Graphics();
    pixiAppRef.current.stage.removeChildren();
    pixiGraphics.clear();

    // Add new items to the scene
    gameState.forEach((item) => {
      pixiGraphics
        .circle(item.Position.X * 10, item.Position.Y * 10, 10)
        .fill(
          item.Type === 1 ? 0xff0000 : item.Type === 2 ? 0x0000ff : 0x00ff00
        );
      pixiAppRef.current.stage.addChild(pixiGraphics);
      // graphics.endFill();
    });
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
      <div
        ref={pixiContainerRef}
        style={{ border: "1px solid black", width: "1000px", height: "1000px" }}
      ></div>
    </div>
  );
};

export default SimulationViewer_copy;
