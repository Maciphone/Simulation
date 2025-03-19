import React, { useState, useEffect, useRef } from "react";
import * as signalR from "@microsoft/signalr";
import { useParams, useLocation, useNavigate } from "react-router-dom";
import * as PIXI from "pixi.js";
import { Application, Graphics } from "pixi.js";
import { use } from "react";
import { useDispatch } from "react-redux";
import { setSimulationIdRedux } from "../redux/simulationSlice";
import { createSimulation } from "../sevices/apiService";
import {
  roomExist,
  createSignalRConnection,
  // getSimulationId,
  sendSimulationId,
  getSimulationIdAsync,
} from "../sevices/hubService";
const SimulationViewerMaster = () => {
  //const { simulationId } = useParams();
  //query paraméterek lekérése
  const location = useLocation();
  const searchParams = new URLSearchParams(location.search);
  const gameMasterId = searchParams.get("gameMasterId");
  const rows = searchParams.get("rows");
  const columns = searchParams.get("columns");

  const [simulationData, setSimulationData] = useState(null);
  const [statistic, setStatistic] = useState(null);
  const [sum, setSum] = useState(0);
  const [winner, setWinner] = useState(null);

  const [connection, setConnection] = useState(null);
  const pixiContainerRef = useRef(null);
  const pixiAppRef = useRef(null);
  const pixiFlag = useRef(false);

  const navigate = useNavigate();

  const dispatch = useDispatch();
  const [simulationId, setSimulationId] = useState("");

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
      if (!simulationData) {
        return;
      }

      console.log("simulationData", simulationData);
      if (pixiFlag.current) return; // otherwise react appends it twice, in strict mode
      pixiFlag.current = true;
      const app = new Application();
      pixiAppRef.current = app;
      const rows = parseInt(simulationData.rows) * 10;
      const columns = parseInt(simulationData.columns) * 10;
      console.log("rows", rows);
      console.log("columns", columns);
      await app.init({ background: "#AA0000", width: columns, height: rows });
      pixiContainerRef.current.appendChild(app.canvas);
    }
    init();
    //<button onClick={getData}>getData</button>;
  }, [simulationData]);

  useEffect(() => {
    const startConnection = async () => {
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
    startConnection();
  }, []);

  //new game setup
  const handleNewGame = async (event) => {
    if (!connection) return;

    event.preventDefault();
    console.log("submit pushed");
    console.log("simulationData", simulationData);
    const itemCount = simulationData.itemCount;
    console.log("itemCount", itemCount);
    const initialData = { rows, columns, itemCount };
    console.log("initialData", initialData);

    try {
      const simulationId = await createSimulation(initialData);
      console.log(simulationId); //pipa
      if (simulationId) {
        dispatch(setSimulationIdRedux(simulationId));
        //const newInitialData = { rows, columns, itemCount, simulationId };
        const newInitialData = {
          rows: Number(rows), // Számként küldjük
          columns: Number(columns),
          itemCount: Number(itemCount),
          simulationId: simulationId,
        };
        console.log("newInitialData", newInitialData);
        console.log("gameMasterId", gameMasterId);
        await sendSimulationId(connection, gameMasterId, newInitialData);
        //setSimulationId(simulationId);
        joinSimulation();
        fetchStartSimulation();
      } else {
        throw new Error("No simulationId");
      }
    } catch (error) {
      console.error("Error: no initialisation", error);
    }
  };

  const fetchStartSimulation = async () => {
    try {
      const response = await fetch("/api/BackendSimulation/play", {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        credentials: "include",
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

  const pause = async () => {
    try {
      const response = await fetch("/api/BackendSimulation/pause", {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        cewdentials: "include",
        body: JSON.stringify({ simulationId }),
      });
      if (!response.ok) {
        throw new Error(`Hiba: ${response.status}`);
      }

      console.log("pause");
    } catch (error) {
      console.error("Szimuláció szüneteltetése sikertelen:", error);
    }
  };

  const resume = async () => {
    try {
      const response = await fetch("/api/BackendSimulation/resume", {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        credentials: "include",
        body: JSON.stringify({ simulationId }),
      });
      if (!response.ok) {
        throw new Error(`Hiba: ${response.status}`);
      }

      console.log("Szimuláció elindítva!");
    } catch (error) {
      console.error("Szimuláció folytatása sikertelen:", error);
    }
  };

  const end = async () => {
    try {
      const response = await fetch("/api/BackendSimulation/end", {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        credentials: "include",
        body: JSON.stringify({ simulationId }),
      });
      if (!response.ok) {
        throw new Error(`Hiba: ${response.status}`);
      }

      console.log("Szimuláció elindítva!");
    } catch (error) {
      console.error("Szimuláció folytatása sikertelen:", error);
    }
  };

  //lehív: row, column, simulationId
  useEffect(() => {
    if (!connection) return;
    const getData = async () => {
      console.log("GameMaster ID:", gameMasterId);
      try {
        connection
          .invoke("JoinViewer", gameMasterId)
          .then(() => console.log(`Csatlakoztál a ${gameMasterId} csoporthoz.`))
          .catch((err) => console.error("Hiba a csatlakozás során: ", err));
        connection.on("ReceiveSimulationId", (state) => {
          const stringData = JSON.stringify(state, null, 2);
          const parsedData = JSON.parse(stringData);
          console.log("SIMULATIONDATA", parsedData);
          setSimulationData(parsedData); // Beállítjuk az állapotot
          setSimulationId(parsedData.simulationId); // Beállítjuk a szimuláció ID-t
        });
      } catch (err) {
        console.error("Hiba a csatlakozás során: ", err);
      }
    };
    getData();
  }, [connection, gameMasterId, simulationId]);

  const joinSimulation = () => {
    console.log("joinSimulation");
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
      connection.on("ReceiveStatistic", (state) => {
        //const statistic = JSON.parse(state);
        const parsedStatistic = JSON.parse(JSON.stringify(state));
        setStatistic(parsedStatistic);
      });
      connection.on("ReceiveWinner", (state) => {
        const parsedWinner = JSON.parse(JSON.stringify(state));
        setWinner(parsedWinner);
        console.log("🏆 Winner:", parsedWinner);
      });
    } else {
      console.error("Nincs kapcsolat vagy nincs szimuláció ID!");
    }
  };

  useEffect(() => {
    if (winner) {
      alert(`Winner: ${winner}`);
      setWinner(null);
    }
  }, [winner]);

  useEffect(() => {
    if (statistic) {
      const result = statistic.Stone + statistic.Scissor + statistic.Paper;
      setSum(result);
    }
  }, [statistic]);

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

  const goHome = () => {
    navigate("/");
  };

  return (
    <div style={{ padding: "20px", fontFamily: "Arial, sans-serif" }}>
      <h1>Simulation Viewer</h1>
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
      {gameMasterId && (
        <div>
          <h2>Room name: {gameMasterId}</h2>
        </div>
      )}
      <div>
        <button onClick={pause} style={{ padding: "5px 10px" }}>
          Pause
        </button>
      </div>
      <div>
        <button onClick={resume} style={{ padding: "5px 10px" }}>
          Resume
        </button>
      </div>
      <div>
        <button onClick={end} style={{ padding: "5px 10px" }}>
          End
        </button>
      </div>
      <div>
        <button onClick={handleNewGame} style={{ padding: "5px 10px" }}>
          NewGame
        </button>
      </div>
      {statistic && (
        <div className="p-4 border border-gray-300 rounded-md">
          <h2 className="text-lg font-bold mb-2">📊 Statisztikai adatok</h2>
          <ul className="list-disc list-inside">
            <li className="text-gray-700">Stone: {statistic.Stone}</li>
            <li className="text-gray-700">Scissor: {statistic.Scissor}</li>
            <li className="text-gray-700">Paper: {statistic.Paper}</li>
            <li className="text-gray-700">Sum: {sum}</li>
          </ul>
        </div>
      )}
      <div
        ref={pixiContainerRef}
        style={{ border: "1px solid black", width: "1000px", height: "1000px" }}
      ></div>
      <div>
        <button onClick={goHome}>home</button>
      </div>
    </div>
  );
};

export default SimulationViewerMaster;
