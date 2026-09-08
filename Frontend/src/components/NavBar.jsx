import React from "react";
import { Link } from "react-router-dom";
import "./NavBar.css";

const NavBar = () => {
  return (
    <nav>
      <ul>
        <li>
          <Link to="/">Home</Link>
        </li>
        <li>
          <Link to="/simulation">Simulation Handling</Link>
        </li>
        <li>
          <Link to="/simulationView">Simulation Viewer</Link>
        </li>
        <li>
          <Link to="/pixi">Pixi Test</Link>
        </li>
        <li>
          <Link to="/slave">Simulation Viewer Slave</Link>
        </li>
        <li>
          <Link to="/simulation/:simulationId">Simulation Viewer Master</Link>
        </li>
      </ul>
    </nav>
  );
};

export default NavBar;
