
import { createSlice } from "@reduxjs/toolkit";

const initialState = {
    simulationId: null,
    gameMasterId: null,
    rows: 0,
    columns: 0,
    itemCount: 0,
    isRunning: false,
    isPaused: false,
    statistics: {},
    winner: null,
    userName: null
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
            const { rows, columns, itemCount, simulationId } = action.payload;

            if (rows !== undefined) state.rows = rows;
            if (columns !== undefined) state.columns = columns;
            if (itemCount !== undefined) state.itemCount = itemCount;
            if (simulationId !== undefined) state.simulationId = simulationId;
        },
        // setSimulationParams: (state, action) => {
        //     state.rows = action.payload.rows;
        //     state.columns = action.payload.columns;
        //     state.itemCount = action.payload.itemCount;
        //     state.simulationId = action.payload.simulationId;
        // },
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
        setUserNameRedux: (state, action) => {
            state.userName = action.payload;
        },
        removeUserName: (state) => {
            state.userName = null;
        }
    },
});

export const {
    setSimulationIdRedux, setGameMasterId, setSimulationParams,
    setIsRunning, setIsPaused, setStatistics, setWinner, setUserNameRedux, removeUserName
} = simulationSlice.actions;

export default simulationSlice.reducer;