import React from "react";
import Typography from "@material-ui/core/Typography";
import withStyles from "@material-ui/core/styles/withStyles";
import { Styled, StyleGenerator } from '../../utils'
import Translation from "../../translation/Translation";
import Async, { AsyncErrorHandler } from "../components/Asnyc";
import { rest, Notification } from "../../communication/rest";
import { Card } from "@material-ui/core";

type ClassNames = never;

const styles: StyleGenerator<ClassNames> = () => ({
});

export interface NotificationsProps extends Styled<ClassNames> {

}

export interface NotificationsState {
    notifications: Notification[];
}

class NotificationsComponent extends React.Component<NotificationsProps, NotificationsState> {

    state: NotificationsState = {
        notifications: [],
    }

    private loading: Promise<any>;

    componentDidMount() {
      this.loading = this.refreshDevices();
    }
  
    refreshDevices() {
      return rest.getNotifiations().then(notifications => {
        this.setState({
            notifications: notifications,
        })
        return notifications;
      }, AsyncErrorHandler(this))
    }

    render() {
        const { classes } = this.props;
        return <>
            <Typography variant='h3'>{Translation.translate("Notifications")}</Typography>
            <Async task={this.loading}>
                {
                    this.state.notifications.map(n => <Card key={0}>
                        <Typography variant='body1'>{n.key}</Typography>
                    </Card>)
                }
            </Async>
        </>;
    }
}

export const Notifications = withStyles(styles)(NotificationsComponent);
