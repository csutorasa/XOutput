import Card from '@mui/material/Card';
import Typography from '@mui/material/Typography';
import React from 'react';
import { Link } from 'react-router-dom';
import { useInputsQuery } from '../../queries/useInputsQuery';
import { Async } from '../components/Asnyc';

const InputDevicesComponent = () => {
  const { data: inputs, isLoading, isSuccess, error } = useInputsQuery();

  return (
    <Async isLoading={isLoading} isSuccess={isSuccess} error={error}>
      {() => (
        <>
          {inputs.map((i) => (
            <Card key={i.id}>
              <Typography variant="h4">
                <Link to={`inputs/${i.id}`}>
                  {i.name} ({i.id})
                </Link>
              </Typography>
              <div>{i.deviceApi}</div>
              {i.sources.filter((s) => s.type.startsWith('Axis')).length} axes {i.sources.filter((s) => s.type === 'Button').length} buttons
            </Card>
          ))}
        </>
      )}
    </Async>
  );
};

export const InputDevices = InputDevicesComponent;
