import React from 'react';
import { Link, RouteComponentProps, withRouter } from 'react-router-dom';
import ListItem from '@material-ui/core/ListItem';

export interface MainMenuListItemProps extends RouteComponentProps {
  onClick: () => void;
  path: string;
}

class MainMenuListItemComponent extends React.Component<MainMenuListItemProps> {
  render() {
    const { location } = this.props;
    return (
      <ListItem
        button
        component={Link}
        to={this.props.path}
        onClick={() => (this.props.onClick ? this.props.onClick() : null)}
        selected={this.props.path === location.pathname}
      >
        {this.props.children}
      </ListItem>
    );
  }
}

export const MainMenuListItem = withRouter(MainMenuListItemComponent);
export default MainMenuListItem;
