import React, { ReactNode } from 'react';
import { Link, RouteComponentProps, withRouter } from 'react-router-dom';
import ListItem from '@mui/material/ListItem';

export type MainMenuListItemProps = RouteComponentProps & {
  onClick: () => void;
  path: string;
  children: ReactNode;
};

const MainMenuListItemComponent = ({ location, path, onClick, children }: MainMenuListItemProps) => {
  return (
    <ListItem button component={Link} to={path} onClick={() => (onClick ? onClick() : null)} selected={path === location.pathname}>
      {children}
    </ListItem>
  );
};

export const MainMenuListItem = withRouter(MainMenuListItemComponent);
export default MainMenuListItem;
