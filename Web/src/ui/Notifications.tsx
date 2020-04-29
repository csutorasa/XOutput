import React from "react";
import { Switch, Route, Redirect, RouteChildrenProps } from "react-router";
import { DeviceSelector } from "./emulation/DeviceSelector";
import { TT } from "./TranslatedText";
import { Link } from "react-router-dom";
import IconButton from "@material-ui/core/IconButton";
import MenuIcon from "@material-ui/icons/Menu";
import Typography from "@material-ui/core/Typography";
import withStyles from "@material-ui/core/styles/withStyles";
import { Styled, StyleGenerator } from '../utils'
import Translation from "../translation/Translation";

type ClassNames = never;

const styles: StyleGenerator<ClassNames> = () => ({
});

export interface NotificationsProps extends Styled<ClassNames> {

}

export interface NotificationsState {
    
}

class NotificationsComponent extends React.Component<NotificationsProps, NotificationsState> {

    state: NotificationsState = {
        
    }

    render() {
        const { classes } = this.props;
        return <>
            Notifications
        </>;
    }
}

export const Notifications = withStyles(styles)(NotificationsComponent);
