import Card from '@mui/material/Card';
import React from 'react';
import { useEmulatedControllersQuery } from '../../queries/useEmulatedControllersQuery';
import { Async } from '../components/Asnyc';

const ControllersComponent = () => {
  const { data: controllers, isLoading, isSuccess, error } = useEmulatedControllersQuery();

  return (
    <Async isLoading={isLoading} isSuccess={isSuccess} error={error}>
      {() => (
        <>
          {controllers.map((c) => (
            <Card key={c.id}>
              <div>{c.name}</div>
            </Card>
          ))}
        </>
      )}
    </Async>
  );
};

export const Controllers = ControllersComponent;
