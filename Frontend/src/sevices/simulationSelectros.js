

export const selectSimulationId = (state) => state.simulation.simulationId;
export const selectGameMasterId = (state) => state.simulation.gameMasterId;
export const selectRows = (state) => state.simulation.rows;
export const selectColumns = (state) => state.simulation.columns;
export const selectItemCount = (state) => state.simulation.itemCount;
export const selectIsRunning = (state) => state.simulation.isRunning;
export const selectIsPaused = (state) => state.simulation.isPaused;
export const selectStatistics = (state) => state.simulation.statistics;
export const selectWinner = (state) => state.simulation.winner;
export const selectUserName = (state) => state.simulation.userName;