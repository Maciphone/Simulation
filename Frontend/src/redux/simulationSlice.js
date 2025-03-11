
import { createSlice } from "@reduxjs/toolkit";

const initialState = {
    simulationId: localStorage.getItem("simulationId") || "",
}

const simulationSlice = createSlice({
    name: "simulation",
    initialState,
    reducers: {
        setSimulationIdRedux: (state, action) => {
            state.simulationId = action.payload;
            localStorage.setItem("simulationId", action.payload);
        },
        clearSimulationId: (state) => {
            state.simulationId = "";
            localStorage.removeItem("simulationId");
        },
    },
});

export const { setSimulationIdRedux, clearSimulationId } = simulationSlice.actions;
export default simulationSlice.reducer;