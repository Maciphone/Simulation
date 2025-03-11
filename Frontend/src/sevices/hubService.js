
import * as signalR from "@microsoft/signalr";

//  SignalR kapcsolat létrehozása
const createSignalRConnection = () => {
    const connection = new signalR.HubConnectionBuilder()
        .withUrl("/simulationHub", { withCredentials: true })
        .withAutomaticReconnect()
        .configureLogging(signalR.LogLevel.Information)
        .build();

    connection
        .start()
        .then(() => console.log("🔗 SignalR kapcsolat létrejött!"))
        .catch((err) => console.error("Hiba a SignalR kapcsolatnál:", err));

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
const sendSimulationId = async (connection, gameMasterId, simulationId) => {

    if (!connection || !gameMasterId) {
        console.log("Nincs kapcsolat vagy nincs GameMaster ID!");
        return;
    }

    try {
        connection
            .invoke("JoinGameMaster", gameMasterId, simulationId)
            .then(() => console.log(` Csatlakoztál a ${gameMasterId} csoporthoz.`))
            .catch((err) => console.error("Hiba a csatlakozás során: ", err));

    } catch (err) {
        console.error("❌ Hiba a csatlakozás során:", err);

    }
}


const getSimulationId = async (connection, gameMasterId) => {
    if (!connection || !gameMasterId) {
        console.error("❌ Nincs kapcsolat vagy GameMaster ID!");
        return null;
    }
    return new Promise((resolve, reject) => {
        try {
            connection.invoke("JoinGameMaster", gameMasterId, null)
                .then(() => console.log(`✅ Csatlakoztál a ${gameMasterId} csoporthoz.`));

            connection.on("ReceiveSimulationId", (state) => {
                console.log(` Szimuláció ID megkapva: ${state}`);
                resolve(state); // Most várjuk meg, és visszaadjuk
            });

        } catch (err) {
            console.error(" Hiba a szimuláció ID lekérésekor:", err);
            reject(err);
        }
    });
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


export { createSignalRConnection, roomExist, getSimulationId, sendSimulationId, leaveGameMaster };
