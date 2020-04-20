import React from "react";
import { Switch, Route, Redirect, RouteChildrenProps } from "react-router";
import { DeviceSelector } from "./deviceselector";
import { TranslatedText as TT } from "./translatedtext";
import { Emulation, EmulationProps } from "./emulation/emulation";
import { Link } from "react-router-dom";
import { ControllersPage } from "./controllers/controllers";
import AppBar from "@material-ui/core/AppBar";
import Toolbar from "@material-ui/core/Toolbar";
import Button from "@material-ui/core/Button";
import withStyles, { Styles } from "@material-ui/core/styles/withStyles";
import { InputsPage } from "./input/inputs";
import { InputDetailsPage, InputDetailsProps } from "./input/details";

const styles: Styles<any, any, any> = () => ({
    menubarButton: {
        color: 'white',
    },
    mainContent: {
        margin: '8px',
    }
});

export class MainMenuComponent extends React.Component<any, any, any> {
    render() {
        const { classes } = this.props;
        return <>
            <Switch>
                <Route path="/emulation" />
                <Route>
                    <AppBar position="static">
                        <Toolbar>
                            <Link to="/">
                                <Button className={classes.menubarButton}><TT text="ActiveControllers" /></Button>
                            </Link>
                            <Link to="/devices">
                                <Button className={classes.menubarButton}><TT text="Devices" /></Button>
                            </Link>
                            <Link to="/inputs">
                                <Button className={classes.menubarButton}><TT text="InputDevices" /></Button>
                            </Link>
                        </Toolbar>
                    </AppBar>
                </Route>
            </Switch>
            <div className={classes.mainContent}>
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
                    <Route path="/inputs" exact>
                        <InputsPage></InputsPage>
                    </Route>
                    <Route path="/inputs/:id"component={(props: RouteChildrenProps<InputDetailsProps>) => (
                        <InputDetailsPage id={props.match.params.id}></InputDetailsPage>
                    )}>
                    </Route>
                    <Route>
                        <Redirect to="/" />
                    </Route>
                </Switch>
            </div>
        </>;
    }
}

export const MainMenu = withStyles(styles)(MainMenuComponent);
