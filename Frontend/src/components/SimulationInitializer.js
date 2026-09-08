import React, { useState } from "react";
import { useNavigate } from "react-router-dom";
import { useDispatch } from "react-redux";
import { setSimulationId } from "../features/simulationIdSlice";

const SimulationInitializer = () => {
    const navigate = useNavigate();
    const dispatch = useDispatch();

