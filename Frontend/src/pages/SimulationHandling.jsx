import React, { useState } from "react";
import { useNavigate } from "react-router-dom";

export default function SimulationHandling() {
  const navigate = useNavigate();
  //initialise
  const [rows, setRows] = useState(0);
  const [columns, setColumns] = useState(0);
  const [itemCount, setItemCount] = useState(0);

  //get simulationId
  const [simulationId, setSimulationId] = useState("waiting for id");

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
    // const fetchStartSimulation = async () => {
    //   try {
    //     const response = await fetch("/api/BackendSimulation/play", {
    //       method: "POST",
    //       headers: {
    //         "Content-Type": "application/json",
    //       },
    //       body: JSON.stringify({ simulationId }),
    //     });

    //     if (!response.ok) {
    //       throw new Error(`Hiba: ${response.status}`);
    //     }

    //     console.log("Szimuláció elindítva!");
    //   } catch (error) {
    //     console.error("Szimuláció indítás sikertelen:", error);
    //   }
    // };
    // fetchStartSimulation();
    navigate(`/simulation/${simulationId}`);
  };

  const handleSubmit = (event) => {
    event.preventDefault();
    const initialData = { rows, columns, itemCount };

    // Send initialData to the backend server
    fetch("api/BackendSimulation/initialize", {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      credentials: "include",
      body: JSON.stringify(initialData),
    })
      .then((response) => response.json())
      .then((data) => {
        setSimulationId(data.simulationId);
      })
      .catch((error) => {
        console.error("Error:", error);
      });
  };

  return (
    <div>
      <h1>Simulation Handling</h1>
      <form onSubmit={handleSubmit}>
        <div>
          <label>
            Rows:
            <input
              type="number"
              name="rows"
              value={rows}
              onChange={handleInputChange}
            />
          </label>
        </div>
        <div>
          <label>
            Columns:
            <input
              type="number"
              name="columns"
              value={columns}
              onChange={handleInputChange}
            />
          </label>
        </div>
        <div>
          <label>
            Item Count:
            <input
              type="number"
              name="itemCount"
              value={itemCount}
              onChange={handleInputChange}
            />
          </label>
        </div>
        <button type="submit">Submit</button>
      </form>
      <div>
        <h2>Simulation ID:</h2>
        {simulationId && <p>{simulationId}</p>}
      </div>
      <button onClick={goToSimulation}></button>
    </div>
  );
}
