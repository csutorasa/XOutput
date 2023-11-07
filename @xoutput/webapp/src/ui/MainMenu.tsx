import React, { useEffect, useState } from 'react';
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
import { styled } from '@mui/system';
import { MainMenuListItem } from './MainMenuListItem';
import { notificationClient } from '@xoutput/client';
import Translation from '../translation/Translation';
import { PlaceHolder } from './components/Placeholder';

export type MainMenuProps = {};

export const MainMenu = ({}: MainMenuProps) => {
  const [open, setOpen] = useState(false);
  const [notificationCount, setNotificationCount] = useState(0);

  useEffect(() => {
    notificationClient.getNotifications().then((notifications) => {
      setNotificationCount(notifications.filter((n) => !n.acknowledged).length);
    });
  }, []);

  const Title = styled(Typography)(({ theme }) => ({
    color: theme.palette.common.white,
  }));

  const MenubarButton = styled(IconButton)(({ theme }) => ({
    color: theme.palette.common.white,
  }));

  const DrawerHeader = styled('div')(({ theme }) => ({
    backgroundColor: theme.palette.primary.main,
    color: theme.palette.common.white,
    padding: '10px',
  }));

  return (
    <>
      <AppBar position="static">
        <Toolbar>
          <MenubarButton edge="start" color="inherit" aria-label="menu" onClick={() => setOpen(true)}>
            <MenuIcon />
          </MenubarButton>
          <Link to="/" color="textPrimary">
            <Title variant="h6" style={{ flexGrow: 1 }}>
              XOutput
            </Title>
          </Link>
          <PlaceHolder />
          <Link to="/notifications" color="textPrimary">
            <MenubarButton edge="start" color="inherit" aria-label="menu">
              <Badge badgeContent={notificationCount} color="secondary">
                <NotificationsIcon />
              </Badge>
            </MenubarButton>
          </Link>
        </Toolbar>
      </AppBar>
      <Drawer anchor="left" open={open} onClose={() => setOpen(false)}>
        <div style={{ width: '360px', maxWidth: '360px' }}>
          <DrawerHeader>
            <Typography variant="h4">XOutput</Typography>
          </DrawerHeader>
          <List component="nav">
            <MainMenuListItem path="/emulated/controllers" onClick={() => setOpen(false)}>
              <ListItemIcon>
                <SportsEsportsIcon />
              </ListItemIcon>
              <ListItemText primary={Translation.translate('ActiveControllers')} />
            </MainMenuListItem>
            <MainMenuListItem path="/mapped/controllers" onClick={() => setOpen(false)}>
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
