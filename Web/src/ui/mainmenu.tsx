import React from "react";
import { Switch, Route, Redirect, RouteChildrenProps } from "react-router";
import { DeviceSelector } from "./emulation/deviceselector";
import { TT } from "./TranslatedText";
import { XboxEmulation, EmulationProps } from "./emulation/xbox";
import { Link } from "react-router-dom";
import { Controllers } from "./controllers/Controllers";
import AppBar from "@material-ui/core/AppBar";
import Toolbar from "@material-ui/core/Toolbar";
import IconButton from "@material-ui/core/IconButton";
import MenuIcon from "@material-ui/icons/Menu";
import Button from "@material-ui/core/Button";
import Typography from "@material-ui/core/Typography";
import withStyles, { Styles } from "@material-ui/core/styles/withStyles";
import { Inputs } from "./input/Inputs";
import { InputDetails, InputDetailsProps } from "./input/Details";
import { Ds4Emulation } from "./emulation/ds4";
import { Styled, StyleGenerator } from '../utils'

type ClassNames = 'menubarButton' | 'mainContent' | 'title';

const styles: StyleGenerator<ClassNames> = () => ({
    menubarButton: {
        color: 'white',
    },
    mainContent: {
        margin: '8px',
    },
    title: {
      flexGrow: 1,
    },
});

export interface MainMenuProps extends Styled<ClassNames> {

}

class MainMenuComponent extends React.Component<MainMenuProps> {
    render() {
        const { classes } = this.props;
        return <>
            <Switch>
                <Route path="/emulation" />
                <Route>
                    <AppBar position="static">
                        <Toolbar>
                            <IconButton edge="start" className={classes.menubarButton} color="inherit" aria-label="menu">
                                <MenuIcon />
                            </IconButton>
                            <Typography variant="h6" className={classes.title}>
                                XOutput
                            </Typography>
                            <Link to="/">
                                <Button className={classes.menubarButton}><TT text="ActiveControllers" /></Button>
                            </Link>
                            <Link to="/inputs">
                                <Button className={classes.menubarButton}><TT text="InputDevices" /></Button>
                            </Link>
                            <Link to="/devices">
                                <Button className={classes.menubarButton}><TT text="OnlineDevices" /></Button>
                            </Link>
                        </Toolbar>
                    </AppBar>
                </Route>
            </Switch>
            <div className={classes.mainContent}>
                <Switch>
                    <Route path="/" exact>
                        <Controllers></Controllers>
                    </Route>
                    <Route path="/devices">
                        <DeviceSelector></DeviceSelector>
                    </Route>
                    <Route path="/emulation/MicrosoftXbox360/:emulator" component={(props: RouteChildrenProps<{emulator: string}>) => (
                        <XboxEmulation deviceType='MicrosoftXbox360' emulator={props.match.params.emulator}></XboxEmulation>
                    )} />
                    <Route path="/emulation/SonyDualShock4/:emulator" component={(props: RouteChildrenProps<{emulator: string}>) => (
                        <Ds4Emulation deviceType='SonyDualShock4' emulator={props.match.params.emulator}></Ds4Emulation>
                    )} />
                    <Route path="/inputs" exact>
                        <Inputs></Inputs>
                    </Route>
                    <Route path="/inputs/:id" component={(props: RouteChildrenProps<{id: string}>) => (
                        <InputDetails id={props.match.params.id}></InputDetails>
                    )} />
                    <Route>
                        <Redirect to="/" />
                    </Route>
                </Switch>
            </div>
        </>;
    }
}

export const MainMenu = withStyles(styles)(MainMenuComponent);
