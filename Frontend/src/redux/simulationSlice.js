
import { createSlice } from "@reduxjs/toolkit";

const initialState = {
    simulationId: null,
    gameMasterId: null,
    rows: 0,
    columns: 0,
    itemCount: 0,
    connection: null,
    isRunning: false,  // Szimuláció fut-e
    isPaused: false,   // Szimuláció szüneteltetve van-e
    statistics: {},    // Játék statisztikák
    winner: null,      // Nyertes neve
};

const simulationSlice = createSlice({
    name: "simulation",
    initialState,
    reducers: {
        setSimulationIdRedux: (state, action) => {
            state.simulationId = action.payload;
        },
        setGameMasterId: (state, action) => {
            state.gameMasterId = action.payload;
        },
        setSimulationParams: (state, action) => {
            state.rows = action.payload.rows;
            state.columns = action.payload.columns;
            state.itemCount = action.payload.itemCount;
        },
        setConnection: (state, action) => {
            state.connection = action.payload;
        },
        setIsRunning: (state, action) => {
            state.isRunning = action.payload;
        },
        setIsPaused: (state, action) => {
            state.isPaused = action.payload;
        },
        setStatistics: (state, action) => {
            state.statistics = action.payload;
        },
        setWinner: (state, action) => {
            state.winner = action.payload;
        },
    },
});

export const {
    setSimulationIdRedux, setGameMasterId, setSimulationParams, setConnection,
    setIsRunning, setIsPaused, setStatistics, setWinner
} = simulationSlice.actions;

export default simulationSlice.reducer;