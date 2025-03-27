import React, { useEffect, useState } from "react";
import { useDispatch, useSelector } from "react-redux";
import { removeUserName, setUserNameRedux } from "../redux/simulationSlice";
import { use } from "react";

export default function Login() {
  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");
  const dispatch = useDispatch();
  const userNameLogedIn = useSelector((state) => state.simulation.userName);

  const handleSubmit = async (e) => {
    e.preventDefault();
    console.log("Username:", username);
    console.log("Password:", password);

    try {
      const response = await fetch("/api/auth/login", {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify({ username, password }),
      });
      if (!response.ok) {
        alert("wrong username or password");
        throw new Error(`Login failed: ${response.status}`);
      }
      dispatch(setUserNameRedux(username));
    } catch (error) {
      console.error("Error:", error);
    }
  };
  useEffect(() => {
    console.log("userNameLogedIn", userNameLogedIn);
  }, [userNameLogedIn]);

  const logOut = async () => {
    dispatch(removeUserName());
    try {
      await fetch("/api/auth/logout", {
        method: "POST",
        credentials: "include",
      });
    } catch (error) {
      console.error("Error:", error);
    }
  };

  return (
    <div className="flex items-center justify-center min-h-screen bg-gray-100">
      <div className="bg-white p-8 rounded shadow-md w-96">
        {userNameLogedIn && <h4>Logged in as: {userNameLogedIn}</h4>}
        <h1 className="text-2xl font-bold mb-6 text-center">Login</h1>
        <form onSubmit={handleSubmit}>
          <div className="mb-4">
            <label
              htmlFor="username"
              className="block text-gray-700 font-medium mb-2"
            >
              Username
            </label>
            <input
              type="text"
              id="username"
              value={username}
              onChange={(e) => setUsername(e.target.value)}
              className="w-full px-4 py-2 border rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
              placeholder="Enter your username"
              required
            />
          </div>
          <div className="mb-6">
            <label
              htmlFor="password"
              className="block text-gray-700 font-medium mb-2"
            >
              Password
            </label>
            <input
              type="password"
              id="password"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              className="w-full px-4 py-2 border rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
              placeholder="Enter your password"
              required
            />
          </div>
          <button
            type="submit"
            className="w-full bg-blue-500 text-white py-2 px-4 rounded-lg hover:bg-blue-600 transition duration-300"
          >
            Login
          </button>
        </form>
        <button
          onClick={logOut}
          className="w-full bg-blue-500 text-white py-2 px-4 rounded-lg hover:bg-blue-600 transition duration-300"
        >
          Logout
        </button>
      </div>
    </div>
  );
}
