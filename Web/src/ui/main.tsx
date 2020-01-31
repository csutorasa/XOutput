import React from "react";
import { HashRouter, Route, Switch, Redirect } from "react-router-dom";
import { MainMenu } from "./mainmenu";

export const RootElement = <HashRouter>
    <MainMenu></MainMenu>
</HashRouter>;
