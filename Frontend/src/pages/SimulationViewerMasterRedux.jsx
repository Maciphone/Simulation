import React, { useState, useEffect, useRef } from "react";
import * as signalR from "@microsoft/signalr";
import { useParams, useLocation, useNavigate } from "react-router-dom";
import * as PIXI from "pixi.js";
import { Application, Graphics } from "pixi.js";
import { use } from "react";
import { useDispatch, useSelector } from "react-redux";
import { createSimulation } from "../sevices/apiService";

//redux actions
import {
  setSimulationIdRedux,
  setGameMasterId,
  setSimulationParams,
  setIsRunning,
  setIsPaused,
  setStatistics,
  setWinner,
} from "../redux/simulationSlice";

import {
  roomExist,
  createSignalRConnection,
  // getSimulationId,
  sendSimulationId,
  getSimulationIdAsync,
} from "../sevices/hubService";

const SimulationViewerMasterRedux = () => {
  const [statistic, setStatistic] = useState(null);
  const location = useLocation();
  const searchParams = new URLSearchParams(location.search);
  const [connectionStatus, setConnectionStatus] = useState("Disconnected");

  //const [connection, setConnection] = useState(null);
  const pixiContainerRef = useRef(null);
  const pixiAppRef = useRef(null);
  const pixiFlag = useRef(false);

  const navigate = useNavigate();
  const dispatch = useDispatch();

  const connection = useRef(null);

  //redux reducer
  const {
    simulationId,
    gameMasterId,
    rows,
    columns,
    itemCount,
    isRunning,
    isPaused,
    statistics,
    winner,
  } = useSelector((state) => state.simulation);

  useEffect(() => {
    dispatch(setGameMasterId(searchParams.get("gameMasterId")));
  }, [dispatch, searchParams]);

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
  }, [dispatch]);

  //pixiJs initailisation
  useEffect(() => {
    async function init() {
      if (!rows || !columns) {
        return;
      }

      if (pixiFlag.current) return; // otherwise react appends it twice, in strict mode
      pixiFlag.current = true;
      const app = new Application();
      pixiAppRef.current = app;
      const rowsPixi = parseInt(rows) * 10;
      const columnsPixi = parseInt(columns) * 10;
      console.log("rows", rows);
      console.log("columns", columns);
      await app.init({
        background: "#AA0000",
        width: columnsPixi,
        height: rowsPixi,
      });
      pixiContainerRef.current.appendChild(app.canvas);
    }
    init();
    //<button onClick={getData}>getData</button>;
  }, [rows, columns]);

  useEffect(() => {
    const startConnection = async () => {
      if (!connection.current) {
        connection.current = new signalR.HubConnectionBuilder()
          .withUrl("/simulationHub", { withCredentials: true })
          .withAutomaticReconnect()
          .configureLogging(signalR.LogLevel.Information)
          .build();

        connection.current.onclose(() => {
          setConnectionStatus("Disconnected");
          console.log("🔌 SignalR kapcsolat leállítva");
        });

        try {
          await connection.current.start();
          console.log("✅ Kapcsolódás sikeres!");
          setConnectionStatus("Connected");
        } catch (err) {
          setConnectionStatus("Disconnected");
          console.error("❌ Kapcsolódási hiba: ", err);
        }
      }
    };

    startConnection();

    return () => {
      if (connection.current) {
        connection.current.stop();
        console.log("🔌 SignalR kapcsolat leállítva");
      }
    };
  }, []);

  //new game setup
  const handleNewGame = async (event) => {
    if (!connection.current) return;

    event.preventDefault();
    console.log("submit pushed");

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
        await joinSimulation();
        await fetchStartSimulation();
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
      //redux action
      dispatch(setIsRunning(true));
      dispatch(setIsPaused(false));

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

      dispatch(setIsPaused(true));
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

      dispatch(setIsPaused(false));
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

      dispatch(setIsRunning(false));
      dispatch(setIsPaused(false));

      console.log("Szimuláció elindítva!");
    } catch (error) {
      console.error("Szimuláció folytatása sikertelen:", error);
    }
  };

  //lehív: row, column, simulationId
  useEffect(() => {
    if (connectionStatus !== "Connected") return;
    const getData = async () => {
      console.log("GameMaster ID:", gameMasterId);
      try {
        connection.current
          .invoke("JoinViewer", gameMasterId)
          .then(() => console.log(`Csatlakoztál a ${gameMasterId} csoporthoz.`))
          .catch((err) => console.error("Hiba a csatlakozás során: ", err));
        connection.current.on("ReceiveSimulationId", (state) => {
          const stringData = JSON.stringify(state, null, 2);
          const parsedData = JSON.parse(stringData);
          console.log("SIMULATIONDATA", parsedData);
          dispatch(setSimulationIdRedux(parsedData.simulationId)); // Beállítjuk a szimuláció ID-t
        });
      } catch (err) {
        console.error("Hiba a csatlakozás során: ", err);
      }
    };
    getData();
  }, [gameMasterId, dispatch]);

  const joinSimulation = () => {
    console.log("joinSimulation");
    if (connectionStatus !== "Connected" || !simulationId) {
      console.error("Nincs kapcsolat vagy nincs szimuláció ID!");
      return;
    }
    connection.current
      .invoke("JoinSimulation", simulationId)
      .then(() => console.log(`Csatlakoztál a ${simulationId} csoporthoz.`))
      .catch((err) => console.error("Hiba a csatlakozás során: ", err));

    connection.current.on("JoinedSimulation", (simulationId) => {
      console.log(`Sikeresen csatlakoztál a ${simulationId} csoporthoz!`);
    });

    connection.current.on("ReceiveGameState", (state) => {
      const gameState = JSON.parse(state);
      updatePixiScene(gameState);
      // console.log("Új játékállapot érkezett:", gameState);
    });
    connection.current.on("ReceiveStatistic", (state) => {
      //const statistic = JSON.parse(state);
      const parsedStatistic = JSON.parse(JSON.stringify(state));
      setStatistic(parsedStatistic);
    });
    connection.current.on("ReceiveWinner", (state) => {
      const parsedWinner = JSON.parse(JSON.stringify(state));
      dispatch(setWinner(parsedWinner));
      console.log("🏆 Winner:", parsedWinner);
    });
    console.error("Nincs kapcsolat vagy nincs szimuláció ID!");
  };

  // useEffect(() => {
  //   if (winner) {
  //     alert(`Winner: ${winner}`);
  //     setWinner(null);
  //   }
  // }, [winner]);

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
            <li className="text-yellow-700">Stone: {statistic.Stone}</li>
            <li className="text-gray-700">Scissor: {statistic.Scissor}</li>
            <li className="text-red-700">Paper: {statistic.Paper}</li>
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

export default SimulationViewerMasterRedux;
