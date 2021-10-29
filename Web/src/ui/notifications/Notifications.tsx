import React from 'react';
import Typography from '@material-ui/core/Typography';
import withStyles from '@material-ui/core/styles/withStyles';
import red from '@material-ui/core/colors/red';
import yellow from '@material-ui/core/colors/yellow';
import CancelIcon from '@material-ui/icons/Cancel';
import WarningIcon from '@material-ui/icons/Warning';
import InfoIcon from '@material-ui/icons/Info';
import { Styled, StyleGenerator, Classes } from '../../utils';
import Translation from '../../translation/Translation';
import Async from '../components/Asnyc';
import { Card, Button } from '@material-ui/core';
import moment from 'moment';
import { Notification } from '../../api';
import { notificationClient } from '../../client';

type ClassNames = 'card' | 'placeholder' | 'cancelIcon' | 'warningIcon' | 'infoIcon';

const styles: StyleGenerator<ClassNames> = () => ({
  card: {
    padding: '10px',
    alignItems: 'center',
  },
  placeholder: {
    flexGrow: 1,
  },
  cancelIcon: {
    padding: '0 10px 0 0',
    color: red[500],
  },
  warningIcon: {
    padding: '0 10px 0 0',
    color: yellow[500],
  },
  infoIcon: {
    padding: '0 10px 0 0',
  },
});

export interface NotificationsProps extends Styled<ClassNames> {}

export interface NotificationsState {
  notifications: Notification[];
}

class NotificationsComponent extends React.Component<NotificationsProps, NotificationsState> {
  state: NotificationsState = {
    notifications: [],
  };

  private loading: Promise<Notification[]>;

  componentDidMount() {
    this.loading = this.refreshDevices();
    this.forceUpdate();
  }

  refreshDevices(): Promise<Notification[]> {
    return notificationClient.getNotifications().then((notifications) => {
      this.setState({
        notifications,
      });
      return notifications;
    });
  }

  acknowledge(id: string) {
    return notificationClient.acknowledge(id).then(() => {
      this.loading = this.refreshDevices();
      this.forceUpdate();
    });
  }

  createIcon(level: string, classes: Classes<ClassNames>) {
    switch (level) {
      case 'Error':
        return <CancelIcon className={classes.cancelIcon} />;
      case 'Warning':
        return <WarningIcon className={classes.warningIcon} />;
      case 'Information':
        return <InfoIcon className={classes.infoIcon} />;
    }
  }

  getTime(createdAt: string): string {
    const d = moment(createdAt);
    return `${d.format('LLL')} (${d.fromNow()})`;
  }

  render() {
    const { classes } = this.props;
    return (
      <>
        <Typography variant="h3">{Translation.translate('Notifications')}</Typography>
        <Async task={this.loading}>
          {(notifications: Notification[]) =>
            notifications.map((n) => (
              <Card key={n.id} className={classes.card} style={{ display: 'flex' }}>
                <div>{this.createIcon(n.level, classes)}</div>
                <div>
                  <Typography variant="body1">{Translation.translate(n.key, n.parameters)}</Typography>
                  <Typography variant="body2">{this.getTime(n.createdAt)}</Typography>
                </div>
                <div className={classes.placeholder} />
                {n.acknowledged ? null : (
                  <Button variant="contained" color="primary" onClick={() => this.acknowledge(n.id)}>
                    {Translation.translate('Acknowledge')}
                  </Button>
                )}
              </Card>
            ))
          }
        </Async>
      </>
    );
  }
}

export const Notifications = withStyles(styles)(NotificationsComponent);
