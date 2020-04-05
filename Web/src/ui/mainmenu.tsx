import React from "react";
import { Switch, Route, Redirect, RouteChildrenProps } from "react-router";
import { AdminPage } from "./admin/admin";
import { DeviceSelector } from "./deviceselector";
import { TranslatedText as TT } from "./translatedtext";
import { Emulation, EmulationProps } from "./emulation/emulation";
import { Link } from "react-router-dom";
import { Translation } from "../translation/translation";
import { ControllersPage } from "./controllers/controllers";

export class MainMenu extends React.Component<any, any, any> {

    render() {
        return <>
            <Switch>
                <Route path="/emulation" />
                <Route>
                <div>
                    <Link to="/">
                        <button><TT text="Home"/></button>
                    </Link>
                    <Link to="/devices">
                        <button><TT text="Devices"/></button>
                    </Link>
                    <Link to="/admin">
                        <button><TT text="Admin"/></button>
                    </Link>
                </div>
                </Route>
            </Switch>
            <Switch>
                <Route path="/" exact>
                    <ControllersPage></ControllersPage>
                </Route>
                <Route path="/devices">
                    <DeviceSelector></DeviceSelector>
                </Route>
                <Route path="/emulation/:deviceType/:emulator" component={(props: RouteChildrenProps<EmulationProps>) => (
                    <Emulation deviceType={props.match.params.deviceType} emulator={props.match.params.emulator}></Emulation>
                )}>
                </Route>
                <Route path="/admin">
                    <AdminPage></AdminPage>
                </Route>
                <Route>
                    <Redirect to="/"/>
                </Route>
            </Switch>
        </>;
    }
}
