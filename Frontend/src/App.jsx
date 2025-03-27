import { useState } from "react";
import "./App.css";
import SimulationViewer from "./pages/SimulationViewer";
import { Routes, Route } from "react-router-dom";
import { Provider } from "react-redux";
import { store } from "./redux/store";
import SimulationHandling from "./pages/SimulationHandling";
import PixiTest from "./pages/pixiTest";
import SimulationViewerMaster from "./pages/SimulationViewerMaster";
import SimulationViewerSlave from "./pages/SimulationViewerSlave";
import Main from "./pages/Index";
import NavBar from "./components/NavBar";
import SimulationViewerMasterRedux from "./pages/SimulationViewerMasterRedux";
import Login from "./pages/Login";
import Register from "./pages/Register";

function App() {
  return (
    <Provider store={store}>
      <div>
        <NavBar />
        <Routes>
          <Route path="/simulationView" element={<SimulationViewer />} />
          <Route path="/pixi" element={<PixiTest />} />
          <Route path="/simulation" element={<SimulationHandling />} />
          <Route path="/" element={<Main />} />
          <Route path="/slave" element={<SimulationViewerSlave />} />
          <Route
            path="/simulation/:simulationId"
            element={<SimulationViewerMaster />}
          />
          <Route path="/login" element={<Login />} />
          <Route path="/register" element={<Register />} />
        </Routes>
      </div>
    </Provider>
  );
}

export default App;
