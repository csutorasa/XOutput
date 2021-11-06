import Card from '@mui/material/Card';
import React from 'react';
import { useInputsQuery } from '../../queries/useInputsQuery';
import { Async } from '../components/Asnyc';

const InputsComponent = () => {
  const { data: inputs, isLoading, isSuccess, error } = useInputsQuery();

  return (
    <Async isLoading={isLoading} isSuccess={isSuccess} error={error}>
      {() => (
        <>
          {inputs.map((i) => (
            <Card key={i.id}>
              <div>{i.name}</div>
              <div>{i.deviceApi}</div>
              {i.sources.filter((s) => s.type.startsWith('Axis')).length} axes {i.sources.filter((s) => s.type === 'Button').length} buttons
            </Card>
          ))}
        </>
      )}
    </Async>
  );
};

export const Inputs = InputsComponent;
