import { Application, Graphics } from "pixi.js";
import React, { useEffect, useRef, useState } from "react";

export default function PixiTest() {
  const pixiContainerRef = useRef(null);
  const [dots, setDots] = useState([]);
  const pixiAppRef = useRef();
  const pixiFlag = useRef(false);

  useEffect(() => {
    async function init() {
      if (pixiFlag.current) return; // otherwise react appends it twice, in strict mode
      pixiFlag.current = true;
      const app = new Application();
      pixiAppRef.current = app;
      await app.init({ background: "#AA0000", width: 1000, height: 1000 });
      pixiContainerRef.current.appendChild(app.canvas);
    }

    init();
  }, []);

  const drawStg = () => {
    const x = Math.random() * 500;
    const y = Math.random() * 500;
    const pixiGraphics = new Graphics();
    pixiGraphics.circle(x, y, 5).fill(0xffffff);
    pixiAppRef.current.stage.addChild(pixiGraphics);
    setDots((dots) => [...dots, pixiGraphics]);
  };

  const clearDots = () => {
    for (const dot of dots) {
      dot.clear();
    }
    setDots([]);
  };

  return (
    <div>
      pixiTest
      <div
        ref={pixiContainerRef}
        style={{ border: "1px solid black", width: "1000px", height: "1000px" }}
      ></div>
      <button onClick={drawStg}>draw</button>
      <button onClick={clearDots}>clear</button>
    </div>
  );
}
