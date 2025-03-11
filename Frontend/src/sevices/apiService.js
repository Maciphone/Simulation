const createSimulation = async (initialData) => {
  try {
    const response = await fetch("/api/BackendSimulation/initialize", {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      credentials: "include",
      body: JSON.stringify(initialData),
    });
    if (!response.ok) {
      throw new Error(`Hiba: ${response.status}`);
    }
    const data = await response.json();
    return data.simulationId;
  } catch (error) {
    console.error("Error: no initialisation", error);
    throw error;
  }
};

const getToken = async () => {
  try {
    const response = await fetch("/api/auth/guest", {
      // Ha proxy van beállítva, elég csak "/api"
      method: "POST",
      // headers: {
      //   "Content-Type": "application/json",
      // },
      credentials: "include",
    });

    if (!response.ok) {
      throw new Error(`Hiba: ${response.status}`);
    }

    // const data = await response.json();
    // setToken(data.token);
    // console.log("Vendég token:", data.token);
    console.log("Vendég token sikeresen lekérve! cookieba mentve");
  } catch (error) {
    console.error("Token lekérés sikertelen:", error);
  }
};

export { createSimulation, getToken };

