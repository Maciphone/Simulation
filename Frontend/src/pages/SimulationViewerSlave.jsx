import React, { useState, useEffect, useRef } from "react";
import * as signalR from "@microsoft/signalr";
import { useParams, useLocation, useNavigate } from "react-router-dom";
import * as PIXI from "pixi.js";
import { Application, Graphics } from "pixi.js";
import { use } from "react";
import { useDispatch, useSelector } from "react-redux";
import { selectGameMasterId } from "../sevices/simulationSelectros";
import {
  setGameMasterId,
  setSimulationIdRedux,
  setSimulationParams,
} from "../redux/simulationSlice";

const SimulationViewerSlave = () => {
  const dispatch = useDispatch();

  //query paraméterek lekérése
  //const location = useLocation();
  //const searchParams = new URLSearchParams(location.search);
  //const [gameMasterId, setGameMasterId] = useState("");

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

  //const [simulationData, setSimulationData] = useState(null);

  const [connection, setConnection] = useState(null);
  const pixiContainerRef = useRef(null);
  const pixiAppRef = useRef(null);
  const pixiFlag = useRef(false);

  const navigate = useNavigate();

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
      if (!rows || !columns) {
        return;
      }

      if (pixiFlag.current) return; // otherwise react appends it twice, in strict mode
      pixiFlag.current = true;
      const app = new Application();
      pixiAppRef.current = app;
      const rowsPixi = parseInt(rows) * 10;
      const columnsPixi = parseInt(columns) * 10;

      await app.init({
        background: "#AA0000",
        width: columnsPixi,
        height: rowsPixi,
      });
      pixiContainerRef.current.appendChild(app.canvas);
    }
    init();
    //<button onClick={getData}>getData</button>;
  }, [columns, rows]);

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

  const receiveGameParameters = async () => {
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
          console.log("simulationDate", parsedData);
          dispatch(setSimulationParams(parsedData));
          dispatch(setSimulationIdRedux(parsedData.simulationId));
        });
      } catch (err) {
        console.error("Hiba a csatlakozás során: ", err);
      }
    };
    getData();
  };

  const joinSimulation = () => {
    console.log("Joining simulation...", simulationId);
    if (connection) {
      console.log("Joining simulation...");
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

  const goHome = () => {
    navigate("/");
  };
  const handleSubmit = async (event) => {
    event.preventDefault();
    receiveGameParameters();
  };

  const handleInputChange = (event) => {
    const result = event.target.value;
    dispatch(setGameMasterId(result));
    console.log(result);
  };

  return (
    <div style={{ padding: "20px", fontFamily: "Arial, sans-serif" }}>
      <h1>Simulation Viewer</h1>
      <div>
        <form onSubmit={handleSubmit} className="space-y-4">
          <div>
            <label className="block text-sm font-medium text-gray-700">
              Room name:
              <input
                type="text"
                name="gameMasterId"
                value={gameMasterId}
                onChange={handleInputChange}
                className="mt-1 block w-full border border-gray-300 rounded-md shadow-sm focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm"
              />
            </label>
          </div>
          <button type="submit" style={{ padding: "5px 10px" }}>
            I am press enter
          </button>
        </form>
      </div>
      <div>
        <button onClick={joinSimulation} style={{ padding: "5px 10px" }}>
          Join Simulation
        </button>
      </div>
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

export default SimulationViewerSlave;
