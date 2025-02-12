import { useState } from "react";
import "./App.css";
import SimulationViewer from "./pages/SimulationViewer";
import { Routes, Route } from "react-router-dom";
import SimulationHandling from "./pages/SimulationHandling";

function App() {
  return (
    <div>
      <Routes>
        <Route path="/" element={<SimulationViewer />} />
        <Route path="/initialize" element={<SimulationHandling />} />
        <Route
          path="/simulation/:simulationId"
          element={<SimulationViewer />}
        />
      </Routes>
    </div>
  );
}

export default App;
