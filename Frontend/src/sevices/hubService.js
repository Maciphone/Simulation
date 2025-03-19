
import * as signalR from "@microsoft/signalr";

let connection = null;

//  SignalR kapcsolat létrehozása
const createSignalRConnection = () => {
    if (!connection) {
        connection = new signalR.HubConnectionBuilder()
            .withUrl("/simulationHub", { withCredentials: true })
            .withAutomaticReconnect()
            .configureLogging(signalR.LogLevel.Information)
            .build();

        connection
            .start()
            .then(() => console.log("🔗 SignalR kapcsolat létrejött!"))
            .catch((err) => console.error("Hiba a SignalR kapcsolatnál:", err));

    }
    return connection;
};

const roomExist = (connection, gameMasterId) => {

    if (!connection || !gameMasterId) {
        console.error(" Nincs kapcsolat vagy GameMaster ID!");
        return;
    }
    try {
        let result = connection.invoke("RoomExists", gameMasterId)
        return result;
    }
    catch (err) {
        console.error("Hiba a csatlakozás során: ", err);


    };
}

// 🎮 GameMaster szoba csatlakozás
const sendSimulationId = async (connection, gameMasterId, initialData) => {

    if (!connection || !gameMasterId) {
        console.log("Nincs kapcsolat vagy nincs GameMaster ID!");
        return;
    }

    try {
        connection
            .invoke("JoinGameMaster", gameMasterId, initialData)
            .then(() => console.log(` Csatlakoztál a ${gameMasterId} csoporthoz.`))
            .catch((err) => console.error("Hiba a csatlakozás során: ", err));

        connection.on("ReceiveSimulationId", (state) => {
            const stringData = JSON.stringify(state, null, 2);
            const parsedData = JSON.parse(stringData);
            console.log("🎲 SzimulációDataPARES:", parsedData);

            console.log(`🎲 Szimuláció ID: ${state.simulationId}`);
            // console.log(`🎲 SzimulációData: ${JSON.parse(state)}`);
            console.log("🎲 SzimulációData:", JSON.stringify(state, null, 2));
        }
        );

    } catch (err) {
        console.error("❌ Hiba a csatlakozás során:", err);

    }
}
// const sendSimulationId = async (connection, gameMasterId, initialData) => {
//     if (!connection || !gameMasterId) {
//         console.log("❌ Nincs kapcsolat vagy nincs GameMaster ID!");
//         return;
//     }

//     try {
//         1️⃣ Először feliratkozunk az eseményre
//         connection.off("ReceiveSimulationId"); // Leiratkozás az esetleges régi figyelőkről
//         connection.on("ReceiveSimulationId", (state) => {
//             try {
//                 console.log("📩 Kapott Simulation ID állapot:", state);
//                 const parsedData = JSON.parse(JSON.stringify(state));
//                 console.log("🎲 SzimulációData PARES:", parsedData);
//                 console.log("🎲 Szimuláció ID:", parsedData.simulationId);
//             } catch (error) {
//                 console.error("❌ JSON parse hiba a Simulation ID fogadásakor:", error);
//             }
//         });

//         2️⃣ Most küldjük el az adatokat
//         console.log("📡 Simulation ID küldése...");
//         await connection.invoke("JoinGameMaster", gameMasterId, initialData);
//         console.log(`✅ Sikeresen csatlakoztál a ${gameMasterId} csoporthoz!`);

//     } catch (err) {
//         console.error("❌ Hiba a Simulation ID küldése során:", err);
//     }
// };


const getSimulationIdAsync = async (connection, gameMasterId) => {
    if (!connection || !gameMasterId) {
        console.error("❌ Nincs kapcsolat vagy GameMaster ID!");
        return null;
    }
    try {
        var simulationId = connection.invoke("GetSimulationIdForGameMaster", gameMasterId)
        return simulationId
    } catch (error) {
        console.error("Hiba a szimuláció ID lekérésekor:", error);
        return null;
    }
}
// try {
//     connection.invoke("JoinGameMaster", gameMasterId, null)
//         .then(() => console.log(`Csatlakoztál a ${gameMasterId} csoporthoz.`))
//     const simulationId = await
//         connection.on("ReceiveSimulationId", (state) => {
//             console.log(`🎲 Szimuláció ID: ${state}`);
//         });
//     return simulationId;
// } catch (err) {
//     console.error("Hiba a szimuláció ID lekérésekor:", err);
//     return null;
// }
// };
const leaveGameMaster = async (connection, gameMasterId) => {
    if (!connection || !gameMasterId) {
        console.error("❌ Nincs kapcsolat vagy GameMaster ID!");
        return null;
    }

    try {
        connection.invoke("LeaveGameMaster", gameMasterId)
            .then(() => console.log(`🚪 Elhagytad a ${gameMasterId} csoporthoz.`))
        return true;
    } catch (err) {
        console.error("❌ Hiba a kilépés során:", err);
        return false;
    }
}


export { createSignalRConnection, roomExist, sendSimulationId, getSimulationIdAsync, leaveGameMaster };


// const getSimulationId = async (connection, gameMasterId) => {
//     if (!connection || !gameMasterId) {
//         console.error("❌ Nincs kapcsolat vagy GameMaster ID!");
//         return null;
//     }
//     return new Promise((resolve, reject) => {
//         try {
//             connection.invoke("JoinGameMaster", gameMasterId, null)
//                 .then(() => console.log(`✅ Csatlakoztál a ${gameMasterId} csoporthoz.`));

//             connection.on("ReceiveSimulationId", (state) => {
//                 console.log(` Szimuláció ID megkapva: ${state}`);
//                 resolve(state); // Most várjuk meg, és visszaadjuk
//             });

//         } catch (err) {
//             console.error(" Hiba a szimuláció ID lekérésekor:", err);
//             reject(err);
//         }
//     });
// }