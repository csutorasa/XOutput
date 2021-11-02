import React, { ReactElement } from 'react';
import Typography from '@mui/material/Typography';
import withStyles from '@mui/styles/withStyles';
import red from '@mui/material/colors/red';
import yellow from '@mui/material/colors/yellow';
import CancelIcon from '@mui/icons-material/Cancel';
import WarningIcon from '@mui/icons-material/Warning';
import InfoIcon from '@mui/icons-material/Info';
import { Styled, StyleGenerator, Classes } from '../../utils';
import Translation from '../../translation/Translation';
import { Async } from '../components/Asnyc';
import { Card, Button } from '@mui/material';
import moment from 'moment';
import { notificationClient } from '@xoutput/client';
import { useNotificationsQuery } from '../../queries/useNotificationsQuery';

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

export type NotificationsProps = {};

type InternalNotificationsProps = Styled<ClassNames> & NotificationsProps;

const NotificationsComponent = ({ classes }: InternalNotificationsProps) => {
  const { data: notifications, isLoading, isSuccess, error, refetch } = useNotificationsQuery();

  function acknowledge(id: string): Promise<void> {
    return notificationClient.acknowledge(id).then(() => {
      return refetch().then(() => null);
    });
  }

  function createIcon(level: string, classes: Classes<ClassNames>): ReactElement {
    switch (level) {
      case 'Error':
        return <CancelIcon className={classes.cancelIcon} />;
      case 'Warning':
        return <WarningIcon className={classes.warningIcon} />;
      case 'Information':
        return <InfoIcon className={classes.infoIcon} />;
    }
  }

  function getTime(createdAt: string): string {
    const d = moment(createdAt);
    return `${d.format('LLL')} (${d.fromNow()})`;
  }

  return (
    <>
      <Typography variant="h3">{Translation.translate('Notifications')}</Typography>
      <Async isLoading={isLoading} isSuccess={isSuccess} error={error}>
        {() => (
          <>
            {notifications.map((n) => (
              <Card key={n.id} className={classes.card} style={{ display: 'flex' }}>
                <div>{createIcon(n.level, classes)}</div>
                <div>
                  <Typography variant="body1">{Translation.translate(n.key, n.parameters)}</Typography>
                  <Typography variant="body2">{getTime(n.createdAt)}</Typography>
                </div>
                <div className={classes.placeholder} />
                {n.acknowledged ? null : (
                  <Button variant="contained" color="primary" onClick={() => acknowledge(n.id)}>
                    {Translation.translate('Acknowledge')}
                  </Button>
                )}
              </Card>
            ))}
          </>
        )}
      </Async>
    </>
  );
};

export const Notifications = withStyles(styles)(NotificationsComponent);
