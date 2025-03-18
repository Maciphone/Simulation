import React from "react";
import { useNavigate } from "react-router-dom";

export default function Main() {
  const navigate = useNavigate();

  const goLogin = () => {
    console.log("login");
    navigate("/login");
  };
  return (
    <div>
      <p>Main</p>
      <div>
        lorem ipsum novita es spiritus intus bericus et vin oet very tas
      </div>
      <div>
        wanna login?
        <div onClick={goLogin}>oh yes</div>
      </div>
    </div>
  );
}
