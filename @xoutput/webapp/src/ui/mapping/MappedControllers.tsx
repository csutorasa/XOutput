import Card from '@mui/material/Card';
import React from 'react';
import { useMappedControllersQuery } from '../../queries/useMappedControllersQuery';
import { Async } from '../components/Asnyc';

const MappedControllersComponent = () => {
  const { data: controllers, isLoading, isSuccess, error } = useMappedControllersQuery();

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

export const MappedControllers = MappedControllersComponent;
