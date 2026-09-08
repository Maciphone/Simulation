import React from "react";
import { useNavigate } from "react-router-dom";

export default function Main() {
  const navigate = useNavigate();

  const goLogin = () => {
    console.log("login");
    navigate("/login");
  };

  const goRegister = () => {
    navigate("/register");
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
      <div>
        wanna register?
        <div onClick={goRegister}>oh yes</div>
      </div>
    </div>
  );
}
