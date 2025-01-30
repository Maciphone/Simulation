import React, { useRef, useEffect } from "react";

const SimulationAnimation = ({ gameState }) => {
    const canvasRef = useRef(null);

    useEffect(() => {
        const canvas = canvasRef.current;
        const ctx = canvas.getContext("2d");

        // Canvas törlése
        ctx.clearRect(0, 0, canvas.width, canvas.height);

        // Kirajzoljuk az összes itemet
        gameState.forEach(item => {
            ctx.beginPath();
            ctx.arc(item.X, item.Y, 10, 0, Math.PI * 2, true); // Egy kör rajzolása
            ctx.fillStyle = item.Type === 1 ? "red" : item.Type === 2 ? "blue" : "green"; // Szín a típus alapján
            ctx.fill();
            ctx.closePath();
        });
    }, [gameState]); // Újrarajzolás, amikor a gameState változik

    return (
        <canvas
            ref={canvasRef}
            width={1000}
            height={1000}
            style={{ border: "1px solid black" }}
        ></canvas>
    );
};

export default SimulationAnimation;