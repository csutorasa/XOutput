import React from "react";
import { HashRouter } from "react-router-dom";
import { MainMenu } from "./mainmenu";

export const RootElement = <HashRouter>
    <MainMenu></MainMenu>
</HashRouter>;
export default RootElement;
