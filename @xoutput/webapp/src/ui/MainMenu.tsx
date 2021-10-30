import React, { useEffect, useState } from 'react';
import { withRouter } from 'react-router';
import { Link } from 'react-router-dom';
import AppBar from '@mui/material/AppBar';
import Toolbar from '@mui/material/Toolbar';
import IconButton from '@mui/material/IconButton';
import MenuIcon from '@mui/icons-material/Menu';
import Drawer from '@mui/material/Drawer';
import Badge from '@mui/material/Badge';
import List from '@mui/material/List';
import Divider from '@mui/material/Divider';
import ListItemIcon from '@mui/material/ListItemIcon';
import ListItemText from '@mui/material/ListItemText';
import SportsEsportsIcon from '@mui/icons-material/SportsEsports';
import NotificationsIcon from '@mui/icons-material/Notifications';
import InputIcon from '@mui/icons-material/Input';
import LanguageIcon from '@mui/icons-material/Language';
import Typography from '@mui/material/Typography';
import withStyles from '@mui/styles/withStyles';
import { Styled, StyleGenerator } from '../utils';
import { MainMenuListItem } from './MainMenuListItem';
import { notificationClient } from '@xoutput/client';
import Translation from '../translation/Translation';

type ClassNames = 'menubarButton' | 'drawerRoot' | 'drawerHeader' | 'placeholder' | 'title';

const styles: StyleGenerator<ClassNames> = (theme) => ({
  menubarButton: {
    color: theme.palette.common.white,
  },
  drawerRoot: {
    width: '360px',
    maxWidth: '360px',
  },
  drawerHeader: {
    backgroundColor: theme.palette.primary.main,
    color: theme.palette.common.white,
    padding: '10px',
  },
  placeholder: {
    flexGrow: 1,
  },
  title: {
    flexGrow: 1,
    color: theme.palette.common.white,
  },
});

export type MainMenuProps = {};

export type InternalMainMenuProps = Styled<ClassNames> & MainMenuProps;

const MainMenuComponent = ({ classes }: InternalMainMenuProps) => {
  const [open, setOpen] = useState(false);
  const [notificationCount, setNotificationCount] = useState(0);

  useEffect(() => {
    notificationClient.getNotifications().then((notifications) => {
      setNotificationCount(notifications.filter((n) => !n.acknowledged).length);
    });
  }, []);

  return (
    <>
      <AppBar position="static">
        <Toolbar>
          <IconButton edge="start" className={classes.menubarButton} color="inherit" aria-label="menu" onClick={() => setOpen(true)}>
            <MenuIcon />
          </IconButton>
          <Link to="/" color="textPrimary">
            <Typography variant="h6" className={classes.title}>
              XOutput
            </Typography>
          </Link>
          <div className={classes.placeholder}></div>
          <Link to="/notifications" color="textPrimary">
            <IconButton edge="start" className={classes.menubarButton} color="inherit" aria-label="menu">
              <Badge badgeContent={notificationCount} color="secondary">
                <NotificationsIcon />
              </Badge>
            </IconButton>
          </Link>
        </Toolbar>
      </AppBar>
      <Drawer anchor="left" open={open} onClose={() => setOpen(false)}>
        <div className={classes.drawerRoot}>
          <div className={classes.drawerHeader}>
            <Typography variant="h4">XOutput</Typography>
          </div>
          <List component="nav">
            <MainMenuListItem path="/controllers" onClick={() => setOpen(false)}>
              <ListItemIcon>
                <SportsEsportsIcon />
              </ListItemIcon>
              <ListItemText primary={Translation.translate('ActiveControllers')} />
            </MainMenuListItem>
            <MainMenuListItem path="/inputs" onClick={() => setOpen(false)}>
              <ListItemIcon>
                <InputIcon />
              </ListItemIcon>
              <ListItemText primary={Translation.translate('InputDevices')} />
            </MainMenuListItem>
            <MainMenuListItem path="/devices" onClick={() => setOpen(false)}>
              <ListItemIcon>
                <LanguageIcon />
              </ListItemIcon>
              <ListItemText primary={Translation.translate('OnlineDevices')} />
            </MainMenuListItem>
            <MainMenuListItem path="/inputreader" onClick={() => setOpen(false)}>
              <ListItemIcon>
                <LanguageIcon />
              </ListItemIcon>
              <ListItemText primary={Translation.translate('InputReader')} />
            </MainMenuListItem>
          </List>
          <Divider />
          <List component="nav">
            <MainMenuListItem path="/notifications" onClick={() => setOpen(false)}>
              <ListItemIcon>
                <NotificationsIcon />
              </ListItemIcon>
              <ListItemText primary={Translation.translate('Notifications')} />
            </MainMenuListItem>
          </List>
        </div>
      </Drawer>
    </>
  );
};

export const MainMenu = withRouter(withStyles(styles)(MainMenuComponent));
