import { useState } from "react";
import "./App.css";
import SimulationViewer from "./pages/SimulationViewer";
import { Routes, Route } from "react-router-dom";
import { Provider } from "react-redux";
import { store } from "./redux/store";
import SimulationHandling from "./pages/SimulationHandling";
import SimulationViewer_copy from "./pages/SimulationViewer_copy";
import PixiTest from "./pages/pixiTest";

function App() {
  return (
    <Provider store={store}>
      <div>
        <Routes>
          <Route path="/" element={<SimulationViewer />} />
          <Route path="/pixi" element={<PixiTest />} />
          <Route path="/simulation" element={<SimulationHandling />} />
          <Route path="/initialize" element={<SimulationHandling />} />
          <Route
            path="/simulation/:simulationId"
            element={<SimulationViewer_copy />}
          />
        </Routes>
      </div>
    </Provider>
  );
}

export default App;
