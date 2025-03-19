import "tailwindcss";
import React, { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import { useDispatch } from "react-redux";
import { setSimulationIdRedux } from "../redux/simulationSlice";
import { createSimulation, getToken } from "../sevices/apiService";
import {
  roomExist,
  createSignalRConnection,
  // getSimulationId,
  sendSimulationId,
  getSimulationIdAsync,
} from "../sevices/hubService";
import * as signalR from "@microsoft/signalr";

export default function SimulationHandling() {
  const navigate = useNavigate();
  const dispatch = useDispatch();
  //initialise
  const [rows, setRows] = useState(0);
  const [columns, setColumns] = useState(0);
  const [itemCount, setItemCount] = useState(0);
  const [connection, setConnection] = useState(null);
  const [gameMasterId, setGameMasterId] = useState("");
  const [canSet, setCanSet] = useState(true);
  const [messages, setMessages] = useState([]);

  //get simulationId
  const [simulationId, setSimulationId] = useState("waiting for id");

  const [token, setToken] = useState("");

  useEffect(() => {
    getToken();
  }, []);

  useEffect(() => {
    const newConnection = createSignalRConnection();
    setConnection(newConnection);
  }, []);

  const handleInputChange = (event) => {
    const { name, value } = event.target;
    if (name === "rows") {
      setRows(Number(value));
    } else if (name === "columns") {
      setColumns(Number(value));
    } else if (name === "itemCount") {
      setItemCount(Number(value));
    }
  };

  const goToSimulation = () => {
    navigate(
      `/simulation/${simulationId}?gameMasterId=${gameMasterId}&rows=${rows}&columns=${columns}`
    );
  };

  const handleSubmit = async (event) => {
    event.preventDefault();
    console.log("submit pushed");
    const initialData = { rows, columns, itemCount };

    try {
      const simulationId = await createSimulation(initialData);
      console.log(simulationId);
      if (simulationId) {
        dispatch(setSimulationIdRedux(simulationId));
        setSimulationId(simulationId);
      } else {
        throw new Error("No simulationId");
      }
    } catch (error) {
      console.error("Error: no initialisation", error);
    }
  };

  const createHubPassSimulationId = async () => {
    if (connection && gameMasterId && simulationId) {
      try {
        let exist = await roomExist(connection, gameMasterId);
        if (!exist) {
          console.log("Nincs ilyen szoba!");
          const initialData = { rows, columns, simulationId, itemCount };
          console.log("initialData", initialData);
          await sendSimulationId(connection, gameMasterId, initialData);
          var properId = await getSimulationIdAsync(connection, gameMasterId);
          console.log("properId", properId);
        } else {
          console.log("Van ilyen szoba!");
        }
      } catch (error) {
        console.error("Hiba a csatlakozás során:", error);
      }
    }
  };

  const createHubPassSimulationIdNewRound = async () => {
    if (connection && gameMasterId && simulationId) {
      try {
        let exist = await roomExist(connection, gameMasterId);
        if (exist) {
          console.log("Nincs ilyen szoba!");
          const initialData = { rows, columns, simulationId };
          console.log("initialData", initialData);
          await sendSimulationId(connection, gameMasterId, initialData);
          var properId = await getSimulationIdAsync(connection, gameMasterId);
          console.log("properId", properId);
        } else {
          console.log("Van ilyen szoba!");
        }
      } catch (error) {
        console.error("Hiba a csatlakozás során:", error);
      }
    }
  };

  return (
    <div className="p-8">
      <h1 className="text-2xl font-bold mb-4">Simulation Handling</h1>
      <form onSubmit={handleSubmit} className="space-y-4">
        <div>
          <label className="block text-sm font-medium text-gray-700">
            Rows:
            <input
              type="number"
              name="rows"
              value={rows}
              onChange={handleInputChange}
              className="mt-1 block w-full border border-gray-300 rounded-md shadow-sm focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm"
            />
          </label>
        </div>
        <div>
          <label className="block text-sm font-medium text-gray-700">
            Columns:
            <input
              type="number"
              name="columns"
              value={columns}
              onChange={handleInputChange}
              className="mt-1 block w-full border border-gray-300 rounded-md shadow-sm focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm"
            />
          </label>
        </div>
        <div>
          <label className="block text-sm font-medium text-gray-700">
            Item Count:
            <input
              type="number"
              name="itemCount"
              value={itemCount}
              onChange={handleInputChange}
              className="mt-1 block w-full border border-gray-300 rounded-md shadow-sm focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm"
            />
          </label>
        </div>
        <button type="submit" className="test">
          Submit
        </button>
      </form>
      <div className="mt-8">
        <div>
          <h1 className="text-xl font-bold mb-2">
            Game Master Szoba Csatlakozás
          </h1>
          <input
            type="text"
            placeholder="Add meg a GameMasterID-t (hubname)"
            value={gameMasterId}
            onChange={(e) => setGameMasterId(e.target.value)}
            className="mt-1 block w-full border border-gray-300 rounded-md shadow-sm focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm"
          />
          <button
            onClick={createHubPassSimulationId}
            className="mt-2 px-4 py-2 bg-green-500 text-white rounded-md"
          >
            Csatlakozás
          </button>
        </div>
        <h2 className="text-lg font-bold mt-4">Simulation ID:</h2>
        {simulationId && <p className="text-gray-700">{simulationId}</p>}
      </div>
      <button
        onClick={goToSimulation}
        className="mt-4 px-4 py-2 bg-purple-500 text-white rounded-md"
      >
        go simulation
      </button>
      <button
        onClick={getToken}
        className="mt-4 px-4 py-2 bg-yellow-500 text-white rounded-md"
      >
        gettoken
      </button>
    </div>
  );
}
