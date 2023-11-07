import React, { ReactElement } from 'react';
import Typography from '@mui/material/Typography';
import red from '@mui/material/colors/red';
import yellow from '@mui/material/colors/yellow';
import CancelIcon from '@mui/icons-material/Cancel';
import WarningIcon from '@mui/icons-material/Warning';
import InfoIcon from '@mui/icons-material/Info';
import Translation from '../../translation/Translation';
import { Async } from '../components/Asnyc';
import Card from '@mui/material/Card';
import Button from '@mui/material/Button';
import moment from 'moment';
import { notificationClient } from '@xoutput/client';
import { useNotificationsQuery } from '../../queries/useNotificationsQuery';
import { PlaceHolder } from '../components/Placeholder';

export type NotificationsProps = {};

export const Notifications = ({ }: NotificationsProps) => {
  const { data: notifications, isLoading, isSuccess, error, refetch } = useNotificationsQuery();

  function acknowledge(id: string): Promise<void> {
    return notificationClient.acknowledge(id).then(() => {
      return refetch().then(() => null);
    });
  }

  function createIcon(level: string): ReactElement {
    switch (level) {
      case 'Error':
        return <CancelIcon style={{ padding: '0 10px 0 0', color: red[500]}} />;
      case 'Warning':
        return <WarningIcon style={{ padding: '0 10px 0 0', color: yellow[500]}} />;
      case 'Information':
        return <InfoIcon style={{ padding: '0 10px 0 0' }} />;
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
              <Card key={n.id} style={{ display: 'flex', padding: '10px', alignItems: 'center' }}>
                <div>{createIcon(n.level)}</div>
                <div>
                  <Typography variant="body1">{Translation.translate(n.key, n.parameters)}</Typography>
                  <Typography variant="body2">{getTime(n.createdAt)}</Typography>
                </div>
                <PlaceHolder />
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
