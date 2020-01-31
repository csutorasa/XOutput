import React from "react";
import { Switch, Route, Redirect, RouteChildrenProps } from "react-router";
import { AdminPage } from "./admin/admin";
import { DeviceSelector } from "./deviceselector";
import { Emulation, EmulationProps } from "./emulation/emulation";
import { Link } from "react-router-dom";

export class MainMenu extends React.Component<any, any, any> {

    render() {
        return <>
            <div>
                <Link to="/">
                    <button>Home</button>
                </Link>
                <Link to="/devices">
                    <button>Devices</button>
                </Link>
                <Link to="/admin">
                    <button>Admin</button>
                </Link>
            </div>
            <Switch>
                <Route path="/" exact>
                    EMPTY
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
