import React from 'react';
import { Switch, Route, Redirect, RouteChildrenProps, withRouter, RouteComponentProps } from 'react-router';
import { DeviceSelector } from './emulation/deviceselector';
import { XboxEmulation } from './emulation/xbox';
import { Link } from 'react-router-dom';
import { Controllers } from './controllers/controllers';
import AppBar from '@material-ui/core/AppBar';
import Toolbar from '@material-ui/core/Toolbar';
import IconButton from '@material-ui/core/IconButton';
import MenuIcon from '@material-ui/icons/Menu';
import Drawer from '@material-ui/core/Drawer';
import Badge from '@material-ui/core/Badge';
import List from '@material-ui/core/List';
import Divider from '@material-ui/core/Divider';
import ListItemIcon from '@material-ui/core/ListItemIcon';
import ListItemText from '@material-ui/core/ListItemText';
import SportsEsportsIcon from '@material-ui/icons/SportsEsports';
import NotificationsIcon from '@material-ui/icons/Notifications';
import InputIcon from '@material-ui/icons/Input';
import LanguageIcon from '@material-ui/icons/Language';
import Typography from '@material-ui/core/Typography';
import withStyles from '@material-ui/core/styles/withStyles';
import { Inputs } from './input/inputs';
import { InputDetails } from './input/details';
import { Ds4Emulation } from './emulation/ds4';
import { Styled, StyleGenerator } from '../utils';
import Translation from '../translation/Translation';
import { Notifications } from './notifications/Notifications';
import { MainMenuListItem } from './MainMenuListItem';
import { rest, Notification } from '../client/rest';
import { Home } from './home/home';

type ClassNames = 'menubarButton' | 'mainContent' | 'title' | 'drawerRoot' | 'drawerHeader' | 'placeholder';

const styles: StyleGenerator<ClassNames> = (theme) => ({
  menubarButton: {
    color: theme.palette.common.white,
  },
  mainContent: {
    margin: '8px',
  },
  title: {
    flexGrow: 1,
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
});

export interface MainMenuProps extends Styled<ClassNames>, RouteComponentProps {}

export interface MainMenuState {
  menuOpen: boolean;
  notificationCount: number;
}

class MainMenuComponent extends React.Component<MainMenuProps, MainMenuState> {
  state: MainMenuState = {
    menuOpen: false,
    notificationCount: 0,
  };

  componentDidMount() {
    this.props.history.listen(() => {
      this.refreshDevices();
    });
    this.refreshDevices();
  }

  changeMenu(open: boolean): void {
    this.setState({
      menuOpen: open,
    });
  }

  refreshDevices(): Promise<Notification[]> {
    return rest.getNotifiations().then((notifications) => {
      this.setState({
        notificationCount: notifications.filter((n) => !n.acknowledged).length,
      });
      return notifications;
    });
  }

  render() {
    const { classes, location } = this.props;
    return (
      <>
        <Switch>
          <Route path="/emulation" />
          <Route>
            <AppBar position="static">
              <Toolbar>
                <IconButton
                  edge="start"
                  className={classes.menubarButton}
                  color="inherit"
                  aria-label="menu"
                  onClick={() => this.changeMenu(true)}
                >
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
                    <Badge badgeContent={this.state.notificationCount} color="secondary">
                      <NotificationsIcon />
                    </Badge>
                  </IconButton>
                </Link>
              </Toolbar>
            </AppBar>
            <Drawer anchor="left" open={this.state.menuOpen} onClose={() => this.changeMenu(false)}>
              <div className={classes.drawerRoot}>
                <div className={classes.drawerHeader}>
                  <Typography variant="h4">XOutput</Typography>
                </div>
                <List component="nav">
                  <MainMenuListItem path="/controllers" onClick={() => this.changeMenu(false)}>
                    <ListItemIcon>
                      <SportsEsportsIcon />
                    </ListItemIcon>
                    <ListItemText primary={Translation.translate('ActiveControllers')} />
                  </MainMenuListItem>
                  <MainMenuListItem path="/inputs" onClick={() => this.changeMenu(false)}>
                    <ListItemIcon>
                      <InputIcon />
                    </ListItemIcon>
                    <ListItemText primary={Translation.translate('InputDevices')} />
                  </MainMenuListItem>
                  <MainMenuListItem path="/devices" onClick={() => this.changeMenu(false)}>
                    <ListItemIcon>
                      <LanguageIcon />
                    </ListItemIcon>
                    <ListItemText primary={Translation.translate('OnlineDevices')} />
                  </MainMenuListItem>
                </List>
                <Divider />
                <List component="nav">
                  <MainMenuListItem path="/notifications" onClick={() => this.changeMenu(false)}>
                    <ListItemIcon>
                      <NotificationsIcon />
                    </ListItemIcon>
                    <ListItemText primary={Translation.translate('Notifications')} />
                  </MainMenuListItem>
                </List>
              </div>
            </Drawer>
          </Route>
        </Switch>
        <div className={classes.mainContent}>
          <Switch>
            <Route path="/" exact>
              <Home></Home>
            </Route>
            <Route path="/controllers">
              <Controllers></Controllers>
            </Route>
            <Route path="/devices">
              <DeviceSelector></DeviceSelector>
            </Route>
            <Route
              path="/emulation/MicrosoftXbox360/:emulator"
              component={(props: RouteChildrenProps<{ emulator: string }>) => (
                <XboxEmulation deviceType="MicrosoftXbox360" emulator={props.match.params.emulator}></XboxEmulation>
              )}
            />
            <Route
              path="/emulation/SonyDualShock4/:emulator"
              component={(props: RouteChildrenProps<{ emulator: string }>) => (
                <Ds4Emulation deviceType="SonyDualShock4" emulator={props.match.params.emulator}></Ds4Emulation>
              )}
            />
            <Route path="/inputs" exact>
              <Inputs></Inputs>
            </Route>
            <Route
              path="/inputs/:id"
              component={(props: RouteChildrenProps<{ id: string }>) => <InputDetails id={props.match.params.id}></InputDetails>}
            />
            <Route path="/notifications" exact>
              <Notifications></Notifications>
            </Route>
            <Route>
              <Redirect to="/" />
            </Route>
          </Switch>
        </div>
      </>
    );
  }
}

export const MainMenu = withRouter(withStyles(styles)(MainMenuComponent));
export default MainMenu;
